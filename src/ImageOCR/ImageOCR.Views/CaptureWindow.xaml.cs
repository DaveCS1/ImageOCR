using System.Windows;
using Prism.Services.Dialogs;

namespace ImageOCR.Views
{
    public partial class CaptureWindow : Window, IDialogWindow
    {
        public IDialogResult Result { get; set; }
        public CaptureWindow()
        {
            InitializeComponent();
        }
    }
}