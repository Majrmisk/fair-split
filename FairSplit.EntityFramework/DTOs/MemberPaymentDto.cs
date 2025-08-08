using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairSplit.EntityFramework.DTOs
{
    public class MemberPaymentDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }

        // Relationship to the Transaction
        public Guid TransactionId { get; set; }
        public virtual TransactionDto Transaction { get; set; }

        // Relationship to the Member
        public Guid MemberId { get; set; }
        public virtual MemberDto Member { get; set; }
    }
}
