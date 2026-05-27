classDiagram
    direction LR

    class MainViewModel {
        +CurrentView object
        +IsOverlayVisible bool
        +ShowStartScreen() void
    }

    class ViewModelBase {
        #IEventAggregator eventAggregator
    }

    class NotifyPropertyChanged {
        +PropertyChanged
        +OnPropertyChanged(string property) void
    }

    class StartScreenViewModel {
        +PlayCommand ICommand
        +OptionsCommand ICommand
        +ExitCommand ICommand
        +GameGuideCommand ICommand
    }

    class SelectLevelScreenViewModel {
        +ReturnCommand ICommand
        +PlayCommand ICommand
        +NextLevelCommand ICommand
        +PreviousLevelCommand ICommand
    }

    class PlayLevelScreenViewModel {
        +Towers ObservableCollection~Tower~
        +StartGameCommand ICommand
        +ReturnToMenuCommand ICommand
        +SelectTowerCommand ICommand
        +UpgradeTowerCommand ICommand
        +Initialize(Canvas gameCanvas) void
        +DropTower(DragEventArgs e) void
    }

    class GameOverScreenViewModel {
        +CurrentWaveText string
        +HighscoreText string
        +ContinueCommand ICommand
    }

    class GameManager {
        +Gold int
        +PlayerHealth int
        +LoadLevel(LevelData level) void
        +PlaceTower(Point dropPosition, TowerData towerData) Tower
        +UpgradeTower(Tower baseTower, TowerData upgrade) void
        +StartWave(int waveNumber) void
        +Dispose() void
    }

    class Tower {
        +Position Point
        +TowerID int
        +Type string
        +Effect string
        +TowerName string
        +Tier int
        +Damage double
        +Range double
        +AttackSpeed double
        +Cost int
        +NotifyTowerChanged() void
    }

    class Enemy {
        +Data EnemyData
        +Position Point
        +IsAlive bool
        +ReachedEnd bool
        +Update(double deltaTime) void
        +TakeDamage(double amount) void
        +ApplyEffect(EffectData effect) void
    }

    class WaveGenerator {
        +GenerateWave(List~EnemyData~ allEnemies, int waveNumber) List~EnemyData~
    }

    class LevelData {
        +LevelName string
        +PathTiles List~Point~
        +BlockedTiles List~Point~
        +BackgroundImagePath string
    }

    class TowerData {
        +TowerID int
        +Type string
        +Effect string
        +UpgradeID int
        +TowerName string
        +Tier int
        +Damage double
        +Range double
        +AttackSpeed double
        +Cost int
    }

    class EnemyData {
        +EnemyID int
        +Level int
        +Type string
        +Health double
        +MovementSpeed double
        +Damage int
        +GoldDrop int
    }

    class EffectData {
        +Name string
        +Type string
        +DamageFactor double
        +Duration double
        +ValuePercent double
        +ValueFlat double
    }

    class LevelLoader {
        +LoadLevelFromJson(string filePath) LevelData
    }

    class EnemyLoader {
        +LoadEnemiesFromDatabase(string path) List~EnemyData~
    }

    class TowerLoader {
        +LoadTowersFromDatabase(string path) List~TowerData~
    }

    class EffectLoader {
        +LoadEffectsFromDatabase(string dbPath) List~EffectData~
    }

    class HighscoreManager {
        +GetBestWave(string dbPath, string levelName) int
        +UpdateBestWave(string dbPath, string levelName, int wave) void
    }

    class TowerDefenseDbContext {
        +EnemyLevels DbSet
        +EnemyTypes DbSet
        +TowerTypes DbSet
        +TowerUpgrades DbSet
        +Effects DbSet
        +Highscores DbSet
    }

    class NavigationRequest {
        +Target string
        +LevelPath string
    }

    class GameOverData {
        +ReachedWave int
        +LevelName string
    }

    NotifyPropertyChanged <|-- ViewModelBase
    ViewModelBase <|-- MainViewModel
    ViewModelBase <|-- StartScreenViewModel
    ViewModelBase <|-- SelectLevelScreenViewModel
    ViewModelBase <|-- PlayLevelScreenViewModel
    ViewModelBase <|-- GameOverScreenViewModel
    NotifyPropertyChanged <|-- Tower

    MainViewModel "1" --> "0..1" StartScreenViewModel : shows
    MainViewModel "1" --> "0..1" SelectLevelScreenViewModel : shows
    MainViewModel "1" --> "0..1" PlayLevelScreenViewModel : shows
    MainViewModel "1" --> "0..1" GameOverScreenViewModel : shows
    MainViewModel ..> NavigationRequest : receives
    MainViewModel ..> GameOverData : receives

    PlayLevelScreenViewModel "1" --> "1" GameManager : controls
    PlayLevelScreenViewModel "1" --> "0..*" Tower : displays
    PlayLevelScreenViewModel "1" --> "0..*" TowerData : available towers

    GameManager "1" --> "1" LevelData : current level
    GameManager "1" --> "0..*" Tower : placed towers
    GameManager "1" --> "0..*" Enemy : active enemies
    GameManager "1" --> "0..*" TowerData : tower definitions
    GameManager "1" --> "0..*" EnemyData : enemy definitions
    GameManager "1" --> "0..*" EffectData : effect definitions

    Enemy "1" --> "1" EnemyData : stats
    Enemy "1" --> "0..*" EffectData : active effects
    Tower ..> TowerData : created from
    WaveGenerator ..> EnemyData : creates waves

    LevelLoader ..> LevelData : loads
    EnemyLoader ..> EnemyData : loads
    TowerLoader ..> TowerData : loads
    EffectLoader ..> EffectData : loads

    EnemyLoader ..> TowerDefenseDbContext : reads from
    TowerLoader ..> TowerDefenseDbContext : reads from
    EffectLoader ..> TowerDefenseDbContext : reads from
    HighscoreManager ..> TowerDefenseDbContext : reads/writes
