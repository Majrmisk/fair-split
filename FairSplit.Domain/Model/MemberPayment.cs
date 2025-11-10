namespace FairSplit.Domain.Model
{
    public class MemberPayment(Guid id, Member member, decimal amount)
    {
        public Guid Id { get; } = id;
        public Member Member { get; } = member;
        public decimal Amount { get; } = amount;

        public MemberPayment(Member member, decimal amount) : this(Guid.NewGuid(), member, amount) { }
    }
}
