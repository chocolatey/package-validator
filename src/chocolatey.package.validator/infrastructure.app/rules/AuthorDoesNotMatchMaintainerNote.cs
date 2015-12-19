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
    using chocolatey.package.validator.infrastructure.rules;
    using NuGet;

    public class AuthorDoesNotMatchMaintainerNote : BasePackageRule
    {
        public override string ValidationFailureMessage { get { return
            @"The package maintainer field (owners) matches the software author field (authors) in the nuspec.  The reviewer will ensure that the package maintainer is also the software author. [More...](https://github.com/chocolatey/package-validator/wiki/AuthorDoesNotMatchMaintainer)";
        }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var owners = string.Join(",", package.Owners.or_empty_list_if_null()).to_lower();
            var authors = string.Join(",", package.Authors.or_empty_list_if_null()).to_lower();

            return owners != authors;
        }
    }
}