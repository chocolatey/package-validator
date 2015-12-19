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
    using System.Reflection;
    using chocolatey.package.validator.infrastructure.rules;
    using NuGet;

    public class NuspecDoesNotContainTemplatedValuesRequirement : BasePackageRule
    {
        public override string ValidationFailureMessage
        {
            get
            {
                return
@"Nuspec file contains templated values which should be removed. [More...](https://github.com/chocolatey/package-validator/wiki/NuspecDoesNotContainTemplatedValues)";
            }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var properties = package.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(package, null).to_string().to_lower();
                if (propertyValue.Contains("__replace")
                    || propertyValue.Contains("space_separated")
                    || propertyValue.Contains("tag1")
                )
                {
                    return false;
                }
            }

            return true;
        }
    }
}
