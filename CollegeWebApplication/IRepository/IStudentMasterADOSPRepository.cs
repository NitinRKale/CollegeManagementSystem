using CollegeWebApplication.Models;

namespace CollegeWebApplication.IRepository
{
    public interface IStudentMasterADOSPRepository
    {
        IEnumerable<StudentMaster> GetAllStudentMasters();
        StudentMaster GetStudentMasterById(int id);
        Task<string> AddStudentMaster(StudentMaster studentMaster);
        Task<string> UpdateStudentMaster(StudentMaster studentMaster);
        Task<string> DeleteStudentMaster(int id);

        //Task<List<StateMaster>> GetAllStateAsync();
        //Task<List<CityMaster>> GetAllCityAsync(int stateId);
        //Task<List<CourseMaster>> GetAllCourseAsync();
    }
}
