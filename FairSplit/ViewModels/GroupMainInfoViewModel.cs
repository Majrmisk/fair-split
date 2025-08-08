using FairSplit.Commands;
using FairSplit.Domain.Model;
using FairSplit.Services;
using FairSplit.Stores;
using FairSplit.ViewModels.Components;
using FairSplit.ViewModels.Enitities;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace FairSplit.ViewModels
{
    public class GroupMainInfoViewModel : ViewModelBase
    {
        private static readonly SolidColorBrush BUTTON_COLOR = 
            new(Color.FromArgb(0xFF, 0x29, 0x2A, 0x2A));


        private readonly ObservableCollection<PersonViewModel> _persons;
        private readonly Core _core;
        private readonly NavigationStore _groupNavigationStore;

        private Group Group => _core.CurrentGroup;
        private PersonViewModel? _selectedPerson;
        private SolidColorBrush _overviewButtonColor;

        public IEnumerable<PersonViewModel> Persons => _persons;
        public ViewModelBase CurrentViewModel => _groupNavigationStore.CurrentViewModel;
        public string Name => Group.Name;

        public PersonViewModel? SelectedPerson
        {
            get => _selectedPerson;
            set 
            {
                if (_selectedPerson == value)
                {
                    return;
                }
                _selectedPerson = value;
                OnPropertyChanged();
                if (value != null)
                {
                    PersonSelectedCommand.Execute(this);
                }
            }
        }

        public SolidColorBrush OverviewButtonColor
        {
            get => _overviewButtonColor;
            set 
            { 
                _overviewButtonColor = value; 
                OnPropertyChanged();
            }
        }

        public ICommand BackToSelectGroupsCommand { get; }
        public ICommand PersonSelectedCommand { get; }
        public ICommand GroupOverviewSelectedCommand { get; }

        public GroupMainInfoViewModel(
            Core core, 
            NavigationService selectGroupViewNavigationService, 
            NavigationStore groupNavigationStore)
        {
            _core = core;
            _overviewButtonColor = BUTTON_COLOR;
            _persons = new(Group.GetAllMembers().Select(person => new PersonViewModel(person)));

            _groupNavigationStore = groupNavigationStore;
            _groupNavigationStore.CurrentViewModel = new GroupOverViewModel(core, groupNavigationStore);
            _groupNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

            BackToSelectGroupsCommand = new NavigateCommand(selectGroupViewNavigationService);
            GroupOverviewSelectedCommand = new NavigateCommand(new(groupNavigationStore, () =>
                {
                    SelectedPerson = null;
                    OverviewButtonColor = BUTTON_COLOR;
                    return new GroupOverViewModel(core, groupNavigationStore);
                }));
            PersonSelectedCommand = new NavigateCommand(new(groupNavigationStore, () =>
            {
                OverviewButtonColor = new SolidColorBrush(Colors.Transparent);
                return new MemberOverViewModel(SelectedPerson, core, groupNavigationStore);
            }));
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
