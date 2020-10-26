using System;
using System.Drawing;
using System.IO;
//using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace PinnaFace.Core
{
    public static class ImageUtil
    {
        public static BitmapImage ToImage(byte[] toImage)
        {
            var image = new BitmapImage();
            if (toImage != null)
            {
                using (var ms = new MemoryStream(toImage))
                {

                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                }
            }
            return image;
        }
        public static BitmapImage ToImageFromUrl(string imageUrl)
        {
            var image = new BitmapImage();
            try
            {
                image = new BitmapImage(new Uri(imageUrl));
            }catch{}
            return image;
        }

        public static byte[] ToBytes(BitmapImage image)
        {
            if (image == null || image.UriSource == null)
                return null;

            var imageArray = new byte[0];
            try
            {
                imageArray = File.ReadAllBytes(image.UriSource.AbsolutePath);
            }
            catch
            {
                //MessageBox.Show("Problem getting photo.....", "Photo problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //MessageBox.Show("image directory may contain spaces, change the location of the image and try agin.....",
                //    "Invalid directory location", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return imageArray;
        }

        public static Image GetImage(byte[] toImage)
        {
            var byteBlobData = toImage;
            if (byteBlobData == null) return null;
            var stmBlobData = new MemoryStream(byteBlobData);
            return Image.FromStream(stmBlobData);

        }

        public static string GetPhotoPath()
        {
            var photoPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
                             "\\PinnaFace\\Photo\\";
            if (!Directory.Exists(photoPath))
                Directory.CreateDirectory(photoPath);
            return photoPath;
        }

        public static string GetDestinationPhotoPath()
        {
            const string photoPath = "E:\\Dev\\PinnaFacePortal\\PinnaFace.Web\\Content\\img";
                //Environment.GetFolderPath(Environment.SpecialFolder.MyComputer) + "E:\\Dev\\Photo\\";
            if (!Directory.Exists(photoPath))
                Directory.CreateDirectory(photoPath);
            return photoPath;
        }
    }
}
