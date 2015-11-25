﻿// Copyright © 2015 - Present RealDimensions Software, LLC
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

namespace chocolatey.package.validator.infrastructure.app.rules
{
    using NuGet;
    using infrastructure.rules;

    public class DescriptionRequirement : BasePackageRule
    {
        private const ValidationLevelType VALIDATION_LEVEL = ValidationLevelType.Requirement;
        private const string VALIDATION_FAILURE_MESSAGE = "Description (description) in the nuspec file is required.";

        public DescriptionRequirement()
            : base(VALIDATION_LEVEL, VALIDATION_FAILURE_MESSAGE)
        {
        }

        protected override PackageValidationOutput is_valid(IPackage package)
        {
            return !string.IsNullOrWhiteSpace(package.Description);
        }
    }
}