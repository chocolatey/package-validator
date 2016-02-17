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
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Management.Automation;
    using infrastructure.rules;
    using NuGet;

    public class InstallScriptsShouldContainValidPowerShellRequirement : BasePackageRule
    {
        public override string ValidationFailureMessage
        {
            get
            {
                return
@"Installation Scripts should contain valid PowerShell. [More...](https://github.com/chocolatey/package-validator/wiki/InstallationScriptsShouldContainValidPowerShell)";
            }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var valid = true;

            foreach (var file in package.GetFiles().or_empty_list_if_null())
            {
                string extension = Path.GetExtension(file.Path).to_lower();
                if (extension != ".ps1" && extension != ".psm1") continue;

                var contents = file.GetStream().ReadToEnd().to_lower();

                Collection<PSParseError> errors = null;
                var tokens = PSParser.Tokenize(contents, out errors);

                if (errors.Count() != 0) valid = false;
            }

            return valid;
        }
    }
}