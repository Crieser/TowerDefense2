using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTowerDefense.GameLogic
{
    //Table EnemyLevels & EnemyTypes
    public class EnemyData
    {
        public int EnemyID { get; set; }
        public int Level { get; set; }
        public string Type { get; set; }
        public string Vulnerability { get; set; }
        public double IncreasedDamageTakenPercent { get; set; }
        public string Resistance { get; set; }
        public double DecreasedDamageTakenPercent { get; set; }

        public double Health { get; set; }
        public double MovementSpeed { get; set; }
        public int Damage { get; set; }
        public int GoldDrop { get; set; }

        public string TexturePath => Type == "Boss"
            ? "pack://application:,,,/Resources/Enemies/Boss.png"
            : $"pack://application:,,,/Resources/Enemies/Enemy{Type}.png";
    }
}
