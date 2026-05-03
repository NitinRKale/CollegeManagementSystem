using CollegeWebApplication.Models;

namespace CollegeWebApplication.IRepository
{
    public interface IStateMasterADORepository
    {
        IEnumerable<StateMaster> GetAllStateMasters();
        StateMaster GetStateMasterById(int id);
        string AddStateMaster(StateMaster stateMaster);
        string UpdateStateMaster(StateMaster stateMaster);
        string DeleteStateMaster(int id);
    }
}
