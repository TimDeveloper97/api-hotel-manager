using AutoMapper;
using HotelAPI.Contracts;
using HotelAPI.Models.Hotel;
using HotelAPI.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelsRepository _hotelsRepository;
        private readonly IMapper _mapper;

        public HotelsController(IHotelsRepository hotelsRepository, IMapper mapper)
        {
            this._hotelsRepository = hotelsRepository;
            this._mapper = mapper;
        }

        // GET: api/<HotelsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels()
        {
            var hotels = await _hotelsRepository.GetAllAsync();
            var getHotels = _mapper.Map<List<HotelDto>>(hotels);
            return Ok(getHotels);
        }

        // GET api/<HotelsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            var hotel = await _hotelsRepository.GetAsync(id);
            if (hotel == null) return NotFound();
            return Ok(_mapper.Map<HotelDto>(hotel));
        }

        // POST api/<HotelsController>
        [HttpPost]
        public async Task<ActionResult<Hotel>> PostHotel([FromBody] CreateHotelDto createHotel)
        {
            var hotel = _mapper.Map<Hotel>(createHotel);
            await _hotelsRepository.AddAsync(hotel);

            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }

        // PUT api/<HotelsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, [FromBody] HotelDto hotelDto)
        {
            if (id != hotelDto.Id) return BadRequest();

            var hotel = await _hotelsRepository.GetAsync(id);
            if (hotel == null) return NotFound();

            _mapper.Map(hotelDto, hotel);

            try
            {
                await _hotelsRepository.UpdateAsync(hotel);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await HotelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // DELETE api/<HotelsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var hotel = await _hotelsRepository.GetAsync(id);
            if (hotel == null) return NotFound();

            await _hotelsRepository.DeleteAsync(id);

            return Ok();
        }

        private async Task<bool> HotelExists(int id)
        {
            return await _hotelsRepository.ExistAsync(id);
        }
    }
}
