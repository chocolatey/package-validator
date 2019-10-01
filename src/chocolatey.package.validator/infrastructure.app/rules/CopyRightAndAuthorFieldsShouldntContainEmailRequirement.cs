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
    using System.Text.RegularExpressions;
    using NuGet;
    using infrastructure.rules;

    public class CopyrightAndAuthorFieldsShouldntContainEmailRequirement : BasePackageRule
    {
        // http://www.regular-expressions.info/email.html
        private const string EmailRegexPattern = @"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}";

        public override string ValidationFailureMessage
        {
            get
            {
                return
@"Email address should not be used in the Author and Copyright fields of the nuspec file. [More...](https://github.com/chocolatey/package-validator/wiki/NuspecDoesNotContainEmail)";
            }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            if (string.IsNullOrWhiteSpace(package.Copyright) && package.Authors == null) return true;

            var regex = new Regex(EmailRegexPattern);

            return !regex.IsMatch(package.Copyright.to_lower()) && !regex.IsMatch(package.Authors.Join(" ").to_lower());
        }
    }
}