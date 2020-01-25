using System.Windows;

namespace ImageOCR.Models
{
    public interface ICapture
    {
        string TakeScreenshot(Rect rect);
    }
}