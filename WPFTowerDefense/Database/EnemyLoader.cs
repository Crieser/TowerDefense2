using System;
using System.Collections.Generic;
using System.Data.SQLite;
using WPFTowerDefense.GameLogic;

namespace WPFTowerDefense.Database
{
    // Table EnemyLevels & EnemyTypes
    public static class EnemyLoader
    {
        public static List<EnemyData> LoadEnemiesFromDatabase(string path)
        {
            var enemies = new List<EnemyData>();

            try
            {
                using var conn = new SQLiteConnection($"Data Source={path}");
                conn.Open();

                string query = @"
                    SELECT el.EnemyID, el.Level, el.Health, el.MovementSpeed, el.Damage, el.GoldDrop,
                           et.Type, et.Vulnerability, et.IncreasedDamageTakenPercent,
                           et.Resistance, et.DecreasedDamageTakenPercent
                    FROM EnemyLevels el
                    JOIN EnemyTypes et ON el.EnemyID = et.EnemyID
                ";

                using var cmd = new SQLiteCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    enemies.Add(new EnemyData
                    {
                        EnemyID = reader.GetInt32(0),
                        Level = reader.GetInt32(1),
                        Health = reader.GetDouble(2),
                        MovementSpeed = reader.GetDouble(3),
                        Damage = reader.GetInt32(4),
                        GoldDrop = reader.GetInt32(5),
                        Type = reader.GetString(6),
                        Vulnerability = reader.GetString(7),
                        IncreasedDamageTakenPercent = reader.GetDouble(8),
                        Resistance = reader.GetString(9),
                        DecreasedDamageTakenPercent = reader.GetDouble(10),
                    });
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"[SQLite Error] Failed to load enemies from database: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[General Error] Unexpected error while loading enemies: {ex.Message}");
            }

            return enemies;
        }
    }
}
