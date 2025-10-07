using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using LightView.Utils;

namespace LightView.Models
{
    public class ImageModel
    {
        public string Name { get; set; }
        public Uri Path { get; set; }
        public ImageSource ImageSource => new BitmapImage(Path);
        public ImageType Type { get; set; }
        public string PathOringinalString => Path.OriginalString;

        public static ImageModel CreateFromPath(string path) => new ImageModel
        {
            Name = System.IO.Path.GetFileName(path),
            Path = new Uri(path),
            Type = System.IO.Path.GetFileName(path).CheckImageType()
        };
    }
}
