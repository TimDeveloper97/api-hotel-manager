using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Data
{
    public class HotelDbContext : IdentityDbContext<ApiUser>
    {
        public HotelDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasData(new Country
            {
                Id = 1,
                Name = "Jamaica",
                ShortName = "JM",
            },
            new Country
            {
                Id = 2,
                Name = "Bahamas",
                ShortName = "BS",
            },
            new Country
            {
                Id = 3,
                Name = "Jenifer",
                ShortName = "JF",
            });

            modelBuilder.Entity<Hotel>().HasData(new Hotel
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
