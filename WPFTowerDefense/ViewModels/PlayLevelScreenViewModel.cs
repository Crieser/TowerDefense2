using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private int _currentWave;
        private Tower _selectedTower;
        private TowerData _upgradeOption1;
        private TowerData _upgradeOption2;
        private string _upgradeTooltip1;
        private string _upgradeTooltip2;
        private Visibility _upgradeOption1Visibility = Visibility.Collapsed;
        private Visibility _upgradeOption2Visibility = Visibility.Collapsed;
        private string _waveText = "Wave: 0";
        private string _goldText = "Gold: 500";
        private string _healthText = "HP: 100";
        private Visibility _startGameButtonVisibility = Visibility.Visible;
        private Visibility _returnToMenuButtonVisibility = Visibility.Collapsed;
        private Visibility _insufficientGoldVisibility = Visibility.Collapsed;
        private Visibility _upgradePanelVisibility = Visibility.Collapsed;
        private double _upgradePanelX;
        private double _upgradePanelY;

        public PlayLevelScreenViewModel(IEventAggregator eventAggregator, string levelPath) : base(eventAggregator)
        {
            _levelPath = levelPath;
            Towers = new ObservableCollection<Tower>();

            StartGameCommand = new ActionCommand(StartGameCommandExecute, StartGameCommandCanExecute);
            ReturnToMenuCommand = new ActionCommand(ReturnToMenuCommandExecute, ReturnToMenuCommandCanExecute);
            SelectTowerCommand = new ActionCommand(SelectTowerCommandExecute, SelectTowerCommandCanExecute);
            UpgradeTowerCommand = new ActionCommand(UpgradeTowerCommandExecute, UpgradeTowerCommandCanExecute);
            SellTowerCommand = new ActionCommand(SellTowerCommandExecute, SellTowerCommandCanExecute);

            _waveDelayTimer.Interval = TimeSpan.FromSeconds(5);
            _waveDelayTimer.Tick += WaveDelayTimerTick;
        }

        public ObservableCollection<Tower> Towers { get; private set; }
        public ICommand StartGameCommand { get; private set; }
        public ICommand ReturnToMenuCommand { get; private set; }
        public ICommand SelectTowerCommand { get; private set; }
        public ICommand UpgradeTowerCommand { get; private set; }
        public ICommand SellTowerCommand { get; private set; }

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

        public Visibility UpgradePanelVisibility
        {
            get { return _upgradePanelVisibility; }
            set
            {
                if (_upgradePanelVisibility == value)
                {
                    return;
                }

                _upgradePanelVisibility = value;
                OnPropertyChanged(nameof(UpgradePanelVisibility));
            }
        }

        public double UpgradePanelX
        {
            get { return _upgradePanelX; }
            set
            {
                if (_upgradePanelX == value)
                {
                    return;
                }

                _upgradePanelX = value;
                OnPropertyChanged(nameof(UpgradePanelX));
            }
        }

        public double UpgradePanelY
        {
            get { return _upgradePanelY; }
            set
            {
                if (_upgradePanelY == value)
                {
                    return;
                }

                _upgradePanelY = value;
                OnPropertyChanged(nameof(UpgradePanelY));
            }
        }

        public TowerData UpgradeOption1
        {
            get { return _upgradeOption1; }
            set
            {
                if (_upgradeOption1 == value)
                {
                    return;
                }

                _upgradeOption1 = value;
                OnPropertyChanged(nameof(UpgradeOption1));
                OnPropertyChanged(nameof(UpgradeOption1Text));
            }
        }

        public TowerData UpgradeOption2
        {
            get { return _upgradeOption2; }
            set
            {
                if (_upgradeOption2 == value)
                {
                    return;
                }

                _upgradeOption2 = value;
                OnPropertyChanged(nameof(UpgradeOption2));
                OnPropertyChanged(nameof(UpgradeOption2Text));
            }
        }

        public string UpgradeOption1Text { get { return UpgradeOption1 == null ? string.Empty : UpgradeOption1.TowerName; } }
        public string UpgradeOption2Text { get { return UpgradeOption2 == null ? string.Empty : UpgradeOption2.TowerName; } }
        public string SellTowerText { get { return _selectedTower == null ? "Sell" : $"Sell (+{GetSellValue(_selectedTower)})"; } }
        public string SellTowerTooltip { get { return _selectedTower == null ? string.Empty : $"Refunds 70% of this tower's value: {GetSellValue(_selectedTower)} gold"; } }

        public Visibility UpgradeOption1Visibility
        {
            get { return _upgradeOption1Visibility; }
            set
            {
                if (_upgradeOption1Visibility == value)
                {
                    return;
                }

                _upgradeOption1Visibility = value;
                OnPropertyChanged(nameof(UpgradeOption1Visibility));
            }
        }

        public Visibility UpgradeOption2Visibility
        {
            get { return _upgradeOption2Visibility; }
            set
            {
                if (_upgradeOption2Visibility == value)
                {
                    return;
                }

                _upgradeOption2Visibility = value;
                OnPropertyChanged(nameof(UpgradeOption2Visibility));
            }
        }

        public string UpgradeTooltip1
        {
            get { return _upgradeTooltip1; }
            set
            {
                if (_upgradeTooltip1 == value)
                {
                    return;
                }

                _upgradeTooltip1 = value;
                OnPropertyChanged(nameof(UpgradeTooltip1));
            }
        }

        public string UpgradeTooltip2
        {
            get { return _upgradeTooltip2; }
            set
            {
                if (_upgradeTooltip2 == value)
                {
                    return;
                }

                _upgradeTooltip2 = value;
                OnPropertyChanged(nameof(UpgradeTooltip2));
            }
        }

        public void Initialize(Canvas gameCanvas)
        {
            if (_gameManager != null)
            {
                return;
            }

            // Load all game data once the play canvas is ready.
            _gameCanvas = gameCanvas;

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

            _gameManager.OnGoldChanged = newGold => GoldText = $"Gold: {newGold}";
            _gameManager.OnHealthChanged = newHp => HealthText = $"HP: {newHp}";
            _gameManager.OnPlayerDefeated = OnPlayerDefeated;
            _gameManager.OnWaveEnded = OnWaveEnded;
        }

        public void ClearUpgradePanel(object originalSource)
        {
            if (originalSource is not Button)
            {
                ClearSelectedTower();
            }
        }

        public void SetDragEffects(DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent("TowerID")
                ? DragDropEffects.Copy
                : DragDropEffects.None;

            e.Handled = true;
        }

        public void DropTower(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("TowerID"))
            {
                return;
            }

            // Convert the dragged tower id into the matching tower data.
            Point pos = e.GetPosition(_gameCanvas);
            var towerId = e.Data.GetData("TowerID")?.ToString();

            int id = int.Parse(towerId);
            var towerData = _gameManager.GetTowerData().FirstOrDefault(t => t.TowerID == id);

            if (towerData != null)
            {
                Tower tower = _gameManager.PlaceTower(pos, towerData);

                if (tower != null)
                {
                    Towers.Add(tower);
                }
            }
            else
            {
                MessageBox.Show("TowerData not found.");
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

        private bool SelectTowerCommandCanExecute(object parameter)
        {
            return parameter is Tower;
        }

        private void SelectTowerCommandExecute(object parameter)
        {
            Tower tower = parameter as Tower;
            if (tower == null)
            {
                return;
            }

            // Show only upgrades for the next tier of the selected tower.
            _selectedTower = tower;

            var upgrades = _gameManager.GetTowerData()
                .Where(u => u.TowerID == tower.TowerID && u.Tier == tower.Tier + 1)
                .ToList();

            UpgradeOption1 = upgrades.ElementAtOrDefault(0);
            UpgradeOption2 = upgrades.ElementAtOrDefault(1);
            UpgradeTooltip1 = UpgradeOption1 == null ? string.Empty : BuildUpgradeTooltip(tower, UpgradeOption1);
            UpgradeTooltip2 = UpgradeOption2 == null ? string.Empty : BuildUpgradeTooltip(tower, UpgradeOption2);
            UpgradeOption1Visibility = UpgradeOption1 == null ? Visibility.Collapsed : Visibility.Visible;
            UpgradeOption2Visibility = UpgradeOption2 == null ? Visibility.Collapsed : Visibility.Visible;
            UpgradePanelX = tower.Position.X - 110;
            UpgradePanelY = tower.Position.Y - 30;
            UpgradePanelVisibility = Visibility.Visible;
            OnPropertyChanged(nameof(SellTowerText));
            OnPropertyChanged(nameof(SellTowerTooltip));
        }

        private bool UpgradeTowerCommandCanExecute(object parameter)
        {
            return parameter is TowerData && _selectedTower != null;
        }

        private void UpgradeTowerCommandExecute(object parameter)
        {
            TowerData upgrade = parameter as TowerData;
            if (upgrade == null || _selectedTower == null)
            {
                return;
            }

            _gameManager.UpgradeTower(_selectedTower, upgrade);
            ClearSelectedTower();
        }

        private bool SellTowerCommandCanExecute(object parameter)
        {
            return _selectedTower != null;
        }

        private void SellTowerCommandExecute(object parameter)
        {
            if (_selectedTower == null)
            {
                return;
            }

            Tower towerToSell = _selectedTower;
            _gameManager.SellTower(towerToSell);
            Towers.Remove(towerToSell);
            ClearSelectedTower();
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

        private int GetSellValue(Tower tower)
        {
            return (int)(tower.Cost * 0.7);
        }

        private void ClearSelectedTower()
        {
            UpgradePanelVisibility = Visibility.Collapsed;
            _selectedTower = null;
            UpgradeOption1 = null;
            UpgradeOption2 = null;
            UpgradeTooltip1 = string.Empty;
            UpgradeTooltip2 = string.Empty;
            UpgradeOption1Visibility = Visibility.Collapsed;
            UpgradeOption2Visibility = Visibility.Collapsed;
            OnPropertyChanged(nameof(SellTowerText));
            OnPropertyChanged(nameof(SellTowerTooltip));
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

            // Keep the UI wave counter in sync with the game manager.
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
