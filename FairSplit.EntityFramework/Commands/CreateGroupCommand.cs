using FairSplit.Domain.Commands;
using FairSplit.Domain.Model;
using FairSplit.EntityFramework.DTOs;

namespace FairSplit.EntityFramework.Commands
{
    public class CreateGroupCommand(FairSplitDbContextFactory factory) : ICreateGroupCommand
    {
        public async Task Execute(Group group)
        {
            using FairSplitDbContext context = factory.Create();
            var groupDto = new GroupDto
            {
                Id = group.Id,
                Name = group.Name
            };

            var memberDtos = group.GetAllMembers().Select(m => new MemberDto
            {
                Id = m.Id,
                Name = m.Name,
                GroupId = groupDto.Id
            }).ToList();

            var transactionDtos = group.GetAllTransactions().Select(t =>
            {
                var transactionDto = new TransactionDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    TotalAmount = t.TotalAmount,
                    TransactionTime = t.TransactionTime,
                    IsPaidOff = t.IsPaidOff,
                    Category = (int)t.Category,
                    GroupId = groupDto.Id,
                    PayerId = t.Payer.Id
                };

                var memberPaymentDtos = t.Recipients.Select(r => new MemberPaymentDto
                {
                    Id = r.Id,
                    MemberId = r.Member.Id,
                    Amount = r.Amount,
                    TransactionId = transactionDto.Id,
                }).ToList();

                transactionDto.Recipients = memberPaymentDtos;

                return transactionDto;
            }).ToList();

            groupDto.Members = memberDtos;
            groupDto.Transactions = transactionDtos;

            context.Groups.Add(groupDto);
            await context.SaveChangesAsync();
        }
    }
}
