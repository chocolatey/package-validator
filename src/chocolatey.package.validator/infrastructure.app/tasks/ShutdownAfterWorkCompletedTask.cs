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
    using System.Text;
    using System.Timers;
    using infrastructure.messaging;
    using infrastructure.tasks;
    using messaging;

    public class ShutdownAfterWorkCompletedTask : ITask
    {
        private const int DEFAULT_MINUTES = 4;
        private const int FOLLOW_UP_MINUTES = 2;
        private readonly Timer _timer = new Timer();
        private IDisposable _subscription;

        /// <summary>
        ///   Initializes a task. This should be initialized to run on a schedule, a trigger, a subscription to event messages,
        ///   etc, or some combination of the above.
        /// </summary>
        public void initialize()
        {
            _subscription = EventManager.subscribe<ImportFilesCompleteMessage>(set_timer, null, null);
            this.Log().Info(() => "{0} is now ready and waiting for {1}".format_with(GetType().Name, typeof(ImportFilesCompleteMessage).Name));
        }

        /// <summary>
        ///   Synchronizes this instance.
        /// </summary>
        public void synchronize()
        {
            _timer.Stop();

            var canShutdown = !TaskTracker.are_active_tasks();

            if (!canShutdown)
            {
                var tasks = TaskTracker.get_active_tasks();
                var activeTasks = new StringBuilder();
                foreach (var task in tasks.or_empty_list_if_null())
                {
                    activeTasks.Append("{0}; ".format_with(task));
                }

                this.Log().Info("Still waiting on the following tasks: {0}".format_with(activeTasks.ToString()));
            }
            else
            {
                this.Log().Info("Signalling for shutdown... all tasks have completed.");
                EventManager.publish(new ShutdownMessage());
            }

            this.Log().Info("Waiting for {0} minutes to check again for ability to shutdown.".format_with(FOLLOW_UP_MINUTES));
            _timer.Interval = TimeSpan.FromMinutes(FOLLOW_UP_MINUTES).TotalMilliseconds;
            _timer.Start();
        }

        /// <summary>
        ///   Shuts down a task that is in a waiting state. Turns off all schedules, triggers or subscriptions.
        /// </summary>
        public void shutdown()
        {
            if (_subscription != null) _subscription.Dispose();

            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
        }

        private void set_timer(ImportFilesCompleteMessage message)
        {
            _timer.Interval = TimeSpan.FromMinutes(DEFAULT_MINUTES).TotalMilliseconds;
            _timer.Elapsed += (sender, args) => synchronize();
            _timer.Start();
            this.Log().Info(
                () => "{0} will check back in {1} minutes to see if the system can shut down".format_with(
                    GetType().Name,
                    DEFAULT_MINUTES));
        }
    }
}
