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

    public class ScriptsDoNotContainChocoCommandsRequirement : BasePackageRule
    {
        private const ValidationLevelType VALIDATION_LEVEL = ValidationLevelType.Requirement;
        private const string VALIDATION_FAILURE_MESSAGE =
            @"Your script contains a choco command. This is not allowed. Perhaps you should take a dependency on a package? This could also flag a comment or message with the following words:
  * choco install
  * cinst
  * choco upgrade
  ";

        public ScriptsDoNotContainChocoCommandsRequirement()
            : base(VALIDATION_LEVEL, VALIDATION_FAILURE_MESSAGE)
        {
        }

        protected override PackageValidationOutput is_valid(IPackage package)
        {
            var valid = true;

            var files = package.GetFiles().or_empty_list_if_null();
            foreach (var packageFile in files)
            {
                string extension = Path.GetExtension(packageFile.Path).to_lower();
                if (extension != ".ps1" && extension != ".psm1") continue;

                var contents = packageFile.GetStream().ReadToEnd().to_lower();

                // leaving out choco uninstall - this is usually found in messages when uninstalling meta packages (especially with dtgm).

                if (contents.Contains("choco install") || contents.Contains("cinst")
                    || contents.Contains("choco upgrade")) valid = false;
            }

            return valid;
        }
    }
}
