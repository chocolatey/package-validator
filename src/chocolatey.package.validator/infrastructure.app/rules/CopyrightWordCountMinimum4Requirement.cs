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

namespace chocolatey.package.validator.infrastructure.app.rules
{
    using System.Linq;
    using NuGet;
    using infrastructure.rules;

    public class CopyrightWordCountMinimum4Requirement : BasePackageRule
    {
        private const string VALIDATION_FAILURE_MESSAGE = "If you are going to fill in copyright, please use more than 4 characters.";

        public CopyrightWordCountMinimum4Requirement()
            : base(VALIDATION_FAILURE_MESSAGE)
        {
        }

        protected override PackageValidationOutput is_valid(IPackage package)
        {
            if (string.IsNullOrWhiteSpace(package.Copyright)) return true;

            return package.Copyright.to_string().Count() > 4;
        }
    }
}
