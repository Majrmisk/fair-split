using FairSplit.Commands.Abstracts;
using FairSplit.Domain.Model;
using FairSplit.Services;
using FairSplit.ViewModels;
using System.ComponentModel;

namespace FairSplit.Commands
{
    public class SelectGroupCommand : CommandBase
    {
        private readonly SelectGroupViewModel _selectGroupViewModel;
        private readonly NavigationService _groupMainInfoViewNavigationService;
        private readonly Core _core;

        public SelectGroupCommand(SelectGroupViewModel selectGroupViewModel,
            NavigationService groupMainInfoViewNavigationService,
            Core core)
        {
            _selectGroupViewModel = selectGroupViewModel;
            _groupMainInfoViewNavigationService = groupMainInfoViewNavigationService;
            _core = core;

            _selectGroupViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return _selectGroupViewModel.SelectedGroup != null && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            _core.CurrentGroup = _core.GetGroupWithName(_selectGroupViewModel.SelectedGroup.Name);
            _groupMainInfoViewNavigationService.Navigate();
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(SelectGroupViewModel.SelectedGroup))
            {
                OnCanExecuteChanged();
            }

        }
    }
}
