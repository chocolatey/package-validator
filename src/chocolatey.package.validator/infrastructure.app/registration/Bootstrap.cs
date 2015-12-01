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

namespace chocolatey.package.validator.infrastructure.app.registration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Timers;
    using infrastructure.configuration;
    using log4net;

    /// <summary>
    ///   Application bootstrapping - sets up logging and errors for the app domain
    /// </summary>
    public class Bootstrap
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Bootstrap));
        private static readonly Lazy<Timer> _timer = new Lazy<Timer>(() => new Timer());
        private static readonly Lazy<ConcurrentDictionary<Type, IList<Exception>>> _exceptions =
            new Lazy<ConcurrentDictionary<Type, IList<Exception>>>(() => new ConcurrentDictionary<Type, IList<Exception>>());

        /// <summary>
        ///   Gets the Exceptions dictionary
        /// </summary>
        protected static ConcurrentDictionary<Type, IList<Exception>> Exceptions { get { return _exceptions.Value; } }

        /// <summary>
        ///   Initializes this instance.
        /// </summary>
        public static void initialize()
        {
            _logger.Info("================================");
            // initialization code 
            _logger.Debug("XmlConfiguration is now operational");
        }

        /// <summary>
        ///   Startups this instance.
        /// </summary>
        public static void startup()
        {
            _logger.InfoFormat("Performing bootstrapping operations for '{0}'.", ApplicationParameters.Name);

            AppDomain.CurrentDomain.UnhandledException += domain_unhandled_exception;
            MailSettingsSmtpFolderConverter.convert_relative_to_absolute_pickup_directory_location();

            // todo: move this out to a config value
            _timer.Value.Interval = TimeSpan.FromMinutes(15).TotalMilliseconds;
            _timer.Value.Elapsed += check_and_send_error_summary;
            _timer.Value.Start();
        }

        /// <summary>
        ///   Shutdowns this instance.
        /// </summary>
        public static void shutdown()
        {
            _logger.InfoFormat("Performing Shutdown operations for '{0}'.", ApplicationParameters.Name);
            _timer.Value.Stop();
            _timer.Value.Dispose();
        }

        /// <summary>
        ///   Handles unhandled exception for the application domain.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">
        ///   The <see cref="System.UnhandledExceptionEventArgs" /> instance containing the event data.
        /// </param>
        private static void domain_unhandled_exception(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            handle_exception(ex);
        }

        /// <summary>
        ///   Handles exceptions in a universal way.
        /// </summary>
        /// <param name="exception">The <see cref="System.Exception" /> instance containing the exception.</param>
        public static void handle_exception(Exception exception)
        {
            if (exception != null)
            { 
                var exceptionMessage = exception.ToString();

                var exceptions = Exceptions.GetOrAdd(exception.GetType(), create_exception_list);
                exceptions.Add(exception);

                _logger.WarnFormat(
                    "{0} had an error on {1} (with user {2}):{3}{4}",
                    ApplicationParameters.Name,
                    Environment.MachineName,
                    Environment.UserName,
                    Environment.NewLine,
                    exceptionMessage);
            }
        }

        /// <summary>
        ///   Creates the exception list.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>New List of Exceptions</returns>
        private static IList<Exception> create_exception_list(Type e)
        {
            return new List<Exception>();
        }

        /// <summary>
        ///   Checks the exceptions dictionary and send error summary.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">
        ///   The <see cref="System.Timers.ElapsedEventArgs" /> instance containing the event data.
        /// </param>
        private static void check_and_send_error_summary(object sender, ElapsedEventArgs e)
        {
            var exceptionMessage = new StringBuilder();
            foreach (KeyValuePair<Type, IList<Exception>> exceptionList in Exceptions.ToList().or_empty_list_if_null())
            {
                if (exceptionList.Value != null && exceptionList.Value.Count != 0)
                {
                    exceptionMessage.Clear();
                    exceptionMessage.Append(
                        "There are {0} exceptions of '{1}'.".format_with(exceptionList.Value.Count, exceptionList.Key.Name));
                    exceptionMessage.Append(exceptionList.Value[0]);

                    _logger.ErrorFormat(
                        "{0} had error(s) on {1} (with user {2}):{3}{4}",
                        ApplicationParameters.Name,
                        Environment.MachineName,
                        Environment.UserName,
                        Environment.NewLine,
                        exceptionMessage);
                }

                IList<Exception> tempExceptions = new List<Exception>();
                Exceptions.TryRemove(exceptionList.Key, out tempExceptions);
            }
        }
    }
}
