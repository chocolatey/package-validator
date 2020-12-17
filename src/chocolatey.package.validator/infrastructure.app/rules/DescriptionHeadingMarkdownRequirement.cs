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

    public class DescriptionHeadingMarkdownRequirement : BasePackageRule
    {
        private const string HeadingRegexPattern = @"^(#+)([^\s#].*)$";

        public override string ValidationFailureMessage
        {
            get
            {
                return
@"Package Description should not contain invalid Markdown Headings. [More...](https://docs.chocolatey.org/en-us/community-repository/moderation/package-validator/rules/cpmr0030)";
            }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            if (package.Description == null)
            {
                return true;
            }

            var regex = new Regex(HeadingRegexPattern, RegexOptions.Multiline);

            return !regex.IsMatch(package.Description);
        }
    }
}
