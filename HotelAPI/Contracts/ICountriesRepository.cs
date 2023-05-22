
namespace HotelAPI.Contracts
{
    public interface ICountriesRepository : IGenericRepository<Country> {
        Task<Country> GetDetails(int id);
    }

}
