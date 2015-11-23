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

namespace chocolatey.package.validator.host.infrastructure.registration
{
    using System;
    using System.Collections.Generic;
    using SimpleInjector;

    public static class SimpleInjectorExtensions
    {
        public static void register_all<TService>(this Container container, IEnumerable<Func<TService>> instanceCreators)
            where TService : class
        {
            foreach (var instanceCreator in instanceCreators.or_empty_list_if_null())
            {
                container.RegisterSingle(typeof(TService), instanceCreator);
            }

            container.RegisterAll<TService>(typeof(TService));
        }
    }
}
