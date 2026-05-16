using Prism.Events;
using WPFTowerDefense.Common;

namespace WPFTowerDefense.ViewModels
{
    public class ViewModelBase : NotifyPropertyChanged
    {
        public ViewModelBase(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
        }

        protected IEventAggregator EventAggregator { get; }
    }
}
