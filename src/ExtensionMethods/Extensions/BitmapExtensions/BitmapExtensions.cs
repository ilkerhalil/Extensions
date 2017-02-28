using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Extensions.BitmapExtensions
{
    public static class AscArt
    {
        // Typical width/height for ASCII characters
        private const double FontAspectRatio = 0.6;

        // Available character set, ordered by decreasing intensity (brightness)
        private const string OutputCharSet = "@%#*+=-:. ";

        // Alternate char set uses more chars, but looks less realistic
        private const string OutputCharSetAlternate = "$@B%8&WM#*oahkbdpqwmZO0QLCJUYXzcvunxrjft/\\|()1{}[]?-_+~<>i!lI;:,\"^`'. ";

        public static void GenerateAsciiArt(this Bitmap bmInput, string outputFile, int outputWidth)
        {
            // pixelChunkWidth/pixelChunkHeight - size of a chunk of pixels that will
            // map to 1 character.  These are doubles to avoid progressive rounding
            // error.
            var pixelChunkWidth = bmInput.Width / (double)outputWidth;
            var pixelChunkHeight = pixelChunkWidth / FontAspectRatio;

            // Calculate output height to capture entire image
            var outputHeight = (int)Math.Round((double)bmInput.Height / pixelChunkHeight);

            // Generate output image, row by row
            var pixelOffSetTop = 0.0;
            var sbOutput = new StringBuilder();

            for (var row = 1; row <= outputHeight; row++)
            {
                var pixelOffSetLeft = 0.0;

                for (var col = 1; col <= outputWidth; col++)
                {
                    // Calculate brightness for this set of pixels by averaging
                    // brightness across all pixels in this pixel chunk
                    var brightSum = 0.0;
                    var pixelCount = 0;
                    for (var pixelLeftInd = 0; pixelLeftInd < (int)pixelChunkWidth; pixelLeftInd++)
                        for (var pixelTopInd = 0; pixelTopInd < (int)pixelChunkHeight; pixelTopInd++)
                        {
                            // Each call to GetBrightness returns value between 0.0 and 1.0
                            var x = (int)pixelOffSetLeft + pixelLeftInd;
                            var y = (int)pixelOffSetTop + pixelTopInd;
                            if ((x >= bmInput.Width) || (y >= bmInput.Height)) continue;
                            brightSum += bmInput.GetPixel(x, y).GetBrightness();
                            pixelCount++;
                        }

                    // Average brightness for this entire pixel chunk, between 0.0 and 1.0
                    var pixelChunkBrightness = brightSum / pixelCount;

                    // Target character is just relative position in ordered set of output characters
                    var outputIndex = (int)Math.Floor(pixelChunkBrightness * OutputCharSet.Length);
                    if (outputIndex == OutputCharSet.Length)
                        outputIndex--;

                    var targetChar = OutputCharSet[outputIndex];

                    sbOutput.Append(targetChar);

                    pixelOffSetLeft += pixelChunkWidth;
                }
                sbOutput.AppendLine();
                pixelOffSetTop += pixelChunkHeight;
            }

            // Dump output string to file
            File.WriteAllText(outputFile, sbOutput.ToString());
        }
    }
}
