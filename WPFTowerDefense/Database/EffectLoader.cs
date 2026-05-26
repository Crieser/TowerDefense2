using System;
using System.Collections.Generic;
using System.Linq;
using WPFTowerDefense.GameLogic;

namespace WPFTowerDefense.Database
{
    // Table: Effect
    public static class EffectLoader
    {
        public static List<EffectData> LoadEffectsFromDatabase(string dbPath)
        {
            try
            {
                using var db = new TowerDefenseDbContext(dbPath);

                return db.Effects
                    .Select(effect => new EffectData
                    {
                        Name = effect.Name,
                        Type = effect.Type,
                        DamageFactor = effect.DamageFactor,
                        Duration = effect.Duration,
                        ValuePercent = effect.ValuePercent,
                        ValueFlat = effect.ValueFlat
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[General Error] Unexpected error while loading effects: {ex.Message}");
                return new List<EffectData>();
            }
        }
    }
}
