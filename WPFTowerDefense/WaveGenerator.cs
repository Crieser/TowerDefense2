using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTowerDefense.GameLogic;

namespace WPFTowerDefense
{
    // WaveGenerator is responsible for generating enemy waves based on the current wave number
    public static class WaveGenerator
    {
        public static List<EnemyData> GenerateWave(List<EnemyData> allEnemies, int waveNumber)
        {
            var result = new List<EnemyData>();
            var rand = new Random();

            int level = 1;
            double level2Chance = Math.Clamp((waveNumber - 10) * 0.1, 0, 1);
            double level3Chance = Math.Clamp((waveNumber - 20) * 0.1, 0, 1);
            double level4Chance = Math.Clamp((waveNumber - 30) * 0.1, 0, 1);
            double level5Chance = Math.Clamp((waveNumber - 40) * 0.1, 0, 1);
            double roll = rand.NextDouble();

            if (roll < level5Chance)
                level = 5;
            else if (roll < level5Chance + level4Chance)
                level = 4;
            else if (roll < level5Chance + level4Chance + level3Chance)
                level = 3;
            else if (roll < level5Chance + level4Chance + level3Chance + level2Chance)
                level = 2;

            int enemyCount = 5 + waveNumber * 2;

            if (waveNumber == 5)
            {
                var grassEnemies = allEnemies.Where(e => e.Type == "Grass" && e.Level == 1).ToList();
                result.AddRange(grassEnemies.Take(enemyCount - 2));
                result.Add(allEnemies.First(e => e.Type == "Fire" && e.Level == 1));
                result.Add(allEnemies.First(e => e.Type == "Water" && e.Level == 1));
                return result;
            }

            List<string> types = waveNumber <= 5
                ? new List<string> { "Grass" }
                : new List<string> { "Grass", "Fire", "Water", "Normal" };

            for (int i = 0; i < enemyCount; i++)
            {
                string type = types[rand.Next(types.Count)];
                var candidates = allEnemies.Where(e => e.Type == type && e.Level == level).ToList();
                if (candidates.Count > 0)
                    result.Add(candidates[rand.Next(candidates.Count)]);
            }

            return result;
        }
    }
}
