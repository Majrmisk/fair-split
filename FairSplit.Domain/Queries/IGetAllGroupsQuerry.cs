using FairSplit.Domain.Model;

namespace FairSplit.Domain.Queries
{
    public interface IGetAllGroupsQuery
    {
        Task<IEnumerable<Group>> Execute();
    }
}
