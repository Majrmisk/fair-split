using FairSplit.Domain.Model;

namespace FairSplit.Domain.Commands
{
    public interface ICreateTransactionCommand
    {
        Task Execute(Transaction transaction, Guid groupId);
    }
}
