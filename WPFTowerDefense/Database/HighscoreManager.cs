using System;
using System.Linq;

namespace WPFTowerDefense.Database
{
    // Table: Highscores
    public static class HighscoreManager
    {
        public static int GetBestWave(string dbPath, string levelName)
        {
            try
            {
                using var db = new TowerDefenseDbContext(dbPath);

                var highscore = db.Highscores
                    .FirstOrDefault(score => score.GameLevel == levelName);

                return highscore == null ? 0 : highscore.BestWave;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[General Error] Unexpected error in GetBestWave: {ex.Message}");
                return 0;
            }
        }

        public static void UpdateBestWave(string dbPath, string levelName, int wave)
        {
            try
            {
                using var db = new TowerDefenseDbContext(dbPath);

                var highscore = db.Highscores
                    .FirstOrDefault(score => score.GameLevel == levelName);

                if (highscore == null || wave <= highscore.BestWave)
                {
                    return;
                }

                highscore.BestWave = wave;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[General Error] Unexpected error in UpdateBestWave: {ex.Message}");
            }
        }
    }
}
