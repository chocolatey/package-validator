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

namespace chocolatey.package.validator.infrastructure.services
{
    using System.Collections.Generic;
    using System.Net.Mail;

    /// <summary>
    ///   For sending messages
    /// </summary>
    public interface INotificationSendService
    {
        /// <summary>
        ///   Sends a message.
        /// </summary>
        /// <param name="from">The sender.</param>
        /// <param name="to">The recipient.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        void send(string @from, string to, string subject, string message);

        /// <summary>
        ///   Sends a message.
        /// </summary>
        /// <param name="from">The sender.</param>
        /// <param name="to">The recipient.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        void send(string @from, IEnumerable<string> to, string subject, string message);

        /// <summary>
        ///   Sends a message.
        /// </summary>
        /// <param name="from">The sender.</param>
        /// <param name="to">The recipient.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <param name="useHtmlBody">Whether to use html or not.</param>
        void send(string @from, IEnumerable<string> to, string subject, string message, bool useHtmlBody);

        /// <summary>
        ///   Sends a message
        /// </summary>
        /// <param name="from">The sender.</param>
        /// <param name="to">The recipient.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <param name="attachments">The attachments.</param>
        /// <param name="useHtmlBody">
        ///   if set to <c>true</c> [use HTML body].
        /// </param>
        void send(
            string @from,
            IEnumerable<string> to,
            string subject,
            string message,
            IEnumerable<Attachment> attachments,
            bool useHtmlBody);
    }
}
