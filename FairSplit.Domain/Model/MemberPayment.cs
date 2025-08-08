namespace FairSplit.Domain.Model
{
    public class MemberPayment
    {
        public Guid Id { get; }
        public Member Member { get; }
        public decimal Amount { get; }

        public MemberPayment(Guid id, Member member, decimal amount)
        {
            Id = id;
            Member = member;
            Amount = amount;
        }

        public MemberPayment(Member member, decimal amount) : this(Guid.NewGuid(), member, amount) { }
    }
}
