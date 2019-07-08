using Avalonia.Controls;

namespace ImageLabelingAvalonia.Models
{
    /// Model to represent a single image
    public class ImageLabel
    {
        /// File name of the image
        public string Filename { get; set; }
        /// Full file path (with name included) of the image
        public string Filepath { get; set; }
        /// The image itself
        public Image Image { get; set; }
        /// Whether this image is tagged ot not
        public bool isTagged { get; set; }
        /// The string of the tag
        public string Tag { get; set; }
    }
}