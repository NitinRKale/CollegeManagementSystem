using CollegeWebApplication.Models;

namespace CollegeWebApplication.IRepository
{
    public interface ICityMasterADOSPRepository
    {
        IEnumerable<CityMaster> GetAllCities();

        IEnumerable<CityMaster> GetCitiesByState(int stateId);

        CityMaster GetCityById(int cityId);
        void AddCity(CityMaster stateMaster);
        void UpdateCity(CityMaster stateMaster);
        void DeleteCity(int cityId);
    }
}
