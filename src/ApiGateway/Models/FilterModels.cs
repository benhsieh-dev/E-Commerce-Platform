namespace ApiGateway.Models
{
    public class PropertyFilter
    {
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public PropertyType? PropertyType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinGuests { get; set; }
        public int? MaxGuests { get; set; }
        public bool? InstantBookOnly { get; set; }
    }

    public class BookingFilter
    {
        public Guid? GuestId { get; set; }
        public Guid? HostId { get; set; }
        public Guid? PropertyId { get; set; }
        public BookingStatus? Status { get; set; }
        public DateTime? CheckInDateFrom { get; set; }
        public DateTime? CheckInDateTo { get; set; }
        public DateTime? CheckOutDateFrom { get; set; }
        public DateTime? CheckOutDateTo { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
    }
}