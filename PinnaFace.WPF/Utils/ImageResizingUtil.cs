using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using PinnaFace.Core;

namespace PinnaFace.WPF
{
    public static class ImageResizingUtil
    {
        public static bool ResizeEnjazImage(string srcFileName, string finalFinalName)
        {
            try
            {
                var tempFolder = PathUtil.GetFolderPath() + "Temp\\";
                if (!Directory.Exists(tempFolder))
                    Directory.CreateDirectory(tempFolder);

                var newWidth = 1.0;
                var newHeight = 1.0;

                var fileLengthBefore = new FileInfo(srcFileName).Length;
                using (var srcImage = Image.FromFile(srcFileName))
                {
                    newWidth = srcImage.Width;
                    newHeight = srcImage.Height;
                }

                var sourceFileName = srcFileName;
                var scaleFactor = GetScaleFactor(newWidth, newHeight, 200.00, 200.00);
                while (fileLengthBefore > 17000 || newWidth > 200 || newHeight > 200)
                {
                    var fileName = Path.Combine(tempFolder, Guid.NewGuid() + ".jpg");
                    Resize(sourceFileName, fileName, scaleFactor);
                    fileLengthBefore = new FileInfo(fileName).Length;
                    using (var srcImage = Image.FromFile(fileName))
                    {
                        newWidth = srcImage.Width;
                        newHeight = srcImage.Height;
                    }
                    sourceFileName = fileName;
                    scaleFactor = 0.95;
                }


                var enjazitFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\EnjazPersonalPhotos\\";
                if (!Directory.Exists(enjazitFolder))
                    Directory.CreateDirectory(enjazitFolder);

                var destFile = Path.Combine(enjazitFolder, finalFinalName + ".jpg");
                if (File.Exists(destFile))
                    File.Delete(destFile);
                File.Move(sourceFileName, destFile);
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public static string ResizeImages(string srcFileName)
        {
            try
            {
                var tempFolder = PathUtil.GetFolderPath() + "Temp\\";
                if (!Directory.Exists(tempFolder))
                    Directory.CreateDirectory(tempFolder);

                var newWidth = 1.0;
                var newHeight = 1.0;

                using (var srcImage = Image.FromFile(srcFileName))
                {
                    newWidth = srcImage.Width;
                    newHeight = srcImage.Height;
                }

                var scaleFactor = GetScaleFactor(newWidth, newHeight, 400.00, 600.00);

                var finalFinalName = Path.Combine(tempFolder, Guid.NewGuid() + ".jpg");
                Resize(srcFileName, finalFinalName, scaleFactor);

                return finalFinalName;
            }
            catch
            {
                return srcFileName;
            }
        }

        private static double GetScaleFactor(double newWidth, double newHeight, double maxWidth, double maxHeight)
        {
            try
            {
                var aspectRatio = newWidth / newHeight;
                double boxRatio = maxWidth / maxHeight;
                double scaleFactor = 0;

                if (boxRatio > aspectRatio) //Use height, since that is the most restrictive dimension of box. 
                    scaleFactor = maxHeight / newHeight;
                else
                    scaleFactor = maxWidth / newWidth;

                return scaleFactor;
            }
            catch
            {
                return 1.00;
            }
        }

        public static void Resize(string imageFile, string outputFile, double scaleFactor)
        {
            try
            {
                using (var srcImage = Image.FromFile(imageFile))
                {
                    var newWidth = (int)(srcImage.Width * scaleFactor);
                    var newHeight = (int)(srcImage.Height * scaleFactor);
                    using (var newImage = new Bitmap(newWidth, newHeight))
                    using (var graphics = Graphics.FromImage(newImage))
                    {
                        graphics.SmoothingMode = SmoothingMode.HighQuality;//.AntiAlias;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.DrawImage(srcImage, new Rectangle(0, 0, newWidth, newHeight));
                        newImage.Save(outputFile, ImageFormat.Jpeg);
                    }
                }
            }
            catch 
            {
            }
        }

    }
}