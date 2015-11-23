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

namespace chocolatey.package.validator.infrastructure.filesystem
{
    /// <summary>
    ///   Implementation of IKnownFolder for RMA
    /// </summary>
    public class KnownFolder : IKnownFolder
    {
        private readonly string _directory;

        /// <summary>
        ///   Initializes a new instance of the <see cref="KnownFolder" /> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        public KnownFolder(string directory)
        {
            _directory = directory;
        }

        /// <summary>
        ///   Gets the directory path for a known folder.
        /// </summary>
        /// <value>The directory path for a known folder.</value>
        public string Directory { get { return _directory; } }
    }
}
