using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace chocolatey.package.validator.infrastructure.app.rules
{
    using System.IO;
    using infrastructure.rules;
    using NuGet;

    public class InstallScriptsShouldntUseInstallArgumentsRequirement : BasePackageRule
    {
        public override string ValidationFailureMessage { get { return 
@"Do not use the environment variables for accessing the Chocolatey Installation Arguments  Instead, use the passed in Package Parameters. [More...](https://github.com/chocolatey/package-validator/wiki/ScriptContainsUsageOfInstallationArguments)"; } }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var valid = true;

            var files = package.GetFiles().or_empty_list_if_null();

            foreach (var packageFile in files)
            {
                string extension = Path.GetExtension(packageFile.Path).to_lower();
                if (extension != ".ps1" && extension != ".psm1") continue;

                var contents = packageFile.GetStream().ReadToEnd().to_lower();

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
