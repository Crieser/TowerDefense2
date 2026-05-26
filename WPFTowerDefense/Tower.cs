using System;
using System.Windows;
using WPFTowerDefense.Common;

namespace WPFTowerDefense
{
    // Represents a tower in the tower defense game.
    public class Tower : NotifyPropertyChanged
    {
        public Point Position { get; set; }
        public double VisualX { get; set; }
        public double VisualY { get; set; }
        public string TexturePath { get; set; }
        public int TowerID { get; set; }
        public string Type { get; set; }
        public string Effect { get; set; }
        public int UpgradeID { get; set; }
        public string TowerName { get; set; }
        public int Tier { get; set; }
        public int Version { get; set; }
        public double AoERadius { get; set; }
        public double Damage { get; set; }
        public double Range { get; set; }
        public double AttackSpeed { get; set; }
        public int Cost { get; set; }

        public DateTime LastAttackTime = DateTime.MinValue;

        public double AttackCooldown => 0.5 / AttackSpeed;
        public double PixelRange => Range * 80;

        public string InfoText
        {
            get
            {
                string effectText = string.IsNullOrWhiteSpace(Effect) ? "None" : Effect;

                return
                    $"{TowerName}\n" +
                    $"Cost: {Cost}\n" +
                    $"Damage: {Damage:0.##}\n" +
                    $"Range: {Range:0.##}\n" +
                    $"Attack Speed: {AttackSpeed:0.##}\n" +
                    $"AoE Radius: {AoERadius:0.##}\n" +
                    $"Effect: {effectText}";
            }
        }

        public void NotifyTowerChanged()
        {
            OnPropertyChanged(nameof(TowerName));
            OnPropertyChanged(nameof(Tier));
            OnPropertyChanged(nameof(Version));
            OnPropertyChanged(nameof(AoERadius));
            OnPropertyChanged(nameof(Damage));
            OnPropertyChanged(nameof(Range));
            OnPropertyChanged(nameof(AttackSpeed));
            OnPropertyChanged(nameof(Effect));
            OnPropertyChanged(nameof(UpgradeID));
            OnPropertyChanged(nameof(InfoText));
        }
    }
}
