namespace FairSplit.EntityFramework.DTOs
{
    public class GroupDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public virtual ICollection<MemberDto> Members { get; set; } = [];
        public virtual ICollection<TransactionDto> Transactions { get; set; } = [];
    }
}
