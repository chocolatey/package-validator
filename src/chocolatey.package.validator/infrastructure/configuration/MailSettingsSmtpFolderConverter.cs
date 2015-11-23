// Copyright © 2015 - Present RealDimensions Software, LLC
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// 
// You may obtain a copy of the License at
// 
// 	http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace chocolatey.package.validator.infrastructure.configuration
{
    using System.Configuration;
    using System.IO;
    using System.Net.Configuration;
    using System.Net.Mail;
    using System.Reflection;
    using System.Web;

    /// <summary>
    ///   Converts a smtp folder relative path to an absolute
    /// </summary>
    /// <remarks>
    ///   Based on
    ///   http://www.singular.co.nz/blog/archive/2007/12/19/programmatically-setting-the-smtpclient-pickup-directory-location-at-runtime.aspx
    /// </remarks>
    public static class MailSettingsSmtpFolderConverter
    {
        private static bool? _isUsingPickupDirectory;

        /// <summary>
        ///   Gets a value indicating whether the default SMTP Delivery method is SpecifiedPickupDirectory
        /// </summary>
        private static bool IsUsingPickupDirectory
        {
            get
            {
                if (!_isUsingPickupDirectory.HasValue)
                {
                    var smtp = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                    _isUsingPickupDirectory = smtp.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory;
                }

                return _isUsingPickupDirectory.Value;
            }
        }

        public static void convert_relative_to_absolute_pickup_directory_location()
        {
            if (IsUsingPickupDirectory) set_pickup_directory_location();
        }

        /// <summary>
        ///   Sets the default PickupDirectoryLocation for the SmtpClient.
        /// </summary>
        /// <remarks>
        ///   This method should be called to set the PickupDirectoryLocation for the SmtpClient at runtime (Application_Start)
        ///   Reflection is used to set the private variable located in the internal class for the SmtpClient's mail configuration:
        ///   System.Net.Mail.SmtpClient.MailConfiguration.Smtp.SpecifiedPickupDirectory.PickupDirectoryLocation The folder must
        ///   exist.
        /// </remarks>
        private static void set_pickup_directory_location()
        {
            const BindingFlags instanceFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            PropertyInfo prop;
            object mailConfiguration, smtp, specifiedPickupDirectory;

            // get static internal property: MailConfiguration
            prop = typeof(SmtpClient).GetProperty("MailConfiguration", BindingFlags.Static | BindingFlags.NonPublic);
            mailConfiguration = prop.GetValue(null, null);

            // get internal property: Smtp
            prop = mailConfiguration.GetType().GetProperty("Smtp", instanceFlags);
            smtp = prop.GetValue(mailConfiguration, null);

            // get internal property: SpecifiedPickupDirectory
            prop = smtp.GetType().GetProperty("SpecifiedPickupDirectory", instanceFlags);
            specifiedPickupDirectory = prop.GetValue(smtp, null);

            FieldInfo field = specifiedPickupDirectory.GetType().GetField("pickupDirectoryLocation", instanceFlags);

            var path = (string)field.GetValue(specifiedPickupDirectory);
            var absolutePath = Path.GetFullPath(path);

            if (HttpContext.Current != null) absolutePath = HttpContext.Current.Server.MapPath(path);
            if (!Directory.Exists(absolutePath)) Directory.CreateDirectory(absolutePath);

            field.SetValue(specifiedPickupDirectory, absolutePath);
        }
    }
}
