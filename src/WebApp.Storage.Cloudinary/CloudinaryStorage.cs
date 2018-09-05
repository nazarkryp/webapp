using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using WebApp.Storage.Cloudinary.Configuration;
using WebApp.Storage.Models;

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using WebApp.Storage.Cloudinary.Models;

namespace WebApp.Storage.Cloudinary
{
    public class CloudinaryStorage : IStorage<string>
    {
        private readonly ICloudinaryConfiguration _configuration;
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public CloudinaryStorage(ICloudinaryConfiguration configuration)
        {
            _configuration = configuration;

            var account = new Account(
                _configuration.CloudName,
                _configuration.ClientKey,
                _configuration.ClientSecret);

            _cloudinary = new CloudinaryDotNet.Cloudinary(account);
        }

        public async Task<IUploadResult<string>> UploadAsync(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var parameters = new RawUploadParams
            {
                File = new FileDescription(Guid.NewGuid().ToString("N"), stream),
                Folder = _configuration.Directory,
                UniqueFilename = true
            };

            var result = await Task.Run(() => _cloudinary.Upload(parameters));

            string thumbnail, small;

            if (result.ResourceType == ResourceType.Video.ToString().ToLower())
            {
                thumbnail = _cloudinary.Api.UrlVideoUp.Transform(new Transformation()
                    .Width(300).Height(300).Crop("fill")).BuildUrl($"{result.PublicId}.{result.Format}");

                small = _cloudinary.Api.UrlVideoUp.Transform(new Transformation()
                    .Width(250).Height(250).Crop("crop").Chain()).BuildUrl($"{result.PublicId}.{result.Format}");
            }
            else
            {
                thumbnail = _cloudinary.Api.UrlImgUp.Transform(new Transformation()
                    .Width(300).Height(300).Gravity("face").Crop("fill")).BuildUrl($"{result.PublicId}.{result.Format}");

                small = _cloudinary.Api.UrlImgUp.Transform(new Transformation()
                    .Width(250).Height(250).Gravity("face").Radius("max").Crop("crop").Chain()
                    .Width(125).Crop("scale")).BuildUrl($"{result.PublicId}.{result.Format}");
            }

            return new CloudinaryUploadResult
            {
                Identifier = result.PublicId,
                OriginalUri = result.Uri.ToString(),
                Thumbnail = thumbnail,
                Small = small
            };
        }

        public async Task<Dictionary<string, string>> RemoveAsync(IEnumerable<string> objectIds)
        {
            var result = await Task.Run(() => _cloudinary.DeleteResources(objectIds.ToArray()));

            return result.Deleted;
        }
    }
}
