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
    using messaging;

    /// <summary>
    ///   Interface for MessageSubscriptionManagerService
    /// </summary>
    public interface IMessageSubscriptionManagerService
    {
        /// <summary>
        ///   Publishes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message to publish.</param>
        void publish<TMessage>(TMessage message) where TMessage : class, IMessage;

        /// <summary>
        ///   Subscribes to the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="handleMessage">The message handler.</param>
        /// <param name="handleError">The error handler.</param>
        /// <param name="filter">The message filter.</param>
        /// <returns>The subscription as Disposable</returns>
        IDisposable subscribe<TMessage>(
            Action<TMessage> handleMessage,
            Action<Exception> handleError,
            Func<TMessage, bool> filter)
            where TMessage : class, IMessage;
    }
}
