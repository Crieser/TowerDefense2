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
    /// Interaction logic for StartScreen.xaml
    /// </summary>
    public partial class StartScreen : Page
    {
        private MainWindow _mainWindow;

        public StartScreen(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new SelectLevelScreen(_mainWindow));
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new OptionsScreen(_mainWindow));
        }

        private void ExitWindow_Closed(object sender, EventArgs e)
        {
            _mainWindow.HideDimOverlay();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.ShowDimOverlay();

            ExitConfirmationWindow exitWindow = new ExitConfirmationWindow();
            exitWindow.Owner = Window.GetWindow(this);

            exitWindow.Closed += ExitWindow_Closed;

            exitWindow.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new GameGuideScreen(_mainWindow));
        }
    }

}
