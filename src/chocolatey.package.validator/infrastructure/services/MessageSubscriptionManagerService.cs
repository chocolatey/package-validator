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
    using System.Reactive.Linq;
    using EnsureThat;
    using messaging;
    using Reactive.EventAggregator;

    /// <summary>
    ///   Implementation of IMessageSubscriptionManagerService
    /// </summary>
    public class MessageSubscriptionManagerService : IMessageSubscriptionManagerService
    {
        private readonly IEventAggregator _eventAggregator;

        // http://joseoncode.com/2010/04/29/event-aggregator-with-reactive-extensions/
        // https://github.com/shiftkey/Reactive.EventAggregator

        /// <summary>
        ///   Initializes a new instance of the <see cref="MessageSubscriptionManagerService" /> class.
        /// </summary>
        /// <param name="eventAggregator">The event Aggregator.</param>
        public MessageSubscriptionManagerService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        /// <summary>
        ///   Publishes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message to publish.</param>
        public void publish<TMessage>(TMessage message) where TMessage : class, IMessage
        {
            Ensure.That(() => message).IsNotNull();

            this.Log()
                .Debug(() => "Sending message '{0}' out if there are subscribers...".format_with(typeof(TMessage).Name));

            _eventAggregator.Publish(message);
        }

        /// <summary>
        ///   Subscribes to the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="handleMessage">The message handler.</param>
        /// <param name="handleError">The error handler.</param>
        /// <param name="filter">The message filter.</param>
        /// <returns>
        ///   The <see cref="IDisposable" />.
        /// </returns>
        public IDisposable subscribe<TMessage>(
            Action<TMessage> handleMessage,
            Action<Exception> handleError,
            Func<TMessage, bool> filter)
            where TMessage : class, IMessage
        {
            if (filter == null) filter = (message) => true;

            if (handleError == null) handleError = (ex) => { };

            var subscription = _eventAggregator.GetEvent<TMessage>().Where(filter).Subscribe(handleMessage, handleError);

            return subscription;
        }
    }
}
