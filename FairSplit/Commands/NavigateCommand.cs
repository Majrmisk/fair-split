using FairSplit.Commands.Abstracts;
using FairSplit.Services;

namespace FairSplit.Commands
{
    public class NavigateCommand(NavigationService navigationService) : CommandBase
    {
        public override void Execute(object? parameter)
        {
            navigationService.Navigate();
        }
    }
}
