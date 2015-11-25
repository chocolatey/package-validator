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

namespace chocolatey.package.validator.infrastructure.app.tasks
{
    using System;
    using filesystem;
    using infrastructure.messaging;
    using infrastructure.tasks;
    using messaging;

    public class CleanupDownloadedPackageTask : ITask
    {
        private readonly IFileSystem _fileSystem;
        private IDisposable _subscription;

        public CleanupDownloadedPackageTask(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void initialize()
        {
            _subscription = EventManager.subscribe<PackageFinishedValidationMessage>(delete_directory, null, null);
            this.Log().Info(() => "{0} is now ready and waiting for {1}".format_with(GetType().Name, typeof(PackageFinishedValidationMessage).Name));
        }

        public void shutdown()
        {
            if (_subscription != null) _subscription.Dispose();
        }

        private void delete_directory(PackageFinishedValidationMessage message)
        {
            _fileSystem.delete_directory_if_exists(message.TempDownloadLocation, recursive: true);
        }
    }
}
