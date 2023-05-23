using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelAPI.Data.Configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(new Hotel
            {
                Id = 1,
                Name = "Sapa",
                Address = "Sapa",
                CountryId = 1,
                Rating = 4.5
            },
            new Hotel
            {
                Id = 2,
                Name = "Ha Noi",
                Address = "Ha Noi",
                CountryId = 2,
                Rating = 1.5
            },
            new Hotel
            {
                Id = 3,
                Name = "Lao Cai",
                Address = "Lao Cai",
                CountryId = 3,
                Rating = 3.5
            });
        }
    }
}
