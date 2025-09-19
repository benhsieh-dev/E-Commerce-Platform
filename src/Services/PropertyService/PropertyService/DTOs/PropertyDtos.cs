using System.ComponentModel.DataAnnotations;
using PropertyService.Models;

namespace PropertyService.DTOs
{
    public class CreatePropertyDto
    {
        [Required]
        public Guid HostId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;
        
        [Range(-90, 90)]
        public double Latitude { get; set; }
        
        [Range(-180, 180)]
        public double Longitude { get; set; }
        
        [Required]
        public PropertyType PropertyType { get; set; }
        
        [Range(1, 50)]
        public int MaxGuests { get; set; }
        
        [Range(0, 20)]
        public int Bedrooms { get; set; }
        
        [Range(0, 20)]
        public int Bathrooms { get; set; }
        
        [Required]
        [Range(1, 10000)]
        public decimal PricePerNight { get; set; }
        
        [Range(0, 100)]
        public decimal? WeeklyDiscount { get; set; }
        
        [Range(0, 100)]
        public decimal? MonthlyDiscount { get; set; }
        
        public List<string> Images { get; set; } = new();
        
        public List<string> Amenities { get; set; } = new();
        
        public bool InstantBook { get; set; } = false;
        
        [Range(1, 365)]
        public int MinNights { get; set; } = 1;
        
        [Range(1, 365)]
        public int MaxNights { get; set; } = 365;
    }

    public class UpdatePropertyDto
    {
        [StringLength(200)]
        public string? Title { get; set; }
        
        [StringLength(2000)]
        public string? Description { get; set; }
        
        [StringLength(500)]
        public string? Address { get; set; }
        
        [StringLength(100)]
        public string? City { get; set; }
        
        [StringLength(100)]
        public string? Country { get; set; }
        
        [Range(-90, 90)]
        public double? Latitude { get; set; }
        
        [Range(-180, 180)]
        public double? Longitude { get; set; }
        
        public PropertyType? PropertyType { get; set; }
        
        [Range(1, 50)]
        public int? MaxGuests { get; set; }
        
        [Range(0, 20)]
        public int? Bedrooms { get; set; }
        
        [Range(0, 20)]
        public int? Bathrooms { get; set; }
        
        [Range(1, 10000)]
        public decimal? PricePerNight { get; set; }
        
        [Range(0, 100)]
        public decimal? WeeklyDiscount { get; set; }
        
        [Range(0, 100)]
        public decimal? MonthlyDiscount { get; set; }
        
        public List<string>? Images { get; set; }
        
        public List<string>? Amenities { get; set; }
        
        public bool? IsActive { get; set; }
        
        public bool? InstantBook { get; set; }
        
        [Range(1, 365)]
        public int? MinNights { get; set; }
        
        [Range(1, 365)]
        public int? MaxNights { get; set; }
    }

    public class PropertyResponseDto
    {
        public Guid Id { get; set; }
        public Guid HostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public PropertyType PropertyType { get; set; }
        public int MaxGuests { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal? WeeklyDiscount { get; set; }
        public decimal? MonthlyDiscount { get; set; }
        public List<string> Images { get; set; } = new();
        public List<string> Amenities { get; set; } = new();
        public bool IsActive { get; set; }
        public bool InstantBook { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<DateRange> UnavailableDates { get; set; } = new();
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int MinNights { get; set; }
        public int MaxNights { get; set; }
    }

    public class PropertyListResponseDto
    {
        public List<PropertyResponseDto> Properties { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class PropertySearchDto
    {
        public string? Location { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int? Guests { get; set; }
        public PropertyType? PropertyType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<string>? Amenities { get; set; }
        public bool? InstantBook { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class SetUnavailableDatesDto
    {
        [Required]
        public List<DateRange> UnavailableDates { get; set; } = new();
    }

    public class CheckAvailabilityDto
    {
        [Required]
        public DateTime CheckIn { get; set; }
        
        [Required]
        public DateTime CheckOut { get; set; }
    }

    public class AvailabilityResponseDto
    {
        public bool IsAvailable { get; set; }
        public string? Message { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? Nights { get; set; }
    }
}