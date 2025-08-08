using FairSplit.EntityFramework.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairSplit.EntityFramework
{
    public class FairSplitDbContext : DbContext
    {
        public FairSplitDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<GroupDto> Groups { get; set; }
        public DbSet<MemberDto> Members { get; set; }
        public DbSet<TransactionDto> Transactions { get; set; }
        public DbSet<MemberPaymentDto> MemberPayments { get; set; }
    }
}
