using System.IO;
using Avalonia;
using Avalonia.Logging.Serilog;
using ImageLabelingAvalonia.ViewModels;
using ImageLabelingAvalonia.Views;
using ImageLabelingAvalonia.Models;

namespace ImageLabelingAvalonia
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(DirectoryInfo input, DirectoryInfo output=null, string name = "null", string[] classes = null, bool resume = false)
        {
            if(!input.Exists)
            {
                System.Console.WriteLine("The input directory does not exists. Please enter an existing directory.");
                return;
            }
            if(resume)
            {

            }
            else
            {
                if(output != null  && !output.Exists)
                {
                    System.Console.WriteLine("The out directory does not exists. Please enter an existing directory.");
                    return;
                }
                if(output == null)
                    output = input.Parent;


                if(name == null)
                    name = input.Name + "-labeling";


                if(classes != null && (classes.Length < 2 || classes.Length > 9))
                {
                    System.Console.WriteLine("The number of classes must be between 2 and 9.");
                    return;
                }
                if(classes == null)
                    classes = new[] {"OK", "UNSURE", "NOTOK"};

                ImageLabeling.input_path = input.FullName;
                ImageLabeling.output_path = output.FullName;
                ImageLabeling.labeling_name = name;
                ImageLabeling.classes = classes;
                ImageLabeling.isResuming = resume;

                // ImageLabeling.input_path = @"/Users/ben.ghassen/Documents/Workstuff/Pole/inputfiles";
                // ImageLabeling.output_path = @"/Users/ben.ghassen/Documents/Workstuff/Pole/";
                // ImageLabeling.labeling_name = "inputfiles-labels";
                // ImageLabeling.classes = new[] {"abc", "def", "ghi", "jkl"};
                // ImageLabeling.isResuming = resume;
            }
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
