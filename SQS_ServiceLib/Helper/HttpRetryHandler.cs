using System.Net;

namespace SQS_ServiceLib.Helper
{
    internal class HttpRetryHandler: DelegatingHandler
    {
        // Try for maximum time."
        private readonly int maxRetries;

        // Retry Interval in seconds
        private readonly int retryInterval;

        // Configurable status code where retry is required
        private readonly IEnumerable<HttpStatusCode> retyForStatusCodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRetryHandler"/> class.
        /// Constructor to form retry mechanism for HTTP Client
        /// </summary>
        /// <param name="retyForStatusCodes">Retry for HTTP status code. If null then will retry for InternalServerError and ServiceUnavailable.</param>
        /// <param name="maxRetryCount">Maximum retry for failure</param>
        /// <param name="retryInterval">Interval between the retry</param>
        public HttpRetryHandler(IEnumerable<HttpStatusCode>? retyForStatusCodes = null, int maxRetryCount = 3, int retryInterval = 5)
        : base(new HttpClientHandler())
        {
            this.maxRetries = maxRetryCount;
            this.retryInterval = retryInterval;
            this.retyForStatusCodes = retyForStatusCodes ?? new List<HttpStatusCode> { { HttpStatusCode.InternalServerError }, { HttpStatusCode.ServiceUnavailable }, { HttpStatusCode.RequestTimeout }, { HttpStatusCode.GatewayTimeout } };
        }

        /// <summary>
        /// Send Request as <see langword="async"/>
        /// </summary>
        /// <param name="request">HTTP request message</param>
        /// <param name="cancellationToken">Threading cancellation token</param>
        /// <returns>HTTP Response message</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new();
            if (this.maxRetries <= 0)
            {
                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                for (int i = 1; i <= this.maxRetries + 1; i++)
                {
                    response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    if (!this.retyForStatusCodes.Any(x => x == response.StatusCode))
                    {
                        return await Task.FromResult(response); ;
                    }

                    await Task.Delay(this.retryInterval * 1000 * i, cancellationToken).ConfigureAwait(false);
                }
            }

            return await Task.FromResult(response);
        }
    }
}
