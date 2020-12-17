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

    public class InstallScriptsWithInstallBinFileHaveUninstallScriptGuideline : BasePackageRule
    {
        public override string ValidationFailureMessage { get { return
@"This package uses Install/Generate/Add-BinFile without also calling Uninstall/Remove-BinFile in the chocolateyUninstall.ps1. Please correct this issue. [More...](https://docs.chocolatey.org/en-us/community-repository/moderation/package-validator/rules/cpmr0036)";
            }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var valid = true;
            var has_removals = false;

            var files = Utility.get_chocolatey_automation_scripts(package);
            foreach (var packageFile in files.or_empty_list_if_null())
            {
                var contents = packageFile.Value.to_lower();

                if (contents.Contains("install-binfile") || contents.Contains("generate-binfile") || contents.Contains("add-binfile")) valid = false;
                if (contents.Contains("remove-binfile") || contents.Contains("uninstall-binfile")) has_removals = true;
            }

            return valid || has_removals;
        }
    }
}
