namespace AirbnbClone.Shared.Events
{
    public class BookingCreatedEvent
    {
        public Guid BookingId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid GuestId { get; set; }
        public Guid HostId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}