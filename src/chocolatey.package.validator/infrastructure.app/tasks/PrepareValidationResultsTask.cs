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

namespace chocolatey.package.validator.infrastructure.app.tasks
{
    using System;
    using System.Linq;
    using System.Text;
    using infrastructure.messaging;
    using infrastructure.rules;
    using infrastructure.tasks;
    using messaging;

    public class PrepareValidationResultsTask : ITask
    {
        private IDisposable _subscription;

        public void initialize()
        {
            _subscription = EventManager.subscribe<PackageValidationResultMessage>(process_results, null, null);
            this.Log().Info(() => "{0} is now ready and waiting for {1}".format_with(GetType().Name, typeof(PackageValidationResultMessage).Name));
        }

        public void shutdown()
        {
            if (_subscription != null) _subscription.Dispose();
        }

        private void process_results(PackageValidationResultMessage message)
        {
            var resultsMessage = new StringBuilder();

            var failedRequirements = message.ValidationResults.Where(r => r.Validated == false && r.ValidationLevel == ValidationLevelType.Requirement);
            if (failedRequirements.Count() != 0)
            {
                this.Log().Info("{0} v{1} failed validation.".format_with(message.PackageId,message.PackageVersion));
                resultsMessage.Append("##### Requirements{0}".format_with(Environment.NewLine));
                resultsMessage.Append("Requirements represent the minimum quality of a package that is acceptable. When a package version has failed requirements, the package version requires fixing and/or response by the maintainer. Provided a Requirement has flagged correctly, it ***must*** be fixed before the package version can be approved. The exact same version should be uploaded during moderation review.{0}{0}".format_with(Environment.NewLine));
            }
            foreach (var failedRequirement in failedRequirements.or_empty_list_if_null())
            {
                resultsMessage.Append("* " + failedRequirement.ValidationFailureMessage + Environment.NewLine);
            }

            var flaggedGuidelines = message.ValidationResults.Where(r => r.Validated == false && r.ValidationLevel == ValidationLevelType.Guideline);
            if (flaggedGuidelines.Count() != 0)
            {
                resultsMessage.Append("{0}##### Guidelines{1}".format_with(resultsMessage.Length ==0 ? string.Empty : Environment.NewLine, Environment.NewLine));
                resultsMessage.Append("Guidelines are strong suggestions that improve the quality of a package version. These are considered something to fix for next time to increase the quality of the package. Over time Guidelines can become Requirements. A package version can be approved without addressing Guideline comments but will reduce the quality of the package.{0}{0}".format_with(Environment.NewLine));
            }
            foreach (var flaggedGuideline in flaggedGuidelines.or_empty_list_if_null())
            {
                resultsMessage.Append("* " + flaggedGuideline.ValidationFailureMessage + Environment.NewLine);
            }
            
            var flaggedSuggestions = message.ValidationResults.Where(r => r.Validated == false && (r.ValidationLevel == ValidationLevelType.Suggestion));
            if (flaggedSuggestions.Count() != 0)
            {
                resultsMessage.Append("{0}##### Suggestions{1}".format_with(resultsMessage.Length == 0 ? string.Empty : Environment.NewLine, Environment.NewLine));
                resultsMessage.Append("Suggestions are either newly introduced items that will later become Guidelines or items that are don't carry enough weight to become a Guideline. Either way they should be considered. A package version can be approved without addressing Suggestion comments.{0}{0}".format_with(Environment.NewLine));
            }
            foreach (var flaggedSuggestion in flaggedSuggestions.or_empty_list_if_null())
            {
                resultsMessage.Append("* " + flaggedSuggestion.ValidationFailureMessage + Environment.NewLine);
            }
            
            var flaggedNotes = message.ValidationResults.Where(r => r.Validated == false && (r.ValidationLevel == ValidationLevelType.Note));
            if (flaggedNotes.Count() != 0)
            {
                resultsMessage.Append("{0}##### Notes{1}".format_with(resultsMessage.Length == 0 ? string.Empty : Environment.NewLine, Environment.NewLine));
                resultsMessage.Append("Notes typically flag things for both you and the reviewer to go over. Sometimes this is the use of things that may or may not be necessary given the constraints of what you are trying to do and/or are harder for automation to flag for other reasons. Items found in Notes might be Requirements depending on the context. A package version can be approved without addressing Note comments.{0}{0}".format_with(Environment.NewLine));
            }
            foreach (var flaggedNote in flaggedNotes.or_empty_list_if_null())
            {
                resultsMessage.Append("* " + flaggedNote.ValidationFailureMessage + Environment.NewLine);
            }


            var validationMessages = string.Empty;
            if (failedRequirements.Count() == 0)
            {
                this.Log().Info("{0} v{1} passed validation.".format_with(message.PackageId, message.PackageVersion));
                validationMessages = "**NOTE**: No [required changes](https://github.com/chocolatey/package-validator/wiki#requirements) that the validator checks have been flagged! It is appreciated if you fix other items, but only Requirements will hold up a package version from approval. A human review could still turn up issues a computer may not easily find.{0}{0}".format_with(Environment.NewLine);
            }

            validationMessages += resultsMessage.ToString();
            if (string.IsNullOrWhiteSpace(resultsMessage.ToString()))
            {
                this.Log().Info("{0} v{1} had no findings!".format_with(message.PackageId, message.PackageVersion));
                validationMessages = "Congratulations! This package passed automatic validation review without flagging on any issues the validator currently checks. A human review could still turn up issues a computer may not easily find.";
            }

            EventManager.publish(new FinalPackageValidationResultMessage(message.PackageId, message.PackageVersion, validationMessages, !failedRequirements.Any()));
        }
    }
}
