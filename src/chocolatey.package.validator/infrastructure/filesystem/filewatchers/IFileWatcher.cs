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

namespace chocolatey.package.validator.infrastructure.filesystem.filewatchers
{
    using System;

    /// <summary>
    ///   File Watcher Interface
    /// </summary>
    public interface IFileWatcher : IDisposable
    {
        /// <summary>
        ///   Occurs when [file found event].
        /// </summary>
        event FileFoundEventHandler file_found_event;

        /// <summary>
        ///   Gets the file path.
        /// </summary>
        /// <value>The file path.</value>
        string FilePath { get; }

        /// <summary>
        ///   Gets or sets a value indicating whether [rename file].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [rename file]; otherwise, <c>false</c>.
        /// </value>
        bool RenameFile { get; set; }

        /// <summary>
        ///   Gets or sets the Interval in seconds.
        /// </summary>
        /// <value>The Interval in seconds.</value>
        double IntervalInSeconds { get; set; }

        /// <summary>
        ///   Starts the watching.
        /// </summary>
        void start_watching();

        /// <summary>
        ///   Stops the watching.
        /// </summary>
        void stop_watching();
    }
}
