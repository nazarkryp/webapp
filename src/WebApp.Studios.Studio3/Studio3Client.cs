using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using AngleSharp.Dom;
using AngleSharp.Parser.Html;

namespace WebApp.Studios.Studio3
{
    public class Studio3Client : StudioClient, IStudioClient
    {
        private const string BaseAddress = Studio3ClientConstants.BaseAddress;
        private const string UpdatesStringFormat = Studio3ClientConstants.UpdatesStringFormat;

        public Studio3Client()
            : base(false)
        {
        }

        public string StudioName => Studio3ClientConstants.StudioName;

        public async Task<int> GetPagesCountAsync()
        {
            var requestUri = string.Format(UpdatesStringFormat, BaseAddress, 1);
            var content = await GetAsync(requestUri);
            var parser = new HtmlParser();
            var document = await parser.ParseAsync(content);

            var last = document.QuerySelectorAll("a.page-link").LastOrDefault();
            var href = last?.GetAttribute("href");
            var pageString = href.Split('/').Last();

            return int.Parse(pageString);
        }

        #region Public Methods

        public async Task<IEnumerable<StudioMovie>> GetMoviesAsync(int page)
        {
            var requestUri = string.Format(UpdatesStringFormat, BaseAddress, page);
            var content = await GetAsync(requestUri);
            var parser = new HtmlParser();
            var document = await parser.ParseAsync(content);

            var items = document.QuerySelectorAll(".card.m-1").ToList();

            var movies = items.Select(ParseElement).ToList();

            return movies;
        }

        public async Task<StudioMovie> GetMovieDetailsAsync(string movieUri)
        {
            var content = await GetAsync(movieUri);

            return await ParseDetailsAsync(content, movieUri);
        }

        public async Task<StudioMovie> ParseDetailsAsync(string html, string movieUri)
        {
            var parser = new HtmlParser();
            var document = await parser.ParseAsync(html);

            var title = document.QuerySelector(".video-titles h1").TextContent.Trim();
            var categories = document.QuerySelector(".tags-box").QuerySelectorAll("a")?.Select(e => e.TextContent.Trim()).Distinct();
            var description = document.QuerySelector(".box-container")?.TextContent?.Trim();
            var attachments = document.QuerySelector(".photo-slider-guest").QuerySelectorAll("a").Select(e => e.GetAttribute("href")).Distinct();
            var models = document.QuerySelector(".pornstars-box").QuerySelectorAll("a.name").Select(e => e.TextContent.Trim()).Distinct();
            var dateString = document.QuerySelector("time").TextContent;
            var date = DateTime.Parse(dateString);
            var durationString = document.QuerySelector(".info-panel.duration")?.QuerySelector(".duration")?.TextContent?.Trim();
            durationString = durationString.Contains("min") ? Regex.Match(durationString, @"\d+").Value : durationString;
            var duration = TimeSpan.FromMinutes(int.Parse(durationString));
            duration = duration > TimeSpan.FromHours(23) ? TimeSpan.FromHours(23) : duration;

            var studioMovie = new StudioMovie
            {
                Title = title,
                Description = description,
                Categories = categories,
                Models = models,
                Attachments = attachments,
                Uri = movieUri,
                Date = date,
                Duration = duration
            };

            return studioMovie;
        }

        #endregion

        #region Private Methods

        private static StudioMovie ParseElement(IElement element)
        {
            var movie = new StudioMovie();

            movie.Title = element.QuerySelector(".card-title a")?.TextContent;
            movie.Models = element.QuerySelector("h6.card-subtitle")?.QuerySelectorAll("a").Select(e => e.TextContent);
            var dateString = element.QuerySelector("small.text-muted")?.TextContent;
            movie.Date = DateTime.Parse(dateString);
            var src = element.QuerySelectorAll(".card-img-top")?.FirstOrDefault(e => e.HasAttribute("src"))?.GetAttribute("src");
            movie.Attachments = new[] { src };
            var durationString = element.QuerySelector(".card-info")?.QuerySelectorAll(".d-inline-flex")?.FirstOrDefault(e => e.InnerHtml.Contains("video_icon_white"))?.TextContent?.Trim();
            durationString = durationString.Contains("min") ? Regex.Match(durationString, @"\d+").Value : durationString;
            movie.Duration = TimeSpan.FromMinutes(int.Parse(durationString));
            movie.Duration = movie.Duration > TimeSpan.FromHours(23) ? TimeSpan.FromHours(23) : movie.Duration;
            movie.Uri = $"{BaseAddress}{element.QuerySelector(".card-title a")?.GetAttribute("href")}";

            return movie;
        }

        #endregion
    }
}
