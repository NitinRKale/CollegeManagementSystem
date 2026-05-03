using CollegeWebApplication.Models;

namespace CollegeWebApplication.IRepository
{
    public interface IStudentMasterADOSPRepository
    {
        IEnumerable<StudentMasterADO> GetAllStudentMasters();
        StudentMasterADO GetStudentMasterById(int id);
        Task<string> AddStudentMaster(StudentMasterADO studentMaster);
        Task<string> UpdateStudentMaster(StudentMasterADO studentMaster);
        Task<string> DeleteStudentMaster(int id);
    }
}
