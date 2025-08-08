using FairSplit.Domain.Model;
using FairSplit.Services;

namespace FairSplit.Commands
{
    public class DeleteGroupCommand : NavigateCommand
    {
        private readonly Core _core;

        public DeleteGroupCommand(Core core, NavigationService selectGroupViewNavigationService) : base(selectGroupViewNavigationService)
        {
            _core = core;
        }

        public override void Execute(object? parameter)
        {
            _core.DeleteCurrentGroup();
            base.Execute(parameter);
        }
    }
}
