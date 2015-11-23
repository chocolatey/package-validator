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
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;

    public static class NameValueCollectionExtensions
    {
        public static string @join(this NameValueCollection collection, string separator)
        {
            var collectionString = new StringBuilder();

            var items = collection.AllKeys.SelectMany(
                collection.GetValues,
                (k, v) => new
                {
                    key = k,
                    value = v
                });
            foreach (var item in items)
            {
                collectionString.AppendFormat("{0}={1}{2}", item.key, item.value, separator);
            }

            collectionString.Remove(collectionString.Length - 1, 1);

            return collectionString.ToString();
        }
    }
}
