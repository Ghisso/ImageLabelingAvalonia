namespace ImageLabelingAvalonia.Models
{
    public static class ImageLabeling
    {
        public static readonly string input_path;
        public static readonly string output_path;
        public static readonly string labeling_name;
        public static readonly string csv_name;
        public static readonly string[] classes;
        public static readonly bool isResuming;

        static ImageLabeling()
        {
            input_path = @"/Users/ben.ghassen/Documents/Workstuff/Pole/inputfiles";
            output_path = @"/Users/ben.ghassen/Documents/Workstuff/Pole";
            labeling_name = "inputfiles-labeling";
            csv_name = "results.csv";
            classes = new string [] { "OK", "UNSURE", "NOTOK"};
            isResuming = false;
        }
    }
}