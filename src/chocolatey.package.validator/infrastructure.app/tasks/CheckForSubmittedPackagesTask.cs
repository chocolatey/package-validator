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
    using System.Linq;
    using System.Timers;
    using configuration;
    using infrastructure.messaging;
    using infrastructure.tasks;
    using messaging;
    using services;
    using webservices;

    public class CheckForSubmittedPackagesTask : ITask
    {
        private readonly IConfigurationSettings _configurationSettings;
        private const double TIMER_INTERVAL = 600000;
        private const string SERVICE_ENDPOINT = "/api/v2/submitted/";
        private readonly Timer _timer = new Timer();
        private IDisposable _subscription;

        public CheckForSubmittedPackagesTask(IConfigurationSettings configurationSettings)
        {
            _configurationSettings = configurationSettings;
        }

        public void initialize()
        {
            _subscription = EventManager.subscribe<StartupMessage>((message) => timer_elapsed(null, null), null, null);
            _timer.Interval = TIMER_INTERVAL;
            _timer.Elapsed += timer_elapsed;
            _timer.Start();
            this.Log().Info(() => "{0} will check for new package submissions every {1} minutes".format_with(GetType().Name, TIMER_INTERVAL / 60000));
        }

        public void shutdown()
        {
            if (_subscription != null) _subscription.Dispose();

            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
        }

        private void timer_elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            this.Log().Info(() => "Checking for submitted packages.");

            var submittedPackagesUri = NuGetService.get_service_endpoint_url(_configurationSettings.PackagesUrl, SERVICE_ENDPOINT);

            var service = new FeedContext_x0060_1(submittedPackagesUri);

            // this only returns 40 results but at least we'll have something to start with
            IList<V2FeedPackage> submittedPackages = service.Packages.Where(p => p.PackageTestResultStatus == null || p.PackageTestResultStatus == "Pending" || p.PackageTestResultStatus == "Unknown").or_empty_list_if_null().ToList();

            this.Log().Info("Pulled {0} packages in submitted status for review.".format_with(submittedPackages.Count));

            foreach (var package in submittedPackages)
            {
                this.Log().Info("{0} found in submitted state.".format_with(package.Title));
                EventManager.publish(new SubmitPackageMessage(package.Id, package.Version));
            }

            this.Log().Info(() => "Finished checking for submitted packages.");

            _timer.Start();
        }
    }
}
