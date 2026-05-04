using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTowerDefense.Database
{
    // Table: Highscores
    public static class HighscoreManager
    {
        public static int GetBestWave(string dbPath, string levelName)
        {
            try
            {
                using var conn = new SQLiteConnection($"Data Source={dbPath}");
                conn.Open();

                using var cmd = new SQLiteCommand("SELECT BestWave FROM Highscores WHERE GameLevel = @level", conn);
                cmd.Parameters.AddWithValue("@level", levelName);

                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"[SQLite Error] Failed to get best wave: {ex.Message}");
                return 0;
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
                int currentBest = GetBestWave(dbPath, levelName);
                if (wave <= currentBest) return;

                using var conn = new SQLiteConnection($"Data Source={dbPath}");
                conn.Open();

                using var cmd = new SQLiteCommand("UPDATE Highscores SET BestWave = @wave WHERE GameLevel = @level", conn);
                cmd.Parameters.AddWithValue("@wave", wave);
                cmd.Parameters.AddWithValue("@level", levelName);
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"[SQLite Error] Failed to update best wave: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[General Error] Unexpected error in UpdateBestWave: {ex.Message}");
            }
        }
    }
}
