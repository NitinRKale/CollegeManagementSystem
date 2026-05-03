using CollegeWebApplication.IRepository;
using CollegeWebApplication.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CollegeWebApplication.Repository
{
    public class CityMasterADORepository : ICityMasterADORepository
    {
        private readonly string _connectionString;

        public CityMasterADORepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("CollegeConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'CollegeConnection' not found.");
            _connectionString = connectionString;
        }

        public IEnumerable<CityMaster> GetAllCitiesAsync()
        {
            List<CityMaster> cityList = new List<CityMaster>();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = _connectionString;

                SqlCommand cmd = new SqlCommand();
                string strQuery = "SELECT CM.CityId,CM.CityName,SM.StateName FROM CityMaster CM INNER JOIN StateMaster SM ON CM.StateId = SM.StateId";
                cmd.CommandText = strQuery; //"SELECT CM.CityId,CM.CityName,SM.StateName FROM CityMaster CM INNER JOIN StateMaster SM ON CM.StateId = SM.StateId";
                //cmd.CommandText = "SELECT * FROM CityMaster";
                cmd.Connection = conn;
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cityList.Add(new CityMaster
                        {
                            CityId = (int)reader["CityId"],
                            CityName = reader["CityName"]?.ToString() ?? string.Empty
                            //StateName = reader["StateName"]?.ToString() ?? string.Empty
                        });
                    }
                }
            }
            return cityList;
        }

        public IEnumerable<CityMaster> GetCitiesByStateIdAsync(int stateId)
        {
            var cityList = new List<CityMaster>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT CityId, CityName FROM CityMaster WHERE StateId = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", stateId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cityList.Add(new CityMaster
                        {
                            CityId = (int)reader["CityId"],
                            CityName = reader["CityName"]?.ToString() ?? string.Empty
                        });
                    }
                }
            }
            return cityList;
        }

        public void AddCityAsync(CityMaster cityMaster)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("INSERT INTO CityMaster (CityName, StateId) VALUES (@CityName, @StateId)", conn);
                cmd.Parameters.AddWithValue("@CityName", cityMaster.CityName);
                cmd.Parameters.AddWithValue("@StateId", cityMaster.StateId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateCityAsync(CityMaster cityMaster)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("UPDATE CityMaster SET CityName = @CityName, StateId= @StateId WHERE CityId = @CityId", conn);
                cmd.Parameters.AddWithValue("@DepartmentName", cityMaster.CityName);
                cmd.Parameters.AddWithValue("@StateId", cityMaster.StateId);
                cmd.Parameters.AddWithValue("@CityId", cityMaster.CityId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteCityAsync(int cityId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("DELETE FROM CityMaster WHERE CityId = @CityId", conn);
                cmd.Parameters.AddWithValue("@CityId", cityId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public CityMaster GetCityByIdAsync(int cityId)
        {
            CityMaster cityMaster = null;
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT CityId, CityName FROM CityMaster WHERE CityId = @Id", conn);
                cmd.Parameters.AddWithValue("@CityId", cityId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cityMaster = new CityMaster
                        {
                            CityId = (int)reader["CityId"],
                            CityName = reader["CityName"]?.ToString() ?? string.Empty
                        };
                    }
                }
            }
            return cityMaster;
        }

        public IEnumerable<StateMaster> GetStatesAsync()
        {
            List<StateMaster> stateMaster = new List<StateMaster>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("SELECT StateId, StateName FROM StateMaster", conn);               
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stateMaster.Add( new StateMaster
                        {
                            StateId = (int)reader["StateId"],
                            StateName = reader["StateName"]?.ToString() ?? string.Empty
                        });
                    }
                }
            }
            return stateMaster;
        }
    }
}
