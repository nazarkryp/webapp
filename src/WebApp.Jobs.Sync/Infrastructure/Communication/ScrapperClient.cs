using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace WebApp.Jobs.Sync.Infrastructure.Communication
{
    internal class ScrapperClient : IScrapperClient, IDisposable
    {
        private List<ProxyClient> _proxyClients;
        private int current = 0;

        public ScrapperClient(IScrapperConfiguration configuration)
        {
            _proxyClients = CreateHttpClients(configuration.Proxies);
        }

        private ProxyClient Next()
        {
            _proxyClients = _proxyClients.Where(e => !e.HasProxy || e.HasProxy && (e.Status == ProxyStatus.None || e.Status == ProxyStatus.Valid)).ToList();

            if (!_proxyClients.Any())
            {
                throw new Exception("No Valid Client");
            }

            var client = _proxyClients.ElementAt(current);

            if (_proxyClients.Count == 1)
            {
                return client;
            }

            if (current == _proxyClients.Count - 1)
            {
                current = 0;
            }

            client = _proxyClients.ElementAt(current);

            current++;

            return client;
        }

        public async Task<string> GetAsync(string uri)
        {
            ProxyClient client;

            while ((client = Next()) != null)
            {
                try
                {
                    var response = await client.GetAsync(uri);
                    var content = await response.Content.ReadAsStringAsync();

                    return content;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    // ignored
                }
            }

            throw new Exception("No valid client");
        }

        public void Dispose()
        {
            foreach (var proxyClient in _proxyClients)
            {
                proxyClient.Dispose();
            }
        }

        private static List<ProxyClient> CreateHttpClients(IReadOnlyCollection<string> proxies = null)
        {
            var httpClients = proxies?.Count > 0 ? new List<ProxyClient>(proxies.Count) : new List<ProxyClient>(1);

            if (proxies?.Count > 0)
            {
                foreach (var proxy in proxies)
                {
                    var handler = new HttpClientHandler
                    {
                        UseProxy = true,
                        Proxy = new WebProxy
                        {
                            Address = new Uri(proxy),
                            UseDefaultCredentials = false
                        }
                    };

                    var proxyClient = new ProxyClient(handler);
                    httpClients.Add(proxyClient);
                }
            }
            else
            {
                var client = new ProxyClient(new HttpClientHandler());
                httpClients.Add(client);
            }

            return httpClients;
        }
    }

    public class ProxyClient : HttpClient
    {
        private readonly HttpClientHandler _handler;
        private int errors = 0;

        private string ipAddress;

        public ProxyClient(HttpClientHandler handler)
            : base(handler)
        {
            _handler = handler;
        }

        public bool HasProxy => _handler.UseProxy;

        public ProxyStatus Status { get; set; } = ProxyStatus.None;

        private int counter = 0;

        public new async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            await GetClientIpAddress();

            try
            {
                Interlocked.Increment(ref counter);
                Console.Write($"{counter}.\t");

                if (HasProxy)
                {
                    Console.Write($"{ipAddress} - ");
                }

                Console.WriteLine(requestUri);

                var response = await base.GetAsync(requestUri);

                if (!response.IsSuccessStatusCode && HasProxy)
                {
                    errors++;
                }
                else if (errors > 1 && HasProxy)
                {
                    errors = 0;
                }

                return response;
            }
            catch (Exception)
            {
                if (HasProxy)
                {
                    errors++;
                }

                throw;
            }
        }

        public async Task<string> GetClientIpAddress()
        {
            if (!string.IsNullOrEmpty(ipAddress))
            {
                return ipAddress;
            }

            HttpResponseMessage response;

            try
            {
                response = await base.GetAsync("https://api.ipify.org/?format=json");
            }
            catch (Exception e)
            {
                Status = ProxyStatus.Invalid;
                throw new Exception("Proxy is not valid");
            }

            if (!response.IsSuccessStatusCode)
            {
                Status = ProxyStatus.Invalid;
                throw new Exception("Proxy is not valid");
            }

            var content = await response.Content.ReadAsStringAsync();
            ipAddress = JsonConvert.DeserializeObject<ProxyIp>(content).IpAddress;

            return content;
        }
    }

    public enum ProxyStatus
    {
        Invalid = -1,
        None = 0,
        Valid = 1
    }

    public class ProxyIp
    {
        [JsonProperty("ip")]
        public string IpAddress { get; set; }
    }
}
