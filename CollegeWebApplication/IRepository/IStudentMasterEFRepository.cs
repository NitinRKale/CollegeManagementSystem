using CollegeWebApplication.Models;

namespace CollegeWebApplication.IRepository
{
    public interface IStudentMasterEFRepository
    {
        Task<List<StudentMaster>> GetAllStudentMastersAsync();
        Task<StudentMaster> GetStudentMasterByIdAsync(int id);
        Task AddStudentMasterAsync(StudentMaster studentMaster);
        Task UpdateStudentMasterAsync(StudentMaster studentMaster);
        Task DeleteStudentMasterAsync(int id);

        Task<List<StateMaster>> GetAllStateAsync();
        Task<List<CityMaster>> GetAllCityAsync(int stateId);
        Task<List<CourseMaster>> GetAllCourseAsync();
    }
}
