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
using WPFTowerDefense.Database;


namespace WPFTowerDefense
{
    /// <summary>
    /// Interaction logic for GameOverScreen.xaml
    /// </summary>
    public partial class GameOverScreen : Page
    {
        private string levelName;
        private MainWindow _mainWindow;

        public GameOverScreen(int reachedWave, string levelName, MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            this.levelName = levelName;

            CurrentWaveText.Text = $"You reached Wave: {reachedWave}";

            int best = HighscoreManager.GetBestWave("Database/TD.db", levelName);
            HighscoreText.Text = $"Highest Wave on this Level: {best}";
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new StartScreen(_mainWindow));
        }

    }
}
