using FairSplit.Domain.Model.Enums;

namespace FairSplit.Domain.Model
{
    public class Transaction(Guid id, string name, decimal totalAmount, Member payer, DateTime transactionTime, bool isPaidOff,
                       CategoryType category, List<MemberPayment> recipients)
    {
        private decimal total;
        private Member payer;
        private DateTime selectedDate;
        private bool v;
        private CategoryType selectedCategory;
        private List<(Member Recipient, decimal Amount)> recipients;

        public Guid Id { get; } = id;

        public string Name { get; private set; } = name;

        public decimal TotalAmount { get; private set; } = totalAmount;

        public Member Payer { get; private set; } = payer;

        public DateTime TransactionTime { get; private set; } = transactionTime;

        public bool IsPaidOff { get; set; } = isPaidOff;

        public CategoryType Category { get; private set; } = category;

        public List<MemberPayment> Recipients { get; private set; } = recipients;

        public Transaction(string name, decimal total, Member payer, DateTime selectedDate, bool v, CategoryType selectedCategory, List<MemberPayment> recipients)
            : this(Guid.NewGuid(), name, total, payer, selectedDate, false, selectedCategory, recipients) { }
    }
}
