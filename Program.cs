using System.IO;
using Avalonia;
using Avalonia.Logging.Serilog;
using ImageLabelingAvalonia.ViewModels;
using ImageLabelingAvalonia.Views;
using ImageLabelingAvalonia.Models;
using CsvHelper;

namespace ImageLabelingAvalonia
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(DirectoryInfo input, DirectoryInfo output=null, string name = null, string[] classes = null, bool resume = false)
        {
            // check that input exists
            if(!input.Exists)
            {
                System.Console.WriteLine("The input directory does not exists. Please enter an existing directory.");
                return;
            }

            // check output and set it if it is not
            if(output != null  && !output.Exists)
            {
                System.Console.WriteLine("The out directory does not exists. Please enter an existing directory.");
                return;
            }
            if(output == null)
                output = input.Parent;

            // check name and set it if it is not
            if(name == null)
                name = input.Name + "-labels";
                


            if(resume)
            {
                if(!File.Exists(Path.Combine(output.FullName, name, ImageLabeling.csv_name)))
                {
                    System.Console.WriteLine("The 'results.csv' file could not be found in the specified path.");
                    System.Console.WriteLine("Please make sure that the 'results.csv' file exists and is in the right folder.");
                    return;
                }

                var reader = new StreamReader(Path.Combine(output.FullName, name, ImageLabeling.csv_name));
                
                string line = reader.ReadLine();
                if(line == null)
                {
                    System.Console.WriteLine("The csv file seems to be corrupted. Aborting...");
                    return;
                }
                var splits = line.Split(",");
                classes = new string[splits.Length - 1];
                for (int i = 1; i < splits.Length; i++)
                {
                    classes[i-1] = splits[i];
                }
                
                reader.Dispose();
            }
            else
            {
                if(classes != null && (classes.Length < 2 || classes.Length > 9))
                {
                    System.Console.WriteLine("The number of classes must be between 2 and 9.");
                    return;
                }
                if(classes == null)
                    classes = new[] {"OK", "UNSURE", "NOTOK"};

            }

            ImageLabeling.input_path = input.FullName;
            ImageLabeling.output_path = output.FullName;
            ImageLabeling.labeling_name = name;
            ImageLabeling.classes = classes;
            ImageLabeling.isResuming = resume;

            // System.Console.WriteLine($"{ImageLabeling.input_path}");
            // System.Console.WriteLine($"{ImageLabeling.output_path}");
            // System.Console.WriteLine($"{ImageLabeling.labeling_name}");
            // System.Console.WriteLine($"{ImageLabeling.classes[0]}");
            // System.Console.WriteLine($"{ImageLabeling.isResuming}");

            //uncomment below and comment up stuff to use debug ...
            // ImageLabeling.input_path = @"/Users/ben.ghassen/Documents/Workstuff/Pole/inputfiles";
            // ImageLabeling.output_path = @"/Users/ben.ghassen/Documents/Workstuff/Pole/";
            // ImageLabeling.labeling_name = "inputfiles-labels";
            // ImageLabeling.classes = new[] {"abc", "def", "ghi", "jkl"};
            // ImageLabeling.isResuming = resume;
            System.Console.WriteLine("Before BuildAvaloniaApp().Start(AppMain, null);");
            BuildAvaloniaApp().Start(AppMain, null);
        }  

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();

        // Your application's entry point. Here you can initialize your MVVM framework, DI
        // container, etc.
        private static void AppMain(Application app, string[] args)
        {
            var window = new MainWindow();
            app.Run(window);
        }
    }
}
