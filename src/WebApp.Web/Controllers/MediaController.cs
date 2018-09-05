using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using WebApp.Dto;
using WebApp.Messaging;
using WebApp.Services;
using WebApp.Storage;
using WebApp.Web.Messaging;

namespace WebApp.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;
        private readonly IStorage<string> _storage;
        private readonly IEventPublisher _eventPublisher;

        public MediaController(IMediaService mediaService, IStorage<string> storage, IEventPublisher eventPublisher)
        {
            _mediaService = mediaService;
            _storage = storage;
            _eventPublisher = eventPublisher;
        }

        [HttpGet]
        public async Task<IEnumerable<Media>> GetAllMediaAsync()
        {
            return await _mediaService.GetMediaAsync();
        }

        [HttpGet("{mediaId}")]
        public async Task<IActionResult> GetAsync(int mediaId)
        {
            var media = await _mediaService.GetMediaByIdsAsync(new List<int> { mediaId });

            return Ok(media);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMediaAsync(IFormFileCollection files)
        {
            if (files.Count == 0)
            {
                return BadRequest("No files in the request");
            }

            var uploads = new List<Media>();

            var tasks = files.Select(file => _storage.UploadAsync(file.OpenReadStream())).ToList();

            await Task.WhenAll(tasks);

            foreach (var task in tasks)
            {
                var result = await task;
                var media = await _mediaService.CreateMediaAsync(new MediaCreateOptions
                {
                    ObjectId = result.Identifier,
                    OriginalUri = result.OriginalUri,
                    Thumbnail = result.Thumbnail,
                    Small = result.Small,
                    Description = Request.Headers["description"]
                });

                uploads.Add(media);
            }

            return Ok(uploads);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveMediaAsync(MediaDeleteOptions options)
        {
            var media = await _mediaService.GetMediaByIdsAsync(options.MediaIds.ToArray());

            var objectIds = media.Select(e => e.ObjectId);

            // var result = await _storage.RemoveAsync(objectIds);

            await _eventPublisher.Publish(new WebAppEvent
            {
                ObjectIds = objectIds
            });

            return Ok(new
            {
                objectIds = objectIds
            });
        }

        [HttpDelete("all")]
        public async Task<IActionResult> RemoveAllMediaAsync()
        {
            var media = await _mediaService.GetMediaAsync();
            var mediaArr = media as Media[] ?? media.ToArray();
            var objectIds = mediaArr.Select(e => e.ObjectId);

            // var result = await _storage.RemoveAsync(objectIds);

            await _mediaService.RemoveMediaAsync(new MediaDeleteOptions
            {
                MediaIds = mediaArr.Select(e => e.MediaId)
            });

            // return Ok(result);
            return Ok(objectIds);
        }
    }
}
