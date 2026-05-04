using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTowerDefense.GameLogic
{
    // Table TowerTypes & TowerUpgrades
    public class TowerData
    {
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
    }
}
