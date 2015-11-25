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

    public class IconUrlMissingGuideline : BasePackageRule
    {
        public override string ValidationFailureMessage { get { return "The iconUrl should be added if there is one. Please correct this in the nuspec, if applicable.  **NOTE:** We really want to see the IconUrl being used, and some moderators want to see it being used properly, using the rawgit CDN. For further information on how to setup your icon with a Rawgit CDN URL, please visit this [article](https://github.com/chocolatey/choco/wiki/CreatePackages#package-icon-guidelines)."; } }

        protected override PackageValidationOutput is_valid(IPackage package)
        {
            return package.IconUrl != null;
        }
    }
}
