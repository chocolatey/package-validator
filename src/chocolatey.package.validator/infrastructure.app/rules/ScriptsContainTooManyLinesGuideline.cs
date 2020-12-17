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
    using System;
    using System.IO;
    using infrastructure.rules;
    using NuGet;
    using utility;

    public class ScriptsContainTooManyLinesGuideline : BasePackageRule
    {
@"Chocolatey Packages are meant to be thin wrappers on top of the native installation packages, as a result, there is an expectation that the installation scripts will be less than 100 lines. Can the automation scripts be simplified? [More...](https://github.com/chocolatey/package-validator/wiki/ScriptsContainTooManyLines)";
        public override string ValidationFailureMessage { get { return
        }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var valid = true;

            var files = Utility.get_chocolatey_automation_scripts(package);
            foreach (var packageFile in files.or_empty_list_if_null())
            {
                var numberOfLines = packageFile.Value.Split(new []{Environment.NewLine}, StringSplitOptions.None).Length;

                valid = numberOfLines < 100;
            }

            return valid;
        }
    }
}
