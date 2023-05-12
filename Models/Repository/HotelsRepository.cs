using HotelAPI.Contracts;

namespace HotelAPI.Models.Repository
{
    public class HotelsRepository : GenericRepository<Data.Hotel>, IHotelsRepository
    {
        public HotelsRepository(HotelDbContext context) : base(context)
        {
        }
    }
}
