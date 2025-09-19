using ApiGateway.Models;
using ApiGateway.Services;
using ApiGateway.GraphQL.Types;
using GraphQL;
using GraphQL.Types;

namespace ApiGateway.GraphQL.Resolvers
{
    public class BookingResolver
    {
        private readonly IHttpService _httpService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BookingResolver> _logger;
        private readonly UserResolver _userResolver;
        private readonly PropertyResolver _propertyResolver;

        public BookingResolver(
            IHttpService httpService, 
            IConfiguration configuration, 
            ILogger<BookingResolver> logger,
            UserResolver userResolver,
            PropertyResolver propertyResolver)
        {
            _httpService = httpService;
            _configuration = configuration;
            _logger = logger;
            _userResolver = userResolver;
            _propertyResolver = propertyResolver;
        }

        public async Task<Booking?> GetBookingById(Guid id)
        {
            try
            {
                var bookingServiceUrl = _configuration["Services:BookingService"];
                var endpoint = $"{bookingServiceUrl}/api/bookings/{id}";
                
                var booking = await _httpService.GetAsync<Booking>(endpoint);
                
                if (booking != null)
                {
                    await LoadRelatedData(booking);
                }
                
                return booking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching booking {BookingId}", id);
                return null;
            }
        }

        public async Task<List<Booking>> GetBookings(BookingFilterInputType? filter = null, int skip = 0, int take = 20)
        {
            try
            {
                var bookingServiceUrl = _configuration["Services:BookingService"];
                var queryParams = BuildQueryString(filter, skip, take);
                var endpoint = $"{bookingServiceUrl}/api/bookings{queryParams}";
                
                var bookings = await _httpService.GetAsync<List<Booking>>(endpoint) ?? new List<Booking>();
                
                foreach (var booking in bookings)
                {
                    await LoadRelatedData(booking);
                }
                
                return bookings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bookings");
                return new List<Booking>();
            }
        }

        public async Task<List<Booking>> GetBookingsByGuest(Guid guestId, int skip = 0, int take = 20)
        {
            try
            {
                var bookingServiceUrl = _configuration["Services:BookingService"];
                var endpoint = $"{bookingServiceUrl}/api/bookings/guest/{guestId}?skip={skip}&take={take}";
                
                var bookings = await _httpService.GetAsync<List<Booking>>(endpoint) ?? new List<Booking>();
                
                foreach (var booking in bookings)
                {
                    await LoadRelatedData(booking);
                }
                
                return bookings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bookings for guest {GuestId}", guestId);
                return new List<Booking>();
            }
        }

        public async Task<List<Booking>> GetBookingsByHost(Guid hostId, int skip = 0, int take = 20)
        {
            try
            {
                var bookingServiceUrl = _configuration["Services:BookingService"];
                var endpoint = $"{bookingServiceUrl}/api/bookings/host/{hostId}?skip={skip}&take={take}";
                
                var bookings = await _httpService.GetAsync<List<Booking>>(endpoint) ?? new List<Booking>();
                
                foreach (var booking in bookings)
                {
                    await LoadRelatedData(booking);
                }
                
                return bookings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bookings for host {HostId}", hostId);
                return new List<Booking>();
            }
        }

        public async Task<List<Booking>> GetBookingsByProperty(Guid propertyId, int skip = 0, int take = 20)
        {
            try
            {
                var bookingServiceUrl = _configuration["Services:BookingService"];
                var endpoint = $"{bookingServiceUrl}/api/bookings/property/{propertyId}?skip={skip}&take={take}";
                
                var bookings = await _httpService.GetAsync<List<Booking>>(endpoint) ?? new List<Booking>();
                
                foreach (var booking in bookings)
                {
                    await LoadRelatedData(booking);
                }
                
                return bookings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bookings for property {PropertyId}", propertyId);
                return new List<Booking>();
            }
        }

        public async Task<Booking?> CreateBooking(Booking bookingInput, string? token = null)
        {
            try
            {
                var bookingServiceUrl = _configuration["Services:BookingService"];
                var endpoint = $"{bookingServiceUrl}/api/bookings";
                
                var booking = await _httpService.PostAsync<Booking>(endpoint, bookingInput, token);
                
                if (booking != null)
                {
                    await LoadRelatedData(booking);
                }
                
                return booking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return null;
            }
        }

        public async Task<Booking?> UpdateBooking(Guid id, Booking bookingInput, string? token = null)
        {
            try
            {
                var bookingServiceUrl = _configuration["Services:BookingService"];
                var endpoint = $"{bookingServiceUrl}/api/bookings/{id}";
                
                var booking = await _httpService.PutAsync<Booking>(endpoint, bookingInput, token);
                
                if (booking != null)
                {
                    await LoadRelatedData(booking);
                }
                
                return booking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking {BookingId}", id);
                return null;
            }
        }

        public async Task<Booking?> UpdateBookingStatus(Guid id, BookingStatus status, string? token = null)
        {
            try
            {
                var bookingServiceUrl = _configuration["Services:BookingService"];
                var endpoint = $"{bookingServiceUrl}/api/bookings/{id}/status";
                
                var statusUpdate = new { Status = status };
                var booking = await _httpService.PutAsync<Booking>(endpoint, statusUpdate, token);
                
                if (booking != null)
                {
                    await LoadRelatedData(booking);
                }
                
                return booking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status {BookingId}", id);
                return null;
            }
        }

        public async Task<Booking?> CancelBooking(Guid id, string reason, string? token = null)
        {
            try
            {
                var bookingServiceUrl = _configuration["Services:BookingService"];
                var endpoint = $"{bookingServiceUrl}/api/bookings/{id}/cancel";
                
                var cancellationRequest = new { Reason = reason };
                var booking = await _httpService.PostAsync<Booking>(endpoint, cancellationRequest, token);
                
                if (booking != null)
                {
                    await LoadRelatedData(booking);
                }
                
                return booking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
                return null;
            }
        }

        public async Task<bool> CheckIn(Guid id, string? token = null)
        {
            try
            {
                var bookingServiceUrl = _configuration["Services:BookingService"];
                var endpoint = $"{bookingServiceUrl}/api/bookings/{id}/checkin";
                
                var result = await _httpService.PostAsync<dynamic>(endpoint, new { }, token);
                return result != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking in booking {BookingId}", id);
                return false;
            }
        }

        public async Task<bool> CheckOut(Guid id, string? token = null)
        {
            try
            {
                var bookingServiceUrl = _configuration["Services:BookingService"];
                var endpoint = $"{bookingServiceUrl}/api/bookings/{id}/checkout";
                
                var result = await _httpService.PostAsync<dynamic>(endpoint, new { }, token);
                return result != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking out booking {BookingId}", id);
                return false;
            }
        }

        private async Task LoadRelatedData(Booking booking)
        {
            try
            {
                // Load property, guest, and host information
                var loadTasks = new List<Task>
                {
                    Task.Run(async () => booking.Property = await _propertyResolver.GetPropertyById(booking.PropertyId) ?? new Property()),
                    Task.Run(async () => booking.Guest = await _userResolver.GetUserById(booking.GuestId) ?? new User()),
                    Task.Run(async () => booking.Host = await _userResolver.GetUserById(booking.HostId) ?? new User())
                };

                await Task.WhenAll(loadTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading related data for booking {BookingId}", booking.Id);
            }
        }

        private string BuildQueryString(BookingFilterInputType? filter, int skip, int take)
        {
            var queryParams = new List<string> { $"skip={skip}", $"take={take}" };
            
            if (filter != null)
            {
                if (filter.GuestId.HasValue)
                    queryParams.Add($"guestId={filter.GuestId}");
                
                if (filter.HostId.HasValue)
                    queryParams.Add($"hostId={filter.HostId}");
                
                if (filter.PropertyId.HasValue)
                    queryParams.Add($"propertyId={filter.PropertyId}");
                
                if (filter.Status.HasValue)
                    queryParams.Add($"status={filter.Status}");
                
                if (filter.CheckInDateFrom.HasValue)
                    queryParams.Add($"checkInDateFrom={filter.CheckInDateFrom.Value:yyyy-MM-dd}");
                
                if (filter.CheckInDateTo.HasValue)
                    queryParams.Add($"checkInDateTo={filter.CheckInDateTo.Value:yyyy-MM-dd}");
                
                if (filter.CheckOutDateFrom.HasValue)
                    queryParams.Add($"checkOutDateFrom={filter.CheckOutDateFrom.Value:yyyy-MM-dd}");
                
                if (filter.CheckOutDateTo.HasValue)
                    queryParams.Add($"checkOutDateTo={filter.CheckOutDateTo.Value:yyyy-MM-dd}");
                
                if (filter.CreatedFrom.HasValue)
                    queryParams.Add($"createdFrom={filter.CreatedFrom.Value:yyyy-MM-dd}");
                
                if (filter.CreatedTo.HasValue)
                    queryParams.Add($"createdTo={filter.CreatedTo.Value:yyyy-MM-dd}");
            }
            
            return queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        }
    }

    public class BookingFilterInputType
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