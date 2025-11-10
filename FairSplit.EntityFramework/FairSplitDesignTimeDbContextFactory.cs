using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FairSplit.EntityFramework
{
    public class FairSplitDesignTimeDbContextFactory : IDesignTimeDbContextFactory<FairSplitDbContext>
    {
        public FairSplitDbContext CreateDbContext(string[]? args = null)
        {
            return new FairSplitDbContext(new DbContextOptionsBuilder().UseSqlite("Data Source=FairSplit.db").Options);
        }
    }
}
