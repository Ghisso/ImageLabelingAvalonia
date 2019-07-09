using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ImageLabelingAvalonia.ViewModels;
using ImageLabelingAvalonia.Models;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.Collections.Generic;
using ReactiveUI;
using Avalonia.Data;

namespace ImageLabelingAvalonia.Views
{
    /// This is the main window of the app
    public class MainWindow : Window
    {
        private Carousel _carousel;
		private Button _left;
		private Button _right;
        private WrapPanel _bottomPanel;
        private WrapPanel _countPanel;
        private TextBlock _txtBlockFilename;
        private MainWindowViewModel _context;
        private List<Button> LabelButtons = new List<Button>();
        private List<TextBlock> CountTextBlocks = new List<TextBlock>();

        /// The main window of the app
        public MainWindow()
        {
            InitializeComponent();
            MaxWidth = Screens.Primary.Bounds.Width;
            MaxHeight = Screens.Primary.Bounds.Height;
            
            DataContext = new MainWindowViewModel(Screens.Primary.Bounds.Width, Screens.Primary.Bounds.Height, this);
            _context = (DataContext as MainWindowViewModel);
                
            for (int i = 0; i < ImageLabeling.classes.Length; i++)
            {
                var btn = new Button() 
                {
                    Content = ImageLabeling.classes[i],
                    Background = Brushes.Silver,
                    HotKey = new Avalonia.Input.KeyGesture(Avalonia.Input.Key.D1 + i),
                    Width = 100,
                    Margin = new Avalonia.Thickness(10),
                    FontSize = 16
                };
                btn.Command = ReactiveCommand.Create(() => _context.LabelImage((string)btn.Content));
                LabelButtons.Add(btn);
                btn.Click += OnButtonClick;
                _bottomPanel.Children.Add(btn);
                
                System.Console.WriteLine($"For label {ImageLabeling.classes[i]}, count is {_context.TaggedImages.Count(x => x.Tag == ImageLabeling.classes[i]).ToString()}");
                var txtBlock = new TextBlock()
                {
                    Width = 100,
                    Margin = new Avalonia.Thickness(10),
                    FontSize = 16,
                    TextAlignment = Avalonia.Media.TextAlignment.Center
                };
                CountTextBlocks.Add(txtBlock);
                _countPanel.Children.Add(txtBlock);
            }

            // button to bind the exit command
            var tmpButton = new Button()
            {
                IsEnabled = false,
                IsVisible = false,
                HotKey = new Avalonia.Input.KeyGesture(Avalonia.Input.Key.X, Avalonia.Input.InputModifiers.Control),
                Height = 1,
                Width = 1
            };
            tmpButton.Command = ReactiveCommand.Create(() => { _context.OnWindowClosed(null, null); App.Current.Exit(); });
            _bottomPanel.Children.Add(tmpButton);

            _carousel.Items = _context.Images.Select(x => x.Image);
            _carousel.SelectedIndex = _context.CurrentIndex;

            _left.Click += OnPreviousButtonClick;
            
			_right.Click += OnNextButtonClick;

            Closing += _context.OnWindowClosed;
            CheckPreviousNextButtonsStatus();
            CheckButtonStatus();
            updateCountText();
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _carousel = this.FindControl<Carousel>("carousel");
			_left = this.FindControl<Button>("left");
			_right = this.FindControl<Button>("right");
            _bottomPanel = this.FindControl<WrapPanel>("bottomPanel");
            _countPanel = this.FindControl<WrapPanel>("countPanel");
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
        }

        /// This method updates the color of the buttons if they are tagged
        public void CheckButtonStatus()
        {
            foreach (var button in LabelButtons)
            {
                if(_context.Images[_context.CurrentIndex].isTagged && _context.Images[_context.CurrentIndex].Tag == (string)button.Content)
                    button.Background = Brushes.Aqua;
                else
                    button.Background = Brushes.Silver;
            }
        }

        /// This method handles clicking on the Previous button
        public void OnPreviousButtonClick(object sender, RoutedEventArgs e)
        {
            _carousel.Previous();
            _context.UpdateIndex(_carousel.SelectedIndex);
            CheckButtonStatus();
            CheckPreviousNextButtonsStatus();
            updateCountText();
        }

        /// This method handles clicking on the Next button
        public void OnNextButtonClick(object sender, RoutedEventArgs e)
        {
            _carousel.Next();
            _context.UpdateIndex(_carousel.SelectedIndex);
            CheckButtonStatus();
            CheckPreviousNextButtonsStatus();
            updateCountText();
        }
        
        /// This method updates the count of each class (couldn't do it by binding...)
        public void updateCountText()
        {
            for (int i = 0; i < CountTextBlocks.Count; i++)
            {
                CountTextBlocks[i].Text = _context.PerTagCount[ImageLabeling.classes[i]].ToString();
            }
        }
    }
}