using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WPFTowerDefense.GameLogic;

namespace WPFTowerDefense.Database
{
    //Level from JSON
    public static class LevelLoader
    {
        public static LevelData LoadLevelFromJson(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var level = JsonSerializer.Deserialize<LevelData>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return level;
        }
    }
}
