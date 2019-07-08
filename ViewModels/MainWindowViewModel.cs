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
    /// View Model of the Main Window
    public class MainWindowViewModel : ReactiveObject
    {
        /// This is the list of the image files (with full path)
        public List<string> Files { get; private set; } = new List<string>();
        /// This is the list of the image files with just the filename
        public List<string> FileNames { get; private set; } = new List<string>();
        /// this is the list of the images objects
        public List<ImageLabel> Images { get; private set; } = new List<ImageLabel>();
        /// this is the list of the tagged images objects
        public List<ImageLabel> TaggedImages { get; private set; } = new List<ImageLabel>();
        private string[] extensions = new string[] { ".jpg", ".jpeg", ".bmp"};
        /// this is the current index of the carousel
        public int CurrentIndex { get; set; }

        private int currentProgress;
        /// this represents the current progress  of the labeling operation
        public int CurrentProgress
        {
            get { return currentProgress; }
            set { this.RaiseAndSetIfChanged(ref currentProgress, value); }
        }
        

        private string currentFileName;
        /// this represents the filename of the currently displayed image in the carousel
        public string CurrentFileName
        {
            get { return currentFileName; }
            set { this.RaiseAndSetIfChanged(ref currentFileName, value); }
        }

        private int currentTaggedCount;
        /// this represents the current number of tagged images
        public int CurrentTaggedCount
        {
            get { return TaggedImages.Count; }
            set { this.RaiseAndSetIfChanged(ref currentTaggedCount, value); }
        }
        
        
        /// Main view model, which takes width and height of screen to set limits to image size
        public MainWindowViewModel(int width, int height)
        {
            foreach (var file in Directory.EnumerateFiles(ImageLabeling.input_path)
                    .Where( x=> extensions.Any(ext => ext == Path.GetExtension(x).ToLower())).OrderBy(x => x))
            {
                Files.Add(file);
                FileNames.Add(Path.GetFileName(file));
                Images.Add(new ImageLabel() { Filename = Path.GetFileName(file), 
                                              Filepath = file,
                                              Image = new Image()
                                                { 
                                                    Source = new Bitmap(file),
                                                    MaxWidth = width*0.9,
                                                    MaxHeight = height*0.9,
                                                    Stretch = Avalonia.Media.Stretch.Uniform
                                                },
                                                isTagged = false,
                                                Tag = String.Empty});
            }

            if(Files.Count == 0)
            {
                System.Console.WriteLine("There are no image files of the proper extensions in the input filder you selected.");
                System.Console.WriteLine("Please try again with a valid input folder.");
                App.Current.Exit();
            }

            // init the currentX properties
            CurrentIndex = 0;
            CurrentProgress = 0;
            CurrentFileName = FileNames[CurrentIndex];


            if(ImageLabeling.isResuming)
            {
                string line;
                // we already checked that the file exists and contains a valid header in Program.cs
                System.IO.StreamReader file = new System.IO.StreamReader(Path.Combine(ImageLabeling.output_path, ImageLabeling.labeling_name, ImageLabeling.csv_name));  
                
                // loop through the lines to read the records if there are
                while((line = file.ReadLine()) != null)  
                {
                    var splits = line.Split(",");

                    if(splits[0] == "Filepath")
                        continue;

                    var image = Images.Where( x => x.Filepath == splits[0]).First();
                    image.isTagged = true;
                    for (int i = 1; i < splits.Length; i++)
                    {
                        if(splits[i] == "1")
                            image.Tag = ImageLabeling.classes[i-1];
                    }
                    TaggedImages.Add(image);
                }  
                file.Dispose();

                CurrentTaggedCount = TaggedImages.Count;

                // update the current index to be that of the first untagged image
                for (int i = 0; i < Files.Count; i++)
                {
                    if(!TaggedImages.Select(x => x.Filepath).Contains(Files[i]))
                        break;
                    else
                        CurrentIndex++;
                }
                if(CurrentIndex == Files.Count)
                    CurrentIndex = 0;
                
                CurrentFileName = FileNames[CurrentIndex];
                CurrentProgress = (int)(((float)TaggedImages.Count/Images.Count)*100);
            }
        }

        /// this method updates the index and current file name in the VM
        public void UpdateIndex(int index)
        {
            CurrentIndex = index;
            CurrentFileName = FileNames[CurrentIndex];
        }

        /// this method labels or unlables an image
        public void LabelImage(string label)
        {
            if (Images[CurrentIndex].isTagged && Images[CurrentIndex].Tag == label)
            {
                Images[CurrentIndex].isTagged = false;
                Images[CurrentIndex].Tag = string.Empty;
                TaggedImages.Remove(Images[CurrentIndex]);
                CurrentTaggedCount = TaggedImages.Count;
                CurrentProgress = (int)(((float)TaggedImages.Count/Images.Count)*100);
            }
            else if (Images[CurrentIndex].isTagged)
            {
                Images[CurrentIndex].Tag = label;
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

        /// this method is called when we close the window and it writes the CSV and copies the tagged images in their respective folder
        public void OnWindowClosed(object sender, CancelEventArgs e)
        {
            if(ImageLabeling.isResuming)
                // first delete everything in the results folder
                Directory.Delete(Path.Combine(ImageLabeling.output_path, ImageLabeling.labeling_name), true);

            // create or recreate the directories
            Directory.CreateDirectory(Path.Combine(ImageLabeling.output_path, ImageLabeling.labeling_name));
            foreach (var clas in ImageLabeling.classes)
            {
                Directory.CreateDirectory(Path.Combine(ImageLabeling.output_path, ImageLabeling.labeling_name,clas));
            }
            
            
            var records = new List<dynamic>();
            using(var writer = new StreamWriter(Path.Combine(ImageLabeling.output_path, ImageLabeling.labeling_name, ImageLabeling.csv_name)))
            using(var csv = new CsvWriter(writer))
            {
                //hacking this to be able to write the header even if no image is tagged
                string[] header = new string[ImageLabeling.classes.Length + 1];
                header[0] = "Filepath";
                for (int i = 0; i < ImageLabeling.classes.Length; i++)
                {
                    header[i+1] = ImageLabeling.classes[i];
                }
                writer.WriteLine(string.Join(",", header));

                string[] row = new string[ImageLabeling.classes.Length + 1];
                foreach (var image in TaggedImages.OrderBy( x=> x.Filename))
                {
                    row[0] = image.Filepath;
                    for (int i = 0; i < ImageLabeling.classes.Length; i++)
                    {
                        if(image.Tag == ImageLabeling.classes[i])
                        {
                            row[i + 1] = "1";
                            File.Copy(image.Filepath, Path.Combine(ImageLabeling.output_path, ImageLabeling.labeling_name,ImageLabeling.classes[i], image.Filename));
                        }
                            
                        else
                            row[i + 1] = "0";
                    }
                    writer.WriteLine(string.Join(",", row));
                }

                // foreach (var image in TaggedImages.OrderBy(x => x.Filename))
                // {
                //     var record = new ExpandoObject() as IDictionary<string, Object>;
                //     record.Add("Filepath", image.Filepath);
                //     foreach (var clas in ImageLabeling.classes)
                //     {
                //         if(clas == image.Tag)
                //         {
                //             record.Add(clas, "1");
                //             File.Copy(image.Filepath, Path.Combine(ImageLabeling.output_path, ImageLabeling.labeling_name,clas, image.Filename));
                //         }
                            
                //         else
                //             record.Add(clas, "0");
                //     }
                //     records.Add(record);
                // }
                // csv.WriteRecords(records);
            }
        }
    }
}
