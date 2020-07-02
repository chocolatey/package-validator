<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
</Query>

void Main()
{
	Console.WriteLine(url_is_valid(new Uri("https://www.amd.com/en")));
}

public static bool url_is_valid(Uri url)
{
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
		var client = new System.Net.Http.HttpClient();
		client.Timeout = TimeSpan.FromSeconds(30);
		
		client.DefaultRequestHeaders.Add("Connection", "keep-alive");
		
		message.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
		message.Headers.Add("Accept-Language", "en-GB,en-US;q=0.8,en;q=0.6,de-DE;q=0.4,de;q=0.2");
		message.Headers.Add("Upgrade-Insecure-Requests", "1");
		message.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36");
		message.Headers.Add("Accept-Encoding", "gzip, deflate, br");
		message.Headers.Add("Sec-Fetch-Mode", "navigate");
		message.Headers.Add("Sec-Fetch-Dest", "document");
		message.Headers.Add("Sec-Fetch-Site", "cross-site");
		message.Headers.Add("Sec-Fetch-User", "?1");
		
		var response = client.SendAsync(message).GetAwaiter().GetResult();
		return response.StatusCode == System.Net.HttpStatusCode.OK;
	}
    catch (System.Net.WebException ex)
    {	
        if (ex.Status == System.Net.WebExceptionStatus.ProtocolError && ex.Message == "The remote server returned an error: (403) Forbidden." && ex.Response.Headers["Server"] == "cloudflare")
        {
            Console.WriteLine("Error validating Url {0} - {1}", url.ToString(), ex.Message);
            Console.WriteLine("Since this is likely due to the fact that the server is using Cloudflare, is sometimes popping up a Captcha which needs to be solved, obviously not possible by package-validator.");
            Console.WriteLine("This check was put in place as a result of this issue: https://github.com/chocolatey/package-validator/issues/229");
            return true;
        }
        if (ex.Status == System.Net.WebExceptionStatus.SecureChannelFailure || (ex.Status == System.Net.WebExceptionStatus.UnknownError && ex.Message == "The SSL connection could not be established, see inner exception. Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host.."))
        {
            Console.WriteLine("Error validating Url {0} - {1}", url.ToString(), ex.Message);
            Console.WriteLine("Since this is likely due to missing Ciphers on the machine hosting package-validator, this URL will be marked as valid for the time being.");
            return true;
        }

        if (ex.Status == System.Net.WebExceptionStatus.ProtocolError && ex.Message == "The remote server returned an error: (503) Server Unavailable.")
        {
            Console.WriteLine("Error validating Url {0} - {1}", url.ToString(), ex.Message);
            Console.WriteLine("This could be due to Cloudflare DDOS protection acting in front of the site, or another valid reason, as such, this URL will be marked as valid for the time being.");
            return true;
        }

        Console.WriteLine("Web Exception - Error validating Url {0} - {1}", url.ToString(), ex.Message);
        return false;
    }
    catch (Exception ex)
    {
        Console.WriteLine("General Exception - Error validating Url {0} - {1}", url.ToString(), ex.Message);
        return false;
    }
}
// Define other methods and classes here