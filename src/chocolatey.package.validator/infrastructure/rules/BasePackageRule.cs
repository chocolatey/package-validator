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

namespace chocolatey.package.validator.infrastructure.rules
{
    using System;
    using NuGet;

    public abstract class BasePackageRule : IPackageRule
    {
        public ValidationLevelType ValidationLevel { get; private set; }
        public abstract string ValidationFailureMessage { get; }
        public string ProxyAddress { get; private set; }
        public string ProxyUserName { get; private set; }
        public string ProxyPassword { get; private set; }

        protected BasePackageRule()
        {
            set_validation_level();

            // Since a proxy "might" be getting used, attempt to read in the environment variables
            // that control this.  These are only used during Unit Tests.  When actually running
            // package-validator, these are set within the app.config file.
            ProxyAddress = Environment.GetEnvironmentVariable("VALIDATOR_PROXY_ADDRESS");
            ProxyUserName = Environment.GetEnvironmentVariable("VALIDATOR_PROXY_USER_NAME");
            ProxyPassword = Environment.GetEnvironmentVariable("VALIDATOR_PROXY_PASSWORD");
        }

        private void set_validation_level()
        {
            var name = this.GetType().Name.to_lower();
            if (name.EndsWith("requirement"))
            {
                ValidationLevel = ValidationLevelType.Requirement;
            }
            else if (name.EndsWith("suggestion"))
            {
                ValidationLevel = ValidationLevelType.Suggestion;
            }
            else if (name.EndsWith("note"))
            {
                ValidationLevel = ValidationLevelType.Note;
            }
            else
            {
                ValidationLevel = ValidationLevelType.Guideline;
            }
        }

        public PackageValidationResult validate(IPackage package, string proxyAddress, string proxyUserName, string proxyPassword)
        {
            if (package == null) return new PackageValidationResult(false, "Unable to validate null package.", ValidationLevelType.Requirement);

            ProxyAddress = proxyAddress;
            ProxyUserName = proxyUserName;
            ProxyPassword = proxyPassword;

            var validationResults = is_valid(package);

            return new PackageValidationResult(
                validationResults.Validated,
                string.IsNullOrWhiteSpace(validationResults.ValidationFailureMessageOverride) ? ValidationFailureMessage : validationResults.ValidationFailureMessageOverride,
                ValidationLevel
                );
        }

        public abstract PackageValidationOutput is_valid(IPackage package);
    }
}
