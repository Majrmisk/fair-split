using FairSplit.Domain.Model;

namespace FairSplit.ViewModels.Enitities
{
    public class TransactionViewModel(Transaction transaction) : ViewModelBase
    {
        private readonly Transaction _transaction = transaction;

        public string Name => _transaction.Name;
        public string TotalAmount => $"{_transaction.TotalAmount:F2} €";
        public string NameOfPayer => _transaction.Payer.Name;
        public string TransactionTime => _transaction.TransactionTime.ToShortDateString();
        public string Category => _transaction.Category.ToString();
        public string PayedOff => _transaction.IsPaidOff ? "✔" : "✖";
        public Transaction Transaction => _transaction;

        private bool _isSelected = true;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }
}
