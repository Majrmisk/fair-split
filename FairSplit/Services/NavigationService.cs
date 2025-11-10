using FairSplit.Stores;
using FairSplit.ViewModels;

namespace FairSplit.Services
{
    public class NavigationService(NavigationStore navigationStore, Func<ViewModelBase> createViewModel)
    {
        public void Navigate()
        {
            navigationStore.CurrentViewModel = createViewModel();
        }
    }
}
