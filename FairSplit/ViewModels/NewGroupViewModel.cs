using FairSplit.Commands;
using FairSplit.Services;
using FairSplit.Stores;
using FairSplit.ViewModels.Enitities;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FairSplit.ViewModels
{
    public class NewGroupViewModel : ViewModelBase
    {
        private readonly ObservableCollection<PersonViewModel> _persons = [];
        private readonly Core _core;

        public ObservableCollection<PersonViewModel> Persons => _persons;
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public string NameBoxText
        {
            get { return _core.CurrentGroup.Name; }
            set
            {
                _core.CurrentGroup.Name = value;
                OnPropertyChanged();
            }
        }

        public NewGroupViewModel(Core core, NavigationService selectGroupViewNavigationService)
        {
            core.CurrentGroup = new("New Group");
            _core = core;
            SaveCommand = new SaveGroupChangesCommand(core, Persons, selectGroupViewNavigationService);
            CancelCommand = new NavigateCommand(selectGroupViewNavigationService);
        }
    }
}
