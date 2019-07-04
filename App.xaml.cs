using Avalonia;
using Avalonia.Markup.Xaml;

namespace ImageLabelingAvalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
   }
}