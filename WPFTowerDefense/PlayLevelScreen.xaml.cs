using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPFTowerDefense.Database;
using WPFTowerDefense.GameLogic;

namespace WPFTowerDefense
{
    public partial class PlayLevelScreen : Page
    {
        private const int TileSize = 80;
        private const int GridWidth = 16;
        private const int GridHeight = 7;
        private GameManager gameManager;
        private DispatcherTimer waveDelayTimer = new();
        private int currentWave = 0;
        private LevelData currentLevel;
        private Button startGameButton;
        private TextBlock waveText;
        private TextBlock goldText;
        private TextBlock healthText;
        private Tower selectedTower;
        private Button upgradeButton1;
        private Button upgradeButton2;

        // Initializes the level, loads all game data, sets up callbacks and defeat handling
        public PlayLevelScreen(string levelPath)
        {
            InitializeComponent();

            gameManager = new GameManager(GameCanvas, TileSize);
            gameManager.ShowGoldWarning = ShowInsufficientGoldWarning;

            var enemyData = EnemyLoader.LoadEnemiesFromDatabase("Database/TD.db");
            gameManager.SetEnemyData(enemyData);

            var effects = EffectLoader.LoadEffectsFromDatabase("Database/TD.db");
            gameManager.SetEffectData(effects);

            var towerData = TowerLoader.LoadTowersFromDatabase("Database/TD.db");
            gameManager.SetTowerData(towerData);

            currentLevel = LevelLoader.LoadLevelFromJson(levelPath);
            gameManager.LoadLevel(currentLevel);

            gameManager.OnTowerSelected = ShowUpgradeButtons;

            gameManager.OnGoldChanged = (newGold) =>
            {
                goldText.Text = $"Gold: {newGold}";
            };

            gameManager.OnHealthChanged = (newHP) => healthText.Text = $"HP: {newHP}";
            gameManager.OnPlayerDefeated = () =>
            {
                MessageBox.Show("You lost! The enemies got through!", "Game Over", MessageBoxButton.OK, MessageBoxImage.Warning);
            };

            gameManager.OnPlayerDefeated = () =>
            {
                Dispatcher.Invoke(() =>
                {
                    waveDelayTimer.Stop();
                    gameManager.Dispose();
                    gameManager = null;


                    int lastWave = currentWave > 0 ? currentWave - 1 : 0;
                    var mainWindow = (MainWindow)Application.Current.MainWindow;
                    mainWindow.NavigateTo(new GameOverScreen(lastWave, currentLevel.LevelName, mainWindow));
                });
            };

            AddUIElements();

            gameManager.OnWaveEnded = OnWaveEnded;

            waveDelayTimer.Interval = TimeSpan.FromSeconds(5);
            waveDelayTimer.Tick += (s, e) =>
            {
                waveDelayTimer.Stop();
                StartNextWave();
            };
        }
        // Show upgrade buttons for the selected tower
        private void ShowUpgradeButtons(Tower tower)
        {
            selectedTower = tower;

            // Remove old buttons
            UpgradeLayer.Children.Clear();

            // Load available upgrades for this tower
            var upgrades = gameManager.GetTowerData()
                .Where(u => u.TowerID == tower.TowerID && u.Tier == tower.Tier + 1)
                .ToList();

            if (upgrades.Count < 2) return;

            // First upgrade button (Version 1)
            upgradeButton1 = CreateUpgradeButton(upgrades[0], tower);
            Canvas.SetLeft(upgradeButton1, tower.Position.X + 30);
            Canvas.SetTop(upgradeButton1, tower.Position.Y - 30);
            UpgradeLayer.Children.Add(upgradeButton1);

            // Second upgrade button (Version 2)
            upgradeButton2 = CreateUpgradeButton(upgrades[1], tower);
            Canvas.SetLeft(upgradeButton2, tower.Position.X - 110);
            Canvas.SetTop(upgradeButton2, tower.Position.Y - 30);
            UpgradeLayer.Children.Add(upgradeButton2);
        }
        // Creates an upgrade button for the tower upgrade
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

            if (gameManager.Gold < upgrade.Cost)
                btn.IsEnabled = false;

            btn.Click += (s, e) =>
            {
                gameManager.UpgradeTower(baseTower, upgrade);
                UpgradeLayer.Children.Clear(); // Remove buttons after upgrade
            };

            return btn;
        }

        // Tooltip shows how stats change with this upgrade
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
        // Adds UI elements to the TowerBar, including health, wave, gold text, and buttons
        private void AddUIElements()
        {
            var spacer = new StackPanel
            {
                Width = double.NaN,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            TowerBar.Children.Add(spacer);


            var rightSidePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(20, 0, 10, 0)
            };

            var returnToMenuButton = new Button
            {
                Content = "Return to Menu",
                Width = 150,
                Height = 40,
                Visibility = Visibility.Collapsed
            };

            returnToMenuButton.Click += (s, e) =>
            {
                var result = MessageBox.Show("Do you want to end the game and return to the main menu?",
                                             "End Game",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    waveDelayTimer?.Stop();
                    gameManager?.Dispose();
                    gameManager = null;

                    int lastWave = currentWave > 0 ? currentWave - 1 : 0;

                    // ✅ Save highscore manually
                    Database.HighscoreManager.UpdateBestWave("Database/TD.db", currentLevel.LevelName, lastWave);

                    var mainWindow = (MainWindow)Application.Current.MainWindow;
                    mainWindow.NavigateTo(new GameOverScreen(lastWave, currentLevel.LevelName, mainWindow));
                }
            };

            startGameButton = UIElements.CreateStartButton(() =>
            {
                rightSidePanel.Children.Remove(startGameButton);
                returnToMenuButton.Visibility = Visibility.Visible;
                StartNextWave();
            });

            healthText = new TextBlock
            {
                Text = "HP: 100",
                FontSize = 24,
                Foreground = Brushes.Red,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(20, 0, 10, 0)
            };
            waveText = new TextBlock
            {
                Text = "Wave: 0",
                FontSize = 24,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };

            goldText = new TextBlock
            {
                Text = "Gold: 500",
                FontSize = 24,
                Foreground = Brushes.Gold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(20, 0, 0, 0)
            };
            TowerBar.Children.Add(rightSidePanel);
            rightSidePanel.Children.Add(startGameButton);
            rightSidePanel.Children.Add(returnToMenuButton);
            rightSidePanel.Children.Add(healthText);
            rightSidePanel.Children.Add(waveText);
            rightSidePanel.Children.Add(goldText);
        }
        // Shows a warning if the player tries to place a tower without enough gold
        public async void ShowInsufficientGoldWarning()
        {
            InsufficientGoldText.Visibility = Visibility.Visible;
            await Task.Delay(1000);
            InsufficientGoldText.Visibility = Visibility.Collapsed;
        }
        // Starts the next wave of enemies, updates the wave count and updates the UI
        private void StartNextWave()
        {
            if (gameManager == null) return;
            currentWave++;
            waveText.Text = $"Wave: {currentWave}";
            gameManager.StartWave(currentWave);
        }
        // Handles the end of a wave, starts the delay timer before the next wave
        private void OnWaveEnded()
        {
            waveDelayTimer.Start();
        }
        // Handles drag and drop functionality for towers
        private void Tower_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender is Image image && image.Tag != null)
            {
                var data = new DataObject();
                data.SetData("TowerImage", image.Source);
                data.SetData("TowerID", image.Tag);
                DragDrop.DoDragDrop(image, data, DragDropEffects.Copy);
            }
        }
        // Handles the drag over event to determine if the drop is valid
        private void GameCanvas_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = (e.Data.GetDataPresent("TowerImage") && e.Data.GetDataPresent("TowerID"))
                ? DragDropEffects.Copy
                : DragDropEffects.None;

            e.Handled = true;
        }
        // Handles the drop event to place the correct tower on the game canvas
        private void GameCanvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("TowerImage") && e.Data.GetDataPresent("TowerID"))
            {
                Point pos = e.GetPosition(GameCanvas);
                var image = e.Data.GetData("TowerImage") as ImageSource;
                var towerID = e.Data.GetData("TowerID")?.ToString();

                int id = int.Parse(towerID);
                var towerData = gameManager.GetTowerData().FirstOrDefault(t => t.TowerID == id);

                if (towerData != null)
                {
                    gameManager.PlaceTower(pos, image, towerData);
                }
                else
                {
                    MessageBox.Show("TowerData not found.");
                }
            }
        }
        // Handles the mouse down event to clear upgrade buttons when clicking outside of them
        private void Page_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is not Button btn || (btn.Name != "UpgradeOption1" && btn.Name != "UpgradeOption2"))
            {
                UpgradeLayer.Children.Clear();
            }
        }
    }
}
