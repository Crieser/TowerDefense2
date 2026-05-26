using System;
using System.Collections.Generic;
using System.Linq;
using WPFTowerDefense.GameLogic;

namespace WPFTowerDefense.Database
{
    // Table: TowerTypes & TowerUpgrades
    public static class TowerLoader
    {
        public static List<TowerData> LoadTowersFromDatabase(string path)
        {
            try
            {
                using var db = new TowerDefenseDbContext(path);

                return db.TowerTypes
                    .Join(db.TowerUpgrades,
                          towerType => towerType.TowerID,
                          towerUpgrade => towerUpgrade.TowerID,
                          (towerType, towerUpgrade) => new TowerData
                          {
                              TowerID = towerType.TowerID,
                              Type = towerType.Type,
                              Effect = towerType.Effect,
                              UpgradeID = towerUpgrade.UpgradeID,
                              TowerName = towerUpgrade.TowerName,
                              Tier = towerUpgrade.Tier,
                              Version = towerUpgrade.Version,
                              AoERadius = towerUpgrade.AoERadius,
                              Damage = towerUpgrade.Damage,
                              Range = towerUpgrade.Range,
                              AttackSpeed = towerUpgrade.AttackSpeed,
                              Cost = towerUpgrade.Cost
                          })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[General Error] Unexpected error while loading towers: {ex.Message}");
                return new List<TowerData>();
            }
        }
    }
}
