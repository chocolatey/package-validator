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
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using infrastructure.rules;
    using NuGet;

    public class NuspecDoesNotContainEmailRequirement : BasePackageRule
    {
        // http://www.regular-expressions.info/email.html
        private const string EmailRegexPattern = @"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}";

        public override string ValidationFailureMessage
        {
            get
            {
                return
@"Email address should not be used in any field of the nuspec file. [More...](https://github.com/chocolatey/package-validator/wiki/NuspecDoesNotContainEmail)";
            }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var properties = package.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var regex = new Regex(EmailRegexPattern);

            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(package, null).to_string().to_lower();

                // NOTE: Not overally happy with this!
                // Want to be able to each of the values in the IEnumerable, to then check if there is
                // an email contained within one of them.
                if (property.PropertyType.FullName.Contains("IEnumerable`1[[System.String"))
                {
                    propertyValue = string.Join(",", ((IEnumerable<string>)property.GetValue(package, null)).or_empty_list_if_null()).to_lower();
                }

                if (regex.IsMatch(propertyValue))
                {
                    return false;
                }
            }

            return true;
        }
    }
}