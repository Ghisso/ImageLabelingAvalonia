using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ImageLabelingAvalonia.ViewModels;
using ImageLabelingAvalonia.Models;
using Avalonia.Data;
using Avalonia.Interactivity;

namespace ImageLabelingAvalonia.Views
{
    public class MainWindow : Window
    {
        private Carousel _carousel;
		private Button _left;
		private Button _right;
        private StackPanel _bottomPanel;
        private TextBlock _txtBlockFilename;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(Screens.Primary.Bounds.Width, Screens.Primary.Bounds.Height);
            foreach (var clas in ImageLabeling.classes)
            {
                var btn = new Button() 
                {
                    Content = clas
                };
                btn.Click += OnButtonClick;
                _bottomPanel.Children.Add(btn);
            }
            _carousel.Items = (DataContext as MainWindowViewModel).Images.Select(x => x.Image);
            _left.Click += (s, e) =>
            {
                _carousel.Previous();
                CheckPreviousNextButtonsStatus();
            };
			_right.Click += (s, e) =>
            {
                _carousel.Next();
                CheckPreviousNextButtonsStatus();
            };
            CheckPreviousNextButtonsStatus();
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _carousel = this.FindControl<Carousel>("carousel");
			_left = this.FindControl<Button>("left");
			_right = this.FindControl<Button>("right");
            _bottomPanel = this.FindControl<StackPanel>("bottomPanel");
            _txtBlockFilename = this.FindControl<TextBlock>("TxtBlocFilename");
        }

        private void CheckPreviousNextButtonsStatus()
        {
            var x = _carousel.ItemCount;
            if(_carousel.SelectedIndex == 0)
                _left.IsEnabled = false;
            else
                _left.IsEnabled = true;

            if(_carousel.SelectedIndex == (DataContext as MainWindowViewModel).Files.Count - 1)
                _right.IsEnabled = false;
            else
                _right.IsEnabled = true;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel).LabelImage((string)(sender as Button).Content, _carousel.SelectedIndex);
        }
    }
}