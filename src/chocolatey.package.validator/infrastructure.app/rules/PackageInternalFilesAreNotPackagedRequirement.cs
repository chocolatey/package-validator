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
    using NuGet;
    using infrastructure.rules;

    public class PackageInternalFilesAreNotPackagedRequirement : BasePackageRule
    {
        public override string ValidationFailureMessage { get { return
@"The package contains files that are normally not exposed as package files. Please remove them. [More...](https://github.com/chocolatey/package-validator/wiki/PackageInternalFilesIncluded) Possibilities include the following:
  * [Content_Types].xml
  * *.psmdcp
  * _rels\*
  * *.rels
"; } }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var files = package.GetFiles().or_empty_list_if_null();
            return !files.Any(
                f => f.Path.to_lower().Contains("[content_types].xml")
                     || f.Path.to_lower().Contains("%5bcontent_types%5d.xml")
                     || f.Path.to_lower().EndsWith(".psmdcp")
                     || f.Path.to_lower().EndsWith(".rels")
                     || f.Path.to_lower().Contains("_rels\\")
                        );
        }
    }
}
