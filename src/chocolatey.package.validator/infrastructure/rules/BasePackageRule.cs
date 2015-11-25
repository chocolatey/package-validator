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
    using NuGet;

    public abstract class BasePackageRule : IPackageRule
    {
        public ValidationLevelType ValidationLevel { get; private set; }
        public abstract string ValidationFailureMessage { get; }

        protected BasePackageRule()
        {
            set_validation_level();
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

        public PackageValidationResult validate(IPackage package)
        {
            if (package == null) return new PackageValidationResult(false, "Unable to validate null package.", ValidationLevelType.Requirement);

            var validationResults = is_valid(package);

            return new PackageValidationResult(
                validationResults.Validated,
                string.IsNullOrWhiteSpace(validationResults.ValidationFailureMessageOverride) ? ValidationFailureMessage : validationResults.ValidationFailureMessageOverride,
                ValidationLevel
                );
        }

        protected abstract PackageValidationOutput is_valid(IPackage package);
    }
}
