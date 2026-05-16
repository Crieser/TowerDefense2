using System.Windows;
using Prism.Events;
using WPFTowerDefense.ViewModels;

namespace WPFTowerDefense
{
    public partial class ExitConfirmationWindow : Window
    {
        public ExitConfirmationWindow()
        {
            InitializeComponent();
            DataContext = new ExitConfirmationWindowViewModel(new EventAggregator());
        }
    }
}
