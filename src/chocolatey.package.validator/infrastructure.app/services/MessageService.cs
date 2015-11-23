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

namespace chocolatey.package.validator.infrastructure.app.services
{
    using System.Collections.Generic;
    using System.Net.Mail;
    using infrastructure.services;

    /// <summary>
    ///   Sends system specific messages
    /// </summary>
    public class MessageService : IMessageService
    {
        private readonly INotificationSendService _sendService;
        private readonly string _messageFrom;

        /// <summary>
        ///   Initializes a new instance of the <see cref="MessageService" /> class.
        /// </summary>
        /// <param name="sendService">The send service.</param>
        public MessageService(INotificationSendService sendService)
        {
            _sendService = sendService;
            _messageFrom = ApplicationParameters.get_system_email_address();
        }

        /// <summary>
        ///   Sends a message
        /// </summary>
        /// <param name="to">The recipient.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        public void send(string to, string subject, string message)
        {
            this.Log().Debug(() => "Sending message with subject '{0}' to '{1}'".format_with(subject, to));
            _sendService.send(_messageFrom, to, subject, message);
        }

        /// <summary>
        ///   Sends a message
        /// </summary>
        /// <param name="to">List of addresses to.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        public void send(IEnumerable<string> to, string subject, string message)
        {
            this.Log().Debug(() => "Sending message with subject '{0}' to '{1}'".format_with(subject, string.Join(",",to)));
            _sendService.send(_messageFrom, to, subject, message);
        }

        /// <summary>
        ///   Sends a message
        /// </summary>
        /// <param name="to">The recipient.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <param name="attachments">The attachments.</param>
        public void send(IEnumerable<string> to, string subject, string message, IEnumerable<Attachment> attachments)
        {
            this.Log().Debug(() => "Sending message with subject '{0}' to '{1}'".format_with(subject, string.Join(",", to)));
            _sendService.send(_messageFrom, to, subject, message, attachments, useHtmlBody: false);
        }
    }
}
