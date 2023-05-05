namespace HotelAPI.Models.Hotel
{
    public class HotelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Rating { get; set; } = 0;
        public int CountryId { get; set; }
    }
}
