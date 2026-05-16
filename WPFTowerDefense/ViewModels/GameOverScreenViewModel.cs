using System.Windows.Input;
using Prism.Events;
using WPFTowerDefense.Common;
using WPFTowerDefense.Database;
using WPFTowerDefense.Events;

namespace WPFTowerDefense.ViewModels
{
    public class GameOverScreenViewModel : ViewModelBase
    {
        public GameOverScreenViewModel(IEventAggregator eventAggregator, int reachedWave, string levelName) : base(eventAggregator)
        {
            CurrentWaveText = $"You reached Wave: {reachedWave}";
            int best = HighscoreManager.GetBestWave("Database/TD.db", levelName);
            HighscoreText = $"Highest Wave on this Level: {best}";
            ContinueCommand = new ActionCommand(ContinueCommandExecute, ContinueCommandCanExecute);
        }

        public string CurrentWaveText { get; private set; }
        public string HighscoreText { get; private set; }
        public ICommand ContinueCommand { get; private set; }

        private bool ContinueCommandCanExecute(object parameter)
        {
            return true;
        }

        private void ContinueCommandExecute(object parameter)
        {
            EventAggregator.GetEvent<NavigationEvent>().Publish(new NavigationRequest("Start"));
        }
    }
}
