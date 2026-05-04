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
    /// Interaction logic for OptionsScreen.xaml
    /// </summary>
    public partial class OptionsScreen : Page
    {
        private MainWindow _mainWindow;

        public OptionsScreen(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            Loaded += OptionsScreen_Loaded;
        }



        private void OptionsScreen_Loaded(object sender, RoutedEventArgs e)
        {
       
            VolumeSlider.Value = Properties.Settings.Default.VolumeLevel;
            if (VolumeSlider.Value == 0)
            {
                VolumeTextBlock.Text = "Volume: Mute";
            }
        }
        private void Return_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(new StartScreen(_mainWindow));
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Properties.Settings.Default.VolumeLevel = e.NewValue;
            Properties.Settings.Default.Save();
            VolumeTextBlock.Text = $"Volume: {Math.Round(VolumeSlider.Value*100)}";
            if(VolumeSlider.Value == 0)
            {
                VolumeTextBlock.Text = "Volume: Mute";
            }
        }
    }
}
