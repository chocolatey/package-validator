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
    using NuGet;
    using infrastructure.rules;
    using utility;

    public class MailingListUrlValidRequirement : BasePackageRule
    {
        public override string ValidationFailureMessage { get { return
@"The MailingListUrl element in the nuspec file should be a valid Url. Please correct this [More...](https://docs.chocolatey.org/en-us/community-repository/moderation/package-validator/rules/cpmr0031)";
        }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var valid = true;

            if (package.MailingListUrl != null)
            {
                valid = Utility.url_is_valid(package.MailingListUrl, ProxyAddress, ProxyUserName, ProxyPassword);
            }

            return valid;
        }
    }
}
