using FairSplit.Utils;

namespace FairSplit.ViewModels.Enitities
{
    public class PaymentViewModel : ViewModelBase
    {
        private readonly Payment _payment;

        public string From => _payment.From.Name;
        public string To => _payment.To.Name;
        public string Amount => $"{_payment.Amount.ToString("F2")} €";

        public PaymentViewModel(Payment payment)
        {
            _payment = payment;
        }
    }
}
