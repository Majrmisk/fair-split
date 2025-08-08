using FairSplit.Domain.Model;

namespace FairSplit.Utils
{
    public record Payment
    {
        public Member From { get; init; }
        public Member To { get; init; }
        public decimal Amount { get; init; }

        public Payment(Member from, Member to, decimal amount)
        {
            From = from;
            To = to;
            Amount = amount;
        }
    }
}
