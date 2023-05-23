using AutoMapper;
using HotelAPI.Data;
using HotelAPI.Core.Country;
using HotelAPI.Core.Hotel;
using HotelAPI.Core.User;

namespace HotelAPI.Core.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig() {
            //Country
            CreateMap<Data.Country, CreateCountryDto>().ReverseMap();
            CreateMap<Data.Country, GetCountryDto>().ReverseMap();
            CreateMap<Data.Country, CountryDto>().ReverseMap();
            CreateMap<Data.Country, UpdateCountryDto>().ReverseMap();

            //Hotel
            CreateMap<Data.Hotel, HotelDto>().ReverseMap();
            CreateMap<Data.Hotel, CreateHotelDto>().ReverseMap();
            CreateMap<Data.Hotel, UpdateHotelDto>().ReverseMap();

            //
            CreateMap<ApiUserDto, ApiUser>().ReverseMap();
        }
    }
}
