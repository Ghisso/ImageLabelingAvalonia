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
using System.IO;
using Avalonia.Controls.Primitives;

namespace ImageLabelingAvalonia.Views
{
    /// This is the main window of the app
    public class IntroWindow : Window
    {
        private Button _inputButton, _outputButton, _startButton;
        private TextBox _inputTextBox, _outputTextBox, _nameTextBox;
        private TextBlock _errorTextBlock;
        private CheckBox _doResume;
        
         public IntroWindow()
        {
            InitializeComponent();
            _inputButton.Click += OnInputButtonClick;
            _outputButton.Click += OnOutputButtonClick;
            _startButton.Click += OnStartButtonClick;
            _inputTextBox.Text = "";
            _outputTextBox.Text = "";
            _errorTextBlock.Text = string.Empty;
            _nameTextBox.PropertyChanged += (s,e) => {_errorTextBlock.Text = string.Empty;};
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            _inputButton = this.FindControl<Button>("BtnInput");
            _outputButton = this.FindControl<Button>("BtnOutput");
            _inputTextBox = this.FindControl<TextBox>("TxtBoxInput");
            _outputTextBox = this.FindControl<TextBox>("TxtBoxOutput");
            _nameTextBox = this.FindControl<TextBox>("TxtBoxName");
            _errorTextBlock = this.FindControl<TextBlock>("TxtBlockError");
            _startButton = this.FindControl<Button>("BtnStart");
            _doResume = this.FindControl<CheckBox>("ChkBoxResume");
        }

        public async void OnInputButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog() { Title = "Input Folder"};
            var res = await dialog.ShowAsync(this);
            System.Console.WriteLine($"Res : {res}");
            System.Console.WriteLine($"Len res : {res.Length}");
            _inputTextBox.Text = res;
            if(_inputTextBox.Text != "" && _outputTextBox.Text != "")
                _startButton.IsEnabled = true;
            else
                _startButton.IsEnabled = false;
        }

        public async void OnOutputButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog() { Title = "Input Folder"};
            var res = await dialog.ShowAsync(this);
            System.Console.WriteLine($"Res : {res}");
            System.Console.WriteLine($"Len res : {res.Length}");
            _outputTextBox.Text = res;
            if(_inputTextBox.Text != "" && _outputTextBox.Text != "")
                _startButton.IsEnabled = true;
            else
                _startButton.IsEnabled = false;
        }

        public void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            ImageLabeling.input_path = _inputTextBox.Text;
            ImageLabeling.output_path = _outputTextBox.Text;
            ImageLabeling.labeling_name = _nameTextBox.Text;
            ImageLabeling.isResuming = _doResume.IsChecked == null ? false : (bool)_doResume.IsChecked;


            if(!ImageLabeling.isResuming && File.Exists(Path.Combine(ImageLabeling.output_path, ImageLabeling.labeling_name, ImageLabeling.csv_name)))
            {
                _errorTextBlock.Text = "There seems to be already a CSV file in this folder, please use a different folder or name for your output.";
                return;
            }
            
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