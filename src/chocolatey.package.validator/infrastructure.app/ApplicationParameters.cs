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

namespace chocolatey.package.validator.infrastructure.app
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Reflection;
    using System.Web;
    using infrastructure.configuration;

    // ReSharper disable InconsistentNaming

    /// <summary>
    ///   Parameters used application wide are found here.
    /// </summary>
    public static class ApplicationParameters
    {
        /// <summary>
        ///   Gets the name of the application
        /// </summary>
        public const string Name = "chocolatey-package-validator";

        /// <summary>
        ///   The name of the connection string in the config
        /// </summary>
        public const string ConnectionStringName = "chocolatey.package.validator";

        /// <summary>
        ///   The email regular expression
        /// </summary>
        public const string EmailRegularExpression = @"[.\S]+\@[.\S]+\.[.\S]+";

        /// <summary>
        ///   The phone number regular expression, just need the string to contain 10 digits
        /// </summary>
        public const string PhoneNumberRegularExpression = @"(\D*\d){10}\D*";

        /// <summary>
        ///   The zip code regular expression
        /// </summary>
        public const string ZipCodeRegularExpression = @"\d{5}";

        public const char WebSeparatorChar = '/';

        /// <summary>
        ///   Gets a value indicating whether we are in Debug Mode?
        /// </summary>
        public static bool IsDebug
        {
            get { return try_get_config(() => Config.get_configuration_settings().IsDebugMode, false); }
        }

        /// <summary>
        ///   Gets the connection string.
        /// </summary>
        public static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString; }
        }

        /// <summary>
        ///   Gets the file version.
        /// </summary>
        public static string FileVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return versionInfo.FileVersion;
            }
        }

        /// <summary>
        ///   Gets the informational product version.
        /// </summary>
        public static string ProductVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
               
                return versionInfo.ProductVersion;
            }
        }

        /// <summary>
        ///   Gets the current user's name
        /// </summary>
        /// <returns>
        ///   The <see cref="string" />.
        /// </returns>
        public static string get_current_user_name()
        {
            string userName = Environment.UserName;

            if (HttpContext.Current != null)
            {
                var httpUser = HttpContext.Current.User;
                if (httpUser != null && httpUser.Identity != null) userName = httpUser.Identity.Name;
            }

            return userName;
        }

        /// <summary>
        ///   Gets the system email address.
        /// </summary>
        /// <returns>The email address from system.net/mailsettings/from; otherwise null.</returns>
        public static string get_system_email_address()
        {
            return Config.get_configuration_settings().SystemEmailAddress;
        }

        private static T try_get_config<T>(Func<T> func, T defaultValue)
        {
            try
            {
                return func.Invoke();
            } catch (Exception)
            {
                return defaultValue;
            }
        }
    }

    // ReSharper restore InconsistentNaming
}
