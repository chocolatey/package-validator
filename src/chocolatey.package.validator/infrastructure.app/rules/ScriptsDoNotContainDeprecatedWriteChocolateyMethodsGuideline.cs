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
    using System.IO;
    using NuGet;
    using infrastructure.rules;
    using utility;

    public class ScriptsDoNotContainDeprecatedWriteChocolateyMethodsGuideline : BasePackageRule
    {
        public override string ValidationFailureMessage { get { return
                    @"Write-ChocolateySuccess/Write-ChocolateyFailure have been deprecated, but have been found in the package's automation scripts. Remove the usage of those two functions. If there is an error, throw the error. [More...](https://github.com/chocolatey/package-validator/wiki/ScriptsDoNotContainDeprecatedWriteChocolateyMethods) This can also give a false positive for commented code with the words:
  * Write-ChocolateySuccess
  * Write-ChocolateyFailure";
            }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var valid = true;

            var files = Utility.get_chocolatey_automation_scripts(package);
            foreach (var packageFile in files.or_empty_list_if_null())
            {
                var contents = packageFile.Value.to_lower();

                if (contents.Contains("write-chocolateysuccess") || contents.Contains("write-chocolateyfailure")) valid = false;
            }

            return valid;
        }
    }
}
