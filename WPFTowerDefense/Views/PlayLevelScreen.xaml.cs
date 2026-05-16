using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFTowerDefense.ViewModels;

namespace WPFTowerDefense.Views
{
    public partial class PlayLevelScreen : UserControl
    {
        public PlayLevelScreen()
        {
            InitializeComponent();
        }

        private PlayLevelScreenViewModel ViewModel
        {
            get { return DataContext as PlayLevelScreenViewModel; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel?.Initialize(GameCanvas, UpgradeLayer);
        }

        private void Tower_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is Image image)
            {
                ViewModel?.StartTowerDrag(image);
            }
        }

        private void GameCanvas_DragOver(object sender, DragEventArgs e)
        {
            ViewModel?.SetDragEffects(e);
        }

        private void GameCanvas_Drop(object sender, DragEventArgs e)
        {
            ViewModel?.DropTower(e);
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel?.ClearUpgradeButtons(e.OriginalSource);
        }
    }
}
