using System;
using System.Windows;
using System.Windows.Input;
using Prism.Events;
using WPFTowerDefense.Common;
using WPFTowerDefense.Events;

namespace WPFTowerDefense.ViewModels
{
    public class StartScreenViewModel : ViewModelBase
    {
        public StartScreenViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            PlayCommand = new ActionCommand(PlayCommandExecute, PlayCommandCanExecute);
            OptionsCommand = new ActionCommand(OptionsCommandExecute, OptionsCommandCanExecute);
            ExitCommand = new ActionCommand(ExitCommandExecute, ExitCommandCanExecute);
            GameGuideCommand = new ActionCommand(GameGuideCommandExecute, GameGuideCommandCanExecute);
        }

        public ICommand PlayCommand { get; set; }
        public ICommand OptionsCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand GameGuideCommand { get; set; }

        private bool PlayCommandCanExecute(object parameter) 
        { 
            return true; 
        }
        private bool OptionsCommandCanExecute(object parameter) 
        { 
            return true; 
        }
        private bool ExitCommandCanExecute(object parameter) 
        { 
            return true; 
        }
        private bool GameGuideCommandCanExecute(object parameter) 
        { 
            return true; 
        }

        private void PlayCommandExecute(object parameter)
        {
            EventAggregator.GetEvent<NavigationEvent>().Publish(new NavigationRequest("SelectLevel"));
        }

        private void OptionsCommandExecute(object parameter)
        {
            EventAggregator.GetEvent<NavigationEvent>().Publish(new NavigationRequest("Options"));
        }

        private void ExitCommandExecute(object parameter)
        {
            EventAggregator.GetEvent<DimOverlayEvent>().Publish(true);

            ExitConfirmationWindow exitWindow = new ExitConfirmationWindow();
            exitWindow.Owner = Application.Current.MainWindow;
            exitWindow.Closed += ExitWindowClosed;
            exitWindow.ShowDialog();
        }

        private void ExitWindowClosed(object sender, EventArgs e)
        {
            EventAggregator.GetEvent<DimOverlayEvent>().Publish(false);
        }

        private void GameGuideCommandExecute(object parameter)
        {
            EventAggregator.GetEvent<NavigationEvent>().Publish(new NavigationRequest("GameGuide"));
        }
    }
}
