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
                resultsMessage.Append("##### Requirements{0}".format_with(Environment.NewLine));
                resultsMessage.Append("When a package version has failed requirements, the package version requires fixing or response by the maintainer. If items are flagged correctly, they must be fixed before the package version can be approved. The exact same version should be uploaded during moderation review.{0}{0}".format_with(Environment.NewLine));
            }
            foreach (var failedRequirement in failedRequirements.or_empty_list_if_null())
            {
                resultsMessage.Append("* " + failedRequirement.ValidationFailureMessage + Environment.NewLine);
            }

            var flaggedGuidelines = message.ValidationResults.Where(r => r.Validated == false && r.ValidationLevel == ValidationLevelType.Guideline);
            if (flaggedGuidelines.Count() != 0)
            {
                resultsMessage.Append("{0}##### Guidelines{1}".format_with(resultsMessage.Length ==0 ? string.Empty : Environment.NewLine, Environment.NewLine));
                resultsMessage.Append("Guidelines are strong suggestions that improve the quality of a package version. These are considered something to fix for next time to increase the quality of the package. Over time guidelines can become requirements. A package version can be approved without addressing guideline comments but will reduce the quality of the package.{0}{0}".format_with(Environment.NewLine));
            }
            foreach (var flaggedGuideline in flaggedGuidelines.or_empty_list_if_null())
            {
                resultsMessage.Append("* " + flaggedGuideline.ValidationFailureMessage + Environment.NewLine);
            }
            
            var flaggedSuggestions = message.ValidationResults.Where(r => r.Validated == false && (r.ValidationLevel == ValidationLevelType.Suggestion));
            if (flaggedSuggestions.Count() != 0)
            {
                resultsMessage.Append("{0}##### Suggestions{1}".format_with(resultsMessage.Length == 0 ? string.Empty : Environment.NewLine, Environment.NewLine));
                resultsMessage.Append("Suggestions are newly introduced items that should be considered. A package version can be approved without addressing suggestion comments.{0}{0}".format_with(Environment.NewLine));
            }
            foreach (var flaggedSuggestion in flaggedSuggestions.or_empty_list_if_null())
            {
                resultsMessage.Append("* " + flaggedSuggestion.ValidationFailureMessage + Environment.NewLine);
            }
            
            var flaggedNotes = message.ValidationResults.Where(r => r.Validated == false && (r.ValidationLevel == ValidationLevelType.Note));
            if (flaggedNotes.Count() != 0)
            {
                resultsMessage.Append("{0}##### Notes{1}".format_with(resultsMessage.Length == 0 ? string.Empty : Environment.NewLine, Environment.NewLine));
                resultsMessage.Append("Notes typically flag things for both you and the reviewer.{0}{0}".format_with(Environment.NewLine));
            }
            foreach (var flaggedNote in flaggedNotes.or_empty_list_if_null())
            {
                resultsMessage.Append("* " + flaggedNote.ValidationFailureMessage + Environment.NewLine);
            }

            var validationMessages = resultsMessage.ToString();
            if (!string.IsNullOrWhiteSpace(validationMessages))
            {
                var messageToSend = validationMessages;
                // send the message
            }

            //todo if you find any that failed validation, it's time to update the website
            //create a message for updating the website with the validation set
            //EventManager.publish(new PackageValidationResultMessage(message.PackageId, message.PackageVersion, validationResults, DateTime.UtcNow));
        }
    }
}
