using FairSplit.Domain.Model;

namespace FairSplit.ViewModels.Enitities
{
    public class GroupViewModel(Group group) : ViewModelBase
    {
        private readonly Group _group = group;

        public string Name => _group.Name;
    }
}
