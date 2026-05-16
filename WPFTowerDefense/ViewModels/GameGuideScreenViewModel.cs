using System.Windows.Input;
using Prism.Events;
using WPFTowerDefense.Common;
using WPFTowerDefense.Events;

namespace WPFTowerDefense.ViewModels
{
    public class GameGuideScreenViewModel : ViewModelBase
    {
        public GameGuideScreenViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            ReturnCommand = new ActionCommand(ReturnCommandExecute, ReturnCommandCanExecute);
        }

        public ICommand ReturnCommand { get; private set; }

        private bool ReturnCommandCanExecute(object parameter)
        {
            return true;
        }

        private void ReturnCommandExecute(object parameter)
        {
            EventAggregator.GetEvent<NavigationEvent>().Publish(new NavigationRequest("Start"));
        }
    }
}
