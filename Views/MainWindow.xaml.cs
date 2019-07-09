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
        // quick ref to the ViewModel
        private MainWindowViewModel _context;

        // refs to UI elements we need in code
        private Carousel _carousel;
		private Button _left;
		private Button _right;
        private WrapPanel _bottomPanel;
        private WrapPanel _countPanel;
        private TextBlock _txtBlockFilename;
        
        // Lists of tagging buttons and their corresponding count textBlocks
        private List<Button> LabelButtons = new List<Button>();
        private List<TextBlock> CountTextBlocks = new List<TextBlock>();


        /// The main window constructor of the app
        public MainWindow()
        {
            InitializeComponent();

            // get screen width and height
            MaxWidth = Screens.Primary.Bounds.Width;
            MaxHeight = Screens.Primary.Bounds.Height;
            
            DataContext = new MainWindowViewModel(Screens.Primary.Bounds.Width, Screens.Primary.Bounds.Height, this);
            // quick ref to ViewModel
            _context = (DataContext as MainWindowViewModel);
            
            // create buttons for each class and corresponding TextBlock for counts
            for (int i = 0; i < ImageLabeling.classes.Length; i++)
            {
                // create button and set its HotKey to be "1" + value of "i"
                var btn = new Button() 
                {
                    Content = ImageLabeling.classes[i],
                    Background = Brushes.Silver,
                    HotKey = new Avalonia.Input.KeyGesture(Avalonia.Input.Key.D1 + i),
                    Width = 100,
                    Margin = new Avalonia.Thickness(10),
                    FontSize = 16
                };
                // set callback for when using keyboard shortcut
                btn.Command = ReactiveCommand.Create(() => _context.LabelImage((string)btn.Content));
                // set callback for when clicking the button (basically same method as Command)
                btn.Click += OnButtonClick;
                // add the button to the UI as child of the bottom Panel
                _bottomPanel.Children.Add(btn);
                //add button to the list of buttons to be able to get a hold of them later
                LabelButtons.Add(btn);
                
                // create corresponding tag TextBlock
                var txtBlock = new TextBlock()
                {
                    Width = 100,
                    Margin = new Avalonia.Thickness(10),
                    FontSize = 16,
                    TextAlignment = Avalonia.Media.TextAlignment.Center
                };
                // add it to the UI as a child of the count Panel
                _countPanel.Children.Add(txtBlock);
                //add textBlock to the list of buttons to be able to get a hold of them later and update their value
                // couldn't find a way to bind their text property....
                CountTextBlocks.Add(txtBlock);
            }

            // button to bind the exit command, maybe should try bind to Panel if possible
            var tmpButton = new Button()
            {
                IsEnabled = false,
                IsVisible = false,
                HotKey = new Avalonia.Input.KeyGesture(Avalonia.Input.Key.X, Avalonia.Input.InputModifiers.Control),
                Height = 1,
                Width = 1
            };

            // set callback for when using keyboard shortcut
            tmpButton.Command = ReactiveCommand.Create(() => { _context.OnWindowClosed(null, null); App.Current.Exit(); });
            //add to UI, but it's invisible, inactive and super small so OK
            _bottomPanel.Children.Add(tmpButton);

            // set images as source items for the carousel and set its index to first untagged image
            _carousel.Items = _context.Images.Select(x => x.Image);
            _carousel.SelectedIndex = _context.CurrentIndex;

            // set callback for Previous button click
            _left.Click += OnPreviousButtonClick;
            
            // set callback for Next button click
			_right.Click += OnNextButtonClick;

            // set callback for closing the window
            Closing += _context.OnWindowClosed;

            // update UI
            CheckPreviousNextButtonsStatus();
            CheckButtonStatus();
            updateCountText();
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            // get refs of all the UI elements we need in code
            _carousel = this.FindControl<Carousel>("carousel");
			_left = this.FindControl<Button>("left");
			_right = this.FindControl<Button>("right");
            _bottomPanel = this.FindControl<WrapPanel>("bottomPanel");
            _countPanel = this.FindControl<WrapPanel>("countPanel");
            _txtBlockFilename = this.FindControl<TextBlock>("TxtBlocFilename");
        }


        /// This method disable the Previous and Next buttons if we have reached first or last item
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


        /// This method is the one bound to the tagging buttons, calls the Label() method of the ViewModel
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
            // update UI
            CheckButtonStatus();
            CheckPreviousNextButtonsStatus();
            updateCountText();
        }


        /// This method handles clicking on the Next button
        public void OnNextButtonClick(object sender, RoutedEventArgs e)
        {
            _carousel.Next();
            _context.UpdateIndex(_carousel.SelectedIndex);
            // update UI
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