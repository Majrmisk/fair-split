using FairSplit.Domain.Model;
using FairSplit.Services;
using FairSplit.ViewModels.Enitities;
using System.Collections.ObjectModel;

namespace FairSplit.Commands
{
    class SaveGroupChangesCommand : NavigateCommand
    {
        private readonly Core _core;
        private readonly ObservableCollection<PersonViewModel> _persons;

        public SaveGroupChangesCommand(Core core, ObservableCollection<PersonViewModel> persons, NavigationService navigationService) : base(navigationService)
        {
            _core = core;
            _persons = persons;
        }

        public override void Execute(object? parameter)
        {
            _core.CurrentGroup.SetMembers(new(_persons.Select(person => person.Person)));
            _core.SaveCurrentGroup();
            base.Execute(parameter);
        }
    }
}
