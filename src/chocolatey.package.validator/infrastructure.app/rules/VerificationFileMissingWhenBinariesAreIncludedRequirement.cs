// Copyright � 2015 - Present RealDimensions Software, LLC
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
    using infrastructure.rules;
    using NuGet;
    using utility;

    public class VerificationFileMissingWhenBinariesAreIncludedRequirement : BasePackageRule
    {
        public override string ValidationFailureMessage
        {
            get { return @"Binary files (.exe, .msi, .zip) have been included without including a VERIFICATION.txt file. This file is required when including binaries  [More...](https://github.com/chocolatey/package-validator/wiki/VerificationFileMissing)"; }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            //search for VERIFICATION, VERIFICATION.txt, VERIFICATION.md
            var packageFiles = package.GetFiles().or_empty_list_if_null().ToList();

            if (packageFiles.Any(
                f =>
                    StringExtensions.to_lower(f.Path).EndsWith("verification.txt") ||
                    StringExtensions.to_lower(f.Path).EndsWith("verification") ||
                    StringExtensions.to_lower(f.Path).EndsWith("verification.md")
            ))
            {
                return true;
            }

            return !Utility.package_has_binaries(packageFiles);
        }
    }
}
