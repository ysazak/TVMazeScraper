using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TVMazeScraper.Models;

namespace TVMazeScraper
{
    public class TVMazeService : ITVMazeService
    {
        private const int maxPageSize = 250;

        private readonly string showsPath = "shows";
        private readonly TVMazeHttpClient tvMazeHttpClient;
        private const int RateLimitErrorStatusCode = 429;

        public TVMazeService(TVMazeHttpClient tvMazeHttpClient)
        {
            this.tvMazeHttpClient = tvMazeHttpClient;
        }

        public async Task<List<Show>> GetShows(int pageNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            var prms = new Dictionary<string, string> { { "page", pageNumber.ToString() } };
            var showUri = new Uri(this.tvMazeHttpClient.HttpClient.BaseAddress, showsPath);
            var pagedShowsUri = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(showUri.ToString(), prms);

            using (var request = new HttpRequestMessage(HttpMethod.Get, pagedShowsUri))
            using (var response = await tvMazeHttpClient.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                using (Stream s = await response.Content.ReadAsStreamAsync())
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (StreamReader sr = new StreamReader(s))
                        {
                            using (JsonReader reader = new JsonTextReader(sr))
                            {
                                JsonSerializer serializer = new JsonSerializer();

                                return serializer.Deserialize<List<Show>>(reader);
                            }
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return new List<Show>();
                    }
                    else if (response.StatusCode == (HttpStatusCode)RateLimitErrorStatusCode)
                    {
                        throw new RateLimitExceededException
                        {
                            StatusCode = (int)response.StatusCode,
                            Content = "Rate Limit"
                        };
                    }
                    else
                    {
                        string errorContent = null;
                        if (s != null)
                        {
                            using (StreamReader sr = new StreamReader(s))
                            {
                                errorContent = await sr.ReadToEndAsync();
                            }
                        }
                        throw new ExternalApiException
                        {
                            StatusCode = (int)response.StatusCode,
                            Content = errorContent
                        };
                    }
                }
            }

        }

        public async Task<List<Cast>> GetShowCast(int showId, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{showsPath}/{showId}/cast"))
            using (var response = await tvMazeHttpClient.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                using (Stream s = await response.Content.ReadAsStreamAsync())
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (StreamReader sr = new StreamReader(s))
                        {
                            using (JsonReader reader = new JsonTextReader(sr))
                            {
                                JsonSerializer serializer = new JsonSerializer();

                                return serializer.Deserialize<List<Cast>>(reader);
                            }
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return new List<Cast>();
                    }
                    else if (response.StatusCode == (HttpStatusCode)RateLimitErrorStatusCode)
                    {
                        throw new RateLimitExceededException
                        {
                            StatusCode = (int)response.StatusCode,
                            Content = "Rate Limit Exceeded"
                        };
                    }
                    else
                    {
                        string errorContent = null;
                        if (s != null)
                        {
                            using (StreamReader sr = new StreamReader(s))
                            {
                                errorContent = await sr.ReadToEndAsync();
                            }
                        }
                        throw new ExternalApiException
                        {
                            StatusCode = (int)response.StatusCode,
                            Content = errorContent
                        };
                    }
                }
            }
        }
    }

    public class ExternalApiException : Exception
    {
        public int StatusCode { get; set; }
        public string Content { get; set; }
    }

    public class RateLimitExceededException : Exception
    {
        public int StatusCode { get; set; }
        public string Content { get; set; }
    }
}
