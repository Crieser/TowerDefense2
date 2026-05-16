using Prism.Events;

namespace WPFTowerDefense.Events
{
    public class GameOverEvent : PubSubEvent<GameOverData>
    {

    }

    public class GameOverData
    {
        public int ReachedWave { get; }
        public string LevelName { get; }

        public GameOverData(int reachedWave, string levelName)
        {
            ReachedWave = reachedWave;
            LevelName = levelName;
        }
    }
}
