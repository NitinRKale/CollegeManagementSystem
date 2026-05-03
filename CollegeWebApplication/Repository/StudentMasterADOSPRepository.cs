using CollegeWebApplication.IRepository;
using CollegeWebApplication.Models;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;


namespace CollegeWebApplication.Repository
{
    public class StudentMasterADOSPRepository : IStudentMasterADOSPRepository
    {
        private readonly string _connectionString;

        public StudentMasterADOSPRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("CollegeConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'CollegeConnection' not found.");
            _connectionString = connectionString;
        }
        public IEnumerable<StudentMasterADO> GetAllStudentMasters()
        {
            List<StudentMasterADO> studentMasters = new List<StudentMasterADO>();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = _connectionString;

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "usp_StudentMaster_SelectAll";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        studentMasters.Add(new StudentMasterADO
                        {
                            StudentId  = (int)reader["StudentId"],
                            FirstName  = reader["FirstName"]?.ToString() ?? string.Empty,
                            LastName = reader["LastName"]?.ToString() ?? string.Empty,
                            Gender = reader["Gender"]?.ToString() ?? string.Empty,
                            BirthDate = reader["BirthDate"] as DateTime?,
                            MobileNo = reader["MobileNo"]?.ToString() ?? string.Empty,
                            EmailId = reader["EmailId"]?.ToString() ?? string.Empty,
                            Address = reader["Address"]?.ToString() ?? string.Empty,
                            StateName = reader["StateName"]?.ToString() ?? string.Empty,
                            CityName = reader["CityName"]?.ToString() ?? string.Empty,
                            Pincode = reader["Pincode"]?.ToString() ?? string.Empty,
                            CourseName = reader["CourseName"]?.ToString() ?? string.Empty,
                            YearOfStudy = reader["YearOfStudy"]?.ToString() ?? string.Empty,
                            IsActive  = reader["IsActive"] != DBNull.Value && (bool)reader["IsActive"]
                        });
                    }
                }
            }
            return studentMasters;
        }

        public StudentMasterADO GetStudentMasterById(int studentId)
        {
            var studentMaster = new StudentMasterADO();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_StudentMaster_SelectById";
                cmd.Connection = conn;

                cmd.Parameters.AddWithValue("@StudentId", studentId);               

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        studentMaster = new StudentMasterADO
                        {
                            StudentId = reader["StudentId"] != DBNull.Value ? (int)reader["StudentId"] : 0,
                            FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                            LastName = reader["LastName"]?.ToString() ?? string.Empty,
                            Gender = reader["Gender"]?.ToString() ?? string.Empty,
                            BirthDate = reader["BirthDate"] as DateTime?,
                            MobileNo = reader["MobileNo"]?.ToString() ?? string.Empty,
                            EmailId = reader["EmailId"]?.ToString() ?? string.Empty,
                            Address = reader["Address"]?.ToString() ?? string.Empty,
                            StateId = reader["StateId"] != DBNull.Value ? (int)reader["StateId"] : 0,
                            StateName = reader["StateName"]?.ToString() ?? string.Empty,
                            CityId = reader["CityId"] != DBNull.Value ? (int)reader["CityId"] : 0,
                            CityName = reader["CityName"]?.ToString() ?? string.Empty,
                            Pincode = reader["Pincode"]?.ToString() ?? string.Empty,
                            CourseId = reader["CourseId"] != DBNull.Value ? (int)reader["CourseId"] : 0,
                            CourseName = reader["CourseName"]?.ToString() ?? string.Empty,
                            YearOfStudy = reader["YearOfStudy"]?.ToString() ?? string.Empty,
                            IsActive = (bool)reader["IsActive"]                            
                        };
                    }
                }
            }
            return studentMaster;
        }

        public Task<string> AddStudentMaster(StudentMasterADO studentMaster)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "usp_StudentMaster_Insert";
                sqlCommand.Connection = connection;

                sqlCommand.Parameters.AddWithValue("@FirstName", studentMaster.FirstName);
                sqlCommand.Parameters.AddWithValue("@LastName", studentMaster.LastName);
                sqlCommand.Parameters.AddWithValue("@Gender", studentMaster.Gender);
                sqlCommand.Parameters.AddWithValue("@BirthDate", studentMaster.BirthDate);
                sqlCommand.Parameters.AddWithValue("@MobileNo", studentMaster.MobileNo);
                sqlCommand.Parameters.AddWithValue("@EmailId", studentMaster.EmailId);
                sqlCommand.Parameters.AddWithValue("@Address", studentMaster.Address);
                sqlCommand.Parameters.AddWithValue("@StateId", studentMaster.StateId);
                sqlCommand.Parameters.AddWithValue("@CityId", studentMaster.CityId);
                sqlCommand.Parameters.AddWithValue("@Pincode", studentMaster.Pincode);
                sqlCommand.Parameters.AddWithValue("@CourseId", studentMaster.CourseId);
                sqlCommand.Parameters.AddWithValue("@YearOfStudy", studentMaster.YearOfStudy);
                sqlCommand.Parameters.AddWithValue("@IsActive", studentMaster.IsActive);

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

                return Task.FromResult(result ?? "No result returned");
            }
        }

        public Task<string> UpdateStudentMaster(StudentMasterADO studentMaster)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "usp_StudentMaster_Update";
                sqlCommand.Connection = connection;

                sqlCommand.Parameters.AddWithValue("@StudentId", studentMaster.StudentId);
                sqlCommand.Parameters.AddWithValue("@FirstName", studentMaster.FirstName);
                sqlCommand.Parameters.AddWithValue("@LastName", studentMaster.LastName);
                sqlCommand.Parameters.AddWithValue("@Gender", studentMaster.Gender);
                sqlCommand.Parameters.AddWithValue("@BirthDate", studentMaster.BirthDate);
                sqlCommand.Parameters.AddWithValue("@MobileNo", studentMaster.MobileNo);
                sqlCommand.Parameters.AddWithValue("@EmailId", studentMaster.EmailId);
                sqlCommand.Parameters.AddWithValue("@Address", studentMaster.Address);
                sqlCommand.Parameters.AddWithValue("@StateId", studentMaster.StateId);
                sqlCommand.Parameters.AddWithValue("@CityId", studentMaster.CityId);
                sqlCommand.Parameters.AddWithValue("@Pincode", studentMaster.Pincode);
                sqlCommand.Parameters.AddWithValue("@CourseId", studentMaster.CourseId);
                sqlCommand.Parameters.AddWithValue("@YearOfStudy", studentMaster.YearOfStudy);
                sqlCommand.Parameters.AddWithValue("@IsActive", studentMaster.IsActive);

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

                return Task.FromResult(result ?? "No result returned");
            }
        }

        public Task<string> DeleteStudentMaster(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "usp_StudentMaster_DeleteById";
                sqlCommand.Connection = connection;

                sqlCommand.Parameters.AddWithValue("@StudentId", studentId);


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

                return Task.FromResult(result ?? "No result returned");
            }
        }
    }
}
