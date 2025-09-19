using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models
{
    public class Property
    {
        public Guid Id { get; set; }
        public Guid HostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PropertyType PropertyType { get; set; }
        public decimal PricePerNight { get; set; }
        public int MaxGuests { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public List<string> Amenities { get; set; } = new();
        public List<string> Images { get; set; } = new();
        public List<string> HouseRules { get; set; } = new();
        public bool IsInstantBook { get; set; }
        public int MinimumStay { get; set; }
        public int MaximumStay { get; set; }
        public decimal? WeeklyDiscount { get; set; }
        public decimal? MonthlyDiscount { get; set; }
        public PropertyStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<DateTime> UnavailableDates { get; set; } = new();
        
        // Computed properties
        public User Host { get; set; } = null!;
        public List<Review> Reviews { get; set; } = new();
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public enum PropertyType
    {
        Apartment = 0,
        House = 1,
        Condo = 2,
        Villa = 3,
        Cabin = 4,
        Room = 5,
        Studio = 6,
        Loft = 7
    }

    public enum PropertyStatus
    {
        Draft = 0,
        Active = 1,
        Inactive = 2,
        Suspended = 3
    }
}