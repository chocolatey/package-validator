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
    using System;
    using System.Collections.Generic;
    using System.Net.Mail;
    using configuration;
    using Elmah;
    using MarkdownMailer;

    /// <summary>
    ///   Sends a markdown formatted message with Smtp
    /// </summary>
    public class SmtpMarkdownNotificationSendService : INotificationSendService
    {
        /// <summary>
        ///   Sends a message.
        /// </summary>
        /// <param name="from">The sender.</param>
        /// <param name="to">The recipient.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        public void send(string @from, string to, string subject, string message)
        {
            send(
                @from,
                new List<string>
                {
                    to
                },
                subject,
                message,
                null,
                useHtmlBody: false);
        }

        /// <summary>
        ///   Sends a message
        /// </summary>
        /// <param name="from">The sender.</param>
        /// <param name="to">The recipient.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        public void send(string @from, IEnumerable<string> to, string subject, string message)
        {
            send(@from, to, subject, message, null, useHtmlBody: false);
        }

        /// <summary>
        ///   Sends a message.
        /// </summary>
        /// <param name="from">The sender.</param>
        /// <param name="to">The recipient.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <param name="useHtmlBody">Whether to use html or not.</param>
        public void send(string @from, IEnumerable<string> to, string subject, string message, bool useHtmlBody)
        {
            send(@from, to, subject, message, null, useHtmlBody);
        }

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
        public void send(
            string @from,
            IEnumerable<string> to,
            string subject,
            string message,
            IEnumerable<Attachment> attachments,
            bool useHtmlBody)
        {
            var config = Config.get_configuration_settings();

            var emailMessage = new MailMessage();
            emailMessage.From = new MailAddress(@from);

            if (!string.IsNullOrWhiteSpace(config.TestEmailOverride))
            {
                foreach (string emailAddress in config.TestEmailOverride.Split(',', ';'))
                {
                    emailMessage.To.Add(emailAddress);
                }
            } else
            {
                foreach (var emailTo in to)
                {
                    emailMessage.To.Add(emailTo);
                }
            }

            emailMessage.Subject = subject;
            emailMessage.Body = message;
            emailMessage.IsBodyHtml = useHtmlBody;
            foreach (var attachment in attachments.or_empty_list_if_null())
            {
                emailMessage.Attachments.Add(attachment);
            }

            this.Log()
                .Info(
                    () =>
                        "Sending '{0}' a message from '{1}': {2}{3}{4}".format_with(
                            string.Join(",", to),
                            @from,
                            subject,
                            Environment.NewLine,
                            message));

            try
            {
                var mailSender = new MailSender();

                mailSender.Send(emailMessage);
            } catch (Exception ex)
            {
                this.Log()
                    .Error(
                        () =>
                            "Error sending email to '{0}' with subject '{1}':{2}{3}".format_with(
                                emailMessage.To.ToString(),
                                emailMessage.Subject,
                                Environment.NewLine,
                                ex));
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
    }
}
