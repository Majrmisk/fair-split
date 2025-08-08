namespace FairSplit.EntityFramework.DTOs
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime TransactionTime { get; set; }
        public bool IsPaidOff { get; set; }
        public int Category { get; set; }

        // Relationship to a Group
        public Guid GroupId { get; set; }
        public virtual GroupDto Group { get; set; }

        // Relationship to the Payer (one-to-many from Member -> Transaction)
        public Guid PayerId { get; set; }
        public virtual MemberDto Payer { get; set; }

        // Recipients
        public virtual ICollection<MemberPaymentDto> Recipients { get; set; }
    }
}