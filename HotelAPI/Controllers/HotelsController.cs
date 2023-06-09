﻿using AutoMapper;
using HotelAPI.Core;
using HotelAPI.Core.Contracts;
using HotelAPI.Core.Hotel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotelAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels()
        {
            var hotels = await _hotelsRepository.GetAllAsync<HotelDto>();
            return Ok(hotels);
        }

        // GET: api/Hotel/?StartIndex=0&pagesize=25&PageNumber=1
        [HttpGet]
        public async Task<ActionResult<PagedResult<HotelDto>>> GetPagedHotel([FromQuery] QueryParameters queryParameters)
        {
            var pagedHotelResult = await _hotelsRepository.GetAllAsync<HotelDto>(queryParameters);
            return Ok(pagedHotelResult);
        }

        // GET api/<HotelsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelDto>> GetHotel(int id)
        {
            var hotel = await _hotelsRepository.GetAsync<HotelDto>(id);
            return Ok(hotel);
        }

        // POST api/<HotelsController>
        [HttpPost]
        public async Task<ActionResult<Hotel>> PostHotel([FromBody] CreateHotelDto createHotel)
        {
            var hotel = await _hotelsRepository.AddAsync<CreateHotelDto, Hotel>(createHotel);
            return CreatedAtAction(nameof(GetHotel), new { id = hotel.Id }, hotel);
        }

        // PUT api/<HotelsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, [FromBody] HotelDto hotelDto)
        {
            if (id != hotelDto.Id) return BadRequest();

            try
            {
                await _hotelsRepository.UpdateAsync(id, hotelDto);
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
            await _hotelsRepository.DeleteAsync(id);
            return Ok();
        }

        private async Task<bool> HotelExists(int id)
        {
            return await _hotelsRepository.ExistAsync(id);
        }
    }
}
