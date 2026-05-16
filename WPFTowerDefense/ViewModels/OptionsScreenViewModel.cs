using System;
using System.Windows.Input;
using Prism.Events;
using WPFTowerDefense.Common;
using WPFTowerDefense.Events;

namespace WPFTowerDefense.ViewModels
{
    public class OptionsScreenViewModel : ViewModelBase
    {
        private double _volumeLevel;
        private string _volumeText;

        public OptionsScreenViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            ReturnCommand = new ActionCommand(ReturnCommandExecute, ReturnCommandCanExecute);

            VolumeLevel = Properties.Settings.Default.VolumeLevel;
        }

        public ICommand ReturnCommand { get; private set; }

        public double VolumeLevel
        {
            get { return _volumeLevel; }
            set
            {
                if (_volumeLevel == value)
                {
                    return;
                }

                _volumeLevel = value;
                Properties.Settings.Default.VolumeLevel = value;
                Properties.Settings.Default.Save();
                UpdateVolumeText();
                OnPropertyChanged(nameof(VolumeLevel));
            }
        }

        public string VolumeText
        {
            get { return _volumeText; }
            set
            {
                if (_volumeText == value)
                {
                    return;
                }

                _volumeText = value;
                OnPropertyChanged(nameof(VolumeText));
            }
        }

        private bool ReturnCommandCanExecute(object parameter)
        {
            return true;
        }

        private void ReturnCommandExecute(object parameter)
        {
            EventAggregator.GetEvent<NavigationEvent>().Publish(new NavigationRequest("Start"));
        }

        private void UpdateVolumeText()
        {
            if (VolumeLevel == 0)
            {
                VolumeText = "Volume: Mute";
            }
            else
            {
                VolumeText = $"Volume: {Math.Round(VolumeLevel * 100)}";
            }
        }
    }
}
