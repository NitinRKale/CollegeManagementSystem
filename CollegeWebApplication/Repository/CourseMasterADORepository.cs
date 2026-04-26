using CollegeWebApplication.IRepository;
using CollegeWebApplication.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CollegeWebApplication.Repository
{
    public class CourseMasterADORepository : ICourseMasterADORepository
    {
        private readonly string _connectionString;

        public CourseMasterADORepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("CollegeConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'CollegeConnection' not found.");
            _connectionString = connectionString;
        }

        public IEnumerable<CourseMaster> GetAllCourses()
        {
            var courseList = new List<CourseMaster>();
            using (var conn = new SqlConnection(_connectionString))
            {
                // conn.ConnectionString = _connectionString;

                // SqlCommand cmd = new SqlCommand("usp_CourseMaster_SelectAll", conn);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "usp_CourseMaster_SelectAll";
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courseList.Add(new CourseMaster
                        {
                            CourseId = (int)reader["CourseId"],
                            CourseName = reader["CourseName"]?.ToString() ?? string.Empty
                        });
                    }
                }
            }
            return courseList;
        }
        public CourseMaster GetCourseById(int courseId)
        {
            var courseMaster = new CourseMaster();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_CourseMaster_SelectById";
                cmd.Parameters.AddWithValue("@CourseId", courseId);
                cmd.Connection = conn;

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courseMaster = new CourseMaster
                        {
                            CourseId = reader["CourseId"] != DBNull.Value ? (int)reader["CourseId"] : 0,
                            CourseName = reader["CourseName"]?.ToString() ?? string.Empty
                        };
                    }
                }
            }
            return courseMaster;
        }

        public string AddCourseMaster(CourseMaster courseMaster)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "usp_CourseMaster_Insert";
                sqlCommand.Connection = connection;

                sqlCommand.Parameters.AddWithValue("@CourseName", courseMaster.CourseName);

                // Output parameter (VARCHAR)
                SqlParameter outputParam = new SqlParameter("@Result", SqlDbType.VarChar, 50)
                {
                    Direction = ParameterDirection.Output
                };
                sqlCommand.Parameters.Add(outputParam);

                connection.Open();
                sqlCommand.ExecuteNonQuery();

                // Retrieve output value
                string? result = sqlCommand.Parameters["@Result"].Value as string;

                return result ?? "No result returned";
            }
        }

        public string UpdateCourseMaster(CourseMaster courseMaster)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "usp_CourseMaster_Update";
                sqlCommand.Connection = connection;
                sqlCommand.Parameters.AddWithValue("@CourseId", courseMaster.CourseId);
                sqlCommand.Parameters.AddWithValue("@FirstName", courseMaster.CourseName);

                // Output parameter (VARCHAR)
                SqlParameter outputParam = new SqlParameter("@Result", SqlDbType.VarChar, 50)
                {
                    Direction = ParameterDirection.Output
                };
                sqlCommand.Parameters.Add(outputParam);
                connection.Open();
                sqlCommand.ExecuteNonQuery();

                // Retrieve output value
                string? result = sqlCommand.Parameters["@Result"].Value as string;

                return result ?? "No result returned";
            }
        }

        public string DeleteCourseMaster(int courseId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "usp_CourseMaster_Delete";
                sqlCommand.Connection = connection;
                sqlCommand.Parameters.AddWithValue("@CourseId", courseId);

                // Output parameter (VARCHAR)
                SqlParameter outputParam = new SqlParameter("@Result", SqlDbType.VarChar, 50)
                {
                    Direction = ParameterDirection.Output
                };

                sqlCommand.Parameters.Add(outputParam);
                connection.Open();
                sqlCommand.ExecuteNonQuery();


                // Retrieve output value
                string? result = sqlCommand.Parameters["@Result"].Value as string;
                return result ?? "No result returned";
            }
        }
    }
}
