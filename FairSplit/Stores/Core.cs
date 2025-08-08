using FairSplit.Domain.Commands;
using FairSplit.Domain.Queries;

namespace FairSplit.Domain.Model
{
    public class Core
    {
        private readonly ICreateGroupCommand _createGroupCommand;
        private readonly IDeleteGroupCommand _deleteGroupCommand;
        private readonly IUpdateGroupCommand _updateGroupCommand;
        private readonly IGetAllGroupsQuery _getAllGroupsQuery;

        private List<Group> _groups;
        private readonly Group _fakeGroup = new("Fake Group");

        public Group CurrentGroup { get; set; }

        public Core(ICreateGroupCommand createGroupCommand, 
            IDeleteGroupCommand deleteGroupCommand, 
            IUpdateGroupCommand updateGroupCommand, 
            IGetAllGroupsQuery getAllGroupsQuery)
        {
            _createGroupCommand = createGroupCommand;
            _deleteGroupCommand = deleteGroupCommand;
            _updateGroupCommand = updateGroupCommand;
            _getAllGroupsQuery = getAllGroupsQuery;
            CurrentGroup = _fakeGroup;

            _groups = [];

            LoadGroups();
        }

        public IEnumerable<Group> GetAllGroups()
        {
            return _groups;
        }

        public Group? GetGroupWithName(string name)
        {
            return _groups.FirstOrDefault(group => group.Name == name);
        }

        public async void LoadGroups()
        {
            var groups = await _getAllGroupsQuery.Execute();

            _groups.Clear();
            _groups.AddRange(groups);
        }

        public async void DeleteCurrentGroup()
        {
            _groups.Remove(CurrentGroup);

            var toDelete = CurrentGroup;
            CurrentGroup = _fakeGroup;
            await _deleteGroupCommand.Execute(toDelete);
        }

        public async void SaveCurrentGroup()
        {
            if (!_groups.Contains(CurrentGroup))
            {
                _groups.Add(CurrentGroup);
                await _createGroupCommand.Execute(CurrentGroup);
            }
            else
            {
                await _updateGroupCommand.Execute(CurrentGroup);
            }
        }

        public async void UpdateCurrentGroup()
        {
            await _updateGroupCommand.Execute(CurrentGroup);
        }
    }
}
