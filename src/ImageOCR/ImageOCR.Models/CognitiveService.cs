using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ImageOCR.Models
{
    public class CognitiveService : ICognitiveService
    {

        public async Task<string> OCRFromImage(string endPoint, string key, string imageFilePath)
        {
            var credentials = new ApiKeyServiceClientCredentials(key);
            using var client = new ComputerVisionClient(credentials) { Endpoint = endPoint };
            using var imageFileStream = File.OpenRead(imageFilePath);
            var ocrResult = await client.RecognizePrintedTextInStreamAsync(true, imageFileStream);
            if (ocrResult.Regions == null || !ocrResult.Regions.Any()) return string.Empty;

            string ocrText = string.Empty;
            foreach (var region in ocrResult.Regions)
            {
                foreach (var line in region.Lines)
                {
                    foreach (var word in line.Words)
                    {
                        ocrText += word.Text + " ";
                    }
                    ocrText.TrimEnd();
                    ocrText += "\r\n";
                }
            }
            return ocrText;
        }

        public async Task<string> TranslateText(string endPoint, string key, string inputText)
        {
            object[] body = new object[] { new { Text = inputText } };
            var requestBody = JsonConvert.SerializeObject(body);
            var translatedString = string.Empty;

            // HttpClient Dispose警察案件
            using var client = new HttpClient();
            using var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(endPoint);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("Ocp-Apim-Subscription-Key", key);

            HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
            string result = await response.Content.ReadAsStringAsync();
            TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);
            foreach (TranslationResult o in deserializedOutput)
            {
                foreach (Translation t in o.Translations)
                {
                    translatedString += t.Text;
                }
            }
            return translatedString;
        }
    }
}
