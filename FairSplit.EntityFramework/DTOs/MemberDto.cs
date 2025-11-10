namespace FairSplit.EntityFramework.DTOs
{
    public class MemberDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public Guid GroupId { get; set; } 
        public virtual GroupDto? Group { get; set; }
    }
}