﻿using AutoMapper;
using HotelAPI.Core.Contracts;
using HotelAPI.Core.Country;
using HotelAPI.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelAPI.Controllers
{
    [Route("api/v{version:apiVersion}/countries")]
    [ApiController]
    [ApiVersion("2.0")]
    public class CountriesV2Controller : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;
        private readonly ILogger<CountriesV2Controller> _logger;

        public CountriesV2Controller(IMapper mapper, ICountriesRepository countriesRepository, ILogger<CountriesV2Controller> logger)
        {
            _mapper = mapper;
            _countriesRepository = countriesRepository;
            this._logger = logger;
        }

        // GET: api/Countries
        //$select=name,shortname
        //$filter=name eq 'Cuba'
        //$orderby=name
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            var countries = await _countriesRepository.GetAllAsync();
            var records = _mapper.Map<List<GetCountryDto>>(countries);
            return Ok();
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountry(int id)
        {
            var country = await _countriesRepository.GetDetails(id);

            if (country == null)
            {
                throw new NotFoundException(nameof(GetCountries), id);
            }

            var countryDto = _mapper.Map<CountryDto>(country);
            return Ok(countryDto);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateCountry)
        {
            if (id != updateCountry.Id)
            {
                return BadRequest("Invalid Record Id");
            }

            //_context.Entry(country).State = EntityState.Modified;
            var country = await _countriesRepository.GetAsync(id);
            if (country == null) { throw new NotFoundException(nameof(PutCountry), id); }

            _mapper.Map(updateCountry, country);

            try
            {
                await _countriesRepository.UpdateAsync(country);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryExists(id))
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
        [Authorize(Roles = "User")]
        //public async Task<ActionResult<Country>> PostCountry(Country country)
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountryDto)
        {
            var countryNew = _mapper.Map<Country>(createCountryDto);
            await _countriesRepository.AddAsync(countryNew);

            return CreatedAtAction("GetCountry", new { id = countryNew.Id }, countryNew);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _countriesRepository.GetAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            await _countriesRepository.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> CountryExists(int id)
        {
            return await _countriesRepository.ExistAsync(id);
        }
    }
}
