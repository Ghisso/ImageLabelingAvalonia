namespace ImageLabelingAvalonia.Models
{
    public static class ImageLabeling
    {
        public static string input_path;
        public static string output_path;
        public static string labeling_name;
        public static string csv_name = "results.csv";
        public static string[] classes;
        public static bool isResuming;
    }
}