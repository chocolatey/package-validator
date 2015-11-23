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

namespace chocolatey.package.validator.infrastructure.logging.log4netappenders
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Mail;
    using log4net.Appender;

    /// <summary>
    ///   This is a custom appender so that we can set enable SSL properly (and support TLS)
    /// </summary>
    /// <remarks>
    ///   This is based partially on:
    ///   <para>http://stackoverflow.com/questions/10933757/multiple-smtphost-addresses-using-smtpappender-in-log4net</para>
    ///   <para>http://stackoverflow.com/questions/13741312/does-log4net-haven-an-smtp-appender-that-support-tls-encryption</para>
    ///   <para>http://stackoverflow.com/questions/12530128/log4net-smtpappender-for-webemail-not-sending</para>
    /// </remarks>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    public class SmtpCustomAppender : SmtpAppender
    {
        public SmtpCustomAppender()
        {
            Authentication = SmtpAuthentication.None;
            Port = 25; // 0x19;
            // Port = 587; // 0x24b;
            Priority = MailPriority.Normal;
            EnableSsl = false;
        }

        public bool EnableSsl { get; set; }

        /// <summary>
        ///   Send the email message - this overrides the email sender so that we can add enabling SSL
        /// </summary>
        /// <param name="messageBody">the body text to include in the mail</param>
        protected override void SendEmail(string messageBody)
        {
            var client = new SmtpClient();

            if (!string.IsNullOrEmpty(SmtpHost)) client.Host = SmtpHost;

            client.Port = Port;
            client.EnableSsl = EnableSsl;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            switch (Authentication)
            {
                case SmtpAuthentication.Basic :
                    client.Credentials = new NetworkCredential(Username, Password);
                    break;
                case SmtpAuthentication.Ntlm :
                    client.Credentials = CredentialCache.DefaultNetworkCredentials;
                    break;
            }

            var message = new MailMessage
            {
                Body = messageBody,
                From = new MailAddress(From)
            };
            message.To.Add(To);
            message.Subject = Subject;
            message.Priority = Priority;
            client.Send(message);
        }
    }
}
