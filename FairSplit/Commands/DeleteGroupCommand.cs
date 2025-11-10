using FairSplit.Services;
using FairSplit.Stores;

namespace FairSplit.Commands
{
    public class DeleteGroupCommand(Core core, NavigationService selectGroupViewNavigationService) : NavigateCommand(selectGroupViewNavigationService)
    {
        public override void Execute(object? parameter)
        {
            core.DeleteCurrentGroup();
            base.Execute(parameter);
        }
    }
}
