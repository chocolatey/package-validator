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

    public class NotSilentTagIsBeingUsedNote : BasePackageRule
    {
        public override string ValidationFailureMessage { get { return
@"notSilent tag is being used. The reviewer will ensure this is being used appropriately. [More...](https://github.com/chocolatey/package-validator/wiki/notSilentTagIsBeingUsed)";
        }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            if (string.IsNullOrWhiteSpace(package.Tags)) return true;

            return !package.Tags.to_lower().Contains("notsilent");
        }
    }
}
