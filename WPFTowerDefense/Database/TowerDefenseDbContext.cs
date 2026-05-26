using Microsoft.EntityFrameworkCore;

namespace WPFTowerDefense.Database
{
    public class TowerDefenseDbContext : DbContext
    {
        private readonly string _dbPath;

        public TowerDefenseDbContext(string dbPath)
        {
            _dbPath = dbPath;
        }

        public DbSet<EnemyLevelEntity> EnemyLevels { get; set; }
        public DbSet<EnemyTypeEntity> EnemyTypes { get; set; }
        public DbSet<TowerTypeEntity> TowerTypes { get; set; }
        public DbSet<TowerUpgradeEntity> TowerUpgrades { get; set; }
        public DbSet<EffectEntity> Effects { get; set; }
        public DbSet<HighscoreEntity> Highscores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EnemyLevelEntity>().ToTable("EnemyLevels");
            modelBuilder.Entity<EnemyLevelEntity>().HasKey(enemyLevel => new { enemyLevel.EnemyID, enemyLevel.Level });

            modelBuilder.Entity<EnemyTypeEntity>().ToTable("EnemyTypes");
            modelBuilder.Entity<EnemyTypeEntity>().HasKey(enemyType => enemyType.EnemyID);

            modelBuilder.Entity<TowerTypeEntity>().ToTable("TowerTypes");
            modelBuilder.Entity<TowerTypeEntity>().HasKey(towerType => towerType.TowerID);

            modelBuilder.Entity<TowerUpgradeEntity>().ToTable("TowerUpgrades");
            modelBuilder.Entity<TowerUpgradeEntity>().HasKey(towerUpgrade => towerUpgrade.UpgradeID);

            modelBuilder.Entity<EffectEntity>().ToTable("Effect");
            modelBuilder.Entity<EffectEntity>().HasKey(effect => effect.Name);

            modelBuilder.Entity<HighscoreEntity>().ToTable("Highscores");
            modelBuilder.Entity<HighscoreEntity>().HasKey(highscore => highscore.GameLevel);
        }
    }

    public class EnemyLevelEntity
    {
        public int EnemyID { get; set; }
        public int Level { get; set; }
        public double Health { get; set; }
        public double MovementSpeed { get; set; }
        public int Damage { get; set; }
        public int GoldDrop { get; set; }
    }

    public class EnemyTypeEntity
    {
        public int EnemyID { get; set; }
        public string Type { get; set; }
        public string Vulnerability { get; set; }
        public double IncreasedDamageTakenPercent { get; set; }
        public string Resistance { get; set; }
        public double DecreasedDamageTakenPercent { get; set; }
    }

    public class TowerTypeEntity
    {
        public int TowerID { get; set; }
        public string Type { get; set; }
        public string Effect { get; set; }
    }

    public class TowerUpgradeEntity
    {
        public int UpgradeID { get; set; }
        public int TowerID { get; set; }
        public string TowerName { get; set; }
        public int Tier { get; set; }
        public int Version { get; set; }
        public double AoERadius { get; set; }
        public double Damage { get; set; }
        public double Range { get; set; }
        public double AttackSpeed { get; set; }
        public int Cost { get; set; }
    }

    public class EffectEntity
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public double DamageFactor { get; set; }
        public double Duration { get; set; }
        public double ValuePercent { get; set; }
        public double ValueFlat { get; set; }
    }

    public class HighscoreEntity
    {
        public string GameLevel { get; set; }
        public int BestWave { get; set; }
    }
}
