using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTowerDefense.GameLogic
{
    //Table Effect
    public class EffectData
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public double DamageFactor { get; set; }
        public double Duration { get; set; }
        public double ValuePercent { get; set; }
        public double ValueFlat { get; set; }
    }
}
