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
    using System.Collections.Generic;
    using infrastructure.messaging;
    using infrastructure.rules;
    using infrastructure.tasks;
    using messaging;
    using services;

    public class ValidatePackageTask : ITask
    {
        private readonly IPackageValidationService _packageValidationService;
        private IDisposable _subscription;

        public ValidatePackageTask(IPackageValidationService packageValidationService)
        {
            _packageValidationService = packageValidationService;
        }

        public void initialize()
        {
            _subscription = EventManager.subscribe<PackageReadyForValidationMessage>(validate_package, null, null);
            this.Log().Info(() => "{0} is now ready and waiting for {1}".format_with(GetType().Name, typeof(PackageReadyForValidationMessage).Name));
        }

        public void shutdown()
        {
            if (_subscription != null) _subscription.Dispose();
        }

        private void validate_package(PackageReadyForValidationMessage message)
        {
            IEnumerable<PackageValidationResult> validationResults = _packageValidationService.validate_package(message.Package);

            EventManager.publish(new PackageValidationResultMessage(message.Package.Id, message.Package.Version.to_string(), validationResults, DateTime.UtcNow));
            EventManager.publish(new PackageFinishedValidationMessage(message.TempDownloadLocation));
        }
    }
}
