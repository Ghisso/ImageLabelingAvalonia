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
    public class IntroWindow : Window
    {
        private Button _inputButton, _outputButton, _startButton;
        private TextBox _inputTextBox, _outputTextBox, _nameTextBox;
        private CheckBox _doResume;
        
         public IntroWindow()
        {
            InitializeComponent();
            _inputButton.Click += OnInputButtonClick;
            _outputButton.Click += OnOutputButtonClick;
            _startButton.Click += OnStartButtonClick;
            _inputTextBox.Text = string.Empty;
            _outputTextBox.Text = string.Empty;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _inputButton = this.FindControl<Button>("BtnInput");
            _outputButton = this.FindControl<Button>("BtnOutput");
            _inputTextBox = this.FindControl<TextBox>("TxtBoxInput");
            _outputTextBox = this.FindControl<TextBox>("TxtBoxOutput");
            _nameTextBox = this.FindControl<TextBox>("TxtBoxName");
            _startButton = this.FindControl<Button>("BtnStart");
            _doResume = this.FindControl<CheckBox>("ChkBoxResume");
        }

        public async void OnInputButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog() { Title = "Input Folder"};
            _inputTextBox.Text = await dialog.ShowAsync(this);
            if(_inputTextBox.Text != string.Empty && _outputTextBox.Text != string.Empty)
                _startButton.IsEnabled = true;
        }

        public async void OnOutputButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog() { Title = "Input Folder"};
            _outputTextBox.Text = await dialog.ShowAsync(this);
            if(_inputTextBox.Text != string.Empty && _outputTextBox.Text != string.Empty)
                _startButton.IsEnabled = true;
        }

        public void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            ImageLabeling.input_path = _inputTextBox.Text;
            ImageLabeling.output_path = _outputTextBox.Text;
            ImageLabeling.labeling_name = _nameTextBox.Text;
            ImageLabeling.isResuming = _doResume.IsChecked == null ? false : (bool)_doResume.IsChecked;
            
            var window = new MainWindow()
            { 
             WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.Manual,
             WindowState = Avalonia.Controls.WindowState.Maximized,
             SizeToContent = Avalonia.Controls.SizeToContent.WidthAndHeight
            };
            window.Show(); 
            this.Close();
        }
    }
}