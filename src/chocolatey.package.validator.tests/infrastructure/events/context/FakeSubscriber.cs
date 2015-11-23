// Copyright © 2011 - Present RealDimensions Software, LLC
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

namespace chocolatey.package.validator.tests.infrastructure.events.context
{
    using System;
    using validator.infrastructure.services;

    public class FakeSubscriber
    {
        public FakeSubscriber(IMessageSubscriptionManagerService subscriptionManager)
        {
            subscriptionManager.subscribe<FakeMessage>(x =>
                {
                    WasCalled = true;
                    ReceivedMessage = x;
                    CalledAt = DateTime.Now;
                }, null, null);
        }

        public FakeMessage ReceivedMessage { get; private set; }
        public bool WasCalled { get; private set; }
        public DateTime CalledAt { get; private set; }
    }
}