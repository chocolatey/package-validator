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

namespace chocolatey.package.validator.infrastructure.locators
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class TypeLocator : ITypeLocator
    {
        public IEnumerable<Type> get_types_that_inherit_or_implement<T>()
        {
            return get_types_that_inherit_or_implement<T>(GetType().Assembly);
        }

        public IEnumerable<Type> get_types_that_inherit_or_implement<T>(params Assembly[] assemblies)
        {
            IList<Type> list = new List<Type>();

            if (assemblies == null || assemblies.Length == 0)
            {
               throw new ApplicationException("TypeLocator cannot locate types without assemblies");
            }

            foreach (var assembly in assemblies.or_empty_list_if_null())
            {
                foreach (var t in assembly.GetTypes())
                {
                    if (!typeof(T).IsAssignableFrom(t) || t.IsInterface || t.IsAbstract) continue;

                    list.Add(t);
                }
            }
            return list;
        }
    }
}
