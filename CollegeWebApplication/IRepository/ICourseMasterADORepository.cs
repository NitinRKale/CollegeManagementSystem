using CollegeWebApplication.Models;

namespace CollegeWebApplication.IRepository
{
    public interface ICourseMasterADORepository
    {
        IEnumerable<CourseMaster> GetAllCourses();
        CourseMaster GetCourseById(int id);
        string AddCourseMaster(CourseMaster stateMaster);
        string UpdateCourseMaster(CourseMaster stateMaster);
        string DeleteCourseMaster(int id);
    }
}
