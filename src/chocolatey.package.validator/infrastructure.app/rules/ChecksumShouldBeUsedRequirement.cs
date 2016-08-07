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
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Management.Automation;
    using chocolatey.package.validator.infrastructure.rules;
    using NuGet;

    public class ChecksumShouldBeUsedRequirement : BasePackageRule
    {
        public override string ValidationFailureMessage { get { return
@"To provide additional security for package installations, checkbums for downloaded binaries should be provided.  Your script does not have this, and it will need to be changed. [More...](https://github.com/chocolatey/package-validator/wiki/ChecksumShouldBeUsed)";
        }
        }

        public override PackageValidationOutput is_valid(IPackage package)
        {
            var valid = true;

            foreach (var file in package.GetFiles().or_empty_list_if_null())
            {
                string extension = Path.GetExtension(file.Path).to_lower();
                if (extension != ".ps1" && extension != ".psm1") continue;

                var contents = file.GetStream().ReadToEnd().to_lower();

                Collection<PSParseError> errors = null;
                var tokens = PSParser.Tokenize(contents, out errors);

                var methodsThatRequireChecksums = tokens.Where(
                    p => p.Type == PSTokenType.Command && (
                        p.Content.to_lower().Contains("get-chocolateywebfile") ||
                        p.Content.to_lower().Contains("install-chocolateypackage") ||
                        p.Content.to_lower().Contains("install-chocolateypowershellcommand") ||
                        p.Content.to_lower().Contains("install-chocolateyvsixpackage") ||
                        p.Content.to_lower().Contains("install-chocolateyzippackage"))
                    );

                if (methodsThatRequireChecksums.Any())
                {
                    // find all variables and parameters, and check to see if any of them are named checksum
                    var variables = tokens.Where(p => p.Type == PSTokenType.Variable && p.Content.to_lower() == "checksum");
                    var parameters = tokens.Where(p => p.Type == PSTokenType.CommandParameter && p.Content.to_lower() == "checksum");

                    if (!variables.Any() && !parameters.Any())
                    {
                        valid = false;
                    }
                }
            }

            return valid;
        }
    }
}