using Prism.Events;

namespace WPFTowerDefense.Events
{
    public class NavigationEvent : PubSubEvent<NavigationRequest>
    {

    }

    public class NavigationRequest
    {
        public string Target { get; }
        public string LevelPath { get; }

        public NavigationRequest(string target)
        {
            Target = target;
        }

        public NavigationRequest(string target, string levelPath)
        {
            Target = target;
            LevelPath = levelPath;
        }
    }
}
