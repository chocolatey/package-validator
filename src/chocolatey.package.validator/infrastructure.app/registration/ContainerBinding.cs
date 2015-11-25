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
    using Reactive.EventAggregator;
    using SimpleInjector;
    using commands;
    using configuration;
    using filesystem;
    using infrastructure.configuration;
    using infrastructure.logging;
    using infrastructure.messaging;
    using infrastructure.services;
    using locators;
    using services;

    /// <summary>
    ///   The main inversion container registration for the application. Look for other container bindings in client projects.
    /// </summary>
    public class ContainerBinding
    {
        /// <summary>
        ///   Loads the module into the kernel.
        /// </summary>
        /// <param name="container">The container.</param>
        public void register_components(Container container)
        {
            Log.InitializeWith<Log4NetLog>();

            IConfigurationSettings configuration = new ConfigurationSettings();
            Config.initialize_with(configuration);

            container.Register(() => configuration, Lifestyle.Singleton);

            container.Register<IEventAggregator, EventAggregator>(Lifestyle.Singleton);
            container.Register<IMessageSubscriptionManagerService, MessageSubscriptionManagerService>(Lifestyle.Singleton);
            EventManager.initialize_with(container.GetInstance<IMessageSubscriptionManagerService>);

            container.Register<INotificationSendService, SmtpMarkdownNotificationSendService>(Lifestyle.Singleton);
            container.Register<IMessageService, MessageService>(Lifestyle.Singleton);
            container.Register<IEmailDistributionService, EmailDistributionService>(Lifestyle.Singleton);
            container.Register<IDateTimeService, SystemDateTimeUtcService>(Lifestyle.Singleton);
            container.Register<ICommandExecutor, CommandExecutor>(Lifestyle.Singleton);
            container.Register<IFileSystemService, FileSystemService>(Lifestyle.Singleton);
            container.Register<IFileSystem, DotNetFileSystem>(Lifestyle.Singleton);
            container.Register<IRegularExpressionService, RegularExpressionService>(Lifestyle.Singleton);
            container.Register<INuGetService, NuGetService>(Lifestyle.Singleton);
            container.Register<IPackageValidationService, PackageValidationService>(Lifestyle.Singleton);
            container.Register<ITypeLocator, TypeLocator>(Lifestyle.Singleton);

            RegisterOverrideableComponents(container, configuration);
        }

        /// <summary>
        ///   Registers the components that might be overridden in the front end.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="configuration">The configuration.</param>
        private void RegisterOverrideableComponents(Container container, IConfigurationSettings configuration)
        {
            // var singletonLifeStyle = Lifestyle.Singleton;
        }
    }
}
