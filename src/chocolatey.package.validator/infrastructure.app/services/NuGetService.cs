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

namespace chocolatey.package.validator.infrastructure.app.services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using NuGet;
    using configuration;
    using infrastructure.results;
    using logging;
    using registration;
    using results;
    using IFileSystem = filesystem.IFileSystem;

    public class NuGetService : INuGetService
    {
        private readonly IFileSystem _fileSystem;
        public const string API_KEY_HEADER = "X-NuGet-ApiKey";
        public const int MAX_REDIRECTION_COUNT = 20;
        private const string DEFAULT_SERVICE_ENDPOINT = "/api/v2/";

        public NuGetService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public string ApiKeyHeader { get { return API_KEY_HEADER; } }

        public static Uri get_service_endpoint_url(string baseUrl, string path)
        {
            return new Uri(new Uri(baseUrl), path);
        }

        public HttpClient get_client(string baseUrl, string path, string method, string contentType)
        {
            this.Log().Debug(() => "Getting httpclient for '{0}' with '{1}'".format_with(baseUrl, path));
            Uri requestUri = get_service_endpoint_url(baseUrl, path);

            var client = new HttpClient(requestUri)
            {
                ContentType = contentType,
                Method = method,
                UserAgent = "{0}/{1}".format_with(ApplicationParameters.Name, ApplicationParameters.FileVersion)
            };

            return client;
        }

        /// <summary>
        ///   Ensures that success response is received.
        /// </summary>
        /// <param name="client">The client that is making the request.</param>
        /// <param name="expectedStatusCode">The exected status code.</param>
        /// <returns>
        ///   True if success response is received; false if redirection response is received.
        ///   In this case, _baseUri will be updated to be the new redirected Uri and the requrest
        ///   should be retried.
        /// </returns>
        public NuGetServiceGetClientResult ensure_successful_response(HttpClient client, HttpStatusCode? expectedStatusCode = null)
        {
            this.Log().Debug(() => "Reaching out to '{0}'".format_with(client.Uri.to_string()));

            var result = new NuGetServiceGetClientResult
            {
                Success = false
            };

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)client.GetResponse();

                result.Messages.Add(
                    new ResultMessage
                    {
                        MessageType = ResultType.Note,
                        Message = response != null ? response.StatusDescription : "Response was null"
                    });

                if (response != null &&
                    ((expectedStatusCode.HasValue && expectedStatusCode.Value != response.StatusCode) ||
                     // If expected status code isn't provided, just look for anything 400 (Client Errors) or higher (incl. 500-series, Server Errors)
                     // 100-series is protocol changes, 200-series is success, 300-series is redirect.
                     (!expectedStatusCode.HasValue && (int)response.StatusCode >= 400))) Bootstrap.handle_exception(new InvalidOperationException("Failed to process request.{0} '{1}'".format_with(Environment.NewLine, response.StatusDescription)));
                else result.Success = true;

                return result;
            }
            catch (WebException e)
            {
                if (e.Response == null)
                {
                    Bootstrap.handle_exception(e);
                    return result;
                }

                response = (HttpWebResponse)e.Response;
                // Check if the error is caused by redirection
                if (response.StatusCode == HttpStatusCode.MultipleChoices ||
                    response.StatusCode == HttpStatusCode.MovedPermanently ||
                    response.StatusCode == HttpStatusCode.Found ||
                    response.StatusCode == HttpStatusCode.SeeOther ||
                    response.StatusCode == HttpStatusCode.TemporaryRedirect)
                {
                    var location = response.Headers["Location"];
                    Uri newUri;
                    if (!Uri.TryCreate(client.Uri, location, out newUri))
                    {
                        Bootstrap.handle_exception(e);
                        return result;
                    }

                    result.RedirectUrl = newUri.ToString();

                    return result;
                }

                result.Messages.Add(
                    new ResultMessage
                    {
                        MessageType = ResultType.Note,
                        Message = response.StatusDescription
                    });

                if (expectedStatusCode != response.StatusCode) Bootstrap.handle_exception(new InvalidOperationException("Failed to process request.{0} '{1}':{0} {2}".format_with(Environment.NewLine, response.StatusDescription, e.Message), e));
                else result.Success = true;

                return result;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
            }
        }

        public IPackage download_package(string packageId, string packageVersion, string downloadLocation, IConfigurationSettings configuration)
        {
            var version = new SemanticVersion(0, 0, 0, 0);
            if (!string.IsNullOrWhiteSpace(packageVersion)) version = new SemanticVersion(packageVersion);

            var packageManager = get_package_manager(downloadLocation, configuration);

            this.Log().Debug(() => "Searching for {0} v{1} to install from {2}.".format_with(packageId, version.to_string(), packageManager.SourceRepository.Source));

            IPackage availablePackage = packageManager.SourceRepository.FindPackage(packageId, version, allowPrereleaseVersions: true, allowUnlisted: true);
            if (availablePackage == null)
            {
                //todo: do something here
            }

            this.Log().Debug(() => "Installing {0} v{1} from {2}.".format_with(packageId, version.to_string(), packageManager.SourceRepository.Source));

            try
            {
                packageManager.InstallPackage(availablePackage, ignoreDependencies: true, allowPrereleaseVersions: true);
            }
            catch (Exception ex)
            {
                Bootstrap.handle_exception(new System.ApplicationException("Encountered error downloading package {0} v{1}:{2}{3}".format_with(packageId,packageVersion,Environment.NewLine, ex.to_string()),ex));
                return null;
            }
            
            var cachePackage = _fileSystem.combine_paths(Environment.GetEnvironmentVariable("LocalAppData"), "NuGet", "Cache", "{0}.{1}.nupkg".format_with(packageId, version.to_string()));
            if (_fileSystem.file_exists(cachePackage)) _fileSystem.delete_file(cachePackage);

            this.Log().Debug(() => "Returning {0} v{1} package.".format_with(packageId, version.to_string()));

            return packageManager.LocalRepository.FindPackage(packageId, version, allowPrereleaseVersions: true, allowUnlisted: true);
        }

        public IPackageManager get_package_manager(string localPackageDirectory, IConfigurationSettings configuration)
        {
            var nugetLogger = new ServiceNugetLogger();

            NuGet.IFileSystem nugetPackagesFileSystem = get_nuget_file_system(nugetLogger, localPackageDirectory);
            IPackagePathResolver pathResolver = get_path_resolver(nugetPackagesFileSystem);
            var packageManager = new PackageManager(get_remote_repository(configuration, nugetLogger), pathResolver, nugetPackagesFileSystem, get_local_repository(pathResolver, nugetPackagesFileSystem))
            {
                DependencyVersion = DependencyVersion.Highest,
            };

            return packageManager;
        }

        public NuGet.IFileSystem get_nuget_file_system(ILogger nugetLogger, string rootDirectory)
        {
            return new PhysicalFileSystem(rootDirectory)
            {
                Logger = nugetLogger
            };
        }

        public IPackagePathResolver get_path_resolver(NuGet.IFileSystem nugetPackagesFileSystem)
        {
            return new DefaultPackagePathResolver(nugetPackagesFileSystem)
            {
            };
        }

        public IPackageRepository get_local_repository(IPackagePathResolver pathResolver, NuGet.IFileSystem nugetPackagesFileSystem)
        {
            this.Log().Debug(() => "Setting up local repository at '{0}'".format_with(nugetPackagesFileSystem.Root));

            IPackageRepository localRepository = new LocalPackageRepository(pathResolver, nugetPackagesFileSystem);
            localRepository.PackageSaveMode = PackageSaveModes.Nupkg | PackageSaveModes.Nuspec;

            return localRepository;
        }

        public IPackageRepository get_remote_repository(IConfigurationSettings configuration, ILogger nugetLogger)
        {
            this.Log().Debug(() => "Setting up redirected client for '{0}' with '{1}'".format_with(configuration.PackagesUrl, DEFAULT_SERVICE_ENDPOINT));

            return new AggregateRepository(
                new List<IPackageRepository>
                {
                    new DataServicePackageRepository(new RedirectedHttpClient(get_service_endpoint_url(configuration.PackagesUrl, DEFAULT_SERVICE_ENDPOINT)))
                })
            {
                Logger = nugetLogger,
            };
        }
    }
}
