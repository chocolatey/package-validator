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

namespace chocolatey.package.validator.infrastructure.app.configuration
{
    using System;
    using System.Configuration;
    using System.Net.Configuration;

    /// <summary>
    ///   Configuration settings for the application
    /// </summary>
    public class ConfigurationSettings : IConfigurationSettings
    {
        /// <summary>
        ///   Gets the application settings value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A string with the settings value; otherwise an empty string</returns>
        public string get_application_settings_value(string name)
        {
            return ConfigurationManager.AppSettings.Get(name);
        }

        /// <summary>
        ///   Gets the configuration section.
        /// </summary>
        /// <typeparam name="T">The configuration section type</typeparam>
        /// <param name="section">The section.</param>
        /// <returns>The configuration section requested as a strong type; otherwise null</returns>
        public T get_configuration_section<T>(string section) where T : ConfigurationSection
        {
            return ConfigurationManager.GetSection(section) as T;

            // var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // return config.GetSectionGroup(section) as T;
        }

        /// <summary>
        ///   Gets the SMTP email from mail settings section.
        /// </summary>
        /// <param name="settings">The settings section.</param>
        /// <returns>
        ///   The From property on <see cref="SmtpSection" />.
        /// </returns>
        public string get_smtp_email_from_mail_settings_section(SmtpSection settings)
        {
            if (settings == null) return string.Empty;

            return settings.From;
        }

        /// <summary>
        ///   Gets the system email address.
        /// </summary>
        public string SystemEmailAddress
        {
            get
            {
                return
                    get_smtp_email_from_mail_settings_section(
                        get_configuration_section<SmtpSection>("system.net/mailSettings/smtp"));
            }
        }

        /// <summary>
        ///   Gets a value indicating whether this instance is in debug mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is debug mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsDebugMode
        {
            get
            {
                return get_application_settings_value("IsDebugMode")
                    .Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
            }
        }
        
        /// <summary>
        ///   Gets the files path.
        /// </summary>
        public string FilesPath { get { return get_application_settings_value("Path.Files"); } }

        /// <summary>
        ///   Gets an email to use as an override instead of the provided email. If null, use the provided email.
        /// </summary>
        public string TestEmailOverride { get { return get_application_settings_value("TestingEmailOverride"); } }

        public int CommandExecutionTimeoutSeconds
        {
            get { return int.Parse(get_application_settings_value("CommandExecutionTimeoutSeconds")); }
        }

        public string PackagesUrl { get { return get_application_settings_value("PackagesUrl"); } }
        public string PackagesApiKey { get { return get_application_settings_value("PackagesApiKey"); } }

    }
}
