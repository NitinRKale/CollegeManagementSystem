using CollegeWebApplication.IRepository;
using CollegeWebApplication.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CollegeWebApplication.Repository
{
    public class StateMasterADORepository : IStateMasterADORepository
    {
        private readonly string _connectionString;

        public StateMasterADORepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("CollegeConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'CollegeConnection' not found.");
            _connectionString = connectionString;
        }
        public IEnumerable<StateMaster> GetAllStateMasters()
        {
            var stateList = new List<StateMaster>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("usp_StateMaster_SelectAll", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stateList.Add(new StateMaster
                        {
                            StateId = (int)reader["StateId"],
                            StateName = reader["StateName"]?.ToString() ?? string.Empty
                        });
                    }
                }
            }
            return stateList;
        }

        public StateMaster GetStateMasterById(int stateId)
        {
            var stateMaster = new StateMaster();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_StateMaster_SelectById";
                cmd.Parameters.AddWithValue("@StateId", stateId);
                cmd.Connection = conn;

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stateMaster = new StateMaster
                        {
                            StateId = reader["StateId"] != DBNull.Value ? (int)reader["StateId"] : 0,                            
                            StateName = reader["StateName"]?.ToString() ?? string.Empty
                        };
                    }
                }
            }
            return stateMaster;
        }

        public string AddStateMaster(StateMaster stateMaster)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "usp_StateMaster_Insert";
                sqlCommand.Connection = connection;

                sqlCommand.Parameters.AddWithValue("@StateName", stateMaster.StateName);

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

        public string UpdateStateMaster(StateMaster stateMaster)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "usp_StateMaster_Update";
                sqlCommand.Connection = connection;
                sqlCommand.Parameters.AddWithValue("@StateId", stateMaster.StateId);
                sqlCommand.Parameters.AddWithValue("@FirstName", stateMaster.StateName);

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

        public string DeleteStateMaster(int stateId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "usp_StateMaster_Delete";
                sqlCommand.Connection = connection;
                sqlCommand.Parameters.AddWithValue("@StateId", stateId);

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
