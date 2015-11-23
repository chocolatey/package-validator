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
    using System.Net;
    using infrastructure.results;
    using NuGet;
    using registration;
    using results;

    public class NuGetService : INuGetService
    {
        public const string API_KEY_HEADER = "X-NuGet-ApiKey";
        public const int MAX_REDIRECTION_COUNT = 20;

        public string ApiKeyHeader { get { return API_KEY_HEADER; } }

        public static Uri get_service_endpoint_url(string baseUrl, string path)
        {
            return new Uri(new Uri(baseUrl), path);
        }

        public HttpClient get_client(string baseUrl, string path, string method, string contentType)
        {
            this.Log().Debug(() => "Getting httpclient for '{0}' with '{1}'".format_with(baseUrl,path));
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
                     (!expectedStatusCode.HasValue && (int)response.StatusCode >= 400)))

                {
                    Bootstrap.handle_exception(new InvalidOperationException("Failed to process request.{0} '{1}'".format_with(Environment.NewLine, response.StatusDescription)));
                }
                else
                {
                    result.Success = true;
                }

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

                if (expectedStatusCode != response.StatusCode)
                {
                    Bootstrap.handle_exception(new InvalidOperationException("Failed to process request.{0} '{1}':{0} {2}".format_with(Environment.NewLine, response.StatusDescription, e.Message), e));
                }
                else
                {
                    result.Success = true;
                }

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

        //private string get_response_text(HttpWebResponse response)
        //{
        //    if (response == null) return string.Empty;

        //    try
        //    {
        //        var encoding = Encoding.GetEncoding(response.CharacterSet);
        //        using (var responseStream = response.GetResponseStream())
        //        {
        //            using (var reader = new StreamReader(responseStream, encoding))
        //            {
        //                return reader.ReadToEnd();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        this.Log().Warn("Response text is empty: {0}".format_with(ex.Message));
        //        return string.Empty;
        //    }
        //}
    }
}
