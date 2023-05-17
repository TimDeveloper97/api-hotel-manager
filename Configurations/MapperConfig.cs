using AutoMapper;
using HotelAPI.Models.Country;
using HotelAPI.Models.Hotel;
using HotelAPI.Models.User;

namespace HotelAPI.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig() {
            //Country
            CreateMap<Country, CreateCountryDto>().ReverseMap();
            CreateMap<Country, GetCountryDto>().ReverseMap();
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<Country, UpdateCountryDto>().ReverseMap();

            //Hotel
            CreateMap<Hotel, HotelDto>().ReverseMap();
            CreateMap<Hotel, CreateHotelDto>().ReverseMap();
            CreateMap<Hotel, UpdateHotelDto>().ReverseMap();

            //
            CreateMap<ApiUserDto, ApiUser>().ReverseMap();
        }
    }
}
