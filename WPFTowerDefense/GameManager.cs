using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFTowerDefense.GameLogic;

namespace WPFTowerDefense
{
    public class GameManager
    {
        private readonly Canvas gameCanvas;
        private readonly int tileSize;
        private readonly ImageSource pathTileTexture = new BitmapImage(new Uri("pack://application:,,,/Resources/MapTiles/PathLeftRight.png"));
        private readonly ImageSource floorTileTexture = new BitmapImage(new Uri("pack://application:,,,/Resources/MapTiles/GrassFloor.png"));
        private List<EnemyData> loadedEnemyData;
        private List<EffectData> loadedEffects;
        private List<TowerData> loadedTowers;
        private LevelData currentLevel;
        private List<Point> enemyPath = new();
        private readonly List<Tower> placedTowers = new();
        private readonly List<Enemy> activeEnemies = new();
        private int enemiesToSpawn = 5;
        private int enemiesSpawned = 0;
        private TimeSpan spawnInterval = TimeSpan.FromMilliseconds(500);
        private DateTime lastUpdateTime;
        private DateTime lastSpawnTime;
        private int currentWaveNumber = 1;
        private Queue<EnemyData> currentWaveQueue;
        public Action OnWaveEnded { get; set; }
        public int Gold { get; private set; } = 500;
        public Action<int> OnGoldChanged { get; set; }
        public int PlayerHealth { get; private set; } = 100;
        public Action<int> OnHealthChanged { get; set; }
        public Action OnPlayerDefeated { get; set; }
        public Action ShowGoldWarning { get; set; }
        public string currentLevelName;
        public Action<Tower> OnTowerSelected;

        public GameManager(Canvas canvas, int tileSize)
        {
            gameCanvas = canvas;
            this.tileSize = tileSize;
        }

        public void LoadLevel(LevelData level)
        {
            currentLevel = level;
            currentLevelName = level.LevelName;
            enemyPath = level.PathTiles;
            InitializeGrid(16, 8);
        }

        public void InitializeGrid(int gridWidth, int gridHeight)
        {
            gameCanvas.Children.Clear();

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Point tilePos = new(x, y);
                    var tileImage = new Image
                    {
                        Width = tileSize,
                        Height = tileSize,
                        IsHitTestVisible = false,
                        Source = GetTileTexture(tilePos)
                    };

                    Canvas.SetLeft(tileImage, x * tileSize);
                    Canvas.SetTop(tileImage, y * tileSize);
                    gameCanvas.Children.Add(tileImage);
                }
            }
        }

        private ImageSource GetTileTexture(Point tilePos)
        {
            if (!currentLevel.PathTiles.Contains(tilePos))
                return floorTileTexture;

            int index = currentLevel.PathTiles.IndexOf(tilePos);

            if (index == 0)
            {
                Vector dir = currentLevel.PathTiles[1] - tilePos;
                return GetEndCapTile(dir);
            }

            if (index == currentLevel.PathTiles.Count - 1)
            {
                Vector dir = tilePos - currentLevel.PathTiles[index - 1];
                return GetEndCapTile(dir);
            }

            Point prev = currentLevel.PathTiles[index - 1];
            Point next = currentLevel.PathTiles[index + 1];
            return GetPathTileTexture(prev, tilePos, next);
        }

        private ImageSource GetPathTileTexture(Point prev, Point current, Point next)
        {
            Vector dir1 = current - prev;
            Vector dir2 = next - current;

            if ((dir1 == new Vector(1, 0) && dir2 == new Vector(1, 0)) || (dir1 == new Vector(-1, 0) && dir2 == new Vector(-1, 0)))
                return LoadTexture("PathLeftRight.png");
            if ((dir1 == new Vector(0, 1) && dir2 == new Vector(0, 1)) || (dir1 == new Vector(0, -1) && dir2 == new Vector(0, -1)))
                return LoadTexture("PathTopBottom.png");
            if ((dir1 == new Vector(1, 0) && dir2 == new Vector(0, 1)) || (dir1 == new Vector(0, -1) && dir2 == new Vector(-1, 0)))
                return LoadTexture("PathBottomLeft.png");
            if ((dir1 == new Vector(0, 1) && dir2 == new Vector(-1, 0)) || (dir1 == new Vector(1, 0) && dir2 == new Vector(0, -1)))
                return LoadTexture("PathTopLeft.png");
            if ((dir1 == new Vector(-1, 0) && dir2 == new Vector(0, 1)) || (dir1 == new Vector(0, -1) && dir2 == new Vector(1, 0)))
                return LoadTexture("PathBottomRight.png");
            if ((dir1 == new Vector(0, 1) && dir2 == new Vector(1, 0)) || (dir1 == new Vector(-1, 0) && dir2 == new Vector(0, -1)))
                return LoadTexture("PathTopRight.png");

            return pathTileTexture;
        }

        private ImageSource GetEndCapTile(Vector dir)
        {
            if (dir == new Vector(1, 0)) return LoadTexture("PathLeftRight.png");
            if (dir == new Vector(-1, 0)) return LoadTexture("PathLeftRight.png");
            if (dir == new Vector(0, 1)) return LoadTexture("PathTopBottom.png");
            if (dir == new Vector(0, -1)) return LoadTexture("PathTopBottom.png");

            return pathTileTexture;
        }

        private ImageSource LoadTexture(string fileName)
        {
            return new BitmapImage(new Uri($"pack://application:,,,/Resources/MapTiles/{fileName}"));
        }

        private void AddGold(int amount)
        {
            Gold += amount;
            OnGoldChanged?.Invoke(Gold);
        }

        private void RewardWaveEnd()
        {
            AddGold(200);
        }

        public void PlaceTower(Point dropPosition, ImageSource towerImage, TowerData towerData)
        {
            if (Gold < towerData.Cost)
            {
                ShowGoldWarning?.Invoke();
                return;
            }

            int gridX = (int)(dropPosition.X / tileSize);
            int gridY = (int)(dropPosition.Y / tileSize);
            Point gridPos = new(gridX, gridY);

            if (enemyPath.Contains(gridPos))
                return;

            foreach (var tower in placedTowers)
            {
                int towerX = (int)(tower.Position.X / tileSize);
                int towerY = (int)(tower.Position.Y / tileSize);
                if (towerX == gridX && towerY == gridY)
                    return;
            }

            double snappedX = gridX * tileSize;
            double snappedY = gridY * tileSize;

            Tower newTower = new Tower
            {
                Position = new Point(snappedX + tileSize / 2, snappedY + tileSize / 2),
                TowerID = towerData.TowerID,
                Type = towerData.Type,
                Effect = towerData.Effect,
                UpgradeID = towerData.UpgradeID,
                TowerName = towerData.TowerName,
                Tier = towerData.Tier,
                Version = towerData.Version,
                AoERadius = towerData.AoERadius,
                Damage = towerData.Damage,
                Range = towerData.Range,
                AttackSpeed = towerData.AttackSpeed,
                Cost = towerData.Cost
            };

            Gold -= towerData.Cost;
            OnGoldChanged?.Invoke(Gold);

            placedTowers.Add(newTower);

            Image towerImageVisual = new()
            {
                Width = tileSize,
                Height = tileSize,
                Source = towerImage,
                IsHitTestVisible = true
            };

            towerImageVisual.MouseLeftButtonDown += (s, e) =>
            {
                OnTowerSelected?.Invoke(newTower);
                e.Handled = true;
            };

            Canvas.SetLeft(towerImageVisual, snappedX);
            Canvas.SetTop(towerImageVisual, snappedY);
            gameCanvas.Children.Add(towerImageVisual);
        }

        public void SetEnemyData(List<EnemyData> data)
        {
            loadedEnemyData = data;
        }

        public void SetEffectData(List<EffectData> data)
        {
            loadedEffects = data;
        }

        public void SetTowerData(List<TowerData> data)
        {
            loadedTowers = data;
        }

        public List<TowerData> GetTowerData()
        {
            return loadedTowers;
        }

        public void UpgradeTower(Tower baseTower, TowerData upgrade)
        {
            if (Gold < upgrade.Cost) return;

            Gold -= upgrade.Cost;
            OnGoldChanged?.Invoke(Gold);

            baseTower.Tier = upgrade.Tier;
            baseTower.Version = upgrade.Version;
            baseTower.Damage = upgrade.Damage;
            baseTower.Range = upgrade.Range;
            baseTower.AttackSpeed = upgrade.AttackSpeed;
            baseTower.AoERadius = upgrade.AoERadius;
            baseTower.Effect = upgrade.Effect;
            baseTower.UpgradeID = upgrade.UpgradeID;

            Console.WriteLine($"Upgraded to {upgrade.TowerName}");
        }

        public void StartWave(int waveNumber)
        {
            currentWaveNumber = waveNumber;
            currentWaveQueue = new Queue<EnemyData>(
                WaveGenerator.GenerateWave(loadedEnemyData, currentWaveNumber)
            );

            enemiesToSpawn = currentWaveQueue.Count;
            enemiesSpawned = 0;
            lastSpawnTime = DateTime.Now;
            lastUpdateTime = DateTime.Now;

            CompositionTarget.Rendering += GameLoop;
        }

        private void DamagePlayer(int amount)
        {
            PlayerHealth -= amount;
            if (PlayerHealth < 0) PlayerHealth = 0;
            OnHealthChanged?.Invoke(PlayerHealth);

            if (PlayerHealth == 0)
            {
                Database.HighscoreManager.UpdateBestWave("Database/TD.db", currentLevelName, currentWaveNumber - 1);
                OnPlayerDefeated?.Invoke();
            }
        }

        public void Dispose()
        {
            CompositionTarget.Rendering -= GameLoop;
            activeEnemies.Clear();
            placedTowers.Clear();
            currentWaveQueue?.Clear();
            gameCanvas.Children.Clear();

            Debug.WriteLine("GameManager instance disposed.");
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (gameCanvas == null || activeEnemies == null || placedTowers == null)
                return;

            DateTime now = DateTime.Now;
            double deltaTime = (now - lastUpdateTime).TotalSeconds;
            lastUpdateTime = now;

            if (enemiesSpawned < enemiesToSpawn && now - lastSpawnTime > spawnInterval)
            {
                if (currentWaveQueue.TryDequeue(out EnemyData data))
                {
                    Enemy enemy = new Enemy(data, enemyPath, tileSize, gameCanvas);
                    activeEnemies.Add(enemy);
                    enemiesSpawned++;
                    lastSpawnTime = now;
                }
            }

            List<Enemy> enemiesToRemove = new();

            foreach (var enemy in activeEnemies.ToList())
            {
                enemy.Update(deltaTime);

                if (!enemy.IsAlive || enemy.ReachedEnd)
                {
                    if (enemy.ReachedEnd)
                        DamagePlayer(enemy.Data.Damage);
                    else
                        AddGold(enemy.Data.GoldDrop);

                    enemy.RemoveFromCanvas(gameCanvas);
                    enemiesToRemove.Add(enemy);
                }
            }

            foreach (var enemy in enemiesToRemove)
            {
                activeEnemies.Remove(enemy);
            }

            if (enemiesSpawned >= enemiesToSpawn && activeEnemies.Count == 0)
            {
                CompositionTarget.Rendering -= GameLoop;
                RewardWaveEnd();
                OnWaveEnded?.Invoke();
            }

            foreach (var tower in placedTowers)
            {
                foreach (var enemy in activeEnemies.ToList())
                {
                    if (!enemy.IsAlive || enemy.ReachedEnd) continue;

                    double distance = (tower.Position - enemy.Position).Length;
                    if (distance <= tower.PixelRange)
                    {
                        double timeSinceLastShot = (DateTime.Now - tower.LastAttackTime).TotalSeconds;
                        if (timeSinceLastShot >= tower.AttackCooldown)
                        {
                            tower.LastAttackTime = DateTime.Now;
                            Point impactCenter = enemy.Position;

                            foreach (var targetEnemy in activeEnemies)
                            {
                                if (!targetEnemy.IsAlive || targetEnemy.ReachedEnd) continue;

                                double aoeDistance = (targetEnemy.Position - impactCenter).Length;
                                if (aoeDistance <= tower.AoERadius * tileSize)
                                {
                                    double dmg = tower.Damage;

                                    if (targetEnemy.Data.Vulnerability == tower.Type)
                                    {
                                        dmg += tower.Damage * targetEnemy.Data.IncreasedDamageTakenPercent;
                                    }
                                    else if (targetEnemy.Data.Resistance == tower.Type)
                                    {
                                        dmg -= tower.Damage * targetEnemy.Data.DecreasedDamageTakenPercent;
                                    }

                                    dmg = Math.Max(0, dmg);
                                    targetEnemy.TakeDamage(dmg);

                                    if (!string.IsNullOrEmpty(tower.Effect))
                                    {
                                        var effectData = loadedEffects.FirstOrDefault(effect => effect.Name == tower.Effect);
                                        if (effectData != null)
                                        {
                                            targetEnemy.ApplyEffect(effectData);
                                            Debug.WriteLine($"AOE hit: {tower.TowerName} -> {targetEnemy.Data.Type}, Effect: {effectData.Name}");
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
}
