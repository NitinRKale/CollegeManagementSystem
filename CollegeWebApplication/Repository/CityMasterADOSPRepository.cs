using CollegeWebApplication.IRepository;
using CollegeWebApplication.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CollegeWebApplication.Repository
{
    public class CityMasterADOSPRepository : ICityMasterADOSPRepository
    {
        private readonly string _connectionString;

        public CityMasterADOSPRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("CollegeConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'CollegeConnection' not found.");
            _connectionString = connectionString;
        }

        public IEnumerable<CityMaster> GetAllCities()
        {
            var cityList = new List<CityMaster>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("usp_CityMaster_SelectAll", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cityList.Add(new CityMaster
                        {
                            CityId = (int)reader["CityId"],
                            CityName = reader["CityName"]?.ToString() ?? string.Empty,
                            StateId = (int)reader["StateId"]
                            //StateName = reader["StateName"]?.ToString() ?? string.Empty,
                        });
                    }
                }
            }
            return cityList;
        }

        public IEnumerable<CityMaster> GetCitiesByState(int stateId)
        {
            var cityList = new List<CityMaster>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand("usp_CityMaster_SelectByState", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StateId", stateId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cityList.Add(new CityMaster
                        {
                            CityId = (int)reader["CityId"],
                            CityName = reader["CityName"]?.ToString() ?? string.Empty,
                            StateId = (int)reader["StateId"]
                            //StateName = reader["StateName"]?.ToString() ?? string.Empty,
                        });
                    }
                }
            }
            return cityList;
        }

        public CityMaster GetCityById(int cityId)
        {
            CityMaster cityMaster = null;           
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetCityById";
                cmd.Parameters.AddWithValue("@CityId", cityId);
                cmd.Connection = conn;

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cityMaster = new CityMaster
                        {
                            CityId = (int)reader["CityId"],
                            CityName = reader["CityName"]?.ToString() ?? string.Empty,
                            StateId = (int)reader["StateId"]
                            //StateName = reader["StateName"]?.ToString() ?? string.Empty,
                        };
                    }
                }
            }
            return cityMaster;
        }

        public void AddCity(CityMaster cityMaster)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "AddCity";
                cmd.Parameters.AddWithValue("@CityName", cityMaster.CityName);
                cmd.Connection = conn;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateCity(CityMaster cityMaster)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UpdateCity";
                cmd.Parameters.AddWithValue("@CityId", cityMaster.CityId);
                cmd.Parameters.AddWithValue("@CityName", cityMaster.CityName);
                cmd.Connection = conn;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteCity(int cityId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "DeleteCity";
                cmd.Parameters.AddWithValue("@CityId", cityId);
                cmd.Connection = conn;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
