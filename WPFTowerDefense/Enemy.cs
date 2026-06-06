using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFTowerDefense.GameLogic;

namespace WPFTowerDefense
{
    public class Enemy
    {
        public EnemyData Data { get; }
        public Point Position { get; private set; }
        public bool IsAlive => Health > 0;
        public bool ReachedEnd => pathIndex >= path.Count;
        private double MaxHealth;
        private double Health;
        private double BaseSpeed;
        private double CurrentSpeed;
        private readonly List<Point> path;
        private int pathIndex = 0;
        private int tileSize;
        private Image visual;
        private Rectangle healthBarBackground;
        private Rectangle healthBarFill;
        private List<ActiveEffect> activeEffects = new();

        public Enemy(EnemyData data, List<Point> tilePath, int tileSize, Canvas canvas)
        {
            Data = data;
            this.tileSize = tileSize;
            path = tilePath;

            MaxHealth = data.Health;
            Health = MaxHealth;
            BaseSpeed = data.MovementSpeed * tileSize * 4;
            CurrentSpeed = BaseSpeed;

            Position = new Point(path[0].X * tileSize + tileSize / 2, path[0].Y * tileSize + tileSize / 2);

            visual = new Image
            {
                Width = tileSize * 0.6,
                Height = tileSize * 0.6,
                Source = new BitmapImage(new Uri(data.TexturePath)),
                IsHitTestVisible = false
            };

            healthBarBackground = new Rectangle
            {
                Width = 40,
                Height = 6,
                Fill = Brushes.DarkRed,
                RadiusX = 2,
                RadiusY = 2
            };

            healthBarFill = new Rectangle
            {
                Width = 40,
                Height = 6,
                Fill = Brushes.LimeGreen,
                RadiusX = 2,
                RadiusY = 2
            };

            canvas.Children.Add(visual);
            canvas.Children.Add(healthBarBackground);
            canvas.Children.Add(healthBarFill);
            UpdateVisual();
        }

        public void Update(double deltaTime)
        {
            if (ReachedEnd || !IsAlive) return;

            ProcessEffects(deltaTime);

            Point targetTile = path[pathIndex];
            Point target = new Point(targetTile.X * tileSize + tileSize / 2, targetTile.Y * tileSize + tileSize / 2);
            Vector direction = target - Position;

            if (direction.Length < CurrentSpeed * deltaTime)
            {
                Position = target;
                pathIndex++;
            }
            else
            {
                direction.Normalize();
                Position += direction * CurrentSpeed * deltaTime;
            }

            UpdateVisual();
        }

        private void ProcessEffects(double deltaTime)
        {
            bool isStunned = false;
            double slowMultiplier = 1.0;

            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                var effect = activeEffects[i];
                effect.RemainingTime -= deltaTime;

                switch (effect.Type)
                {
                    case "dot":
                        TakeDamage(effect.DamagePerSecond * deltaTime);
                        break;
                    case "slow":
                        slowMultiplier *= 1 + effect.ValuePercent;
                        break;
                    case "stun":
                        isStunned = true;
                        break;
                }

                if (effect.RemainingTime <= 0)
                {
                    activeEffects.RemoveAt(i);
                }
            }

            CurrentSpeed = isStunned ? 0 : BaseSpeed * slowMultiplier;
        }

        private void UpdateVisual()
        {
            Canvas.SetLeft(visual, Position.X - visual.Width / 2);
            Canvas.SetTop(visual, Position.Y - visual.Height / 2);

            double barX = Position.X - 20;
            double barY = Position.Y - visual.Height / 2 - 10;
            double hpPercent = Math.Max(0, Health / MaxHealth);

            healthBarFill.Width = 40 * hpPercent;
            Canvas.SetLeft(healthBarBackground, barX);
            Canvas.SetTop(healthBarBackground, barY);
            Canvas.SetLeft(healthBarFill, barX);
            Canvas.SetTop(healthBarFill, barY);
        }

        public void TakeDamage(double amount)
        {
            Health -= amount;
            if (Health < 0) Health = 0;
        }

        public void RemoveFromCanvas(Canvas canvas)
        {
            canvas.Children.Remove(visual);
            canvas.Children.Remove(healthBarFill);
            canvas.Children.Remove(healthBarBackground);
        }

        public void ApplyEffect(EffectData effect)
        {
            if (effect.Type == "slow")
            {
                var activeSlow = activeEffects.Find(activeEffect => activeEffect.Type == "slow");
                if (activeSlow != null)
                {
                    activeSlow.RemainingTime = effect.Duration;
                    activeSlow.ValuePercent = effect.ValuePercent;
                    activeSlow.DamagePerSecond = effect.DamageFactor;
                    return;
                }
            }

            activeEffects.Add(new ActiveEffect
            {
                Type = effect.Type,
                RemainingTime = effect.Duration,
                ValuePercent = effect.ValuePercent,
                DamagePerSecond = effect.DamageFactor
            });
        }

        private class ActiveEffect
        {
            public string Type;
            public double RemainingTime;
            public double ValuePercent;
            public double DamagePerSecond;
        }
    }
}
