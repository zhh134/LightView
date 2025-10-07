using ImageMagick;
using LightView.Models;
using System;

namespace LightView.Utils
{
    public class ImageResolutionHelper
    {
        /// <summary>
        /// 获取图片分辨率的详细方法
        /// </summary>
        public static ImageResolutionInfo GetDetailedResolutionInfo(string imagePath)
        {
            try
            {
                using (var image = new MagickImage(imagePath))
                {
                    var density = image.Density;

                    return new ImageResolutionInfo
                    {
                        FilePath = imagePath,
                        HorizontalResolution = density.X,
                        VerticalResolution = density.Y,
                        ResolutionUnits = density.Units.ToString(),
                        Width = image.Width,
                        Height = image.Height,
                        Format = image.Format.ToString(),
                        HasResolutionInfo = density.X > 0 && density.Y > 0
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"获取图片分辨率失败: {ex.Message}", ex);
            }
        }
    }

    

}