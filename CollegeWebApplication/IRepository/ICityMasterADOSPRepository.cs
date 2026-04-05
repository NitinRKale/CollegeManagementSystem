using CollegeWebApplication.Models;

namespace CollegeWebApplication.IRepository
{
    public interface ICityMasterADOSPRepository
    {
        IEnumerable<CityMaster> GetAllCitiesAsync();

        IEnumerable<CityMaster> GetCitiesByStateIdIdAsync(int stateId);

        CityMaster GetCityByIdAsync(int cityId);
        void AddCityAsync(CityMaster stateMaster);
        void UpdateCityAsync(CityMaster stateMaster);
        void DeleteCityAsync(int cityId);
    }
}
