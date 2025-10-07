namespace LightView.Models
{
   
    public class ImageResolutionInfo
    {
        public string FilePath { get; set; }
        public double HorizontalResolution { get; set; } // 水平分辨率 (DPI)
        public double VerticalResolution { get; set; }   // 垂直分辨率 (DPI)
        public string ResolutionUnits { get; set; }      // 分辨率单位
        public int Width { get; set; }                   // 图片宽度 (像素)
        public int Height { get; set; }                  // 图片高度 (像素)
        public string Format { get; set; }               // 图片格式
        public bool HasResolutionInfo { get; set; }      // 是否有分辨率信息
    }

}
