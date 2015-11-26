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
    using System.Text.RegularExpressions;
    using infrastructure.rules;
    using NuGet;

    public class IconUrlNotSameDomainAsProjectUrlDomainOrRawGitGuideline : BasePackageRule
    {
        public override string ValidationFailureMessage { get { return "The package IconUrl should ideally come from the same domain name as the Project Url, or hosted on the RawGit CDN.  **NOTE:** For further information on how to setup your icon with a RawGit CDN URL, please visit this [article](https://github.com/chocolatey/choco/wiki/CreatePackages#package-icon-guidelines)."; } }

        protected override PackageValidationOutput is_valid(IPackage package)
        {
            if (package.IconUrl == null) return true;

            var iconUrlDomain = this.get_domain_from_host(package.IconUrl.Host);

            // Since there is an iconUrl, but no Project Url, the check has to return false
            // since the domains are definitely not the same
            if (package.ProjectUrl == null) return false;

            var projectUrlDomain = this.get_domain_from_host(package.ProjectUrl.Host);

            return iconUrlDomain == projectUrlDomain || iconUrlDomain == "rawgit.com";
        }

        private string get_domain_from_host(string host)
        {
            // Use Regular Expression to extract the Domain Name, from the Uri Host
            // Taken from example shown here http://stackoverflow.com/a/17091145/671491
            var match = Regex.Match(host, "([^.]+\\.[^.]{1,3}(\\.[^.]{1,3})?)$");
            return match.Groups[1].Success ? match.Groups[1].Value : string.Empty;
        }
    }
}
