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

namespace chocolatey.package.validator.infrastructure.services
{
    using System;
    using System.IO;
    using app.configuration;
    using EnsureThat;

    /// <summary>
    ///   File System Service for interacting with files/directories/etc
    /// </summary>
    public class FileSystemService : IFileSystemService
    {
        private readonly IConfigurationSettings _configuration;

        /// <summary>
        ///   Initializes a new instance of the <see cref="FileSystemService" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public FileSystemService(IConfigurationSettings configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        ///   Gets the name of the file from the file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The File Name</returns>
        public string get_file_name(string filePath)
        {
            Ensure.That(() => filePath).IsNotNullOrWhiteSpace();

            return Path.GetFileName(filePath);
        }

        /// <summary>
        ///   Gets the file extension.
        /// </summary>
        /// <param name="path">The path to the file, could be just a name of a file.</param>
        /// <returns>
        ///   Extension of the file
        /// </returns>
        public string get_file_extension(string path)
        {
            return Path.GetExtension(path);
        }

        /// <summary>
        ///   Does a file exist?
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>A boolean value indicating if file exists</returns>
        public bool file_exists(string filePath)
        {
            return File.Exists(filePath);
        }

        /// <summary>
        ///   Combines all items together as one path with \ between them
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns>The combined path</returns>
        public string path_combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        /// <summary>
        ///   Gets the full path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The full path</returns>
        public string get_full_path(string path)
        {
            Ensure.That(() => path).IsNotNullOrWhiteSpace();

            return Path.GetFullPath(path);
        }

        /// <summary>
        ///   Gets the full directory path of a file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The directory name</returns>
        public string get_directory_name(string filePath)
        {
            Ensure.That(() => filePath).IsNotNullOrWhiteSpace();

            return Path.GetDirectoryName(filePath);
        }

        /// <summary>
        ///   Creates the directory if it doesn't exist.
        /// </summary>
        /// <param name="directory">The directory.</param>
        public void create_directory_if_not_exists(string directory)
        {
            Ensure.That(() => directory).IsNotNullOrWhiteSpace();

            directory = Path.GetFullPath(directory);

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        }

        /// <summary>
        ///   Saves the specified file name.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="fileStream">The file stream.</param>
        public void save(string filePath, Stream fileStream)
        {
            Ensure.That(() => filePath).IsNotNullOrWhiteSpace();

            filePath = get_full_path(filePath);
            create_directory_if_not_exists(get_directory_name(filePath));

            if (file_exists(filePath)) File.Move(filePath, filePath + ".{0:yyyyMMddHHmmssffff}".format_with(DateTime.Now));

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);

                fs.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        ///   Gets the stream for a file
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The Stream for the input file path</returns>
        public Stream get_stream(string filePath)
        {
            Ensure.That(() => filePath).IsNotNullOrWhiteSpace();
            filePath = get_full_path(filePath);

            if (file_exists(filePath)) return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            return null;
        }

        /// <summary>
        ///   Gets the file text.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The file text</returns>
        public string get_file_text(string filePath)
        {
            Ensure.That(() => filePath).IsNotNullOrWhiteSpace();
            filePath = get_full_path(filePath);

            if (file_exists(filePath)) return File.ReadAllText(filePath);

            return string.Empty;
        }
    }
}
