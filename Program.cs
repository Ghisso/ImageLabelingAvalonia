using System.IO;
using Avalonia;
using Avalonia.Logging.Serilog;
using ImageLabelingAvalonia.Views;
using ImageLabelingAvalonia.Models;

namespace ImageLabelingAvalonia
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            BuildAvaloniaApp().Start(AppMain, args);    
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
            // set window size and location in constructor
            var window = new IntroWindow()
            { 
             WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterScreen,
             SizeToContent = Avalonia.Controls.SizeToContent.WidthAndHeight
            };

            app.Run(window);
        }
    }
}
