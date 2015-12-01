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
    using NuGet;
    using infrastructure.rules;

    public class NuspecEnhancementsMissingSuggestion : BasePackageRule
    {
        public override string ValidationFailureMessage { get { return @"The nuspec has been enhanced to allow you to put in more information related to the software. Please consider adding one or more of the following to the nuspec, if available:
  * docsUrl - points to the location of the wiki or docs of the software
  * mailingListUrl - points to the forum or email list group for the software
  * bugTrackerUrl - points to the location where issues and tickets can be accessed
  * projectSourceUrl - points to the location of the underlying software source";
        }
        }

        protected override PackageValidationOutput is_valid(IPackage package)
        {
            return !string.IsNullOrWhiteSpace(package.DocsUrl.to_string())
                || !string.IsNullOrWhiteSpace(package.BugTrackerUrl.to_string()) 
                || !string.IsNullOrWhiteSpace(package.MailingListUrl.to_string())
                || !string.IsNullOrWhiteSpace(package.ProjectSourceUrl.to_string());
        }
    }
}
