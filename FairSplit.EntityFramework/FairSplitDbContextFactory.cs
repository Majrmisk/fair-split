using Microsoft.EntityFrameworkCore;

namespace FairSplit.EntityFramework
{
    public class FairSplitDbContextFactory(DbContextOptions options)
    {
        public FairSplitDbContext Create()
        {
            return new FairSplitDbContext(options);
        }
    }
}
