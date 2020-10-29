<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
</Query>

void Main()
{
	Console.WriteLine(url_is_valid("using_redir", new Uri("http://chocolatey.org")));
	Console.WriteLine(url_is_valid("requires_Useragent_header", new Uri("https://hamapps.com/php/license.php")));
	Console.WriteLine(url_is_valid("requires_newer_tls_cipher", new Uri("https://talk.atomisystems.com/")));
	Console.WriteLine(url_is_valid("non_http_or_https_scheme", new Uri("git://git.code.sf.net/p/mrviewer/code")));
	Console.WriteLine(url_is_valid("mailto_scheme", new Uri("mailto:someone@yoursite.com")));
	Console.WriteLine(url_is_valid("results_in_too_many_redirects", new Uri("https://help.ea.com/en/origin/origin/")));
	Console.WriteLine(url_is_valid("requires_accept_header", new Uri("https://nbcgib.uesc.br/tinnr/en/")));
	Console.WriteLine(url_is_valid("returns_protocol_error", new Uri("https://trac.mpc-hc.org/")));
	Console.WriteLine(url_is_valid("requires_specific_user_agent", new Uri("https://www.microsoft.com/en-us/edge")));
	Console.WriteLine(url_is_valid("uses_Akamai", new Uri("https://www.dell.com/support/article/en-au/sln311129/dell-command-update?lang=en")));
	Console.WriteLine(url_is_valid("requires_additional_security_headers", new Uri("https://faq.whatsapp.com/")));
	Console.WriteLine(url_is_valid("uses_cloudflare_security_features", new Uri("https://www.audacityteam.org/help/documentation")));
	Console.WriteLine(url_is_valid("uses_additional_ciphers", new Uri("https://www.elster.de/elsterweb/lizenzvertrag/lizenzvertrag_elsterformular")));
	Console.WriteLine(url_is_valid("uses_Akamai_with_operation_timeout", new Uri("https://www.amd.com/en/products/chipsets-am4")));
}

public static bool url_is_valid(string problem, Uri url)
{
	Console.WriteLine(string.Format("{0} - ", problem));
	string proxyUserName = string.Empty;
	string proxyPassword = string.Empty;
	string proxyAddress = string.Empty;
	
	if (url == null)
	{
		return true;
	}

	if (url.Scheme == "mailto")
	{
		// mailto links are not expected/allowed, therefore immediately fail with no further processing
		return false;
	}

	if (!url.Scheme.StartsWith("http"))
	{
		// Currently we can only validate http/https URL's, therefore simply return true for any others.
		return true;
	}

	try
	{
		var message = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);

		System.Net.Http.HttpClient client = null;

		if (string.IsNullOrEmpty(proxyAddress))
		{
			Console.WriteLine("Creating new HttpClient");
			client = new System.Net.Http.HttpClient();
		}
		else
		{
			System.Net.Http.HttpClientHandler httpClientHandler = null;
			if (!string.IsNullOrEmpty(proxyUserName) && !string.IsNullOrEmpty(proxyPassword))
			{
				Console.WriteLine("Creating new HttpClient with authenticated proxy using web address: {0}", proxyAddress);
				httpClientHandler = new System.Net.Http.HttpClientHandler
				{
					Proxy = new System.Net.WebProxy(proxyAddress, BypassOnLocal: false, BypassList: null, Credentials: new System.Net.NetworkCredential(proxyUserName, proxyPassword)),
					UseProxy = true
				};
			}
			else
			{
				Console.WriteLine("Creating new HttpClient with proxy using web address: {0}", proxyAddress);

				httpClientHandler = new System.Net.Http.HttpClientHandler
				{
					Proxy = new System.Net.WebProxy(proxyAddress, BypassOnLocal: false),
					UseProxy = true
				};
			}
			
			httpClientHandler.AllowAutoRedirect = true;

			client = new System.Net.Http.HttpClient(httpClientHandler);
		}

		client.Timeout = TimeSpan.FromSeconds(30);

		client.DefaultRequestHeaders.Add("Connection", "keep-alive");

		message.Headers.Add("Upgrade-Insecure-Requests", "1");
		message.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36");
		message.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
		message.Headers.Add("Sec-Fetch-Mode", "navigate");
		message.Headers.Add("Sec-Fetch-Dest", "document");
		message.Headers.Add("Sec-Fetch-Site", "cross-site");
		message.Headers.Add("Sec-Fetch-User", "?1");
		message.Headers.Add("Accept-Encoding", "gzip, deflate, br");
		message.Headers.Add("Accept-Language", "en-GB,en-US;q=0.8,en;q=0.6,de-DE;q=0.4,de;q=0.2");

		var response = client.SendAsync(message).GetAwaiter().GetResult();

		if (response.ReasonPhrase == "Permanent Redirect")
		{
			Console.WriteLine("Received a HTTP Status Code of 308, which this version of .Net Framework HttpClient doesn't understand, assuming that this is a valid URL.");
			Console.WriteLine("This check was put in place as a result of this issue: https://github.com/chocolatey/package-validator/issues/247");
			return true;
		}
		
		if (response.StatusCode == System.Net.HttpStatusCode.Forbidden && response.Headers.Server.ToString() == "cloudflare")
		{
			Console.WriteLine("Received a Forbidden response validating Url {0}", url.ToString());
			Console.WriteLine("Since this is likely due to the fact that the server is using Cloudflare, is sometimes popping up a Captcha which needs to be solved, obviously not possible by package-validator.");
			Console.WriteLine("This check was put in place as a result of this issue: https://github.com/chocolatey/package-validator/issues/229");
			return true;
		}

		return response.StatusCode == System.Net.HttpStatusCode.OK;
	}
	catch (Exception ex)
	{
		var exceptions = new List<Exception>();
		var currentException = ex;
		exceptions.Add(currentException);

		while (currentException.InnerException != null)
		{
			exceptions.Add(currentException.InnerException);
			currentException = currentException.InnerException;
		}

		foreach (var exception in exceptions)
		{
			if (exception.Message == "Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host." ||
				exception.Message == "An existing connection was forcibly closed by the remote host." ||
				exception.Message == "The request was aborted: Could not create SSL/TLS secure channel.")
			{
				Console.WriteLine("Error validating Url {0} - {1}", url.ToString(), exception.Message);
				Console.WriteLine("Since this is likely due to missing Ciphers on the machine hosting package-validator, this URL will be marked as valid for the time being.");
				return true;
			}
		}

		Console.WriteLine("Error validating Url {0} - {1}", url.ToString(), ex.Message);
		return false;
	}
}
// Define other methods and classes here