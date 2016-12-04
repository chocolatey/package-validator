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
    using System.Management.Automation;
    using infrastructure.rules;
    using NuGet;
    using utility;

    public class ScriptsDoNotContainChocoCommandsRequirement : BasePackageRule
    {
        public override string ValidationFailureMessage
        {
            get
            {
                return @"The package script contains a choco command. This is not allowed. Perhaps there should be a dependency on a package? [More...](https://github.com/chocolatey/package-validator/wiki/ScriptsDoNotContainChocoCommands)";
            }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var valid = true;

            var files = Utility.get_chocolatey_scripts_tokens(package);
            foreach (var packageFile in files.or_empty_list_if_null())
            {
                var tokens = packageFile.Value;
                var flaggingCalls = tokens.Where(
                    p => p.Type == PSTokenType.Command && (
                        p.Content.to_lower().is_equal_to("choco") ||
                        p.Content.to_lower().is_equal_to("cinst") ||
                        p.Content.to_lower().is_equal_to("chocolatey")
                        )
                    );

                if (flaggingCalls.Count() != 0) valid = false;
            }

            return valid;
        }
    }
}
