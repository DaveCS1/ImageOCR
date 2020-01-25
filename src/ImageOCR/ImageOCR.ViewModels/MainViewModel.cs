using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ImageOCR.Models;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace ImageOCR.ViewModels
{
    public class MainViewModel : BindableBase, INotifyPropertyChanged, IDisposable
    {
        public ICapture Capture { get; }
        public ICognitiveService CognitiveService { get; }
        public Configuration Config { get; }
        public ReactiveCommand CaptureCommand { get; set; } = new ReactiveCommand();
        public ReactiveProperty<BitmapSource> ImagePath { get; set; } = new ReactiveProperty<BitmapSource>();
        public ReactiveProperty<string> ImageText { get; set; } = new ReactiveProperty<string>("");
        public ReactiveProperty<string> TranslatedText { get; set; } = new ReactiveProperty<string>("");

        CompositeDisposable DisposeCollection = new CompositeDisposable();
        bool disposedValue = false;
        public void Dispose() => Dispose(true);
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisposeCollection.Dispose();
                }
                disposedValue = true;
            }
        }
        public MainViewModel(IDialogService dialogService,
                             IEventAggregator eventAggregator,
                             ICapture capture,
                             ICognitiveService cognitiveService,
                             Configuration config)
        {
            Capture = capture;
            CognitiveService = cognitiveService;
            Config = config;

            CaptureCommand.Subscribe(() => dialogService.Show("CaptureView", null, _ => { })).AddTo(DisposeCollection);
            eventAggregator.GetEvent<PubSubEvent<Rect>>().Subscribe(async r => await TranslateCapturedImageText(r));
        }

        private async Task TranslateCapturedImageText(Rect rect)
        {
            try
            {
                // Capture
                var capturedImageFilePath = Capture.TakeScreenshot(rect);
                using (var fs = new FileStream(capturedImageFilePath, FileMode.Open, FileAccess.Read))
                {
                    var decoder = BitmapDecoder.Create(fs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    ImagePath.Value = decoder.Frames[0];
                }

                // OCR
                var ocrEndPoint = Config.AppSettings.Settings["OcrEndPoint"].Value;
                var ocrApiKey = Config.AppSettings.Settings["OcrApiKey"].Value;
                var imageText = await CognitiveService.OCRFromImage(ocrEndPoint, ocrApiKey, capturedImageFilePath);
                if (String.IsNullOrEmpty(imageText))
                {
                    ImageText.Value = "Error : Image OCR Failed";
                    return;
                }
                ImageText.Value = imageText;

                // Translate
                var translateEndPoint = Config.AppSettings.Settings["TranslateEndPoint"].Value;
                var translateApiKey = Config.AppSettings.Settings["TranslateApiKey"].Value;
                var translatedText = await CognitiveService.TranslateText(translateEndPoint, translateApiKey, imageText);
                if (String.IsNullOrEmpty(translatedText))
                {
                    TranslatedText.Value = "Error : Translated Failed";
                    return;
                }

                TranslatedText.Value = translatedText;
            }
            catch (Exception e)
            {
                // FIXME: fix me :)
                MessageBox.Show($"Error: {e.Message}");
            }
        }
    }
}