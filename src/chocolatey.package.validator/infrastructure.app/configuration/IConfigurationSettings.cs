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
    public interface IConfigurationSettings
    {

        /// <summary>
        ///   Gets the system email address.
        /// </summary>
        string SystemEmailAddress { get; }

        /// <summary>
        ///   Gets a value indicating whether this instance is in debug mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is debug mode; otherwise, <c>false</c>.
        /// </value>
        bool IsDebugMode { get; }
        
        /// <summary>
        ///   Gets the files path.
        /// </summary>
        string FilesPath { get; }
        
        /// <summary>
        ///   Gets an email to use as an override instead of the provided email. If null, use the provided email.
        /// </summary>
        string TestEmailOverride { get; }

        /// <summary>
        ///   Gets the number of seconds for a command to run before timing out.
        /// </summary>
        int CommandExecutionTimeoutSeconds { get; }

        /// <summary>
        ///   The url used for testing packages and submitting results.
        /// </summary>
        string PackagesUrl { get; }

        /// <summary>
        ///   The api key used for submitting test results to the PackagesUrl.
        /// </summary>
        string PackagesApiKey { get; }
    }
}
