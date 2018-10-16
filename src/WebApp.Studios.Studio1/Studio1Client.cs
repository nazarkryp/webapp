using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;

namespace WebApp.Studios.Studio1
{
    public class Studio1Client : StudioClient, IStudioClient
    {
        private const string BaseAddress = Studio1ClientConstants.BaseAddress;
        private const string UpdatesStringFormat = Studio1ClientConstants.UpdatesStringFormat;

        public Studio1Client()
            : base(true)
        {
        }

        public string StudioName => Studio1ClientConstants.StudioName;

        public async Task<int> GetPagesCountAsync()
        {
            var requestUri = string.Format(UpdatesStringFormat, BaseAddress, 1);
            var encryptedUri = EncryptionHelper.Encrypt(requestUri);
            requestUri = $"{Studio1ClientConstants.Proxy}/v1/proxy?requestUri=base64_{encryptedUri}";

            var config = Configuration.Default.WithDefaultLoader();
            var document = await BrowsingContext.New(config).OpenAsync(requestUri);

            var last = document.QuerySelector(".paginationui-nav.last").Children.FirstOrDefault(e => e.LocalName == "a");
            var href = last?.GetAttribute("href");
            var arr = href.Split('/');

            return int.Parse(href.EndsWith("/") ? arr[arr.Length - 2] : arr[arr.Length - 1]);
        }

        public async Task<IEnumerable<StudioMovie>> GetMoviesAsync(int page)
        {
            var requestUri = string.Format(UpdatesStringFormat, BaseAddress, page);
            var config = Configuration.Default.WithDefaultLoader();
            var encryptedUri = EncryptionHelper.Encrypt(requestUri);
            requestUri = $"{Studio1ClientConstants.Proxy}/v1/proxy?requestUri=base64_{encryptedUri}";

            var document = await BrowsingContext.New(config).OpenAsync(requestUri);
            var items = document.All.Where(element => element.LocalName == "div" && element.ClassList.Contains("release-card-wrap"));

            var movies = items.Select(e => ParseElement(e));

            return movies;
        }

        public async Task<StudioMovie> GetMovieDetailsAsync(string requestUri)
        {
            //var config = Configuration.Default.WithDefaultLoader();
            //var encryptedUri = EncryptionHelper.Encrypt(requestUri);
            //requestUri = $"/v1/proxy?requestUri=base64_{encryptedUri}";
            //var document = await BrowsingContext.New(config).OpenAsync(requestUri);

            var content = await GetAsync(requestUri);

            return await ParseDetailsAsync(content, requestUri);
        }

        private StudioMovie ParseElement(IElement element)
        {
            var movie = new StudioMovie();

            var link = element.QuerySelectorAll("a.sample-picker").FirstOrDefault();
            var time = element.QuerySelector("time");

            var url = $"{BaseAddress}{link?.GetAttribute("href")}";
            var movieUri = new Uri(url);

            if (!string.IsNullOrEmpty(movieUri.Query))
            {
                url = url.Remove(url.IndexOf(movieUri.Query, StringComparison.CurrentCultureIgnoreCase), movieUri.Query.Length);
            }

            movie.Title = link?.GetAttribute("title");
            movie.Uri = url;
            movie.Description = element.QuerySelector(".scene-postcard-description")?.TextContent?.Trim();

            movie.Models = element.QuerySelector(".model-names")?.QuerySelectorAll("a")?.Select(e => e.TextContent.Trim()).Distinct();

            movie.Attachments = link?.Children.Where(e => e.LocalName == "img").Select(e =>
            {
                var value = e.GetAttribute("data-src");
                var uri = value.StartsWith("//", StringComparison.Ordinal) ? $"http://{value.Substring("//".Length)}" : value;

                return uri;
            }).ToArray();

            string timeValue;

            if (!string.IsNullOrEmpty(timeValue = time.InnerHtml))
            {
                if (timeValue.Trim().StartsWith(@"\n"))
                {
                    timeValue = timeValue.Substring(2);
                }

                movie.Date = DateTime.Parse(timeValue.Trim());
            }

            if (string.IsNullOrEmpty(movie.Title))
            {
                movie = ParseLegacyElement(element);
            }

            if (string.IsNullOrEmpty(movie.Description))
            {
                movie.Description = string.Empty;
            }

            return movie;
        }

        private static StudioMovie ParseLegacyElement(IElement element)
        {
            var movie = new StudioMovie();

            var card = element.QuerySelector(".card-image");

            var a = card.QuerySelectorAll("a").FirstOrDefault();

            movie.Title = a?.GetAttribute("title");
            var url = $"{BaseAddress}{a?.GetAttribute("href")}";
            var movieUri = new Uri(url);

            if (!string.IsNullOrEmpty(movieUri.Query))
            {
                url = url.Remove(url.IndexOf(movieUri.Query, StringComparison.CurrentCultureIgnoreCase), movieUri.Query.Length);
            }

            movie.Uri = url;

            var models = element.QuerySelector(".model-names")?.QuerySelectorAll("a")?.Select(e => e.TextContent.Trim()).Distinct();

            movie.Models = models;

            var imgSrc = a?.QuerySelector("img")?.GetAttribute("data-src");

            if (!string.IsNullOrEmpty(imgSrc))
            {
                movie.Attachments = new List<string>
                {
                    imgSrc.StartsWith("//") ? $"https://{imgSrc}" : imgSrc
                };
            }

            movie.Description = element.QuerySelector(".scene-postcard-description")?.TextContent?.Trim();

            var date = element.QuerySelector(".scene-postcard-date").TextContent.Trim();

            if (DateTime.TryParse(date, out var dateTime))
            {
                movie.Date = dateTime;
            }

            return movie;
        }

        public async Task<StudioMovie> ParseDetailsAsync(string html, string requestUri)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseAsync(html);

            var durationElement = document.QuerySelector(".scene-length")?.TextContent;
            TimeSpan? duration = TimeSpan.Zero;
            if (!string.IsNullOrEmpty(durationElement))
            {
                durationElement = Regex.Match(durationElement, @"\d+").Value;
                duration = TimeSpan.FromMinutes(int.Parse(durationElement));
            }
            else
            {
                Console.WriteLine($"Duration in this movie is missing: {requestUri}");
            }

            var description = document.QuerySelector("#scene-description")?.QuerySelector("p");
            var child = description?.QuerySelector(".collapse");

            if (child != null)
            {
                description.RemoveChild(description.QuerySelector(".collapse"));
            }

            var categories = document.QuerySelector(".tag-card-container")?.QuerySelectorAll("a")?.Select(e => e.TextContent.Trim()).Distinct().ToList();

            if (categories == null || !categories.Any())
            {
                categories = new List<string> { "Other" };
            }

            var movie = new StudioMovie
            {
                Attachments = null,
                Duration = duration,
                Description = description?.TextContent.Trim(),
                Categories = categories,
                Models = document.QuerySelector(".related-model").QuerySelectorAll("a").Select(e => e.TextContent.Trim()).Distinct()
            };

            return movie;
        }
    }

    public static class EncryptionHelper
    {
        public static string Encrypt(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);

            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_");
        }

        public static string Decrypt(string input)
        {
            var base64 = input.Replace("-", "+").Replace("_", "/");
            var decryptedBytes = Convert.FromBase64String(base64);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
