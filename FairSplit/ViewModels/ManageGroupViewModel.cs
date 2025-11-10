using FairSplit.Commands;
using FairSplit.Services;
using FairSplit.Stores;
using FairSplit.ViewModels.Enitities;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FairSplit.ViewModels
{
    public class ManageGroupViewModel : ViewModelBase
    {
        private readonly ObservableCollection<PersonViewModel> _persons;
        private readonly Core _core;

        public ObservableCollection<PersonViewModel> Persons => _persons;
        public ICommand BackCommand { get; }
        public ICommand DeleteCommand { get; }

        public string OldName => _core.CurrentGroup.Name;

        public string NameBoxText
        {
            get { return _core.CurrentGroup.Name; }
            set {
                _core.CurrentGroup.Name = value; 
                OnPropertyChanged();
            }
        }

        public ManageGroupViewModel(Core core, NavigationService selectGroupViewNavigationService)
        {
            DeleteCommand = new DeleteGroupCommand(core, selectGroupViewNavigationService);
            _core = core;
            _persons = new(_core.CurrentGroup.GetAllMembers().Select(person => new PersonViewModel(person)));
            BackCommand = new SaveGroupChangesCommand(core, Persons, selectGroupViewNavigationService);
        }
    }
}
