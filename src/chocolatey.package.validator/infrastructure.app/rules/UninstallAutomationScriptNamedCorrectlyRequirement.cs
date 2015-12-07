namespace chocolatey.package.validator.infrastructure.app.rules
{
    using System.Linq;
    using NuGet;
    using infrastructure.rules;

    public class UninstallAutomationScriptNamedCorrectlyRequirement : BasePackageRule
    {
        public override string ValidationFailureMessage { get { return @"The uninstall script should be named chocolateyUninstall.ps1 and be found in the tools folder. Your script is named incorrectly and will need to be renamed."; } }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var files = package.GetFiles().or_empty_list_if_null();
            var hasBadInstallScripts = files.Any(f => StringExtensions.to_lower(f.Path).Contains("uninstall.ps1")) && !files.Any(f => f.Path.to_lower().Contains("chocolateyuninstall.ps1"));

            return !hasBadInstallScripts;
        }
    }
}