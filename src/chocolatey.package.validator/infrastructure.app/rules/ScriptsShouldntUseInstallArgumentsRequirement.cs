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
    using infrastructure.rules;
    using NuGet;
    using utility;

    public class ScriptsShouldntUseInstallArgumentsRequirement : BasePackageRule
    {
        public override string ValidationFailureMessage
        {
            get
            {
                return @"Do not use the environment variables for accessing the Chocolatey Installation Arguments  Instead, use the passed in Package Parameters. [More...](https://github.com/chocolatey/package-validator/wiki/ScriptContainsUsageOfInstallationArguments)";
            }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var valid = true;

            var files = Utility.get_chocolatey_automation_scripts(package);
            foreach (var packageFile in files.or_empty_list_if_null())
            {
                var contents = packageFile.Value.to_lower();

                if (contents.Contains("installarguments") ||
                    contents.Contains("installerarguments") ||
                    contents.Contains("chocolateyinstallarguments"))
                {
                    valid = false;
                }
            }

            return valid;
        }
    }
}
