using FairSplit.Domain.Model.Enums;

namespace FairSplit.Domain.Model
{
    public class Group(Guid id, string name, List<Member> persons, List<Transaction> transactions)
    {
        private readonly List<Transaction> _transactions = transactions;


        private List<Member> _persons = persons;

        public Guid Id { get; } = id;

        public string Name { get; set; } = name;


        public Group(string name) : this(Guid.NewGuid(), name, [], []) { }


        public void AddMember(Member person)
        {
            _persons.Add(person);
        }

        public void AddTransaction(Transaction transaction)
        {
            _transactions.Add(transaction);
        }

        public List<Member> GetAllMembers()
        {
            return _persons;
        }

        public List<Transaction> GetAllTransactions()
        {
            return _transactions;
        }

        public void SetMembers(List<Member> persons)
        {
            _persons = persons;
        }

        public List<Transaction> GetUnsettledTransactions()
        {
            return [.. _transactions.Where(transaction => !transaction.IsPaidOff)];
        }

        public decimal GetTotalSpentByPersonSinceDate(Member person, DateTime? date = null)
        {
            decimal totalSpent = 0;

            foreach (Transaction transaction in _transactions)
            {
                if (transaction.Payer == person && transaction.TransactionTime >= (date ?? DateTime.MinValue))
                {
                    totalSpent += transaction.TotalAmount;
                }
            }

            return Math.Round(totalSpent, 2);
        }

        public decimal GetTotalOwedByPersonSinceDate(Member person, DateTime? date = null)
        {
            decimal totalOwed = 0;

            foreach (Transaction transaction in _transactions)
            {
                if (transaction.Payer == person)
                {
                    continue;
                }
                decimal totalAmount = transaction.TotalAmount;
                foreach (var memberPayment in transaction.Recipients)
                {
                    totalAmount -= memberPayment.Amount;
                    if (memberPayment.Member == person && transaction.TransactionTime >= (date ?? DateTime.MinValue))
                    {
                        totalOwed += memberPayment.Amount;
                    }
                }
                totalOwed += totalAmount / _persons.Count;
            }
            return Math.Round(totalOwed, 2);
        }

        public decimal GetTotalWithCategory(CategoryType category, DateTime? date = null)
        {
            decimal total = 0;

            foreach (Transaction transaction in _transactions)
            {
                if (transaction.Category == category && transaction.TransactionTime >= (date ?? DateTime.MinValue))
                {
                    total += transaction.TotalAmount;
                }
            }

            return Math.Round(total, 2);
        }
    }
}
