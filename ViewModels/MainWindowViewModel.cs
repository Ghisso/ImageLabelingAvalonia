using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using CsvHelper;
using ImageLabelingAvalonia.Models;
using ReactiveUI;

namespace ImageLabelingAvalonia.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        public List<string> Files { get; private set; } = new List<string>();
        public List<string> FileNames { get; private set; } = new List<string>();
        public List<ImageLabel> Images { get; private set; } = new List<ImageLabel>();
        public List<ImageLabel> TaggedImages { get; private set; } = new List<ImageLabel>();
        private string[] extensions = new string[] { ".jpg", ".jpeg", ".bmp"};
        public int CurrentIndex { get; set; }

        private int currentProgress;
        public int CurrentProgress
        {
            get { return currentProgress; }
            set { this.RaiseAndSetIfChanged(ref currentProgress, value); }
        }
        

        private string currentFileName;
        public string CurrentFileName
        {
            get { return currentFileName; }
            set { this.RaiseAndSetIfChanged(ref currentFileName, value); }
        }

        private int currentTaggedCount;
        public int CurrentTaggedCount
        {
            get { return TaggedImages.Count; }
            set { this.RaiseAndSetIfChanged(ref currentTaggedCount, value); }
        }
        
        
        
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
                                              Filepath = file,
                                              Image = new Image()
                                                { 
                                                    Source = new Bitmap(file),
                                                    MaxWidth = width*0.7,
                                                    MaxHeight = height*0.7,
                                                    Stretch = Avalonia.Media.Stretch.Uniform
                                                }, isTagged = false,
                                                Tag = String.Empty});
            }
            CurrentIndex = 0;
            CurrentProgress = 0;
            CurrentFileName = FileNames[CurrentIndex];
        }

        public void UpdateIndex(int index)
        {
            CurrentIndex = index;
            CurrentFileName = FileNames[CurrentIndex];
        }


        public void LabelImage(string label)
        {
            if (Images[CurrentIndex].isTagged)
            {
                Images[CurrentIndex].isTagged = false;
                Images[CurrentIndex].Tag = string.Empty;
                TaggedImages.Remove(Images[CurrentIndex]);
                CurrentTaggedCount = TaggedImages.Count;
                CurrentProgress = (int)(((float)TaggedImages.Count/Images.Count)*100);
            }
            else
            {
                Images[CurrentIndex].isTagged = true;
                Images[CurrentIndex].Tag = label;
                TaggedImages.Add(Images[CurrentIndex]);
                CurrentTaggedCount = TaggedImages.Count;
                CurrentProgress = (int)(((float)TaggedImages.Count/Images.Count)*100);
            }
            System.Console.WriteLine($"Image at index {CurrentIndex} was labeled {Images[CurrentIndex].Tag}");
        }

        public void OnWindowClosed(object sender, CancelEventArgs e)
        {
            System.Console.WriteLine("In closing");


            if(!ImageLabeling.isResuming)
            {
                Directory.CreateDirectory(Path.Combine(ImageLabeling.output_path, ImageLabeling.labeling_name));
                foreach (var clas in ImageLabeling.classes)
                {
                    Directory.CreateDirectory(Path.Combine(ImageLabeling.output_path, ImageLabeling.labeling_name,clas));
                }
            }
            

            var records = new List<dynamic>();
            using(var writer = new StreamWriter(Path.Combine(ImageLabeling.output_path, ImageLabeling.labeling_name, ImageLabeling.csv_name)))
            using(var csv = new CsvWriter(writer))
            {
                foreach (var image in TaggedImages)
                {
                    var record = new ExpandoObject() as IDictionary<string, Object>;
                    record.Add("Filepath", image.Filepath);
                    foreach (var clas in ImageLabeling.classes)
                    {
                        if(clas == image.Tag)
                        {
                            record.Add(clas, "1");
                            File.Copy(image.Filepath, Path.Combine(ImageLabeling.output_path, ImageLabeling.labeling_name,clas, image.Filename));
                        }
                            
                        else
                            record.Add(clas, "0");
                    }
                    records.Add(record);
                }
                csv.WriteRecords(records);
            }
            System.Console.WriteLine("End of closing");
        }
    }
}
