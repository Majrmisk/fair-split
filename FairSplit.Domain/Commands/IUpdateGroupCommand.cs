using FairSplit.Domain.Model;

namespace FairSplit.Domain.Commands
{
    public interface IUpdateGroupCommand
    {
        Task Execute(Group group);
    }
}
