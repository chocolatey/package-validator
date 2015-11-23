namespace chocolatey.package.validator.infrastructure.app.services
{
    using System.Net;
    using NuGet;
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
    }
}