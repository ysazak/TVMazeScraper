using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TVMazeScraper
{
    public class TVMazeHttpClient
    {

        private const int RetryCount = 6;
        private const int WaitSec = 5;
        public TVMazeHttpClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == (System.Net.HttpStatusCode)429)
                .WaitAndRetryAsync(RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(WaitSec,
                                                                            retryAttempt)));
        }
    }
}
