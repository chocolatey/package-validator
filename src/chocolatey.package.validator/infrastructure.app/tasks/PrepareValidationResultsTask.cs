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
                resultsMessage.Append("When a package has failed requirements, the package requires fixing or response by the maintainer. If items are flagged correctly, they must be fixed before the package can be approved.{0}{0}".format_with(Environment.NewLine));
            }
            foreach (var failedRequirement in failedRequirements.or_empty_list_if_null())
            {
                resultsMessage.Append("* " + failedRequirement.ValidationFailureMessage + Environment.NewLine);
            }

            var flaggedGuidelines = message.ValidationResults.Where(r => r.Validated == false && r.ValidationLevel == ValidationLevelType.Guideline);
            if (flaggedGuidelines.Count() != 0)
            {
                resultsMessage.Append("{0}##### Guidelines{0}".format_with(Environment.NewLine));
                resultsMessage.Append("When a package has guidelines suggested, it is items that will improved the quality of the package. These are considered something to fix for next time to increase the quality of the package. Over time one or more Guideline can become a Requirement. A package guideline flag can be approved without fixing the issue.{0}{0}".format_with(Environment.NewLine));
            }
            foreach (var flaggedGuideline in flaggedGuidelines.or_empty_list_if_null())
            {
                resultsMessage.Append("* " + flaggedGuideline.ValidationFailureMessage + Environment.NewLine);
            }
            
            var flaggedSuggestions = message.ValidationResults.Where(r => r.Validated == false && (r.ValidationLevel == ValidationLevelType.Suggestion || r.ValidationLevel == ValidationLevelType.Note));
            if (flaggedSuggestions.Count() != 0)
            {
                resultsMessage.Append("{0}##### Suggestions{0}".format_with(Environment.NewLine));
                resultsMessage.Append("When a package has suggestions, it is items that are newer and should be considered. A package suggestion flag can be approved without fixing the issue.{0}{0}".format_with(Environment.NewLine));
            }
            foreach (var flaggedSuggestion in flaggedSuggestions.or_empty_list_if_null())
            {
                resultsMessage.Append("* " + flaggedSuggestion.ValidationFailureMessage + Environment.NewLine);
            }
            
            //todo if you find any that failed validation, it's time to update the website
            //create a message for updating the website with the validation set
            //EventManager.publish(new PackageValidationResultMessage(message.PackageId, message.PackageVersion, validationResults, DateTime.UtcNow));
        }
    }
}
