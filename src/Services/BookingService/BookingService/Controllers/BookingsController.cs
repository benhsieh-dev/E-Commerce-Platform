using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingService.Data;
using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly BookingDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public BookingsController(BookingDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<BookingListResponseDto>> GetBookings(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? propertyId = null,
            [FromQuery] Guid? guestId = null,
            [FromQuery] Guid? hostId = null,
            [FromQuery] BookingStatus? status = null,
            [FromQuery] PaymentStatus? paymentStatus = null,
            [FromQuery] DateTime? checkInFrom = null,
            [FromQuery] DateTime? checkInTo = null)
        {
            var query = _context.Bookings.Include(b => b.StatusHistory).AsQueryable();

            if (propertyId.HasValue)
                query = query.Where(b => b.PropertyId == propertyId.Value);

            if (guestId.HasValue)
                query = query.Where(b => b.GuestId == guestId.Value);

            if (hostId.HasValue)
                query = query.Where(b => b.HostId == hostId.Value);

            if (status.HasValue)
                query = query.Where(b => b.Status == status.Value);

            if (paymentStatus.HasValue)
                query = query.Where(b => b.PaymentStatus == paymentStatus.Value);

            if (checkInFrom.HasValue)
                query = query.Where(b => b.CheckInDate >= checkInFrom.Value);

            if (checkInTo.HasValue)
                query = query.Where(b => b.CheckInDate <= checkInTo.Value);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var bookings = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingResponseDto
                {
                    Id = b.Id,
                    PropertyId = b.PropertyId,
                    GuestId = b.GuestId,
                    HostId = b.HostId,
                    CheckInDate = b.CheckInDate,
                    CheckOutDate = b.CheckOutDate,
                    NumberOfGuests = b.NumberOfGuests,
                    TotalPrice = b.TotalPrice,
                    ServiceFee = b.ServiceFee,
                    CleaningFee = b.CleaningFee,
                    TaxAmount = b.TaxAmount,
                    FinalAmount = b.FinalAmount,
                    Status = b.Status,
                    PaymentStatus = b.PaymentStatus,
                    SpecialRequests = b.SpecialRequests,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    ConfirmedAt = b.ConfirmedAt,
                    CancelledAt = b.CancelledAt,
                    CancellationReason = b.CancellationReason,
                    CancellationPolicy = b.CancellationPolicy,
                    Nights = b.Nights,
                    PropertyTitle = b.PropertyTitle,
                    PropertyAddress = b.PropertyAddress,
                    PropertyCity = b.PropertyCity,
                    PropertyCountry = b.PropertyCountry,
                    StatusHistory = b.StatusHistory.Select(h => new BookingStatusHistoryDto
                    {
                        Id = h.Id,
                        FromStatus = h.FromStatus,
                        ToStatus = h.ToStatus,
                        Reason = h.Reason,
                        CreatedAt = h.CreatedAt
                    }).OrderBy(h => h.CreatedAt).ToList()
                })
                .ToListAsync();

            return Ok(new BookingListResponseDto
            {
                Bookings = bookings,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingResponseDto>> GetBooking(Guid id)
        {
            var booking = await _context.Bookings
                .Include(b => b.StatusHistory)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            return Ok(new BookingResponseDto
            {
                Id = booking.Id,
                PropertyId = booking.PropertyId,
                GuestId = booking.GuestId,
                HostId = booking.HostId,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                NumberOfGuests = booking.NumberOfGuests,
                TotalPrice = booking.TotalPrice,
                ServiceFee = booking.ServiceFee,
                CleaningFee = booking.CleaningFee,
                TaxAmount = booking.TaxAmount,
                FinalAmount = booking.FinalAmount,
                Status = booking.Status,
                PaymentStatus = booking.PaymentStatus,
                SpecialRequests = booking.SpecialRequests,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt,
                ConfirmedAt = booking.ConfirmedAt,
                CancelledAt = booking.CancelledAt,
                CancellationReason = booking.CancellationReason,
                CancellationPolicy = booking.CancellationPolicy,
                Nights = booking.Nights,
                PropertyTitle = booking.PropertyTitle,
                PropertyAddress = booking.PropertyAddress,
                PropertyCity = booking.PropertyCity,
                PropertyCountry = booking.PropertyCountry,
                StatusHistory = booking.StatusHistory.Select(h => new BookingStatusHistoryDto
                {
                    Id = h.Id,
                    FromStatus = h.FromStatus,
                    ToStatus = h.ToStatus,
                    Reason = h.Reason,
                    CreatedAt = h.CreatedAt
                }).OrderBy(h => h.CreatedAt).ToList()
            });
        }

        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking(CreateBookingDto createBookingDto)
        {
            // Validate dates
            if (createBookingDto.CheckInDate.Date <= DateTime.Today)
                return BadRequest("Check-in date must be in the future.");

            if (createBookingDto.CheckOutDate.Date <= createBookingDto.CheckInDate.Date)
                return BadRequest("Check-out date must be after check-in date.");

            // Check property availability and get details
            var propertyClient = _httpClientFactory.CreateClient();
            var propertyResponse = await propertyClient.GetAsync($"http://localhost:5002/api/properties/{createBookingDto.PropertyId}");
            
            if (!propertyResponse.IsSuccessStatusCode)
                return BadRequest("Property not found or not available.");

            // Check for overlapping bookings
            var hasOverlap = await _context.Bookings.AnyAsync(b =>
                b.PropertyId == createBookingDto.PropertyId &&
                b.Status != BookingStatus.Cancelled &&
                b.Status != BookingStatus.Declined &&
                ((createBookingDto.CheckInDate >= b.CheckInDate && createBookingDto.CheckInDate < b.CheckOutDate) ||
                 (createBookingDto.CheckOutDate > b.CheckInDate && createBookingDto.CheckOutDate <= b.CheckOutDate) ||
                 (createBookingDto.CheckInDate <= b.CheckInDate && createBookingDto.CheckOutDate >= b.CheckOutDate)));

            if (hasOverlap)
                return BadRequest("Property is not available for the selected dates.");

            // Calculate pricing (simplified for now)
            var nights = (createBookingDto.CheckOutDate.Date - createBookingDto.CheckInDate.Date).Days;
            var pricePerNight = 100m; // This should come from PropertyService
            var totalPrice = nights * pricePerNight;
            var serviceFee = totalPrice * 0.1m; // 10% service fee
            var cleaningFee = 50m;
            var taxAmount = (totalPrice + serviceFee + cleaningFee) * 0.08m; // 8% tax

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                PropertyId = createBookingDto.PropertyId,
                GuestId = createBookingDto.GuestId,
                HostId = Guid.NewGuid(), // This should come from PropertyService
                CheckInDate = createBookingDto.CheckInDate,
                CheckOutDate = createBookingDto.CheckOutDate,
                NumberOfGuests = createBookingDto.NumberOfGuests,
                TotalPrice = totalPrice,
                ServiceFee = serviceFee,
                CleaningFee = cleaningFee,
                TaxAmount = taxAmount,
                Status = BookingStatus.Pending,
                PaymentStatus = PaymentStatus.Pending,
                SpecialRequests = createBookingDto.SpecialRequests,
                CancellationPolicy = createBookingDto.CancellationPolicy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PropertyTitle = "Sample Property", // Should come from PropertyService
                PropertyAddress = "Sample Address",
                PropertyCity = "Sample City",
                PropertyCountry = "Sample Country"
            };

            // Add initial status history
            var statusHistory = new BookingStatusHistory
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                FromStatus = BookingStatus.Pending,
                ToStatus = BookingStatus.Pending,
                Reason = "Booking created",
                CreatedAt = DateTime.UtcNow
            };

            booking.StatusHistory.Add(statusHistory);
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, new BookingResponseDto
            {
                Id = booking.Id,
                PropertyId = booking.PropertyId,
                GuestId = booking.GuestId,
                HostId = booking.HostId,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                NumberOfGuests = booking.NumberOfGuests,
                TotalPrice = booking.TotalPrice,
                ServiceFee = booking.ServiceFee,
                CleaningFee = booking.CleaningFee,
                TaxAmount = booking.TaxAmount,
                FinalAmount = booking.FinalAmount,
                Status = booking.Status,
                PaymentStatus = booking.PaymentStatus,
                SpecialRequests = booking.SpecialRequests,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt,
                CancellationPolicy = booking.CancellationPolicy,
                Nights = booking.Nights,
                PropertyTitle = booking.PropertyTitle,
                PropertyAddress = booking.PropertyAddress,
                PropertyCity = booking.PropertyCity,
                PropertyCountry = booking.PropertyCountry,
                StatusHistory = new List<BookingStatusHistoryDto>
                {
                    new BookingStatusHistoryDto
                    {
                        Id = statusHistory.Id,
                        FromStatus = statusHistory.FromStatus,
                        ToStatus = statusHistory.ToStatus,
                        Reason = statusHistory.Reason,
                        CreatedAt = statusHistory.CreatedAt
                    }
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(Guid id, UpdateBookingDto updateBookingDto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            if (booking.Status != BookingStatus.Pending)
                return BadRequest("Only pending bookings can be updated.");

            if (updateBookingDto.CheckInDate.HasValue)
            {
                if (updateBookingDto.CheckInDate.Value.Date <= DateTime.Today)
                    return BadRequest("Check-in date must be in the future.");
                booking.CheckInDate = updateBookingDto.CheckInDate.Value;
            }

            if (updateBookingDto.CheckOutDate.HasValue)
            {
                if (updateBookingDto.CheckOutDate.Value.Date <= booking.CheckInDate.Date)
                    return BadRequest("Check-out date must be after check-in date.");
                booking.CheckOutDate = updateBookingDto.CheckOutDate.Value;
            }

            if (updateBookingDto.NumberOfGuests.HasValue)
                booking.NumberOfGuests = updateBookingDto.NumberOfGuests.Value;

            if (!string.IsNullOrEmpty(updateBookingDto.SpecialRequests))
                booking.SpecialRequests = updateBookingDto.SpecialRequests;

            booking.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmBooking(Guid id, ConfirmBookingDto confirmBookingDto)
        {
            var booking = await _context.Bookings.Include(b => b.StatusHistory).FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
                return NotFound();

            if (booking.Status != BookingStatus.Pending)
                return BadRequest("Only pending bookings can be confirmed.");

            await UpdateBookingStatus(booking, BookingStatus.Confirmed, confirmBookingDto.Message ?? "Booking confirmed by host");
            return NoContent();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(Guid id, CancelBookingDto cancelBookingDto)
        {
            var booking = await _context.Bookings.Include(b => b.StatusHistory).FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
                return NotFound();

            if (booking.Status == BookingStatus.Cancelled || booking.Status == BookingStatus.Completed)
                return BadRequest("Booking cannot be cancelled.");

            booking.CancellationReason = cancelBookingDto.Reason;
            booking.CancelledAt = DateTime.UtcNow;

            await UpdateBookingStatus(booking, BookingStatus.Cancelled, cancelBookingDto.Reason);
            return NoContent();
        }

        [HttpPost("{id}/checkin")]
        public async Task<IActionResult> CheckIn(Guid id, CheckInDto checkInDto)
        {
            var booking = await _context.Bookings.Include(b => b.StatusHistory).FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
                return NotFound();

            if (booking.Status != BookingStatus.Confirmed)
                return BadRequest("Only confirmed bookings can be checked in.");

            if (DateTime.Today < booking.CheckInDate.Date)
                return BadRequest("Cannot check in before the check-in date.");

            await UpdateBookingStatus(booking, BookingStatus.CheckedIn, checkInDto.Notes ?? "Guest checked in");
            return NoContent();
        }

        [HttpPost("{id}/checkout")]
        public async Task<IActionResult> CheckOut(Guid id, CheckOutDto checkOutDto)
        {
            var booking = await _context.Bookings.Include(b => b.StatusHistory).FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
                return NotFound();

            if (booking.Status != BookingStatus.CheckedIn)
                return BadRequest("Only checked-in bookings can be checked out.");

            await UpdateBookingStatus(booking, BookingStatus.CheckedOut, checkOutDto.Notes ?? "Guest checked out");
            
            // Automatically complete the booking after checkout
            await UpdateBookingStatus(booking, BookingStatus.Completed, "Booking completed");
            
            return NoContent();
        }

        [HttpPost("calculate-price")]
        public async Task<ActionResult<BookingPriceResponseDto>> CalculatePrice(BookingPriceCalculationDto calculationDto)
        {
            var nights = (calculationDto.CheckOutDate.Date - calculationDto.CheckInDate.Date).Days;
            if (nights <= 0)
                return BadRequest("Invalid date range.");

            // This should integrate with PropertyService to get actual pricing
            var pricePerNight = 100m; // Placeholder
            var basePrice = nights * pricePerNight;
            var serviceFee = basePrice * 0.1m;
            var cleaningFee = 50m;
            var taxAmount = (basePrice + serviceFee + cleaningFee) * 0.08m;

            return Ok(new BookingPriceResponseDto
            {
                BasePrice = basePrice,
                ServiceFee = serviceFee,
                CleaningFee = cleaningFee,
                TaxAmount = taxAmount,
                TotalPrice = basePrice + serviceFee + cleaningFee + taxAmount,
                Nights = nights,
                PricePerNight = pricePerNight
            });
        }

        private async Task UpdateBookingStatus(Booking booking, BookingStatus newStatus, string reason)
        {
            var oldStatus = booking.Status;
            booking.Status = newStatus;
            booking.UpdatedAt = DateTime.UtcNow;

            var statusHistory = new BookingStatusHistory
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                FromStatus = oldStatus,
                ToStatus = newStatus,
                Reason = reason,
                CreatedAt = DateTime.UtcNow
            };

            _context.BookingStatusHistories.Add(statusHistory);
            await _context.SaveChangesAsync();
        }
    }
}