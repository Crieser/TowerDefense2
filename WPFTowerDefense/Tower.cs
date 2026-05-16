using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFTowerDefense
{
    // Represents a tower in the tower defense game.
    public class Tower
    {
        public Point Position;
        public int TowerID;
        public string Type;
        public string Effect;
        public int UpgradeID;
        public string TowerName;
        public int Tier;
        public int Version;
        public double AoERadius;
        public double Damage;
        public double Range;
        public double AttackSpeed;
        public int Cost;

        public DateTime LastAttackTime = DateTime.MinValue;

        public double AttackCooldown => 0.5 / AttackSpeed;
        public double PixelRange => Range * 80;

    }
}
