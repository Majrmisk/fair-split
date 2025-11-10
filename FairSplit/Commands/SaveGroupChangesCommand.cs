using FairSplit.Services;
using FairSplit.Stores;
using FairSplit.ViewModels.Enitities;
using System.Collections.ObjectModel;

namespace FairSplit.Commands
{
    class SaveGroupChangesCommand(Core core, ObservableCollection<PersonViewModel> persons, NavigationService navigationService) : NavigateCommand(navigationService)
    {
        public override void Execute(object? parameter)
        {
            core.CurrentGroup.SetMembers([.. persons.Select(person => person.Person)]);
            core.SaveCurrentGroup();
            base.Execute(parameter);
        }
    }
}
