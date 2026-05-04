using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFTowerDefense
{
    /// <summary>
    /// Interaction logic for GameGuideScreen.xaml
    /// </summary>
    public partial class GameGuideScreen : Page
    {
        private MainWindow _mainWindow;
        public GameGuideScreen(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new StartScreen(_mainWindow));
        }
    }
}
