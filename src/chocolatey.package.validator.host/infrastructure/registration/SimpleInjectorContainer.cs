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

namespace chocolatey.package.validator.host.infrastructure.registration
{
    using System;
    using SimpleInjector;
    using validator.infrastructure.app;
    using validator.infrastructure.app.registration;
    using validator.infrastructure.container;

    /// <summary>
    ///   The inversion container
    /// </summary>
    public static class SimpleInjectorContainer
    {
        private static readonly Lazy<Container> _container = new Lazy<Container>(() => new Container());

        /// <summary>
        ///   Gets the container.
        /// </summary>
        public static Container Container { get { return _container.Value; } }

        /// <summary>
        ///   Initializes the container
        /// </summary>
        public static void start()
        {
            "SimpleInjectorContainer".Log().Debug("SimpleInjector is starting up");

            Container.Options.AllowOverridingRegistrations = true;
            var originalConstructorResolutionBehavior = Container.Options.ConstructorResolutionBehavior;
            Container.Options.ConstructorResolutionBehavior =
                new SimpleInjectorContainerResolutionBehavior(originalConstructorResolutionBehavior);

            initialize_container(Container);

            if (ApplicationParameters.IsDebug) Container.Verify();
        }

        /// <summary>
        ///   Does any Shutdown for the container
        /// </summary>
        public static void stop()
        {
            "SimpleInjectorContainer".Log().Debug("SimpleInjector has shut down");
        }

        private static void initialize_container(Container container)
        {
            var binding = new ContainerBinding();
            binding.register_components(container);
            var bindingClient = new ContainerBindingConsole();
            bindingClient.register_components(container);
        }
    }
}
