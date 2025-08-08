using FairSplit.Domain.Model;

namespace FairSplit.ViewModels.Enitities
{
    public class PersonViewModel(Member person) : ViewModelBase
    {
        private readonly Member _person = person;

        public Member Person { get { return _person; } set { _person.Name = value.Name; } }

        public string Name
        {
            get => _person.Name;
            set
            {
                if (_person.Name != value)
                {
                    _person.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public PersonViewModel() : this(new("New Person")) { }
    }
}
