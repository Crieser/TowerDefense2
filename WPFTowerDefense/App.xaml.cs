using System.Windows;
using Prism.Events;
using WPFTowerDefense.ViewModels;

namespace WPFTowerDefense
{
    public partial class App : Application
    {
        private IEventAggregator _eventAggregator;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _eventAggregator = new EventAggregator();
            MainWindow mainWindow = new MainWindow();
            MainViewModel mainViewModel = new MainViewModel(_eventAggregator);
            mainWindow.DataContext = mainViewModel;
            mainWindow.Show();
        }
    }
}
