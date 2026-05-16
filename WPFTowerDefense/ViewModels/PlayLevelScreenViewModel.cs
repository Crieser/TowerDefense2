using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Prism.Events;
using WPFTowerDefense.Common;
using WPFTowerDefense.Database;
using WPFTowerDefense.Events;
using WPFTowerDefense.GameLogic;

namespace WPFTowerDefense.ViewModels
{
    public class PlayLevelScreenViewModel : ViewModelBase
    {
        private const int TileSize = 80;
        private readonly string _levelPath;
        private readonly DispatcherTimer _waveDelayTimer = new DispatcherTimer();
        private GameManager _gameManager;
        private LevelData _currentLevel;
        private Canvas _gameCanvas;
        private Canvas _upgradeLayer;
        private int _currentWave;
        private Tower _selectedTower;
        private string _waveText = "Wave: 0";
        private string _goldText = "Gold: 500";
        private string _healthText = "HP: 100";
        private Visibility _startGameButtonVisibility = Visibility.Visible;
        private Visibility _returnToMenuButtonVisibility = Visibility.Collapsed;
        private Visibility _insufficientGoldVisibility = Visibility.Collapsed;

        public PlayLevelScreenViewModel(IEventAggregator eventAggregator, string levelPath) : base(eventAggregator)
        {
            _levelPath = levelPath;

            StartGameCommand = new ActionCommand(StartGameCommandExecute, StartGameCommandCanExecute);
            ReturnToMenuCommand = new ActionCommand(ReturnToMenuCommandExecute, ReturnToMenuCommandCanExecute);

            _waveDelayTimer.Interval = TimeSpan.FromSeconds(5);
            _waveDelayTimer.Tick += WaveDelayTimerTick;
        }

        public ICommand StartGameCommand { get; private set; }
        public ICommand ReturnToMenuCommand { get; private set; }

        public string WaveText
        {
            get { return _waveText; }
            set
            {
                if (_waveText == value)
                {
                    return;
                }

                _waveText = value;
                OnPropertyChanged(nameof(WaveText));
            }
        }

        public string GoldText
        {
            get { return _goldText; }
            set
            {
                if (_goldText == value)
                {
                    return;
                }

                _goldText = value;
                OnPropertyChanged(nameof(GoldText));
            }
        }

        public string HealthText
        {
            get { return _healthText; }
            set
            {
                if (_healthText == value)
                {
                    return;
                }

                _healthText = value;
                OnPropertyChanged(nameof(HealthText));
            }
        }

        public Visibility StartGameButtonVisibility
        {
            get { return _startGameButtonVisibility; }
            set
            {
                if (_startGameButtonVisibility == value)
                {
                    return;
                }

                _startGameButtonVisibility = value;
                OnPropertyChanged(nameof(StartGameButtonVisibility));
            }
        }

        public Visibility ReturnToMenuButtonVisibility
        {
            get { return _returnToMenuButtonVisibility; }
            set
            {
                if (_returnToMenuButtonVisibility == value)
                {
                    return;
                }

                _returnToMenuButtonVisibility = value;
                OnPropertyChanged(nameof(ReturnToMenuButtonVisibility));
            }
        }

        public Visibility InsufficientGoldVisibility
        {
            get { return _insufficientGoldVisibility; }
            set
            {
                if (_insufficientGoldVisibility == value)
                {
                    return;
                }

                _insufficientGoldVisibility = value;
                OnPropertyChanged(nameof(InsufficientGoldVisibility));
            }
        }

        public void Initialize(Canvas gameCanvas, Canvas upgradeLayer)
        {
            if (_gameManager != null)
            {
                return;
            }

            _gameCanvas = gameCanvas;
            _upgradeLayer = upgradeLayer;

            _gameManager = new GameManager(_gameCanvas, TileSize);
            _gameManager.ShowGoldWarning = ShowInsufficientGoldWarning;

            var enemyData = EnemyLoader.LoadEnemiesFromDatabase("Database/TD.db");
            _gameManager.SetEnemyData(enemyData);

            var effects = EffectLoader.LoadEffectsFromDatabase("Database/TD.db");
            _gameManager.SetEffectData(effects);

            var towerData = TowerLoader.LoadTowersFromDatabase("Database/TD.db");
            _gameManager.SetTowerData(towerData);

            _currentLevel = LevelLoader.LoadLevelFromJson(_levelPath);
            _gameManager.LoadLevel(_currentLevel);

            _gameManager.OnTowerSelected = ShowUpgradeButtons;
            _gameManager.OnGoldChanged = newGold => GoldText = $"Gold: {newGold}";
            _gameManager.OnHealthChanged = newHp => HealthText = $"HP: {newHp}";
            _gameManager.OnPlayerDefeated = OnPlayerDefeated;
            _gameManager.OnWaveEnded = OnWaveEnded;
        }

        public void ClearUpgradeButtons(object originalSource)
        {
            if (originalSource is not Button)
            {
                _upgradeLayer?.Children.Clear();
            }
        }

        public void StartTowerDrag(Image image)
        {
            if (image.Tag == null)
            {
                return;
            }

            var data = new DataObject();
            data.SetData("TowerImage", image.Source);
            data.SetData("TowerID", image.Tag);
            DragDrop.DoDragDrop(image, data, DragDropEffects.Copy);
        }

        public void SetDragEffects(DragEventArgs e)
        {
            e.Effects = (e.Data.GetDataPresent("TowerImage") && e.Data.GetDataPresent("TowerID"))
                ? DragDropEffects.Copy
                : DragDropEffects.None;

            e.Handled = true;
        }

        public void DropTower(DragEventArgs e)
        {
            if (e.Data.GetDataPresent("TowerImage") && e.Data.GetDataPresent("TowerID"))
            {
                Point pos = e.GetPosition(_gameCanvas);
                var image = e.Data.GetData("TowerImage") as ImageSource;
                var towerId = e.Data.GetData("TowerID")?.ToString();

                int id = int.Parse(towerId);
                var towerData = _gameManager.GetTowerData().FirstOrDefault(t => t.TowerID == id);

                if (towerData != null)
                {
                    _gameManager.PlaceTower(pos, image, towerData);
                }
                else
                {
                    MessageBox.Show("TowerData not found.");
                }
            }
        }

        private bool StartGameCommandCanExecute(object parameter)
        {
            return true;
        }

        private void StartGameCommandExecute(object parameter)
        {
            StartGameButtonVisibility = Visibility.Collapsed;
            ReturnToMenuButtonVisibility = Visibility.Visible;
            StartNextWave();
        }

        private bool ReturnToMenuCommandCanExecute(object parameter)
        {
            return true;
        }

        private void ReturnToMenuCommandExecute(object parameter)
        {
            var result = MessageBox.Show("Do you want to end the game and return to the main menu?",
                                         "End Game",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            EndGameAndShowGameOver();
        }

        private void ShowUpgradeButtons(Tower tower)
        {
            _selectedTower = tower;
            _upgradeLayer.Children.Clear();

            var upgrades = _gameManager.GetTowerData()
                .Where(u => u.TowerID == tower.TowerID && u.Tier == tower.Tier + 1)
                .ToList();

            if (upgrades.Count < 2)
            {
                return;
            }

            Button upgradeButton1 = CreateUpgradeButton(upgrades[0], tower);
            Canvas.SetLeft(upgradeButton1, tower.Position.X + 30);
            Canvas.SetTop(upgradeButton1, tower.Position.Y - 30);
            _upgradeLayer.Children.Add(upgradeButton1);

            Button upgradeButton2 = CreateUpgradeButton(upgrades[1], tower);
            Canvas.SetLeft(upgradeButton2, tower.Position.X - 110);
            Canvas.SetTop(upgradeButton2, tower.Position.Y - 30);
            _upgradeLayer.Children.Add(upgradeButton2);
        }

        private Button CreateUpgradeButton(TowerData upgrade, Tower baseTower)
        {
            var btn = new Button
            {
                Content = upgrade.TowerName,
                Width = 80,
                Height = 40,
                Background = Brushes.LightBlue,
                Tag = upgrade,
                ToolTip = BuildUpgradeTooltip(baseTower, upgrade)
            };

            if (_gameManager.Gold < upgrade.Cost)
            {
                btn.IsEnabled = false;
            }

            btn.Click += (s, e) =>
            {
                _gameManager.UpgradeTower(baseTower, upgrade);
                _upgradeLayer.Children.Clear();
            };

            return btn;
        }

        private string BuildUpgradeTooltip(Tower baseTower, TowerData upgrade)
        {
            string effectFrom = string.IsNullOrWhiteSpace(baseTower.Effect) ? "None" : baseTower.Effect;
            string effectTo = string.IsNullOrWhiteSpace(upgrade.Effect) ? "None" : upgrade.Effect;

            return
                $"Cost: {upgrade.Cost}\n" +
                $"Damage: {baseTower.Damage:0.##} -> {upgrade.Damage:0.##}\n" +
                $"Range: {baseTower.Range:0.##} -> {upgrade.Range:0.##}\n" +
                $"Attack Speed: {baseTower.AttackSpeed:0.##} -> {upgrade.AttackSpeed:0.##}\n" +
                $"AoE Radius: {baseTower.AoERadius:0.##} -> {upgrade.AoERadius:0.##}\n" +
                $"Effect: {effectFrom} -> {effectTo}";
        }

        private async void ShowInsufficientGoldWarning()
        {
            InsufficientGoldVisibility = Visibility.Visible;
            await Task.Delay(1000);
            InsufficientGoldVisibility = Visibility.Collapsed;
        }

        private void StartNextWave()
        {
            if (_gameManager == null)
            {
                return;
            }

            _currentWave++;
            WaveText = $"Wave: {_currentWave}";
            _gameManager.StartWave(_currentWave);
        }

        private void OnWaveEnded()
        {
            _waveDelayTimer.Start();
        }

        private void WaveDelayTimerTick(object sender, EventArgs e)
        {
            _waveDelayTimer.Stop();
            StartNextWave();
        }

        private void OnPlayerDefeated()
        {
            Application.Current.Dispatcher.Invoke(EndGameAndShowGameOver);
        }

        private void EndGameAndShowGameOver()
        {
            _waveDelayTimer.Stop();

            if (_gameManager != null)
            {
                _gameManager.Dispose();
                _gameManager = null;
            }

            int lastWave = _currentWave > 0 ? _currentWave - 1 : 0;
            HighscoreManager.UpdateBestWave("Database/TD.db", _currentLevel.LevelName, lastWave);
            EventAggregator.GetEvent<GameOverEvent>().Publish(new GameOverData(lastWave, _currentLevel.LevelName));
        }
    }
}
