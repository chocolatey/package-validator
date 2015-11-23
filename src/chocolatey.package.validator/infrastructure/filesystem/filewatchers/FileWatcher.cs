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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Timers;

    /// <summary>
    ///   Implementation of IFileWatcher
    /// </summary>
    public class FileWatcher : IFileWatcher
    {
        private readonly IFileSystem _fileSystem;
        private readonly Timer _timer;
        private readonly string _filePath;
        private bool _renameFile = true;
        private double _intervalInSeconds = 10;
        private bool _disposing;

        /// <summary>
        ///   Initializes a new instance of the <see cref="FileWatcher" /> class.
        /// </summary>
        /// <param name="filePath">
        ///   The file path.
        /// </param>
        /// <param name="fileSystem">
        ///   The file system.
        /// </param>
        public FileWatcher(string filePath, IFileSystem fileSystem)
        {
            _filePath = fileSystem.get_full_path(filePath);
            _fileSystem = fileSystem;
            _timer = new Timer(TimeSpan.FromSeconds(_intervalInSeconds).TotalMilliseconds);
            _timer.Elapsed += timer_elapsed;
        }

        /// <summary>
        ///   Occurs when [file found event].
        /// </summary>
        public event FileFoundEventHandler file_found_event;

        /// <summary>
        ///   Gets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath { get { return _filePath; } }

        public bool RenameFile { get { return _renameFile; } set { _renameFile = value; } }

        /// <summary>
        ///   Gets or sets the Interval in seconds.
        /// </summary>
        /// <value>The Interval in seconds.</value>
        public double IntervalInSeconds
        {
            get { return _intervalInSeconds; }

            set
            {
                _intervalInSeconds = value;
                change_interval_and_reset_timer_if_running();
            }
        }

        /// <summary>
        ///   Looks for file and generates a notification.
        /// </summary>
        public void look_for_file_and_generate_notification()
        {
            try
            {
                IList<string> filesToProcess = new List<string>();

                if (is_directory(FilePath)) filesToProcess = _fileSystem.get_files(FilePath, "*.*", SearchOption.AllDirectories).ToList();
                else if (_fileSystem.file_exists(FilePath)) filesToProcess.Add(FilePath);

                if (filesToProcess.Count != 0)
                {
                    foreach (string file in filesToProcess)
                    {
                        var tempFile = file;
                        if (_renameFile)
                        {
                            var renamedFile = string.Format(
                                "{0}_renamed{1}",
                                _fileSystem.get_file_name_without_extension(file),
                                _fileSystem.get_file_extension(file));
                            tempFile = _fileSystem.combine_paths(_fileSystem.get_directory_name(file), renamedFile);
                            _fileSystem.move_file(file, tempFile);
                        }

                        file_found_event.Invoke(new FileFoundEventArgs(tempFile));
                    }
                }
            } catch (Exception ex)
            {
                this.Log()
                    .Error(
                        () =>
                            string.Format("Exception caught when watching for trigger files:{0}{1}", Environment.NewLine, ex));
            }
        }

        /// <summary>
        ///   Starts the timer.
        /// </summary>
        public void start_watching()
        {
            _timer.Start();
        }

        /// <summary>
        ///   Stops the timer.
        /// </summary>
        public void stop_watching()
        {
            _timer.Stop();
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!_disposing)
            {
                _disposing = true;
                _timer.Stop();
                _timer.Dispose();
            }
        }

        /// <summary>
        ///   Determines whether the specified file path is directory.
        /// </summary>
        /// <param name="filePath">
        ///   The file path.
        /// </param>
        /// <returns>
        ///   <c>true</c> if the specified file path is directory; otherwise, <c>false</c>.
        /// </returns>
        private bool is_directory(string filePath)
        {
            return _fileSystem.directory_exists(filePath);
        }

        /// <summary>
        ///   Changes the timer Interval and resets the timer if running.
        /// </summary>
        private void change_interval_and_reset_timer_if_running()
        {
            _timer.Interval = _intervalInSeconds;
            if (_timer.Enabled)
            {
                stop_watching();
                start_watching();
            }
        }

        /// <summary>
        ///   Timers the elapsed.
        /// </summary>
        /// <param name="sender">
        ///   The sender.
        /// </param>
        /// <param name="e">
        ///   The <see cref="System.Timers.ElapsedEventArgs" /> instance containing the event data.
        /// </param>
        private void timer_elapsed(object sender, ElapsedEventArgs e)
        {
            stop_watching();
            look_for_file_and_generate_notification();
            start_watching();
        }
    }
}
