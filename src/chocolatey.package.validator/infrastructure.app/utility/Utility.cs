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

            if (url.Scheme == "mailto")
            {
                // mailto links are not expected/allowed, therefore immediately fail with no further processing
                return false;
            }

            if (!url.Scheme.StartsWith("http"))
            {
                // Currently we can only validate http/https URL's, therefore simply return true for any others.
                return true;
            }

            try
            {
                var request = (HttpWebRequest) WebRequest.Create(url);
                var cookieContainer = new CookieContainer();

                request.CookieContainer = cookieContainer;
                request.Timeout = 30000;
                //This would allow 301 and 302 to be valid as well
                request.AllowAutoRedirect = true;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.130 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                request.Headers["Sec-Fetch-Mode"] = "navigate";
                request.Headers["Sec-Fetch-Dest"] = "document";
                request.Headers["Sec-Fetch-Site"] = "cross-site";
                request.Headers["Sec-Fetch-User"] = "?1";

                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == System.Net.WebExceptionStatus.ProtocolError && ex.Message == "The remote server returned an error: (403) Forbidden." && ex.Response.Headers["Server"] == "AkamaiGHost")
                {
                    "package-validator".Log().Warn("Error validating Url {0} - {1}", url.ToString(), ex.Message);
                    "package-validator".Log().Warn("Since this is likely due to the fact that the server is using Akamai, which expects request headers to be in a VERY specific order and case, this URL will be marked as valid for the time being.");
                    "package-validator".Log().Warn("This check was put in place as a result of this issue: https://github.com/chocolatey/package-validator/issues/225");
                    return true;
                }

                if (ex.Status == System.Net.WebExceptionStatus.ProtocolError && ex.Message == "The remote server returned an error: (403) Forbidden." && ex.Response.Headers["Server"] == "cloudflare")
                {
                    "package-validator".Log().Warn("Error validating Url {0} - {1}", url.ToString(), ex.Message);
                    "package-validator".Log().Warn("Since this is likely due to the fact that the server is using Cloudflare, is sometimes popping up a Captcha which needs to be solved, obviously not possible by package-validator.");
                    "package-validator".Log().Warn("This check was put in place as a result of this issue: https://github.com/chocolatey/package-validator/issues/229");
                    return true;
                }

                if (ex.Status == WebExceptionStatus.SecureChannelFailure || (ex.Status == WebExceptionStatus.UnknownError && ex.Message == "The SSL connection could not be established, see inner exception. Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host."))
                {
                    "package-validator".Log().Warn("Error validating Url {0} - {1}", url.ToString(), ex.Message);
                    "package-validator".Log().Warn("Since this is likely due to missing Ciphers on the machine hosting package-validator, this URL will be marked as valid for the time being.");
                    return true;
                }

                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Message == "The remote server returned an error: (503) Server Unavailable.")
                {
                    "package-validator".Log().Warn("Error validating Url {0} - {1}", url.ToString(), ex.Message);
                    "package-validator".Log().Warn("This could be due to Cloudflare DDOS protection acting in front of the site, or another valid reason, as such, this URL will be marked as valid for the time being.");
                    return true;
                }

                "package-validator".Log().Warn("Error validating Url {0} - {1}", url.ToString(), ex.Message);
                return false;
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
