﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AngleSharp;
using AngleSharp.Dom;

using WebApp.Studios.Brazzers.Models;

namespace WebApp.Studios.Brazzers
{
    public class BrazzersClient : IStudioClient
    {
        private const string BaseAddress = "https://tour.brazzersnetwork.com";

        public string StudioName => "Brazzers";

        public async Task<int> GetPagesCountAsync()
        {
            var requestUri = $"{BaseAddress}/videos/all-sites/all-pornstars/all-categories/alltime/bydate/1/";
            var encryptedUri = EncryptionHelper.Encrypt(requestUri);
            requestUri = $"https://thephotocloud.com/v1/proxy?requestUri=base64_{encryptedUri}";

            var config = Configuration.Default.WithDefaultLoader();
            var document = await BrowsingContext.New(config).OpenAsync(requestUri);

            var last = document.QuerySelector(".paginationui-nav.last").Children.FirstOrDefault(e => e.LocalName == "a");
            var href = last?.GetAttribute("href");
            var arr = href.Split('/');

            return int.Parse(href.EndsWith("/") ? arr[arr.Length - 2] : arr[arr.Length - 1]);
        }

        public async Task<IEnumerable<IMovie>> GetMoviesAsync(int page)
        {
            var requestUri = $"{BaseAddress}/videos/all-sites/all-pornstars/all-categories/alltime/bydate/{page}/";
            var config = Configuration.Default.WithDefaultLoader();
            var encryptedUri = EncryptionHelper.Encrypt(requestUri);
            requestUri = $"https://thephotocloud.com/v1/proxy?requestUri=base64_{encryptedUri}";

            Console.WriteLine($"Getting: {requestUri}");

            var document = await BrowsingContext.New(config).OpenAsync(requestUri);
            var items = document.All.Where(element => element.LocalName == "div" && element.ClassList.Contains("release-card-wrap"));

            var movies = items.Select(e => ParseElement(e));

            return movies;
        }

        private Movie ParseElement(IElement element)
        {
            var movie = new Movie();

            var link = element.QuerySelectorAll("a.sample-picker").FirstOrDefault();
            var time = element.QuerySelector("time");

            movie.Title = link?.GetAttribute("title");
            movie.Uri = $"{BaseAddress}{link?.GetAttribute("href")}";
            movie.Description = element.QuerySelector(".scene-postcard-description")?.TextContent?.Trim();

            var models = element.QuerySelector(".model-names")?.QuerySelectorAll("a")?.Select(e => e.TextContent);

            movie.Models = models?.Select(e => new Model
            {
                Name = e
            });

            movie.Attachments = link?.Children.Where(e => e.LocalName == "img").Select(e =>
            {
                var value = e.GetAttribute("data-src");
                var uri = value.StartsWith("//", StringComparison.Ordinal) ? $"http://{value.Substring("//".Length)}" : value;

                return new Attachment
                {
                    Uri = uri
                };
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
                var uri = movie.Uri;
            }

            return movie;
        }

        private static Movie ParseLegacyElement(IElement element)
        {
            var movie = new Movie();

            var card = element.QuerySelector(".card-image");

            var a = card.QuerySelectorAll("a").FirstOrDefault();

            movie.Title = a?.GetAttribute("title");
            movie.Uri = $"{BaseAddress}{a?.GetAttribute("href")}";

            var models = element.QuerySelector(".model-names")?.QuerySelectorAll("a")?.Select(e => e.TextContent);

            movie.Models = models?.Select(e => new Model
            {
                Name = e
            });

            var imgSrc = a?.QuerySelector("img")?.GetAttribute("data-src");

            if (!string.IsNullOrEmpty(imgSrc))
            {
                movie.Attachments = new List<IAttachment>
                {
                    new Attachment
                    {
                        Uri = imgSrc.StartsWith("//") ? $"https://{imgSrc}" : imgSrc
                    }
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
