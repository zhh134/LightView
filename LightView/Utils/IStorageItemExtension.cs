using LightView.Models;
using Windows.Storage;
using System;

namespace LightView.Utils
{
    public static class IStorageItemExtension
    {
        public static ImageModel MapToImageModel(this IStorageItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            var imageModel = new ImageModel
            {
                Name = item.Name,
                Path = new Uri(item.Path),
                Type = CheckImageType(item.Name)
            };
            return imageModel;
        }

        public static ImageType CheckImageType(this string name)
        {
            var extension = System.IO.Path.GetExtension(name).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => ImageType.JPEG,
                ".png" => ImageType.PNG,
                ".bmp" => ImageType.BMP,
                ".gif" => ImageType.GIF,
                ".tiff" or ".tif" => ImageType.TIFF,
                _ => ImageType.Unknown,
            };
        }
    }
}
