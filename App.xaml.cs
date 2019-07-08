using Avalonia;
using Avalonia.Markup.Xaml;

namespace ImageLabelingAvalonia
{
    /// This is the app
    public class App : Application
    {
        /// Init Avalonia
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
   }
}