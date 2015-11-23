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
    /// <summary>
    ///   A type representing information about a file
    /// </summary>
    public class FileInformation
    {
        /// <summary>
        ///   Gets or sets the name of the file.
        /// </summary>
        /// <value>
        ///   The name of the file.
        /// </value>
        public string FileName { get; set; }

        /// <summary>
        ///   Gets or sets the file path.
        /// </summary>
        /// <value>
        ///   The file path.
        /// </value>
        public string FilePath { get; set; }

        /// <summary>
        ///   Gets or sets the type of the content.
        /// </summary>
        /// <value>
        ///   The type of the content.
        /// </value>
        public string ContentType { get; set; }
    }
}
