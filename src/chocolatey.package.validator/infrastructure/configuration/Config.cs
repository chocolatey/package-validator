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
    using app.configuration;

    /// <summary>
    ///   Configuration initialization
    /// </summary>
    public static class Config
    {
        private static IConfigurationSettings _configuration = new ConfigurationSettings();

        /// <summary>
        ///   Initializes application configuration with a configuration instance.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public static void initialize_with(IConfigurationSettings configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        ///   Gets the configuration settings.
        /// </summary>
        /// <returns>
        ///   An instance of <see cref="IConfigurationSettings" /> if one has been initialized; defaults to
        ///   <see
        ///     cref="ConfigurationSettings" />
        ///   if one has not been.
        /// </returns>
        public static IConfigurationSettings get_configuration_settings()
        {
            return _configuration;
        }
    }
}
