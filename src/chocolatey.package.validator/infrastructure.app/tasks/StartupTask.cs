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
    using System.Timers;
    using infrastructure.messaging;
    using infrastructure.tasks;
    using messaging;

    public class StartupTask : ITask
    {
        private const double TIMER_INTERVAL = 15000;
        private readonly Timer _timer = new Timer();

        public void initialize()
        {
            _timer.Interval = TIMER_INTERVAL;
            _timer.Elapsed += timer_elapsed;
            _timer.Start();
            this.Log().Info(() => "{0} will send startup message in {1} milliseconds".format_with(GetType().Name, TIMER_INTERVAL));
        }

        public void shutdown()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
        }

        private void timer_elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            this.Log().Info(() => "{0} is sending startup message".format_with(GetType().Name));

            EventManager.publish(new StartupMessage());
        }
    }
}
