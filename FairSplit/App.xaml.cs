using FairSplit.Domain.Commands;
using FairSplit.Domain.Model;
using FairSplit.Domain.Queries;
using FairSplit.EntityFramework;
using FairSplit.EntityFramework.Commands;
using FairSplit.EntityFramework.Queries;
using FairSplit.Stores;
using FairSplit.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FairSplit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly FairSplitDbContextFactory _dbContextFactory;

        private readonly ICreateGroupCommand _createGroupCommand;
        private readonly IDeleteGroupCommand _deleteGroupCommand;
        private readonly IUpdateGroupCommand _updateGroupCommand;
        private readonly IGetAllGroupsQuery _getAllGroupsQuery;

        private readonly Core _core;
        private readonly NavigationStore _navigationStore;

        public App()
        {
            string _connectionString = "Data Source=FairSplit.db";
            _dbContextFactory = new FairSplitDbContextFactory(
                new DbContextOptionsBuilder().UseSqlite(_connectionString).Options);

            _createGroupCommand = new CreateGroupCommand(_dbContextFactory);
            _deleteGroupCommand = new DeleteGroupCommand(_dbContextFactory);
            _updateGroupCommand = new UpdateGroupCommand(_dbContextFactory);
            _getAllGroupsQuery = new GetAllGroupsQuery(_dbContextFactory);

            _core = new Core(_createGroupCommand, _deleteGroupCommand, _updateGroupCommand, _getAllGroupsQuery);
            _navigationStore = new NavigationStore();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            using(FairSplitDbContext context = _dbContextFactory.Create())
            {
                context.Database.Migrate();
            }

            _navigationStore.CurrentViewModel = new SelectGroupViewModel(_core, _navigationStore);

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_navigationStore)
            };
            MainWindow.Show();

            base.OnStartup(e);
        }
    }

}
