namespace HotelAPI.Core.Country
{
    public class CountryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        public List<Hotel.HotelDto> Hotels { get; set; }
    }
}
