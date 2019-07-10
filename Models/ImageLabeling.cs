namespace ImageLabelingAvalonia.Models
{
    /// This class represents the main options of the labeling operation
    public static class ImageLabeling
    {
        /// Folder path of the images to be labeled
        public static string input_path;
        /// Folder path of where to save the results of the labeling
        public static string output_path;
        /// Name of the folder of where to save the results of the labeling
        public static string labeling_name;
        /// Name of the resulting CSV file
        public static string csv_name = "results.csv";
        /// List of labels
        public static string[] classes;
        /// Resuming a previous labeling operation or not
        public static bool isResuming;
        /// List of accepted image extensions
        public  static string[] extensions = new string[] { ".jpg", ".jpeg", ".bmp"};
    }
}