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
    using infrastructure.rules;
    using NuGet;

    public class OperatingSystemIndexFilesAreNotPackagedRequirement : BasePackageRule
    {
        public override string ValidationFailureMessage
        {
            get
            {
                return
@"The package contains Operating System index files, .ds_store or thumbs.db. Please remove all index files from the package. [More...](https://github.com/chocolatey/package-validator/wiki/OperatingSystemIndexFilesIncluded)";
            }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var files = package.GetFiles().or_empty_list_if_null();
            return !files.Any(
                f => f.Path.to_lower().Contains("thumbs.db")
                     || f.Path.to_lower().Contains(".ds_store")
                        );
        }
    }
}
