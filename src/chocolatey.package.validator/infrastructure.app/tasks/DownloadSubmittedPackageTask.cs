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
    using configuration;
    using filesystem;
    using infrastructure.messaging;
    using infrastructure.tasks;
    using messaging;
    using services;

    public class DownloadSubmittedPackageTask : ITask
    {
        private readonly IConfigurationSettings _configurationSettings;
        private readonly INuGetService _nugetService;
        private readonly IFileSystem _fileSystem;
        private IDisposable _subscription;

        public DownloadSubmittedPackageTask(IConfigurationSettings configurationSettings, INuGetService nugetService, IFileSystem fileSystem)
        {
            _configurationSettings = configurationSettings;
            _nugetService = nugetService;
            _fileSystem = fileSystem;
        }

        public void initialize()
        {
            _subscription = EventManager.subscribe<ValidatePackageMessage>(download_package, null, null);
            this.Log().Info(() => "{0} is now ready and waiting for {1}".format_with(GetType().Name, typeof(ValidatePackageMessage).Name));
        }

        public void shutdown()
        {
            if (_subscription != null) _subscription.Dispose();
        }

        private void download_package(ValidatePackageMessage message)
        {
            this.Log().Info(() => "Validating Package: {0} Version: {1}".format_with(message.PackageId, message.PackageVersion));

            var tempInstallLocation = _fileSystem.combine_paths(_fileSystem.get_temp_path(), ApplicationParameters.Name, "TempInstall_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff"));
            _fileSystem.create_directory_if_not_exists(tempInstallLocation);

            var package = _nugetService.download_package(message.PackageId, message.PackageVersion, tempInstallLocation, _configurationSettings);

            EventManager.publish(new PackageReadyForValidationMessage(package, tempInstallLocation));
        }
    }
}
