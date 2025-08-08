using FairSplit.Commands;
using FairSplit.Domain.Model;
using FairSplit.Stores;
using FairSplit.ViewModels.Enitities;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FairSplit.ViewModels.Components
{
    public class MemberOverViewModel : ViewModelBase
    {
        private readonly ObservableCollection<TransactionViewModel> _transactions;
        private readonly PersonViewModel _member;
        private readonly Group _group;

        private decimal _paidMonth;
        private decimal _paid;
        private decimal _owedMonth;
        private decimal _owed;

        public decimal PaidMonth
        {
            get => _paidMonth;
            set
            {
                _paidMonth = value;
                OnPropertyChanged();
            }
        }
        public decimal Paid
        {
            get => _paid;
            set
            {
                _paid = value;
                OnPropertyChanged();
            }
        }
        public decimal Owed
        {
            get => _owed;
            set
            {
                _owed = value;
                OnPropertyChanged();
            }
        }
        public decimal OwedMonth
        {
            get => _owedMonth;
            set
            {
                _owedMonth = value;
                OnPropertyChanged();
            }
        }

        public ICommand NewTransactionCommand { get; }
        public IEnumerable<TransactionViewModel> Transactions => _transactions;

        public MemberOverViewModel(PersonViewModel member, Core core, NavigationStore navigationStore)
        {
            _member = member;
            _group = core.CurrentGroup;
            _transactions = [];
            Refresh();
            NewTransactionCommand = new NavigateCommand(new(navigationStore, 
                () => new NewTransactionViewModel(navigationStore, this, core, member.Person)));
        }

        public void Refresh()
        {
            _transactions.Clear();
            foreach (Transaction transaction in _group.GetAllTransactions())
            {
                if (transaction.Payer == _member.Person)
                {
                    _transactions.Add(new(transaction));
                }
            }

            _paid = _group.GetTotalSpentByPersonSinceDate(_member.Person);
            _owed = _group.GetTotalOwedByPersonSinceDate(_member.Person);
            _paidMonth = _group.GetTotalSpentByPersonSinceDate(_member.Person, DateTime.Now.AddMonths(-1));
            _owedMonth = _group.GetTotalOwedByPersonSinceDate(_member.Person, DateTime.Now.AddMonths(-1));
        }
    }
}
