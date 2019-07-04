namespace ImageLabelingAvalonia.Models
{
    public static class ImageLabeling
    {
        public static readonly string input_path;
        public static readonly string output_path;
        public static readonly string labeling_name;
        public static readonly string csv_name;
        public static readonly string[] classes;

        static ImageLabeling()
        {
            input_path = @"/Users/ben.ghassen/Documents/Workstuff/Pole/images";
            output_path = @"/Users/ben.ghassen/Documents/Workstuff/Pole";
            labeling_name = "images-labeling";
            csv_name = "results.csv";
            classes = new string [] { "Ok", "UNSURE", "NOTOK"};
        }
    }
}