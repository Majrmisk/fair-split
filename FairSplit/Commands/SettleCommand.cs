using FairSplit.Commands.Abstracts;
using FairSplit.Domain.Model;
using FairSplit.Utils;

namespace FairSplit.Commands
{
    public class SettleCommand(Group group) : CommandBase
    {
        public override void Execute(object? parameter)
        {
            TransactionSettler.CalculateBestSettleOptions(group);
        }
    }
}
