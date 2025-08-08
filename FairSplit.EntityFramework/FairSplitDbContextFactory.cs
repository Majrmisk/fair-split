using Microsoft.EntityFrameworkCore;

namespace FairSplit.EntityFramework
{
    public class FairSplitDbContextFactory
    {
        private readonly DbContextOptions _options;


        public FairSplitDbContextFactory(DbContextOptions options)
        {
            _options = options;
        }

        public FairSplitDbContext Create()
        {
            return new FairSplitDbContext(_options);
        }
    }
}
