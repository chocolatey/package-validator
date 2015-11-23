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

    /// <summary>
    ///   Uses information from the system
    /// </summary>
    public class SystemDateTimeUtcService : IDateTimeService
    {
        /// <summary>
        ///   Gets the system date time
        /// </summary>
        /// <returns>The current system date and time</returns>
        public DateTime? get_current_date_time()
        {
            return DateTime.UtcNow;
        }
    }
}
