using FairSplit.Domain.Commands;
using FairSplit.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace FairSplit.EntityFramework.Commands
{
    public class DeleteGroupCommand : IDeleteGroupCommand
    {
        private readonly FairSplitDbContextFactory _factory;

        public DeleteGroupCommand(FairSplitDbContextFactory factory)
        {
            _factory = factory;
        }

        public async Task Execute(Group group)
        {
            using (FairSplitDbContext context = _factory.Create())
            {
                var existingGroupDto = await context.Groups
                    .FirstOrDefaultAsync(g => g.Id == group.Id);

                if (existingGroupDto == null)
                {
                    return;
                }

                context.Groups.Remove(existingGroupDto);
                await context.SaveChangesAsync();
            }
        }
    }
}