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

namespace chocolatey.package.validator
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///   Extensions for IEnumerable
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        ///   Safe for each, returns an empty Enumerable if the list to iterate is null.
        /// </summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>
        ///   Source if not null; otherwise Enumerable.Empty&lt;<see cref="T" />&gt;
        /// </returns>
        public static IEnumerable<T> or_empty_list_if_null<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}
