using FairSplit.Domain.Model;

namespace FairSplit.Domain.Commands
{
    public interface ICreateGroupCommand
    {
        Task Execute(Group group);
    }
}
