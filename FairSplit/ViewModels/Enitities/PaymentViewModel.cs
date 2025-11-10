using FairSplit.Utils;

namespace FairSplit.ViewModels.Enitities
{
    public class PaymentViewModel(Payment payment) : ViewModelBase
    {
        public string From => payment.From.Name;
        public string To => payment.To.Name;
        public string Amount => $"{payment.Amount:F2} €";
    }
}
