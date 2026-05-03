using CollegeWebApplication.Models;

namespace CollegeWebApplication.IRepository
{
    public interface IStateMasterEFRepository
    {
        Task<List<StateMaster>> GetAllStateMastersAsync();
        Task<StateMaster> GetStateMasterByIdAsync(int id);
        Task AddStateMasterAsync(StateMaster stateMaster);
        Task UpdateStateMasterAsync(StateMaster stateMaster);
        Task DeleteStateMasterAsync(int id);
    }
}
