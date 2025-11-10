namespace FairSplit.EntityFramework.DTOs
{
    public class MemberPaymentDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public Guid TransactionId { get; set; }
        public virtual TransactionDto? Transaction { get; set; }
        public Guid MemberId { get; set; }
        public virtual MemberDto? Member { get; set; }
    }
}
