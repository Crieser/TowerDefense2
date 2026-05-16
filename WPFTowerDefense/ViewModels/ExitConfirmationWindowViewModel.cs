using System.Windows;
using System.Windows.Input;
using Prism.Events;
using WPFTowerDefense.Common;

namespace WPFTowerDefense.ViewModels
{
    public class ExitConfirmationWindowViewModel : ViewModelBase
    {
        public ExitConfirmationWindowViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            YesCommand = new ActionCommand(YesCommandExecute, YesCommandCanExecute);
            NoCommand = new ActionCommand(NoCommandExecute, NoCommandCanExecute);
        }

        public ICommand YesCommand { get; private set; }
        public ICommand NoCommand { get; private set; }

        private bool YesCommandCanExecute(object parameter)
        {
            return true;
        }

        private void YesCommandExecute(object parameter)
        {
            Application.Current.Shutdown();
        }

        private bool NoCommandCanExecute(object parameter)
        {
            return parameter is Window;
        }

        private void NoCommandExecute(object parameter)
        {
            Window window = parameter as Window;
            window.Close();
        }
    }
}
