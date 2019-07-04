using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ImageLabelingAvalonia.Models;

namespace ImageLabelingAvalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public List<string> Files { get; private set; } = new List<string>();
        public List<string> FileNames { get; private set; } = new List<string>();
        //public List<Image> Images { get; private set; } = new List<Image>();
        public List<ImageLabel> Images { get; private set; } = new List<ImageLabel>();
        private string[] extensions = new string[] { ".jpg", ".jpeg", ".bmp"};
        
        public MainWindowViewModel(int width, int height)
        {
            foreach (var file in Directory.EnumerateFiles(ImageLabeling.input_path)
                    .Where( x=> extensions.Any(ext => ext == Path.GetExtension(x))).OrderBy(x => x))
            {
                Files.Add(file);
                FileNames.Add(Path.GetFileName(file));
            }

            foreach (var file in Files)
            {
                Images.Add(new ImageLabel() { Filename = Path.GetFileName(file), 
                                              Image = new Image()
                                                { 
                                                    Source = new Bitmap(file),
                                                    MaxWidth = width*0.7,
                                                    MaxHeight = height*0.7,
                                                    Stretch = Avalonia.Media.Stretch.Uniform
                                                }, isTagged = false,
                                                Tag = String.Empty});
            }
        }

        public void LabelImage(string label, int index)
        {
            System.Console.WriteLine($"Image at index {index} was labeled {label}");
        }
    }
}
