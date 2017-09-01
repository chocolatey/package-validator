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
    using NuGet;
    using infrastructure.rules;

    public class PackageIdTooLongWithNoDashesNote : BasePackageRule
    {
        public override string ValidationFailureMessage
        {
            get
            {
                return
@"If this is a brand new package that has never went through approval before, the reviewer will suggest a change to the package id. [More...](https://github.com/chocolatey/package-validator/wiki/PackageIdTooLongWithNoDashes)";
            }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var packageId = package.Id.to_lower();

            packageId = packageId.Replace(".extension", string.Empty)
                                 .Replace(".template", string.Empty)
                                 .Replace(".install", string.Empty)
                                 .Replace(".portable", string.Empty)
                                 .Replace(".powershell", string.Empty)
                                 ;

            if (packageId.Length < 20) return true;

            return packageId.Contains("-");
        }
    }
}
