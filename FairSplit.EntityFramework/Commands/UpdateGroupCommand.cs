using FairSplit.Domain.Commands;
using FairSplit.Domain.Model;
using FairSplit.EntityFramework.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FairSplit.EntityFramework.Commands
{
    public class UpdateGroupCommand : IUpdateGroupCommand
    {
        private readonly FairSplitDbContextFactory _factory;

        public UpdateGroupCommand(FairSplitDbContextFactory factory)
        {
            _factory = factory;
        }

        public async Task Execute(Group group)
        {
            using (var context = _factory.Create())
            {
                var existingGroupDto = await context.Groups
                    .Include(g => g.Members)
                    .Include(g => g.Transactions)
                        .ThenInclude(t => t.Recipients)
                    .FirstOrDefaultAsync(g => g.Id == group.Id);

                if (existingGroupDto == null)
                {
                    return;
                }

                existingGroupDto.Name = group.Name;
                var domainMemberIds = group.GetAllMembers().Select(m => m.Id).ToHashSet();

                existingGroupDto.Members
                    .Where(dto => !domainMemberIds.Contains(dto.Id))
                    .ToList()
                    .ForEach(removed => context.Members.Remove(removed));

                foreach (var member in group.GetAllMembers())
                {
                    var memberDto = existingGroupDto.Members
                        .FirstOrDefault(m => m.Id == member.Id);
                    if (memberDto == null)
                    {
                        memberDto = new MemberDto
                        {
                            Id = member.Id,
                            Name = member.Name,
                            GroupId = existingGroupDto.Id
                        };
                        context.Entry(memberDto).State = EntityState.Added;
                        existingGroupDto.Members.Add(memberDto);
                    }
                    else
                    {
                        memberDto.Name = member.Name;
                        context.Entry(memberDto).State = EntityState.Modified;
                    }
                }

                var domainTransactionIds = group.GetAllTransactions().Select(t => t.Id).ToHashSet();

                existingGroupDto.Transactions
                    .Where(dto => !domainTransactionIds.Contains(dto.Id))
                    .ToList()
                    .ForEach(removedTx => context.Transactions.Remove(removedTx));

                foreach (var domainTx in group.GetAllTransactions())
                {
                    var existingTx = existingGroupDto.Transactions
                        .FirstOrDefault(t => t.Id == domainTx.Id);

                    if (existingTx == null)
                    {
                        existingTx = new TransactionDto
                        {
                            Id = domainTx.Id,
                            GroupId = existingGroupDto.Id,
                            Recipients = []
                        };
                        context.Entry(existingTx).State = EntityState.Added;
                        existingGroupDto.Transactions.Add(existingTx);
                    }
                    else
                    {
                        context.Entry(existingTx).State = EntityState.Modified;
                    }

                    existingTx.Name = domainTx.Name;
                    existingTx.TotalAmount = domainTx.TotalAmount;
                    existingTx.TransactionTime = domainTx.TransactionTime;
                    existingTx.IsPaidOff = domainTx.IsPaidOff;
                    existingTx.Category = (int)domainTx.Category;
                    existingTx.PayerId = domainTx.Payer.Id;

                    var domainRecipIds = domainTx.Recipients.Select(r => r.Id).ToHashSet();

                    existingTx.Recipients
                        .Where(rDto => !domainRecipIds.Contains(rDto.Id))
                        .ToList()
                        .ForEach(old => context.MemberPayments.Remove(old));

                    foreach (var r in domainTx.Recipients)
                    {
                        var existingRecip = existingTx.Recipients
                            .FirstOrDefault(rDto => rDto.Id == r.Id);

                        if (existingRecip == null)
                        {
                            existingRecip = new MemberPaymentDto
                            {
                                Id = r.Id,
                                TransactionId = existingTx.Id,
                                MemberId = r.Member.Id,
                                Amount = r.Amount
                            };

                            context.Entry(existingRecip).State = EntityState.Added;
                            existingTx.Recipients.Add(existingRecip);
                        }
                        else
                        {
                            context.Entry(existingRecip).State = EntityState.Modified;
                            existingRecip.MemberId = r.Member.Id;
                            existingRecip.Amount = r.Amount;
                        }
                    }
                }

                await context.SaveChangesAsync();
            }
        }
    }
}