namespace FairSplit.EntityFramework.DTOs
{
    public class MemberDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        // If a member belongs strictly to one Group:
        public Guid GroupId { get; set; } // foreign key
        public virtual GroupDto Group { get; set; }

        // If a member can belong to multiple Groups:
        //   Remove GroupId and do a many-to-many bridging table
        //   by having `ICollection<GroupDto> Groups` 
        //   and configuring it in fluent.
    }
}