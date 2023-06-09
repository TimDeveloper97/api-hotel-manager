﻿using System.ComponentModel.DataAnnotations;

namespace HotelAPI.Core.Hotel
{
    public abstract class BaseHotelDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public double? Rating { get; set; }
        [Required]
        [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue)]
        public int CountryId { get; set; }
    }
}
