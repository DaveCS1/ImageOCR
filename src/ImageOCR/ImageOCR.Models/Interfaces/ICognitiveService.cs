using System.Threading.Tasks;

namespace ImageOCR.Models
{
    public interface ICognitiveService
    {
        Task<string> OCRFromImage(string endPoint, string key, string imageFilePath);
        Task<string> TranslateText(string endPoint, string key, string inputText);
    }
}
