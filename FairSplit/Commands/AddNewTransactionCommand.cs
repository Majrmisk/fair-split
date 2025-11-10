using FairSplit.Domain.Model;
using FairSplit.Services;
using FairSplit.Stores;
using FairSplit.ViewModels.Components;

namespace FairSplit.Commands
{
    class AddNewTransactionCommand(NavigationService selectGroupViewNavigationService,
        Core core,
        NewTransactionViewModel newTransactionViewModel,
        Member payer) : NavigateCommand(selectGroupViewNavigationService)
    {
        private Group Group => core.CurrentGroup;

        public override void Execute(object? parameter)
        {
            var columnTotals = newTransactionViewModel.ColumnTotals;
            var total = columnTotals.Sum();

            if (total == 0 || String.IsNullOrEmpty(newTransactionViewModel.Name) || !newTransactionViewModel.SelectedDate.HasValue)
            {
                return;
            }

            List<MemberPayment> recipients = [];
            for (int i = 1; i < columnTotals.Count; i++)
            {
                if (columnTotals[i] != 0)
                {
                    recipients.Add(new(Group.GetAllMembers()[i - 1], columnTotals[i]));
                }
            }

            Group.AddTransaction(new Transaction(newTransactionViewModel.Name, total, payer, 
                (DateTime) newTransactionViewModel.SelectedDate, false, newTransactionViewModel.SelectedCategory, recipients));
            core.UpdateCurrentGroup();
            base.Execute(parameter);
        }
    }
}
