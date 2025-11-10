namespace FairSplit.EntityFramework.DTOs
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime TransactionTime { get; set; }
        public bool IsPaidOff { get; set; }
        public int Category { get; set; }
        public Guid GroupId { get; set; }
        public virtual GroupDto? Group { get; set; }
        public Guid PayerId { get; set; }
        public virtual MemberDto? Payer { get; set; }
        public virtual ICollection<MemberPaymentDto> Recipients { get; set; } = [];
    }
}