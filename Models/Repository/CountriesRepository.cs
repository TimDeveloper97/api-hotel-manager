using HotelAPI.Contracts;

namespace HotelAPI.Models.Repository
{
    public class CountriesRepository : GenericRepository<Data.Country>, ICountriesRepository
    {
        public CountriesRepository(HotelDbContext context) : base(context)
        {
        }
    }
}
