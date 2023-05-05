using AutoMapper;
using HotelAPI.Data;
using HotelAPI.Models.Country;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly HotelDbContext _context;
        private readonly IMapper _mapper;

        public CountriesController(HotelDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            var countries = await _context.Countries.ToListAsync();
            var records = _mapper.Map<List<GetCountryDto>>(countries);
            return Ok(records);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            var country = await _context.Countries.Include(x => x.Hotels).FirstOrDefaultAsync(x => x.Id == id);

            if (country == null)
            {
                return NotFound();
            }

            var countryDto = _mapper.Map<CountryDto>(country);
            return Ok(countryDto);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountry)
        {
            if (id != updateCountry.Id)
            {
                return BadRequest("Invalid Record Id");
            }

            //_context.Entry(country).State = EntityState.Modified;
            var country = await _context.Countries.FindAsync(id);
            if (country == null) { return  NotFound(); }

            _mapper.Map(updateCountry, country);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
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

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        //public async Task<ActionResult<Country>> PostCountry(Country country)
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountryDto)
        {
            var countryOld = new Country
            {
                Name = createCountryDto.Name,
                ShortName = createCountryDto.ShortName,
            };

            var countryNew = _mapper.Map<Country>(createCountryDto);

            _context.Countries.Add(countryNew);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = countryNew.Id }, countryNew);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }
    }
}
