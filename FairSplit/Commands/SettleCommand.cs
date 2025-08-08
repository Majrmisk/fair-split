using FairSplit.Commands.Abstracts;
using FairSplit.Domain.Model;
using FairSplit.Utils;

namespace FairSplit.Commands
{
    public class SettleCommand : CommandBase
    {
        private readonly Group _group;

        public SettleCommand(Group group)
        {
            _group = group;
        }

        public override void Execute(object? parameter)
        {
            TransactionSettler.CalculateBestSettleOptions(_group);
        }
    }
}
