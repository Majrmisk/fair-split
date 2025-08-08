using FairSplit.Domain.Model;

namespace FairSplit.Domain.Commands
{
    public interface IDeleteGroupCommand
    {
        Task Execute(Group group);
    }
}
