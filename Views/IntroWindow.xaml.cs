using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ImageLabelingAvalonia.Models;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media;
using Avalonia.Input;

namespace ImageLabelingAvalonia.Views
{
    /// This is the main window of the app
    public class IntroWindow : Window
    {
        private Button _inputButton, _outputButton, _startButton;
        private TextBox _inputTextBox, _outputTextBox, _nameTextBox, _class1, _class2, _class3, _class4, _class5, _class6, _class7, _class8, _class9;
        private TextBlock _errorTextBlock;
        private CheckBox _doResume;
        private List<TextBox> classesTextBoxes = new List<TextBox>();

        private Brush activeBorder = new SolidColorBrush(Colors.Blue);
        private Brush inactiveBorder = new SolidColorBrush(new Color(255,136,136,136));
        
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
            classesTextBoxes.Add(_class1);
            classesTextBoxes.Add(_class2);
            classesTextBoxes.Add(_class3);
            classesTextBoxes.Add(_class4);
            classesTextBoxes.Add(_class5);
            classesTextBoxes.Add(_class6);
            classesTextBoxes.Add(_class7);
            classesTextBoxes.Add(_class8);
            classesTextBoxes.Add(_class9);
            _doResume.Click += OnCheckBoxClick;
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
            _class1 = this.FindControl<TextBox>("TxtBoxClass1");
            _class2 = this.FindControl<TextBox>("TxtBoxClass2");
            _class3 = this.FindControl<TextBox>("TxtBoxClass3");
            _class4 = this.FindControl<TextBox>("TxtBoxClass4");
            _class5 = this.FindControl<TextBox>("TxtBoxClass5");
            _class6 = this.FindControl<TextBox>("TxtBoxClass6");
            _class7 = this.FindControl<TextBox>("TxtBoxClass7");
            _class8 = this.FindControl<TextBox>("TxtBoxClass8");
            _class9 = this.FindControl<TextBox>("TxtBoxClass9");
        }

        
        public async void OnInputButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog() { Title = "Input Folder"};
            var res = await dialog.ShowAsync(this);
            _inputTextBox.Text = res == null ? string.Empty : res;
            if(_inputTextBox.Text != string.Empty && _outputTextBox.Text != string.Empty)
                _startButton.IsEnabled = true;
            else
                _startButton.IsEnabled = false;
        }

        public async void OnOutputButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog() { Title = "Input Folder"};
            var res = await dialog.ShowAsync(this);
            _outputTextBox.Text = res == null ? string.Empty : res;
            if(_inputTextBox.Text != string.Empty && _outputTextBox.Text != string.Empty)
                _startButton.IsEnabled = true;
            else
                _startButton.IsEnabled = false;
        }

        public void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            ImageLabeling.input_path = _inputTextBox.Text;
            ImageLabeling.output_path = _outputTextBox.Text;
            ImageLabeling.isResuming = _doResume.IsChecked == null ? false : (bool)_doResume.IsChecked;


            // check there are images in the input folder
            if(!CheckValidImagesInInputPath())
            {
                _errorTextBlock.Text ="There are no image files of the proper extensions in the input filder you selected.";
                return;
            }


            // check labeling name
            if(_nameTextBox.Text.Length < 1)
            {
                _errorTextBlock.Text = "You must choose a name for the result folder.";
                return;
            }
            ImageLabeling.labeling_name = _nameTextBox.Text;


            // check classes
            string[] classes;
            if(!(bool)_doResume.IsChecked)
            {
                classes = classesTextBoxes.Where(x => x.Text.Length > 0).Select(x => x.Text).ToArray();
                if(classes.Length < 2)
                {
                    _errorTextBlock.Text = "The number of labels must be between 2 and 9.";
                    return;
                }
                // check that classes are unique
                if(classes.Distinct().Count() != classes.Length)
                {
                    _errorTextBlock.Text = "All labels must be unique.";
                    return;
                }
                ImageLabeling.classes = classes;
            }
            else
            {
                // check if resuming but no csv
                if(!CheckCSVExistsIfResuming())
                {
                    _errorTextBlock.Text = "You are trying to resume but there was no CSV found in the folders and names specified.";
                    return;
                }

                using(var reader = new StreamReader(Path.Combine(_outputTextBox.Text, _nameTextBox.Text, ImageLabeling.csv_name)))
                {
                    string header = reader.ReadLine();

                    if(header == null)
                    {
                        _errorTextBlock.Text = "The CSV file seems to be corrupted.";
                        return;
                    }

                    var splits = header.Split(",");

                    classes = new string[splits.Length - 1];
                    for (int i = 1; i < splits.Length; i++)
                    {
                        classes[i-1] = splits[i];
                    }
                }
                ImageLabeling.classes = classes;
            }
            
            
            // check if you are not resuming but output+name folder already has a csv
            if(!ImageLabeling.isResuming && File.Exists(Path.Combine(ImageLabeling.output_path, ImageLabeling.labeling_name, ImageLabeling.csv_name)))
            {
                _errorTextBlock.Text = "There seems to be already a CSV file in this folder, please use a different folder or name for your output.";
                return;
            }

            // System.Console.WriteLine($"{ImageLabeling.input_path}");
            // System.Console.WriteLine($"{ImageLabeling.output_path}");
            // System.Console.WriteLine($"{ImageLabeling.labeling_name}");
            // System.Console.WriteLine($"{ImageLabeling.classes[0]}");
            // System.Console.WriteLine($"{ImageLabeling.isResuming}");
            
            var window = new MainWindow()
            { 
             WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.Manual,
             WindowState = Avalonia.Controls.WindowState.Maximized,
             SizeToContent = Avalonia.Controls.SizeToContent.WidthAndHeight
            };
            window.Show(); 
            this.Close();
        }


        private bool CheckValidImagesInInputPath()
        {
            return Directory.EnumerateFiles(ImageLabeling.input_path)
            .Count(x => ImageLabeling.extensions.Any(ext => ext == Path.GetExtension(x).ToLower())) > 0;
        }


        private bool CheckCSVExistsIfResuming()
        {
            return File.Exists(Path.Combine(_outputTextBox.Text, _nameTextBox.Text, ImageLabeling.csv_name));
        }


        private void OnCheckBoxClick(object sender, RoutedEventArgs e)
        {
            CheckBox check = sender as CheckBox;
            foreach (var box in classesTextBoxes)
            {
                if((bool)check.IsChecked)
                {
                    box.IsEnabled = false;
                    box.BorderBrush = inactiveBorder;
                }
                else
                {
                    box.IsEnabled = true;
                    box.BorderBrush = activeBorder;
                }
            }
        }
    }
}