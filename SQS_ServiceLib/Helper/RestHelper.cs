using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace SQS_ServiceLib.Helper
{
    public class RestHelper : IRestHelper
    {
        // Generic Http Request caller with default retrial in case of specific status codes
        public async Task<HttpResponseMessage> RestConnect(string uri, string payload, string payloadType, Dictionary<string, string> headers, HttpMethod httpMethod, int retryCount = 3, int retrielDurationInSeconds = 5)
        {
            var result = new HttpResponseMessage();
            try
            {
                using var client = new HttpClient(new HttpRetryHandler(null, retryCount, retrielDurationInSeconds));
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                var request = new HttpRequestMessage(httpMethod, uri);
                if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put)
                {
                    var content = new StringContent(payload ?? string.Empty, Encoding.UTF8, payloadType);
                    result.Content = content;
                }

                // Call the API
                result = await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                result.Content = result.Content ?? new StringContent("{\"error\":{\"Error Header: " + ex.Message + Environment.NewLine + "Error Details:" + JsonConvert.SerializeObject(ex) + "\"}}");
                result.StatusCode = HttpStatusCode.InternalServerError;
            }

            return await Task.FromResult(result);
        }
    }
}
