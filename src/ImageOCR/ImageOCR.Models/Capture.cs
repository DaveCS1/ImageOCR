using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ImageOCR.Models
{
    public class Capture : ICapture
    {
        const string TEMP_FILE_PATH = "temp.png";

        public string TakeScreenshot(Rect rect)
        {
            using var screenBmp = new Bitmap((int)rect.Width, (int)rect.Height, PixelFormat.Format32bppArgb);
            using var bmpGraphics = Graphics.FromImage(screenBmp);

            bmpGraphics.CopyFromScreen((int)rect.X, (int)rect.Y, 0, 0, screenBmp.Size);
            var iamge = Imaging.CreateBitmapSourceFromHBitmap(screenBmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            if (File.Exists(TEMP_FILE_PATH)) File.Delete(TEMP_FILE_PATH);
            screenBmp.Save(TEMP_FILE_PATH, ImageFormat.Bmp);
            return Path.GetFullPath(TEMP_FILE_PATH);
        }
    }
}