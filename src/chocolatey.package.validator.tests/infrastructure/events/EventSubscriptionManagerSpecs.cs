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

namespace chocolatey.package.validator.tests.infrastructure.events
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using context;
    using NUnit.Framework;
    using Reactive.EventAggregator;
    using Should;
    using validator.infrastructure.services;

    public class EventSubscriptionManagerSpecs
    {
        public abstract class EventSubscriptionManagerSpecsBase : TinySpec
        {
            protected FakeMessage Message;

            public MessageSubscriptionManagerService SubscriptionManager { get; private set; }

            public override void Context()
            {
                Message = new FakeMessage("yo", 12);
                SubscriptionManager = new MessageSubscriptionManagerService(new EventAggregator());
            }
        }

        public class when_using_eventSubscriptionManager_to_subscribe_to_an_event : EventSubscriptionManagerSpecsBase
        {
            private bool _wasCalled;
            private FakeMessage _localFakeMessage;

            public override void Context()
            {
                base.Context();
                SubscriptionManager.subscribe<FakeMessage>(x =>
                    {
                        _wasCalled = true;
                        _localFakeMessage = x;
                    }, null, null);
            }

            public override void Because()
            {
                SubscriptionManager.publish(Message);
            }

            [Fact]
            public void should_have_called_the_action()
            {
                _wasCalled.ShouldBeTrue();
            }

            [Fact]
            public void should_have_passed_the_message()
            {
                _localFakeMessage.ShouldEqual(Message);
            }

            [Fact]
            public void should_have_passed_the_name_correctly()
            {
                _localFakeMessage.Name.ShouldEqual("yo");
            }

            [Fact]
            public void should_have_passed_the_digits_correctly()
            {
                _localFakeMessage.Digits.ShouldEqual(12d);
            }
        }

        public class when_using_eventSubscriptionManager_with_long_running_event_subscriber : EventSubscriptionManagerSpecsBase
        {
            private bool _wasCalled;
            private FakeMessage _localFakeMessage;

            public override void Context()
            {
                base.Context();
                SubscriptionManager.subscribe<FakeMessage>(m =>
                    {
                        //stuff is happening
                        Thread.Sleep(2000);
                        _wasCalled = true;
                        _localFakeMessage = m;
                        Console.WriteLine("event complete");
                    }, null, null);
            }

            public override void Because()
            {
                SubscriptionManager.publish(Message);
            }

            [Fact]
            public void should_wait_the_event_to_complete()
            {
                Console.WriteLine("event complete should be above this");
                _wasCalled.ShouldBeTrue();
            }

            [Fact]
            public void should_have_passed_the_message()
            {
                _localFakeMessage.ShouldEqual(Message);
            }
        }

        public class when_using_eventSubscriptionManager_to_subscribe_to_an_event_with_a_filter_that_the_event_satisfies : EventSubscriptionManagerSpecsBase
        {
            private bool _wasCalled;
            private FakeMessage _localFakeMessage;

            public override void Context()
            {
                base.Context();
                SubscriptionManager.subscribe<FakeMessage>(x =>
                    {
                        _wasCalled = true;
                        _localFakeMessage = x;
                    }, null, (message) => message.Digits > 3);
            }

            public override void Because()
            {
                SubscriptionManager.publish(Message);
            }

            [Fact]
            public void should_have_called_the_action()
            {
                _wasCalled.ShouldBeTrue();
            }

            [Fact]
            public void should_have_passed_the_message()
            {
                _localFakeMessage.ShouldEqual(Message);
            }

            [Fact]
            public void should_have_passed_the_name_correctly()
            {
                _localFakeMessage.Name.ShouldEqual("yo");
            }

            [Fact]
            public void should_have_passed_the_digits_correctly()
            {
                _localFakeMessage.Digits.ShouldEqual(12d);
            }
        }

        public class when_using_eventSubscriptionManager_to_subscribe_to_an_event_with_a_filter_that_the_event_does_not_satisfy : EventSubscriptionManagerSpecsBase
        {
            private bool _wasCalled;
            private FakeMessage _localFakeMessage;

            public override void Context()
            {
                base.Context();
                SubscriptionManager.subscribe<FakeMessage>(x =>
                    {
                        _wasCalled = true;
                        _localFakeMessage = x;
                    }, null, (message) => message.Digits < 3);
            }

            public override void Because()
            {
                SubscriptionManager.publish(Message);
            }

            [Fact]
            public void should_not_have_called_the_action()
            {
                _wasCalled.ShouldBeFalse();
            }

            [Fact]
            public void should_not_have_passed_the_message()
            {
                _localFakeMessage.ShouldNotEqual(Message);
            }
        }

        public class when_using_eventSubscriptionManager_and_multiple_parties_subscribe_to_the_same_event : EventSubscriptionManagerSpecsBase
        {
            private IList<FakeSubscriber> _list;

            public override void Context()
            {
                base.Context();

                _list = new List<FakeSubscriber>();
                do
                {
                    _list.Add(new FakeSubscriber(SubscriptionManager));
                } while (_list.Count < 5);
            }

            public override void Because()
            {
                SubscriptionManager.publish(Message);
            }
        }

        public class when_using_eventSubscriptionManager_to_send_a_null_event_message : EventSubscriptionManagerSpecsBase
        {
            private bool _errored;

            public override void Because()
            {
                try
                {
                    SubscriptionManager.publish<FakeMessage>(null);
                }
                catch (Exception)
                {
                    _errored = true;
                }
            }

            [Fact]
            public void should_throw_an_error()
            {
                Assert.Throws<ArgumentNullException>(() => SubscriptionManager.publish<FakeMessage>(null));
                _errored.ShouldBeTrue();
            }
        }
    }
}