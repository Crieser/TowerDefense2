using System.Configuration;
using System.Data;
using System.Windows;
using TowerDefense2.ViewModels;

namespace TowerDefense2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            MainViewModel mainViewModel = new MainViewModel();
            mainWindow.DataContext = mainViewModel;

            mainWindow.Show();
        }
    }

}
