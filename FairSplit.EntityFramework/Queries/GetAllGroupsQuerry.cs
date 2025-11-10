using FairSplit.Domain.Model;
using FairSplit.Domain.Model.Enums;
using FairSplit.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace FairSplit.EntityFramework.Queries
{
    public class GetAllGroupsQuery(FairSplitDbContextFactory factory) : IGetAllGroupsQuery
    {
        public async Task<IEnumerable<Group>> Execute()
        {
            using FairSplitDbContext context = factory.Create();

            var groupDtos = await context.Groups
                .Include(g => g.Members)
                .Include(g => g.Transactions)
                    .ThenInclude(t => t.Recipients)
                .Include(g => g.Transactions)
                    .ThenInclude(t => t.Payer)
                .ToListAsync();

            var groups = groupDtos.Select(gDto =>
            {
                var domainMembers = gDto.Members
                    .Select(mDto => new Member(mDto.Id, mDto.Name))
                    .ToList();

                var domainMemberById = domainMembers.ToDictionary(m => m.Id, m => m);

                var domainTransactions = gDto.Transactions.Select(tDto =>
                {
                    var payer = domainMemberById[tDto.PayerId];

                    var recipients = tDto.Recipients.Select(rDto =>
                    {
                        var recipientMember = domainMemberById[rDto.MemberId];
                        return new MemberPayment(rDto.Id, recipientMember, rDto.Amount);
                    })
                    .ToList();

                    return new Transaction(
                        tDto.Id,
                        tDto.Name,
                        tDto.TotalAmount,
                        payer,
                        tDto.TransactionTime,
                        tDto.IsPaidOff,
                        (CategoryType)tDto.Category,
                        recipients
                    );
                }).ToList();

                var domainGroup = new Group(
                    gDto.Id,
                    gDto.Name,
                    domainMembers,
                    domainTransactions
                );

                return domainGroup;
            })
            .ToList();

            return groups;
        }
    }
}
