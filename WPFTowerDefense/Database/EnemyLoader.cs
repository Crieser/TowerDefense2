using System;
using System.Collections.Generic;
using System.Linq;
using WPFTowerDefense.GameLogic;

namespace WPFTowerDefense.Database
{
    // Table EnemyLevels & EnemyTypes
    public static class EnemyLoader
    {
        public static List<EnemyData> LoadEnemiesFromDatabase(string path)
        {
            try
            {
                using var db = new TowerDefenseDbContext(path);

                return db.EnemyLevels
                    .Join(db.EnemyTypes,
                          enemyLevel => enemyLevel.EnemyID,
                          enemyType => enemyType.EnemyID,
                          (enemyLevel, enemyType) => new EnemyData
                          {
                              EnemyID = enemyLevel.EnemyID,
                              Level = enemyLevel.Level,
                              Health = enemyLevel.Health,
                              MovementSpeed = enemyLevel.MovementSpeed,
                              Damage = enemyLevel.Damage,
                              GoldDrop = enemyLevel.GoldDrop,
                              Type = enemyType.Type,
                              Vulnerability = enemyType.Vulnerability,
                              IncreasedDamageTakenPercent = enemyType.IncreasedDamageTakenPercent,
                              Resistance = enemyType.Resistance,
                              DecreasedDamageTakenPercent = enemyType.DecreasedDamageTakenPercent
                          })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[General Error] Unexpected error while loading enemies: {ex.Message}");
                return new List<EnemyData>();
            }
        }
    }
}
