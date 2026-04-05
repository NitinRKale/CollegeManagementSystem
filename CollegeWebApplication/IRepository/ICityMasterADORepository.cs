using CollegeWebApplication.Models;

namespace CollegeWebApplication.IRepository
{
    public interface ICityMasterADORepository
    {
        IEnumerable<CityMaster> GetAllCitiesAsync();

        IEnumerable<CityMaster> GetCitiesByStateIdAsync(int stateId);

        CityMaster GetCityByIdAsync(int cityId);
        void AddCityAsync(CityMaster stateMaster);
        void UpdateCityAsync(CityMaster stateMaster);
        void DeleteCityAsync(int cityId);

        IEnumerable<StateMaster> GetStatesAsync();
    }
}
