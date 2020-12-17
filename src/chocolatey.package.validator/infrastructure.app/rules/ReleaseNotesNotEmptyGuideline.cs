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

    public class ReleaseNotesNotEmptyGuideline : BasePackageRule
    {
@"Release Notes (releaseNotes) are a short description of changes in each version of a package. Please include releasenotes in the nuspec. **NOTE:** To prevent the need to continually update this field, providing a URL to an external list of Release Notes is perfectly acceptable. [More...](https://github.com/chocolatey/package-validator/wiki/ReleaseNotesNotEmpty)";
        public override string ValidationFailureMessage { get { return
        }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            return !string.IsNullOrWhiteSpace(package.ReleaseNotes.to_string());
        }
    }
}
