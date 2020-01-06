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

namespace chocolatey.package.validator.host
{
    using System;
    using System.IO;
    using System.ServiceProcess;
    using chocolatey.package.validator.infrastructure.app.registration;

    // ReSharper disable InconsistentNaming

    internal static class Program
    {
        /// <summary>
        ///   The main entry point for the application.
        /// </summary>
        /// <param name="args">The args.</param>
        private static void Main(string[] args)
        {
            // Ensure that the correct Security Protocol is being used.  Do this as early
            // as possible, rather than having to do it in multiple places in the code
            // base.
            // Use TLS1.2, TLS1.1, TLS1.0, SSLv3
            SecurityProtocol.set_protocol();

            if ((args.Length > 0) && (Array.IndexOf(args, "/console") != -1))
            {
                var service = new Service();
                service.run_as_console(args);
            } else
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                var servicesToRun = new ServiceBase[] { new Service() };
                ServiceBase.Run(servicesToRun);
            }
        }
    }

    // ReSharper restore InconsistentNaming
}
