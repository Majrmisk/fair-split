using FairSplit.Domain.Model;
using FairSplit.Services;
using FairSplit.ViewModels.Components;

namespace FairSplit.Commands
{
    class AddNewTransactionCommand : NavigateCommand
    {
        private readonly Core _core;
        private Group _group => _core.CurrentGroup;
        private readonly NavigationService _selectGroupViewNavigationService;
        private readonly NewTransactionViewModel _newTransactionViewModel;
        private readonly Member _payer;

        public AddNewTransactionCommand(NavigationService selectGroupViewNavigationService,
            Core core,
            NewTransactionViewModel newTransactionViewModel,
            Member payer) : base(selectGroupViewNavigationService)
        {
            _core = core;
            _newTransactionViewModel = newTransactionViewModel;
            _selectGroupViewNavigationService = selectGroupViewNavigationService;
            _payer = payer;
        }

        public override void Execute(object? parameter)
        {
            var columnTotals = _newTransactionViewModel.ColumnTotals;
            var total = columnTotals.Sum();

            if (total == 0 || String.IsNullOrEmpty(_newTransactionViewModel.Name) || !_newTransactionViewModel.SelectedDate.HasValue)
            {
                return;
            }

            List<MemberPayment> recipients = [];
            for (int i = 1; i < columnTotals.Count - 1; i++)
            {
                if (columnTotals[i] != 0)
                {
                    recipients.Add(new(_group.GetAllMembers()[i], columnTotals[i]));
                }
            }

            _group.AddTransaction(new Transaction(_newTransactionViewModel.Name, total, _payer, 
                (DateTime) _newTransactionViewModel.SelectedDate, false, _newTransactionViewModel.SelectedCategory, recipients));
            _core.UpdateCurrentGroup();
            base.Execute(parameter);
        }
    }
}
