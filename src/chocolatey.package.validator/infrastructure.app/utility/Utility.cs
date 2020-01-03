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
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Management.Automation;
    using System.Net;
    using System.Text.RegularExpressions;
    using chocolatey.package.validator.infrastructure.app.registration;
    using NuGet;

    public class Utility
    {
        private static readonly Regex _powerShellScriptRegex = new Regex(@"['""]?(\S+\.psm?1)", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.IgnoreCase);

        private static readonly Regex _UrlRegex = new Regex(@"\b((?:https?://|www\.)\S+)\b", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.IgnoreCase);

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

                    // add any PowerShell scripts that were referenced
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

        public static IDictionary<IPackageFile, ICollection<PSToken>> get_chocolatey_scripts_tokens(IPackage package)
        {
            var scriptsAsTokens = new Dictionary<IPackageFile, ICollection<PSToken>>();
            var scripts = get_chocolatey_automation_scripts(package);
            foreach (var script in scripts.or_empty_list_if_null())
            {
                Collection<PSParseError> errors = null;
                ICollection<PSToken> tokens = PSParser.Tokenize(script.Value, out errors);
                scriptsAsTokens.Add(script.Key, tokens);
            }

            return scriptsAsTokens;
        }

        public static bool package_has_binaries(IEnumerable<IPackageFile> packageFiles)
        {
            return packageFiles.Any(
                f =>
                    StringExtensions.to_lower(f.Path).EndsWith(".exe") ||
                    StringExtensions.to_lower(f.Path).EndsWith(".msi") ||
                    StringExtensions.to_lower(f.Path).EndsWith(".msu") ||
                    StringExtensions.to_lower(f.Path).EndsWith(".msp") ||
                    StringExtensions.to_lower(f.Path).EndsWith(".dll") ||
                    StringExtensions.to_lower(f.Path).EndsWith(".7z")  ||
                    StringExtensions.to_lower(f.Path).EndsWith(".zip") ||
                    StringExtensions.to_lower(f.Path).EndsWith(".gz")  ||
                    StringExtensions.to_lower(f.Path).EndsWith(".tar") ||
                    StringExtensions.to_lower(f.Path).EndsWith(".rar") ||
                    StringExtensions.to_lower(f.Path).EndsWith(".sfx") ||
                    StringExtensions.to_lower(f.Path).EndsWith(".iso") ||
                    StringExtensions.to_lower(f.Path).EndsWith(".dmg") ||
                    StringExtensions.to_lower(f.Path).EndsWith(".so")  ||
                    StringExtensions.to_lower(f.Path).EndsWith(".jar")

            );
        }

        /// <summary>
        ///   Tries to validate an URL
        /// </summary>
        /// <param name="url">Uri object</param>
        public static bool url_is_valid(Uri url)
        {
            if (url == null)
            {
                return true;
            }

            if (!url.Scheme.StartsWith("http"))
            {
                // Currently we can only validate http/https URL's, therefore simply return true for any others.
                return true;
            }

            try
            {
                // Use TLS1.2, TLS1.1, TLS1.0, SSLv3
                SecurityProtocol.set_protocol();

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Timeout = 15000;
                //This would allow 301 and 302 to be valid as well
                request.AllowAutoRedirect = true;
                request.UserAgent = "{0}/{1}".format_with(ApplicationParameters.Name, ApplicationParameters.FileVersion);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                "package-validator".Log().Warn("Error validating Url {0} - {1}", url.ToString(), ex.Message);
                return false;
            }
        }

        /// <summary>
        ///   Fetches all URLs from a string, and validates if the url results in a 200 OK
        /// </summary>
        /// <param name="contents">String that might contain URL</param>
        public static bool all_urls_are_valid(String contents)
        {
            var result = true;

            try
            {
                var matches = _UrlRegex.Matches(contents);
                foreach (Match match in matches)
                {
                    var url = match.Groups[1].Value;
                    var uri = new Uri(url);
                    var urlResult = url_is_valid(uri);
                    if (!urlResult)
                    {
                        result = urlResult;
                    }
                }
            }
            catch (Exception ex)
            {
                "package-validator".Log().Warn("Error when searching for urls - {0}".format_with(ex.Message));
                return false;
            }

            return result;
        }

    }
}
