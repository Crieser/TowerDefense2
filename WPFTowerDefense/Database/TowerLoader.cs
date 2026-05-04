using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTowerDefense.GameLogic;

namespace WPFTowerDefense.Database
{
    // Table: TowerTypes & TowerUpgrades
    public static class TowerLoader
    {
        public static List<TowerData> LoadTowersFromDatabase(string path)
        {
            var towers = new List<TowerData>();

            try
            {
                using var conn = new SQLiteConnection($"Data Source={path}");
                conn.Open();

                string query = @"
                    SELECT t.TowerID, t.Type, t.Effect,
                           u.UpgradeID, u.TowerName, u.Tier, u.Version, u.AoERadius,
                           u.Damage, u.Range, u.AttackSpeed, u.Cost
                    FROM TowerTypes t
                    JOIN TowerUpgrades u ON t.TowerID = u.TowerID
                ";

                using var cmd = new SQLiteCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    towers.Add(new TowerData
                    {
                        TowerID = reader.GetInt32(0),
                        Type = reader.GetString(1),
                        Effect = reader.GetString(2),
                        UpgradeID = reader.GetInt32(3),
                        TowerName = reader.GetString(4),
                        Tier = reader.GetInt32(5),
                        Version = reader.GetInt32(6),
                        AoERadius = reader.GetDouble(7),
                        Damage = reader.GetDouble(8),
                        Range = reader.GetDouble(9),
                        AttackSpeed = reader.GetDouble(10),
                        Cost = reader.GetInt32(11)
                    });
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"[SQLite Error] Failed to load towers from database: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[General Error] Unexpected error while loading towers: {ex.Message}");
            }

            return towers;
        }
    }
}
