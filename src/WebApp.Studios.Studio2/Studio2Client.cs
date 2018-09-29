using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;

namespace WebApp.Studios.Studio2
{
    public class Studio2Client : StudioClient, IStudioClient
    {
        private const string BaseAddress = "https://tour.naughtyamerica.com";

        public Studio2Client()
        : base(true)
        {
        }

        public string StudioName => "Naughty America";

        public async Task<int> GetPagesCountAsync()
        {
            var requestUri = $"{BaseAddress}/new-porn-videos?page=1";

            //var encryptedUri = EncryptionHelper.Encrypt(requestUri);
            //requestUri = $"https://thephotocloud.com/v1/proxy?requestUri=base64_{encryptedUri}";

            //var config = Configuration.Default.WithDefaultLoader();
            //var document = await BrowsingContext.New(config).OpenAsync(requestUri);
            var content = await GetAsync(requestUri);
            var parser = new HtmlParser();
            var document = await parser.ParseAsync(content);

            var last = document.QuerySelectorAll(".pagination a").LastOrDefault();
            var href = last?.GetAttribute("href");
            var uri = new Uri(href);

            var result = HttpUtility.ParseQueryString(uri.Query).Get("page");

            return int.Parse(result);
        }

        public async Task<IEnumerable<StudioMovie>> GetMoviesAsync(int page)
        {
            var requestUri = $"{BaseAddress}/new-porn-videos?page={page}";
            var config = Configuration.Default.WithDefaultLoader();
            //var encryptedUri = EncryptionHelper.Encrypt(requestUri);
            //requestUri = $"https://thephotocloud.com/v1/proxy?requestUri=base64_{encryptedUri}";

            Console.WriteLine($"Getting: {requestUri}");

            //var document = await BrowsingContext.New(config).OpenAsync(requestUri);
            var content = await GetAsync(requestUri);
            var parser = new HtmlParser();
            var document = await parser.ParseAsync(content);
            //var items = document.All.Where(element => element.LocalName == "div" && element.ClassList.Contains("grid-item")).ToList();

            var items = document.QuerySelectorAll(".scene-item").ToList();

            foreach (var element in items)
            {
                var e = ParseLegacyElement(element);
            }

            var movies = items.Select(e => ParseLegacyElement(e));

            return movies;
        }

        public async Task<StudioMovie> GetMovieDetailsAsync(string requestUri)
        {
            // categories grey-text
            var config = Configuration.Default.WithDefaultLoader();
            //var document = await BrowsingContext.New(config).OpenAsync(requestUri);
            var content = await GetAsync(requestUri);
            var parser = new HtmlParser();
            var document = await parser.ParseAsync(content);

            var description = document.QuerySelector(".synopsis.grey-text");
            var child = description.QuerySelector(".light-grey-text");

            if (child != null)
            {
                description.RemoveChild(child);
            }

            var duration = TimeSpan.Zero;
            var durationElement = document.QuerySelector(".duration.light-grey-text")?.TextContent;
            if (durationElement != null)
            {
                durationElement = Regex.Match(durationElement, @"\d+").Value;
                duration = TimeSpan.FromMinutes(int.Parse(durationElement));
            }

            var attachments = document.QuerySelector(".contain-scene-images.desktop-only")?.QuerySelectorAll("img")
                .Select(e =>
                {
                    var src = e.GetAttribute("src");
                    return src.StartsWith("//") ? $"https:{src}" : src;
                });

            var studioMovie = new StudioMovie
            {
                Duration = duration,
                Description = description?.TextContent?.Trim(),
                Categories = document.QuerySelector(".categories.grey-text")?.QuerySelectorAll("a.cat-tag")?.Select(e => e.TextContent).Distinct(),
                Models = document.QuerySelectorAll(".scene-title.grey-text")?.Select(e => e.TextContent).Distinct(),
                Attachments = attachments
            };

            return studioMovie;
        }

        private StudioMovie ParseElement(IElement element)
        {
            var movie = new StudioMovie();

            if (element.Children.FirstOrDefault(child => child.ClassList.Contains("title")) is IHtmlAnchorElement titleElement)
            {
                movie.Title = titleElement.Text;
                movie.Uri = titleElement.Href;
            }

            if (element.Children.FirstOrDefault(child => child.ClassList.Contains("scene-thumb")) is IHtmlAnchorElement
                thumbElement)
            {
                var img = thumbElement.Children.FirstOrDefault(e => e.LocalName == "img") as IHtmlImageElement;

                if (img != null)
                {
                    movie.Attachments = new List<string>
                    {
                        img.Source.StartsWith("about://", StringComparison.Ordinal)
                                ? $"http://{img.Source.Substring("about://".Length)}"
                                : img.Source
                    };
                }
            }

            if (element.Children.FirstOrDefault(child => child.ClassList.Contains("entry-date")) is IHtmlParagraphElement paragraphElement)
            {
                movie.Date = DateTime.Parse(paragraphElement.InnerHtml);
            }

            return movie;
        }

        private static StudioMovie ParseLegacyElement(IElement element)
        {
            var movie = new StudioMovie();

            var a = element.QuerySelector(".contain-img");
            movie.Title = a.GetAttribute("title");
            movie.Uri = a.GetAttribute("href");

            var src = a.QuerySelectorAll("img").FirstOrDefault(e => string.IsNullOrEmpty(e.ClassName))?.GetAttribute("src");

            if (!string.IsNullOrEmpty(src))
            {
                movie.Attachments = new List<string>
                {
                    src.StartsWith("//") ? $"https:{src}" : src
                };
            }

            var date = element.QuerySelector(".entry-date").TextContent;
            movie.Date = DateTime.Parse(date);

            movie.Models = element.QuerySelector(".contain-actors").QuerySelectorAll("a.title").Select(e => e.TextContent).Distinct();

            return movie;
        }
    }
}
