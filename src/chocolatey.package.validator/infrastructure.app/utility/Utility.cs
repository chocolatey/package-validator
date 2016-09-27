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

namespace chocolatey.package.validator.infrastructure.app.utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using NuGet;

    public class Utility
    {
        private static readonly Regex _powerShellScriptRegex = new Regex(@"['""]?(\S+\.psm?1)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.IgnoreCase);

        public static IDictionary<IPackageFile, string> get_chocolatey_automation_scripts(IPackage package)
        {
            var files = package.GetFiles().Where(f => f.Path.to_lower().EndsWith(".ps1") || f.Path.to_lower().EndsWith(".psm1")).or_empty_list_if_null();
            var automationScripts = new Dictionary<IPackageFile, string>();

            foreach (var packageFile in files.or_empty_list_if_null())
            {
                var path = packageFile.Path.to_lower();
                if (path.EndsWith("chocolateyinstall.ps1") || path.EndsWith("chocolateybeforemodify.ps1") || path.EndsWith("chocolateyuninstall.ps1"))
                {
                    var contents = packageFile.GetStream().ReadToEnd();
                    automationScripts.Add(packageFile, contents);
                   
                    try
                    {
                        var matches = _powerShellScriptRegex.Matches(contents);
                        foreach (Match match in matches)
                        {
                            var scriptFileMatch = match.Groups[1].Value;
                            var scriptFile = files.FirstOrDefault(f => f.Path.to_lower().EndsWith(scriptFileMatch));
                            if (scriptFile != null && !automationScripts.ContainsKey(scriptFile))
                            {
                                automationScripts.Add(packageFile, scriptFile.GetStream().ReadToEnd());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        "package-validator".Log().Warn("Error when searching for included powershell scripts - {0}".format_with(ex.Message));
                    }
                }
            }

            return automationScripts;
        }
    }
}
