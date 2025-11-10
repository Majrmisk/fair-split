using FairSplit.Commands;
using FairSplit.Domain.Model;
using FairSplit.Stores;
using System.Collections.ObjectModel;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using FairSplit.Domain.Model.Enums;
using FairSplit.ViewModels.Enitities;

namespace FairSplit.ViewModels.Components
{
    public class GroupOverViewModel : ViewModelBase
    {
        private static readonly Color memberGradientFrom = (Color)ColorConverter.ConvertFromString("#684899");
        private static readonly Color memberGradientTo = (Color)ColorConverter.ConvertFromString("#a58ab8");

        private static readonly Color categoryGradientFrom = (Color)ColorConverter.ConvertFromString("#3c5a36");
        private static readonly Color categoryGradientTo = (Color)ColorConverter.ConvertFromString("#b0ccaa");

        private readonly Group _group;
        private readonly ObservableCollection<TransactionViewModel> _transactions;

        private string _selectedTimeframe;

        public IEnumerable<TransactionViewModel> Transactions => _transactions;

        public SeriesCollection SpentByMembersPie { get; set; } = [];
        public SeriesCollection SpentByCategoryPie { get; set; } = [];

        public List<string> TimeframeOptions { get; } =
        [
            "  Past Day",
            "  Past Week",
            "  Past Month",
            "  Past Year",
            "  All Time"
        ];
        public string SelectedTimeframe
        {
            get => _selectedTimeframe;
            set
            {
                _selectedTimeframe = value;
                UpdatePieCharts(DateFromString(_selectedTimeframe));
                OnPropertyChanged();
            }
        }

        public ICommand SettleCommand { get; }

        public GroupOverViewModel(Core core, NavigationStore groupNavigationStore)
        {
            _group = core.CurrentGroup;
            _selectedTimeframe = TimeframeOptions[2];
            _transactions = new(_group.GetUnsettledTransactions()
                .Select(transaction => new TransactionViewModel(transaction)));
            SettleCommand = new NavigateCommand(new(groupNavigationStore, () => new SettleViewModel(core, groupNavigationStore)));

            SelectedTimeframe = _selectedTimeframe;
        }

        public void UpdatePieCharts(DateTime? from = null)
        {
            var personIndex = 0;
            foreach (Member person in _group.GetAllMembers())
            {
                var series = SpentByMembersPie.FirstOrDefault(s => s.Title == person.Name);
                if (series != null)
                {
                    series.Values = new ChartValues<decimal> { _group.GetTotalSpentByPersonSinceDate(person, from) };
                    continue;
                }
                SpentByMembersPie.Add(new PieSeries
                {
                    Title = person.Name,
                    Values = new ChartValues<decimal> { _group.GetTotalSpentByPersonSinceDate(person, from) },
                    Stroke = Brushes.Transparent,
                    Fill = new SolidColorBrush(InterpolateColor(memberGradientFrom, memberGradientTo, personIndex / (double)_group.GetAllMembers().Count))
                });
                personIndex++;
            }

            foreach (CategoryType category in Enum.GetValues(typeof(CategoryType)))
            {
                var series = SpentByCategoryPie.FirstOrDefault(s => s.Title == category.ToString());
                if (series != null)
                {
                    series.Values = new ChartValues<decimal> { _group.GetTotalWithCategory(category, from) };
                    continue;
                }
                SpentByCategoryPie.Add(new PieSeries
                {
                    Title = category.ToString(),
                    Values = new ChartValues<decimal> { _group.GetTotalWithCategory(category, from) },
                    Stroke = Brushes.Transparent,
                    Fill = new SolidColorBrush(InterpolateColor(categoryGradientFrom, categoryGradientTo, (int)category / (double)(Enum.GetValues(typeof(CategoryType)).Length - 1)))
                });
            }
        }

        private static Color InterpolateColor(Color start, Color end, double fraction)
        {
            byte r = (byte)(start.R + (end.R - start.R) * fraction);
            byte g = (byte)(start.G + (end.G - start.G) * fraction);
            byte b = (byte)(start.B + (end.B - start.B) * fraction);
            return Color.FromRgb(r, g, b);
        }

        private static DateTime? DateFromString(string selectedTimeframe)
        {
            if (string.IsNullOrWhiteSpace(selectedTimeframe))
            {
                return null;
            }

            DateTime? fromDate = null;

            switch (selectedTimeframe.Trim())
            {
                case "Past Day":
                    fromDate = DateTime.Now.AddDays(-1);
                    break;
                case "Past Week":
                    fromDate = DateTime.Now.AddDays(-7);
                    break;
                case "Past Month":
                    fromDate = DateTime.Now.AddMonths(-1);
                    break;
                case "Past Year":
                    fromDate = DateTime.Now.AddYears(-1);
                    break;
                case "All Time":
                    fromDate = null;
                    break;
            }

            return fromDate;
        }
    }
}
