namespace FairSplit.Domain.Model
{
    public class Member(Guid id, string name)
    {
        public Guid Id { get; } = id;

        public string Name { get; set; } = name;

        public Member(string name) : this(Guid.NewGuid(), name) { }
    }
}
