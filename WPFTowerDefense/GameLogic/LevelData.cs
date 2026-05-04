using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace WPFTowerDefense.GameLogic
{
    public class LevelData
    {
        public string LevelName { get; set; }
        [JsonConverter(typeof(PointListJsonConverter))]
        public List<Point> PathTiles { get; set; }

        [JsonConverter(typeof(PointListJsonConverter))]
        public List<Point> BlockedTiles { get; set; }
        public string BackgroundImagePath { get; set; }
    }
}
