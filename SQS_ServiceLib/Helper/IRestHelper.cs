using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQS_ServiceLib.Helper
{
    public interface IRestHelper
    {
        Task<HttpResponseMessage> RestConnect(string uri, string payload, string payloadType, Dictionary<string, string> headers, HttpMethod httpMethod, int retryCount = 3, int retrielDurationInSeconds = 5);
    }
}
