using HotelAPI.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Models.Repository
{
    public class CountriesRepository : GenericRepository<Data.Country>, ICountriesRepository
    {
        private readonly HotelDbContext _context;

        public CountriesRepository(HotelDbContext context) : base(context)
        {
            this._context = context;
        }

        public async Task<Data.Country> GetDetails(int id) => await _context.Countries.Include(x => x.Hotels)
                .FirstOrDefaultAsync(x => x.Id == id);
    }
}
