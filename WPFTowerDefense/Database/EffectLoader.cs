using System;
using System.Collections.Generic;
using System.Data.SQLite;
using WPFTowerDefense.GameLogic;

namespace WPFTowerDefense.Database
{
    // Table: Effect
    public static class EffectLoader
    {
        public static List<EffectData> LoadEffectsFromDatabase(string dbPath)
        {
            var effects = new List<EffectData>();

            try
            {
                using var conn = new SQLiteConnection($"Data Source={dbPath}");
                conn.Open();

                string query = "SELECT Name, Type, DamageFactor, Duration, ValuePercent, ValueFlat FROM Effect";
                using var cmd = new SQLiteCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    effects.Add(new EffectData
                    {
                        Name = reader.GetString(0),
                        Type = reader.GetString(1),
                        DamageFactor = reader.GetDouble(2),
                        Duration = reader.GetDouble(3),
                        ValuePercent = reader.GetDouble(4),
                        ValueFlat = reader.GetDouble(5)
                    });
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"[SQLite Error] Failed to load effects from database: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[General Error] Unexpected error while loading effects: {ex.Message}");
            }

            return effects;
        }
    }
}
