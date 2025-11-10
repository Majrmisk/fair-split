using FairSplit.Commands;
using FairSplit.Domain.Model;
using FairSplit.Utils;
using FairSplit.Stores;
using FairSplit.ViewModels.Enitities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace FairSplit.ViewModels.Components
{
    public class SettleViewModel : ViewModelBase
    {
        private readonly Group _group;
        private readonly ObservableCollection<TransactionViewModel> _transactions;

        public IEnumerable<TransactionViewModel> Transactions => _transactions;

        public ICommand DoneCommand { get; }
        public ICommand CancelCommand { get; }

        public IEnumerable<PaymentViewModel> CalculatedPayments => 
            TransactionSettler.CalculateBestSettleOptions(_group, [.. _transactions
                .Where(t => t.IsSelected)
                .Select(t => t.Transaction)])
            .Select(p => new PaymentViewModel(p));

        public SettleViewModel(Core core, NavigationStore groupNavigationStore)
        {
            _group = core.CurrentGroup;
            _transactions = new(_group.GetUnsettledTransactions()
                .Select(transaction => new TransactionViewModel(transaction)));
            foreach (var transaction in _transactions)
            {
                transaction.PropertyChanged += Transaction_PropertyChanged;
            }

            DoneCommand = new NavigateCommand(
                new(groupNavigationStore, 
                () => 
                    { 
                        SetSettledTransactions(_transactions);
                        core.UpdateCurrentGroup();
                        return new GroupOverViewModel(core, groupNavigationStore); 
                    }));
            CancelCommand = new NavigateCommand(new(groupNavigationStore, () => new GroupOverViewModel(core, groupNavigationStore)));
        }

        private static void SetSettledTransactions(ObservableCollection<TransactionViewModel> transactions)
        {
            foreach (var transaction in transactions)
            {
                if (transaction.IsSelected)
                {
                    transaction.Transaction.IsPaidOff = true;
                }
            }
        }

        private void Transaction_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TransactionViewModel.IsSelected))
            {
                OnPropertyChanged(nameof(CalculatedPayments));
            }
        }
    }
}
