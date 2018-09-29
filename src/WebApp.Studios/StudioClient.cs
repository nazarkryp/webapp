﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApp.Studios
{
    public abstract class StudioClient : IDisposable
    {
        private readonly HttpClient _client;

        protected StudioClient(bool useProxy)
        {
            var httpClientHandler = new HttpClientHandler();

            if (useProxy)
            {
                var proxy = new WebProxy
                {
                    Address = new Uri($"http://85.236.246.217:32356"),
                    UseDefaultCredentials = false
                };

                httpClientHandler.Proxy = proxy;
            }

            _client = new HttpClient(httpClientHandler);
        }

        public async Task<string> GetAsync(string uri)
        {
            try
            {
                var response = await _client.GetAsync(uri);

                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: {uri}");
                    Console.ForegroundColor = ConsoleColor.White;

                    throw new Exception($"HttpClient error: {response.StatusCode}");
                }

                return content;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {uri};\nException: {e.Message}\n");
                Console.ForegroundColor = ConsoleColor.White;
                throw;
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
