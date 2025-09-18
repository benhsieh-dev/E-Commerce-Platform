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
        
        public List<string> Images { get; set; } = new();
        
        public List<string> Amenities { get; set; } = new();
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public List<DateRange> UnavailableDates { get; set; } = new();
    }

    public enum PropertyType
    {
        House,
        Apartment,
        Condo,
        Villa,
        Cabin,
        Hotel,
        BedAndBreakfast
    }

    public class DateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}