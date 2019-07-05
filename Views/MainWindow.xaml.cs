using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ImageLabelingAvalonia.ViewModels;
using ImageLabelingAvalonia.Models;
using Avalonia.Data;
using Avalonia.Interactivity;
using System.Windows.Input;
using Avalonia.Media;
using System.Collections.Generic;
using System.ComponentModel;

namespace ImageLabelingAvalonia.Views
{
    public class MainWindow : Window
    {
        private Carousel _carousel;
		private Button _left;
		private Button _right;
        private StackPanel _bottomPanel;
        private TextBlock _txtBlockFilename;
        private MainWindowViewModel _context;
        private List<Button> LabelButtons = new List<Button>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(Screens.Primary.Bounds.Width, Screens.Primary.Bounds.Height);
            _context = (DataContext as MainWindowViewModel);


            foreach (var clas in ImageLabeling.classes)
            {
                var btn = new Button() 
                {
                    Content = clas,
                    Background = Brushes.Silver
                };
                LabelButtons.Add(btn);
                btn.Click += OnButtonClick;
                _bottomPanel.Children.Add(btn);
            }

            _carousel.Items = _context.Images.Select(x => x.Image);

            _left.Click += (s, e) =>
            {
                _carousel.Previous();
                _context.UpdateIndex(_carousel.SelectedIndex);
                CheckButtonStatus();
                CheckPreviousNextButtonsStatus();
            };

			_right.Click += (s, e) =>
            {
                _carousel.Next();
                _context.UpdateIndex(_carousel.SelectedIndex);
                CheckButtonStatus();
                CheckPreviousNextButtonsStatus();
            };

            Closing += _context.OnWindowClosed;
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

            if(_carousel.SelectedIndex == _context.Files.Count - 1)
                _right.IsEnabled = false;
            else
                _right.IsEnabled = true;
        }


        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            _context.LabelImage((string)(sender as Button).Content);
            if(_context.Images[_context.CurrentIndex].isTagged && _context.Images[_context.CurrentIndex].Tag == (string)(sender as Button).Content)
                (sender as Button).Background = Brushes.Aqua;
            else
                (sender as Button).Background = Brushes.Silver;
        }

        private void CheckButtonStatus()
        {
            foreach (var button in LabelButtons)
            {
                if(_context.Images[_context.CurrentIndex].isTagged && _context.Images[_context.CurrentIndex].Tag == (string)button.Content)
                    button.Background = Brushes.Aqua;
                else
                    button.Background = Brushes.Silver;
            }
        }

        
    }
}