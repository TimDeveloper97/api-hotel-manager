using AutoMapper;
using HotelAPI.Core.Contracts;
using HotelAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Core.Repository
{
    public class CountriesRepository : GenericRepository<Data.Country>, ICountriesRepository
    {
        private readonly HotelDbContext _context;

        public CountriesRepository(HotelDbContext context, IMapper mapper) : base(context, mapper)
        {
            this._context = context;
        }

        public async Task<Data.Country> GetDetails(int id) => await _context.Countries.Include(x => x.Hotels)
                .FirstOrDefaultAsync(x => x.Id == id);
    }
}
