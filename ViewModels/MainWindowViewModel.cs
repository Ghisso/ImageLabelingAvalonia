using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ImageLabelingAvalonia.Models;
using ImageLabelingAvalonia.Views;
using ReactiveUI;

namespace ImageLabelingAvalonia.ViewModels
{
    /// View Model of the Main Window
    public class MainWindowViewModel : ReactiveObject
    {
        private MainWindow _mainWindow;
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
        /// this represents the number of tagged images for each tag
        public Dictionary<string, int> PerTagCount { get; set; } = new Dictionary<string, int>();
        
        
        /// Main view model, which takes width and height of screen to set limits to image size
        public MainWindowViewModel(int width, int height, MainWindow window)
        {
            // get ref of main window
            _mainWindow = window;

            // get all the image files and fill the different lists
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

            // sanity check if no images
            if(Files.Count == 0)
            {
                System.Console.WriteLine("There are no image files of the proper extensions in the input filder you selected.");
                System.Console.WriteLine("Please try again with a valid input folder.");
                App.Current.Exit();
            }

            // init the currentX properties and per tag count
            CurrentIndex = 0;
            CurrentProgress = 0;
            CurrentFileName = FileNames[CurrentIndex];

            foreach (var clas in ImageLabeling.classes)
            {
                PerTagCount[clas] = 0;
            }

            // if resuming, read csv, get class names and tagged images
            if(ImageLabeling.isResuming)
            {
                string line;
                // we already checked that the file exists and contains a valid header in Program.cs
                using(System.IO.StreamReader file = new System.IO.StreamReader(Path.Combine(ImageLabeling.output_path, ImageLabeling.labeling_name, ImageLabeling.csv_name))) 
                {
                    // loop through the lines to read the records if there are
                    while((line = file.ReadLine()) != null)  
                    {
                        var splits = line.Split(",");

                        // this is header, skip it
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
                        PerTagCount[image.Tag]++;
                    }  
                }

                CurrentTaggedCount = TaggedImages.Count;

                // update the current index to be that of the first untagged image
                for (int i = 0; i < Files.Count; i++)
                {
                    if(!TaggedImages.Select(x => x.Filepath).Contains(Files[i]))
                        break;
                    else
                        CurrentIndex++;
                }
                // if all images are tagged, set index to first image
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
            // case : removing tag from tagged image
            if (Images[CurrentIndex].isTagged && Images[CurrentIndex].Tag == label)
            {
                Images[CurrentIndex].isTagged = false;
                Images[CurrentIndex].Tag = string.Empty;
                TaggedImages.Remove(Images[CurrentIndex]);
                CurrentTaggedCount = TaggedImages.Count;
                CurrentProgress = (int)(((float)TaggedImages.Count/Images.Count)*100);
                PerTagCount[label]--;
            }
            // case : change tag of tagged image
            else if (Images[CurrentIndex].isTagged)
            {
                PerTagCount[Images[CurrentIndex].Tag]--;
                Images[CurrentIndex].Tag = label;
                PerTagCount[label]++;
            }
            // case : tag untagged image
            else
            {
                Images[CurrentIndex].isTagged = true;
                Images[CurrentIndex].Tag = label;
                TaggedImages.Add(Images[CurrentIndex]);
                CurrentTaggedCount = TaggedImages.Count;
                CurrentProgress = (int)(((float)TaggedImages.Count/Images.Count)*100);
                PerTagCount[label]++;
            }

            // wanted to call these from mainWindow but doesn't update immediately there don't know why....
            _mainWindow.CheckButtonStatus();
            _mainWindow.updateCountText();
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
            
            
            // write to CSV and copy images
            using(var writer = new StreamWriter(Path.Combine(ImageLabeling.output_path, ImageLabeling.labeling_name, ImageLabeling.csv_name)))
            {
                //hacking this to be able to write the header even if no image is tagged
                string[] header = new string[ImageLabeling.classes.Length + 1];
                header[0] = "Filepath";
                for (int i = 0; i < ImageLabeling.classes.Length; i++)
                {
                    header[i+1] = ImageLabeling.classes[i];
                }
                writer.WriteLine(string.Join(",", header));

                // create row for each tagged image and write it to CSV and copy image to correct folder
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
            }
        }


        /// This is the command for the keyboard shortcuts for left arrow
        public void OnClickPrevious()
        {
            _mainWindow.OnPreviousButtonClick(null, null);
        }


        /// This is the command for the keyboard shortcuts for right arrow
        public void OnClickNext()
        {
            _mainWindow.OnNextButtonClick(null, null);
        }
    }
}
