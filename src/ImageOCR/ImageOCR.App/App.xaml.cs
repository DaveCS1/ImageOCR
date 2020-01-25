using ImageOCR.Models;
using ImageOCR.ViewModels;
using ImageOCR.Views;
using System.Windows;
using System.Configuration;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Unity;
using Unity;

namespace ImageOCR.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = "app.config" }, ConfigurationUserLevel.None);
            containerRegistry.RegisterInstance<Configuration>(configuration);

            containerRegistry.Register<ICapture, Capture>();
            containerRegistry.Register<ICognitiveService, CognitiveService>();

            containerRegistry.RegisterDialog<CaptureView>(nameof(CaptureView));
            containerRegistry.RegisterDialogWindow<CaptureWindow>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.Register<MainView, MainViewModel>();
            ViewModelLocationProvider.Register<CaptureView, CaptureViewModel>();
        }
    }
}