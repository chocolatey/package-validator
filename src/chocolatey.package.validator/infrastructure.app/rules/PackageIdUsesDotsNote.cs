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

    public class PackageIdUsesDotsNote : BasePackageRule
    {
        public override string ValidationFailureMessage { get { return
@"The package id includes dots (.). Usually the package id is separated by '-' instead of dots (except in the case of *.install and *.portable). The reviewer will ensure this is not a new package. [More...](https://docs.chocolatey.org/en-us/community-repository/moderation/package-validator/rules/cpmr0061)";
        }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var packageId = package.Id.to_lower();
            if (
                   packageId.Contains(".portable")
                || packageId.Contains(".commandline")
                || packageId.Contains(".install")
                || packageId.Contains(".extension")
                || packageId.Contains(".template")
                || packageId.Contains(".powershell")
            ) return true;

            return !package.Id.Contains(".");
        }
    }
}
