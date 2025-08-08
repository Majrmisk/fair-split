using FairSplit.Commands;
using FairSplit.Domain.Model;
using FairSplit.Domain.Model.Enums;
using FairSplit.Stores;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

namespace FairSplit.ViewModels.Components
{
    public class NewTransactionViewModel : ViewModelBase
    {
        private DataTable _transactionDataTable = new();
        private DateTime? _selectedDate = DateTime.Now;
        private string _name = "New Transaction";
        private CategoryType _selectedCategory = CategoryType.Unspecified;
        private string _total = "0 €";
        private ObservableCollection<decimal> _columnTotals;

        public IEnumerable<CategoryType> CategoryOptions { get; } = Enum.GetValues(typeof(CategoryType)).Cast<CategoryType>();

        public DataTable TransactionDataTable
        {
            get { return _transactionDataTable; }
            set { _transactionDataTable = value; OnPropertyChanged(); }
        }
        public DateTime? SelectedDate
        {
            get { return _selectedDate; }
            set { _selectedDate = value; OnPropertyChanged(); }
        }
        public CategoryType SelectedCategory
        {
            get { return _selectedCategory; }
            set { _selectedCategory = value; OnPropertyChanged(); }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }
        public string Total
        {
            get { return _total; }
            set { _total = value; OnPropertyChanged(); }
        }
        public ObservableCollection<decimal> ColumnTotals
        {
            get { return _columnTotals; }
            set { _columnTotals = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand CancelCommand { get; }

        public NewTransactionViewModel(NavigationStore navigationStore, 
                                        MemberOverViewModel memberOverViewModel,
                                        Core core,
                                        Member payer)
        {
            _columnTotals = [];
            InitializeColumns(core.CurrentGroup.GetAllMembers());

            AddCommand = new AddNewTransactionCommand(new(navigationStore, () => { memberOverViewModel.Refresh(); return memberOverViewModel; }),
                                                        core,
                                                        this,
                                                        payer);
            CancelCommand = new NavigateCommand(new(navigationStore,
                () => memberOverViewModel));
        }

        private void InitializeColumns(IEnumerable<Member> persons)
        {
            TransactionDataTable.Columns.Add("Shared", typeof(decimal));
            foreach (var person in persons)
            {
                TransactionDataTable.Columns.Add(person.Name, typeof(decimal));
            }
            TransactionDataTable.RowChanged += TransactionDataTable_RowChanged;
            UpdateColumnTotals();
        }

        private void TransactionDataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            UpdateTotal();
            UpdateColumnTotals();
        }

        private void UpdateTotal()
        {
            if (_transactionDataTable == null) return;

            decimal total = 0;

            foreach (DataRow row in _transactionDataTable.Rows)
            {
                foreach (DataColumn column in _transactionDataTable.Columns)
                {
                    if (row[column] != DBNull.Value)
                    {
                        total += Convert.ToDecimal(row[column]);
                    }
                }
            }
            Total = $"{total} €";
        }

        private void UpdateColumnTotals()
        {
            if (TransactionDataTable == null) return;

            ColumnTotals.Clear();

            for (
                int i = 0; i < TransactionDataTable.Columns.Count; i++)
            {
                decimal total = 0;
                foreach (DataRow row in TransactionDataTable.Rows)
                {
                    if (row[i] != DBNull.Value)
                    {
                        total += Convert.ToDecimal(row[i]);
                    }
                }
                ColumnTotals.Add(total);
            }
        }
    }
}
