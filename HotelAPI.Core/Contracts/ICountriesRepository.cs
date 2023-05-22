namespace HotelAPI.Core.Contracts
{
    public interface ICountriesRepository : IGenericRepository<HotelAPI.Data.Country> {
        Task<HotelAPI.Data.Country> GetDetails(int id);
    }

}
