using System;
using System.IO;
using System.Windows.Input;
using Prism.Events;
using WPFTowerDefense.Common;
using WPFTowerDefense.Events;

namespace WPFTowerDefense.ViewModels
{
    public class SelectLevelScreenViewModel : ViewModelBase
    {
        private readonly Level[] _levels = (Level[])Enum.GetValues(typeof(Level));
        private int _currentIndex;
        private string _levelPreviewImage;

        public SelectLevelScreenViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            ReturnCommand = new ActionCommand(ReturnCommandExecute, ReturnCommandCanExecute);
            PlayCommand = new ActionCommand(PlayCommandExecute, PlayCommandCanExecute);
            NextLevelCommand = new ActionCommand(NextLevelCommandExecute, NextLevelCommandCanExecute);
            PreviousLevelCommand = new ActionCommand(PreviousLevelCommandExecute, PreviousLevelCommandCanExecute);

            DisplaySelectedLevel();
        }

        public enum Level
        {
            levelA,
            levelB,
            levelC
        }

        public ICommand ReturnCommand { get; private set; }
        public ICommand PlayCommand { get; private set; }
        public ICommand NextLevelCommand { get; private set; }
        public ICommand PreviousLevelCommand { get; private set; }

        public string LevelPreviewImage
        {
            get { return _levelPreviewImage; }
            set
            {
                if (_levelPreviewImage == value)
                {
                    return;
                }

                _levelPreviewImage = value;
                OnPropertyChanged(nameof(LevelPreviewImage));
            }
        }

        private bool ReturnCommandCanExecute(object parameter) 
        { 
            return true; 
        }
        private bool PlayCommandCanExecute(object parameter) 
        { 
            return true; 
        }
        private bool NextLevelCommandCanExecute(object parameter) 
        { 
            return true; 
        }
        private bool PreviousLevelCommandCanExecute(object parameter) 
        { 
            return true; 
        }

        private void ReturnCommandExecute(object parameter)
        {
            EventAggregator.GetEvent<NavigationEvent>().Publish(new NavigationRequest("Start"));
        }

        private void PlayCommandExecute(object parameter)
        {
            Level selectedLevel = _levels[_currentIndex];
            string levelPath = GetLevelFilePath(selectedLevel);
            EventAggregator.GetEvent<NavigationEvent>().Publish(new NavigationRequest("PlayLevel", levelPath));
        }

        private void NextLevelCommandExecute(object parameter)
        {
            if (_currentIndex < _levels.Length - 1)
            {
                _currentIndex++;
            }
            else
            {
                _currentIndex = 0;
            }

            DisplaySelectedLevel();
        }

        private void PreviousLevelCommandExecute(object parameter)
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
            }
            else
            {
                _currentIndex = _levels.Length - 1;
            }

            DisplaySelectedLevel();
        }

        private string GetLevelFilePath(Level level)
        {
            string fileName = $"{level}.json";
            return Path.Combine("Levels", fileName);
        }

        private void DisplaySelectedLevel()
        {
            Level selectedLevel = _levels[_currentIndex];
            string imageFile = selectedLevel.ToString().ToLower() + ".png";
            LevelPreviewImage = $"/Images/{imageFile}";
        }
    }
}
