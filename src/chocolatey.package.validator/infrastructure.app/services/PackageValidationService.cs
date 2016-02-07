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

namespace chocolatey.package.validator.infrastructure.app.services
{
    using System;
    using System.Collections.Generic;
    using NuGet;
    using infrastructure.rules;
    using locators;
    using IPackageRule = infrastructure.rules.IPackageRule;

    public class PackageValidationService : IPackageValidationService
    {
        private readonly ITypeLocator _typeLocator;
        private IList<IPackageRule> _rules = new List<IPackageRule>();

        public PackageValidationService(ITypeLocator typeLocator)
        {
            _typeLocator = typeLocator;

            load_rules();
        }

        public void load_rules()
        {
            _rules.Clear();
            foreach (var packageRule in _typeLocator.get_types_that_inherit_or_implement<IPackageRule>().or_empty_list_if_null())
            {
                var rule = Activator.CreateInstance(packageRule) as IPackageRule;
                if (rule != null)
                {
                    _rules.Add(rule);
                }
            }
        }

        public IEnumerable<IPackageRule> Rules { get { return _rules; } }

        public IEnumerable<PackageValidationResult> validate_package(IPackage package)
        {
            IList<PackageValidationResult> results = new List<PackageValidationResult>();

            if (package == null)
            {
                results.Add(new PackageValidationResult(false, "Something went very wrong. There is no package to validate. Please contact the site admins for next steps.", ValidationLevelType.Requirement));
            }
            else if (package.Id.is_equal_to("chocolatey"))
            {
                results.Add(new PackageValidationResult(true, "The chocolatey package skips the validation as it should be doing most things that would fail validation.",ValidationLevelType.Note));
            }
            else
            {
                foreach (var rule in Rules.or_empty_list_if_null())
                {
                    results.Add(rule.validate(package));
                }
            }

            return results;
        }
    }
}
