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

namespace chocolatey.package.validator.infrastructure.app.messaging
{
    using System;
    using System.Collections.Generic;
    using infrastructure.messaging;
    using infrastructure.rules;
    
    public class PackageValidationResultMessage : IMessage
    {
        public PackageValidationResultMessage(
            string packageId,
            string packageVersion,
            IEnumerable<PackageValidationResult> validationResults,
            DateTime? testDate
            )
        {
            PackageId = packageId;
            PackageVersion = packageVersion;
            ValidationResults = validationResults;
            TestDate = testDate;
        }

        public string PackageId { get; private set; }
        public string PackageVersion { get; private set; }
        public IEnumerable<PackageValidationResult> ValidationResults { get; private set; }
        public DateTime? TestDate { get; private set; }
    }
}
