using AutoMapper;
using HotelAPI.Core.Contracts;
using HotelAPI.Data;

namespace HotelAPI.Core.Repository
{
    public class HotelsRepository : GenericRepository<Data.Hotel>, IHotelsRepository
    {
        public HotelsRepository(HotelDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}
