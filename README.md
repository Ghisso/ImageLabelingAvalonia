# Image Classification Helper (Avalonia C# version)

## Overview

This app is a simple tool to assist in manually checking and classifying images. It allows you to browse through the images and tag them by pressing a single key. When done, the tagged images will be copied to the specified output directory and a CSV will be created summarizing the results.

## Usage

1. Just run "./ImageLabelingAvalonia" from the standalone folder if using oublished version.
2. Or do "dotnet run" if using from the source code. You must have .Net SDK 2.2 or 3.0 installed on your machine.

There are 5 options on the first window:

 1. input: Path to the folder containing the images.
 2. output: Path to where the output folder will be created.
 3. name: Name of the result folder where the results will be saved.
 4. classes: Class names for the classification. The number of classes must be between 2 and 9.
 5. resume: Indicate if this classification is a resumption of a previous one. There needs to be a results.csv file in the output folder and name in order to resume.

The app creates the result folder in the output folder and creates folders corresponding to each classification class designated in the introduction window. It parses the input directory content and builds a list of the image files (currently limited to ".jpg", ".jpeg", ".png", ".gif", ".bmp" extensions but can be extended).

It then displays the first image (or the first untagged image if resuming) in a window and awaits user input. Accepted user inputs are button presses or the following keyboard shortcuts:

- 1~9 keys: correspond to the class to assign to this image following the order passed in the command line. The image is tagged with the corresponding class and the button corresponding to the tag will apprear as though it is pressed and the count number under the button will be incremented. The next image is then displayed. To untag an image, simply press the button or key number corresponding to its current tag and the button will appear as not pressed and the count under the button will be decreased.
- "←" and "→" keys: to move through the images without classifying them. "←" is for the previous image and "→" is for the next.
- "CTRL+x" key: for exiting the program. You can also just close the window normally.

Exiting the program will copy all the classified images in their respective result folder and create a CSV file containing the file path of the original image and its classification in a one-hot-encoded vector. It is possible to exit the program without having classified all the images and resume it later.

To resume a classification, you must check the "resume" option in the first window, and it will read the CSV file and tag the already classified images according to the CSV file (you can of course change the classification for each image at any time). When checking the "resume" option, the classes fields will become inactive as the classes will be read directly from the CSV file.

In addition, the results CSV will be updated automatically every 1 minute to avoid having crashes nullify your whole work.
