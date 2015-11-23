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

namespace chocolatey.package.validator.infrastructure.messaging
{
    using System;
    using services;

    public static class EventManager
    {
        private static Func<IMessageSubscriptionManagerService> _messageSubscriptionManager;

        /// <summary>
        ///   Gets the manager service.
        /// </summary>
        /// <value>
        ///   The manager service.
        /// </value>
        public static IMessageSubscriptionManagerService ManagerService { get { return _messageSubscriptionManager(); } }

        /// <summary>
        ///   Initializes the Message platform with the subscription manager
        /// </summary>
        /// <param name="messageSubscriptionManager">The message subscription manager.</param>
        public static void initialize_with(Func<IMessageSubscriptionManagerService> messageSubscriptionManager)
        {
            _messageSubscriptionManager = messageSubscriptionManager;
        }

        /// <summary>
        ///   Publishes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message.</param>
        public static void publish<TMessage>(TMessage message) where TMessage : class, IMessage
        {
            if (_messageSubscriptionManager != null) _messageSubscriptionManager().publish(message);
        }

        /// <summary>
        ///   Subscribes to the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="handleMessage">The handle message.</param>
        /// <param name="handleError">The handle error.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The subscription so that a service could unsubscribe</returns>
        public static IDisposable subscribe<TMessage>(
            Action<TMessage> handleMessage,
            Action<Exception> handleError,
            Func<TMessage, bool> filter)
            where TMessage : class, IMessage
        {
            if (_messageSubscriptionManager != null) return _messageSubscriptionManager().subscribe(handleMessage, handleError, filter);

            return null;
        }
    }
}
