using Avalonia.Controls;

namespace ImageLabelingAvalonia.Models
{
    public class ImageLabel
    {
        public string Filename { get; set; }
        public Image Image { get; set; }
        public bool isTagged { get; set; }
        public string Tag { get; set; }
    }
}