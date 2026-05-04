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
    /// Interaction logic for SelectLevelScreen.xaml
    /// </summary>
    public partial class SelectLevelScreen : Page
    {
        private MainWindow _mainWindow;

        private int currentIndex = 0;
        public SelectLevelScreen(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new StartScreen(_mainWindow));
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Level selectedLevel = levels[currentIndex];
            string levelPath = GetLevelFilePath(selectedLevel);

            _mainWindow.NavigateTo(new PlayLevelScreen(levelPath));
        }

        private void ChangeLevel_Click(object sender, RoutedEventArgs e)
        {
            if(sender == PreviousButton)
            {
                PreviousLevel();
            }
            else if (sender == NextButton)
            {
                NextLevel();
            }
            DisplaySelectedLevel();
        }

        public enum Level
        {
            levelA,
            levelB,
            levelC
        }

        private Level[] levels = (Level[])Enum.GetValues(typeof(Level));

        private string GetLevelFilePath(Level level)
        {
            string fileName = $"{level}.json";
            return System.IO.Path.Combine("Levels", fileName);
        }

        public void NextLevel()
        {
            if (currentIndex < levels.Length - 1)
                currentIndex++;
            else
                currentIndex = 0;
        }

        public void PreviousLevel()
        {
            if (currentIndex > 0)
                currentIndex--;
            else
                currentIndex = levels.Length - 1;
        }

        private void DisplaySelectedLevel()
        {
            Level selectedLevel = levels[currentIndex];
            string imageFile = GetImageFileName(selectedLevel);
            string imagePath = System.IO.Path.Combine("Images", imageFile);

            LevelPreview.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
        }

        private string GetImageFileName(Level level)
        {
            return level.ToString().ToLower() + ".png";
        }


    }
}
