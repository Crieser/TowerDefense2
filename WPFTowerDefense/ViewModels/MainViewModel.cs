using System.Windows.Controls;
using Prism.Events;
using WPFTowerDefense.Events;
using WPFTowerDefense.Views;

namespace WPFTowerDefense.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private UserControl _currentView;
        private bool _isDimOverlayVisible;

        public MainViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            EventAggregator.GetEvent<NavigationEvent>()
                .Subscribe(OnNavigationRequested, ThreadOption.UIThread);

            EventAggregator.GetEvent<GameOverEvent>()
                .Subscribe(OnGameOver, ThreadOption.UIThread);

            EventAggregator.GetEvent<DimOverlayEvent>()
                .Subscribe(OnDimOverlayChanged, ThreadOption.UIThread);

            ShowStartScreen();
        }

        public UserControl CurrentView
        {
            get { return _currentView; }
            set
            {
                if (_currentView == value)
                {
                    return;
                }

                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public bool IsDimOverlayVisible
        {
            get { return _isDimOverlayVisible; }
            set
            {
                if (_isDimOverlayVisible == value)
                {
                    return;
                }

                _isDimOverlayVisible = value;
                OnPropertyChanged(nameof(IsDimOverlayVisible));
            }
        }

        public void ShowStartScreen()
        {
            StartScreen view = new StartScreen();
            StartScreenViewModel viewModel = new StartScreenViewModel(EventAggregator);
            view.DataContext = viewModel;
            CurrentView = view;
        }

        private void ShowSelectLevelScreen()
        {
            SelectLevelScreen view = new SelectLevelScreen();
            SelectLevelScreenViewModel viewModel = new SelectLevelScreenViewModel(EventAggregator);
            view.DataContext = viewModel;
            CurrentView = view;
        }

        private void ShowOptionsScreen()
        {
            OptionsScreen view = new OptionsScreen();
            OptionsScreenViewModel viewModel = new OptionsScreenViewModel(EventAggregator);
            view.DataContext = viewModel;
            CurrentView = view;
        }

        private void ShowGameGuideScreen()
        {
            GameGuideScreen view = new GameGuideScreen();
            GameGuideScreenViewModel viewModel = new GameGuideScreenViewModel(EventAggregator);
            view.DataContext = viewModel;
            CurrentView = view;
        }

        private void ShowPlayLevelScreen(string levelPath)
        {
            PlayLevelScreen view = new PlayLevelScreen();
            PlayLevelScreenViewModel viewModel = new PlayLevelScreenViewModel(EventAggregator, levelPath);
            view.DataContext = viewModel;
            CurrentView = view;
        }

        private void ShowGameOverScreen(int reachedWave, string levelName)
        {
            GameOverScreen view = new GameOverScreen();
            GameOverScreenViewModel viewModel = new GameOverScreenViewModel(EventAggregator, reachedWave, levelName);
            view.DataContext = viewModel;
            CurrentView = view;
        }

        private void OnNavigationRequested(NavigationRequest request)
        {
            if (request == null)
            {
                return;
            }

            if (request.Target == "Start")
            {
                ShowStartScreen();
            }
            else if (request.Target == "SelectLevel")
            {
                ShowSelectLevelScreen();
            }
            else if (request.Target == "Options")
            {
                ShowOptionsScreen();
            }
            else if (request.Target == "GameGuide")
            {
                ShowGameGuideScreen();
            }
            else if (request.Target == "PlayLevel")
            {
                ShowPlayLevelScreen(request.LevelPath);
            }
        }

        private void OnGameOver(GameOverData gameOverData)
        {
            if (gameOverData == null)
            {
                return;
            }

            ShowGameOverScreen(gameOverData.ReachedWave, gameOverData.LevelName);
        }

        private void OnDimOverlayChanged(bool isVisible)
        {
            IsDimOverlayVisible = isVisible;
        }
    }
}
