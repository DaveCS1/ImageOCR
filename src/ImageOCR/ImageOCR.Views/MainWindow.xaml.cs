using System.Windows;
using Prism.Ioc;
using Prism.Regions;
using Unity;

namespace ImageOCR.Views
{
    public partial class MainWindow : Window
    {
        [Dependency]
        public IContainerExtension ContainerExtension { get; set; }

        [Dependency]
        public IRegionManager RegionManager { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            RegionManager.AddToRegion("MainRegion", ContainerExtension.Resolve<MainView>());
        }
    }
}