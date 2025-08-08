using FairSplit.Commands;
using FairSplit.Domain.Model;
using FairSplit.Stores;
using FairSplit.ViewModels.Enitities;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FairSplit.ViewModels
{
    public class SelectGroupViewModel : ViewModelBase
    {
		private ObservableCollection<GroupViewModel> _groups;

		public ObservableCollection<GroupViewModel> Groups
		{
			get { return _groups; }
			set { _groups = value; }
		}
		

        public ICommand NewGroupCommand { get; }

        public ICommand SelectGroupCommand { get; set; }

        public ICommand ManageGroupCommand { get; }


        private GroupViewModel? _selectedGroup = null;

        public GroupViewModel? SelectedGroup
        {
			get { return _selectedGroup; }
			set 
			{
                _selectedGroup = value;
				OnPropertyChanged(); 
			}
        }

		public SelectGroupViewModel(Core core, NavigationStore navigationStore)
		{
			_groups = new(core.GetAllGroups().Select(c1 => new GroupViewModel(c1)));
			// TODO: Change this I hate it
			NewGroupCommand = new NavigateCommand(
				new(navigationStore, () => new NewGroupViewModel(core, new(navigationStore, 
											() => new SelectGroupViewModel(core, navigationStore)))));
			SelectGroupCommand = new SelectGroupCommand(this, 
				new(navigationStore, () => new GroupMainInfoViewModel(core, new(navigationStore, 
											() => new SelectGroupViewModel(core, navigationStore)), new())), core);
            ManageGroupCommand = new SelectGroupCommand(this,
                new(navigationStore, () => new ManageGroupViewModel(core, new(navigationStore,
                                            () => new SelectGroupViewModel(core, navigationStore)))), core);
        }
	}
}
