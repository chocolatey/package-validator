namespace chocolatey.package.validator.infrastructure.app.services
{
    using System.Net;
    using NuGet;
    using configuration;
    using results;

    public interface INuGetService
    {
        string ApiKeyHeader { get; }

        HttpClient get_client(string baseUrl, string path, string method, string contentType);

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
        NuGetServiceGetClientResult ensure_successful_response(HttpClient client, HttpStatusCode? expectedStatusCode = null);

        /// <summary>
        /// Downloads a package to the specified folder and returns the package stream
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="packageVersion">The package version.</param>
        /// <param name="downloadLocation">The download location.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        IPackage download_package(string packageId, string packageVersion, string downloadLocation, IConfigurationSettings configuration);
    }
}