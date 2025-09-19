using System.ComponentModel.DataAnnotations;

namespace PropertyService.Models
{
    public class Property
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid HostId { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public string Address { get; set; } = string.Empty;
        
        [Required]
        public string City { get; set; } = string.Empty;
        
        [Required]
        public string Country { get; set; } = string.Empty;
        
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        [Required]
        public PropertyType PropertyType { get; set; }
        
        public int MaxGuests { get; set; }
        
        public int Bedrooms { get; set; }
        
        public int Bathrooms { get; set; }
        
        [Required]
        public decimal PricePerNight { get; set; }
        
        public decimal? WeeklyDiscount { get; set; }
        
        public decimal? MonthlyDiscount { get; set; }
        
        public List<string> Images { get; set; } = new();
        
        public List<string> Amenities { get; set; } = new();
        
        public bool IsActive { get; set; } = true;
        
        public bool InstantBook { get; set; } = false;
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public List<DateRange> UnavailableDates { get; set; } = new();
        
        public double AverageRating { get; set; } = 0.0;
        
        public int ReviewCount { get; set; } = 0;
        
        public int MinNights { get; set; } = 1;
        
        public int MaxNights { get; set; } = 365;
    }

    public enum PropertyType
    {
        House,
        Apartment,
        Condo,
        Villa,
        Cabin,
        Hotel,
        BedAndBreakfast,
        Loft,
        Studio,
        Cottage
    }

    public class DateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }
}