using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyService.Data;
using PropertyService.DTOs;
using PropertyService.Models;

namespace PropertyService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly PropertyDbContext _context;

        public PropertiesController(PropertyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PropertyListResponseDto>> GetProperties(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? location = null,
            [FromQuery] PropertyType? propertyType = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int? minGuests = null,
            [FromQuery] bool? instantBook = null,
            [FromQuery] bool activeOnly = true)
        {
            var query = _context.Properties.AsQueryable();

            if (activeOnly)
                query = query.Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(location))
                query = query.Where(p => p.City.Contains(location) || p.Country.Contains(location) || p.Address.Contains(location));

            if (propertyType.HasValue)
                query = query.Where(p => p.PropertyType == propertyType.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.PricePerNight >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.PricePerNight <= maxPrice.Value);

            if (minGuests.HasValue)
                query = query.Where(p => p.MaxGuests >= minGuests.Value);

            if (instantBook.HasValue)
                query = query.Where(p => p.InstantBook == instantBook.Value);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var properties = await query
                .OrderBy(p => p.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PropertyResponseDto
                {
                    Id = p.Id,
                    HostId = p.HostId,
                    Title = p.Title,
                    Description = p.Description,
                    Address = p.Address,
                    City = p.City,
                    Country = p.Country,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    PropertyType = p.PropertyType,
                    MaxGuests = p.MaxGuests,
                    Bedrooms = p.Bedrooms,
                    Bathrooms = p.Bathrooms,
                    PricePerNight = p.PricePerNight,
                    WeeklyDiscount = p.WeeklyDiscount,
                    MonthlyDiscount = p.MonthlyDiscount,
                    Images = p.Images,
                    Amenities = p.Amenities,
                    IsActive = p.IsActive,
                    InstantBook = p.InstantBook,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    UnavailableDates = p.UnavailableDates,
                    AverageRating = p.AverageRating,
                    ReviewCount = p.ReviewCount,
                    MinNights = p.MinNights,
                    MaxNights = p.MaxNights
                })
                .ToListAsync();

            return Ok(new PropertyListResponseDto
            {
                Properties = properties,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyResponseDto>> GetProperty(Guid id)
        {
            var property = await _context.Properties.FindAsync(id);

            if (property == null)
                return NotFound();

            return Ok(new PropertyResponseDto
            {
                Id = property.Id,
                HostId = property.HostId,
                Title = property.Title,
                Description = property.Description,
                Address = property.Address,
                City = property.City,
                Country = property.Country,
                Latitude = property.Latitude,
                Longitude = property.Longitude,
                PropertyType = property.PropertyType,
                MaxGuests = property.MaxGuests,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                PricePerNight = property.PricePerNight,
                WeeklyDiscount = property.WeeklyDiscount,
                MonthlyDiscount = property.MonthlyDiscount,
                Images = property.Images,
                Amenities = property.Amenities,
                IsActive = property.IsActive,
                InstantBook = property.InstantBook,
                CreatedAt = property.CreatedAt,
                UpdatedAt = property.UpdatedAt,
                UnavailableDates = property.UnavailableDates,
                AverageRating = property.AverageRating,
                ReviewCount = property.ReviewCount,
                MinNights = property.MinNights,
                MaxNights = property.MaxNights
            });
        }

        [HttpPost]
        public async Task<ActionResult<PropertyResponseDto>> CreateProperty(CreatePropertyDto createPropertyDto)
        {
            var property = new Property
            {
                Id = Guid.NewGuid(),
                HostId = createPropertyDto.HostId,
                Title = createPropertyDto.Title,
                Description = createPropertyDto.Description,
                Address = createPropertyDto.Address,
                City = createPropertyDto.City,
                Country = createPropertyDto.Country,
                Latitude = createPropertyDto.Latitude,
                Longitude = createPropertyDto.Longitude,
                PropertyType = createPropertyDto.PropertyType,
                MaxGuests = createPropertyDto.MaxGuests,
                Bedrooms = createPropertyDto.Bedrooms,
                Bathrooms = createPropertyDto.Bathrooms,
                PricePerNight = createPropertyDto.PricePerNight,
                WeeklyDiscount = createPropertyDto.WeeklyDiscount,
                MonthlyDiscount = createPropertyDto.MonthlyDiscount,
                Images = createPropertyDto.Images,
                Amenities = createPropertyDto.Amenities,
                InstantBook = createPropertyDto.InstantBook,
                MinNights = createPropertyDto.MinNights,
                MaxNights = createPropertyDto.MaxNights,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProperty), new { id = property.Id }, new PropertyResponseDto
            {
                Id = property.Id,
                HostId = property.HostId,
                Title = property.Title,
                Description = property.Description,
                Address = property.Address,
                City = property.City,
                Country = property.Country,
                Latitude = property.Latitude,
                Longitude = property.Longitude,
                PropertyType = property.PropertyType,
                MaxGuests = property.MaxGuests,
                Bedrooms = property.Bedrooms,
                Bathrooms = property.Bathrooms,
                PricePerNight = property.PricePerNight,
                WeeklyDiscount = property.WeeklyDiscount,
                MonthlyDiscount = property.MonthlyDiscount,
                Images = property.Images,
                Amenities = property.Amenities,
                IsActive = property.IsActive,
                InstantBook = property.InstantBook,
                CreatedAt = property.CreatedAt,
                UpdatedAt = property.UpdatedAt,
                UnavailableDates = property.UnavailableDates,
                AverageRating = property.AverageRating,
                ReviewCount = property.ReviewCount,
                MinNights = property.MinNights,
                MaxNights = property.MaxNights
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(Guid id, UpdatePropertyDto updatePropertyDto)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound();

            if (!string.IsNullOrEmpty(updatePropertyDto.Title))
                property.Title = updatePropertyDto.Title;
            
            if (!string.IsNullOrEmpty(updatePropertyDto.Description))
                property.Description = updatePropertyDto.Description;
            
            if (!string.IsNullOrEmpty(updatePropertyDto.Address))
                property.Address = updatePropertyDto.Address;
            
            if (!string.IsNullOrEmpty(updatePropertyDto.City))
                property.City = updatePropertyDto.City;
            
            if (!string.IsNullOrEmpty(updatePropertyDto.Country))
                property.Country = updatePropertyDto.Country;
            
            if (updatePropertyDto.Latitude.HasValue)
                property.Latitude = updatePropertyDto.Latitude.Value;
            
            if (updatePropertyDto.Longitude.HasValue)
                property.Longitude = updatePropertyDto.Longitude.Value;
            
            if (updatePropertyDto.PropertyType.HasValue)
                property.PropertyType = updatePropertyDto.PropertyType.Value;
            
            if (updatePropertyDto.MaxGuests.HasValue)
                property.MaxGuests = updatePropertyDto.MaxGuests.Value;
            
            if (updatePropertyDto.Bedrooms.HasValue)
                property.Bedrooms = updatePropertyDto.Bedrooms.Value;
            
            if (updatePropertyDto.Bathrooms.HasValue)
                property.Bathrooms = updatePropertyDto.Bathrooms.Value;
            
            if (updatePropertyDto.PricePerNight.HasValue)
                property.PricePerNight = updatePropertyDto.PricePerNight.Value;
            
            if (updatePropertyDto.WeeklyDiscount.HasValue)
                property.WeeklyDiscount = updatePropertyDto.WeeklyDiscount.Value;
            
            if (updatePropertyDto.MonthlyDiscount.HasValue)
                property.MonthlyDiscount = updatePropertyDto.MonthlyDiscount.Value;
            
            if (updatePropertyDto.Images != null)
                property.Images = updatePropertyDto.Images;
            
            if (updatePropertyDto.Amenities != null)
                property.Amenities = updatePropertyDto.Amenities;
            
            if (updatePropertyDto.IsActive.HasValue)
                property.IsActive = updatePropertyDto.IsActive.Value;
            
            if (updatePropertyDto.InstantBook.HasValue)
                property.InstantBook = updatePropertyDto.InstantBook.Value;
            
            if (updatePropertyDto.MinNights.HasValue)
                property.MinNights = updatePropertyDto.MinNights.Value;
            
            if (updatePropertyDto.MaxNights.HasValue)
                property.MaxNights = updatePropertyDto.MaxNights.Value;

            property.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(Guid id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound();

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/check-availability")]
        public async Task<ActionResult<AvailabilityResponseDto>> CheckAvailability(Guid id, CheckAvailabilityDto checkAvailabilityDto)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound("Property not found.");

            if (!property.IsActive)
                return BadRequest("Property is not available for booking.");

            var checkIn = checkAvailabilityDto.CheckIn.Date;
            var checkOut = checkAvailabilityDto.CheckOut.Date;
            var nights = (checkOut - checkIn).Days;

            if (nights < property.MinNights)
                return Ok(new AvailabilityResponseDto
                {
                    IsAvailable = false,
                    Message = $"Minimum stay is {property.MinNights} nights."
                });

            if (nights > property.MaxNights)
                return Ok(new AvailabilityResponseDto
                {
                    IsAvailable = false,
                    Message = $"Maximum stay is {property.MaxNights} nights."
                });

            var isUnavailable = property.UnavailableDates.Any(date =>
                (checkIn >= date.StartDate && checkIn <= date.EndDate) ||
                (checkOut >= date.StartDate && checkOut <= date.EndDate) ||
                (checkIn <= date.StartDate && checkOut >= date.EndDate));

            if (isUnavailable)
                return Ok(new AvailabilityResponseDto
                {
                    IsAvailable = false,
                    Message = "Property is not available for the selected dates."
                });

            var totalPrice = nights * property.PricePerNight;

            if (nights >= 7 && property.WeeklyDiscount.HasValue)
                totalPrice *= (1 - property.WeeklyDiscount.Value / 100);
            else if (nights >= 30 && property.MonthlyDiscount.HasValue)
                totalPrice *= (1 - property.MonthlyDiscount.Value / 100);

            return Ok(new AvailabilityResponseDto
            {
                IsAvailable = true,
                Message = "Property is available for the selected dates.",
                TotalPrice = totalPrice,
                Nights = nights
            });
        }

        [HttpPut("{id}/unavailable-dates")]
        public async Task<IActionResult> SetUnavailableDates(Guid id, SetUnavailableDatesDto setUnavailableDatesDto)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound();

            property.UnavailableDates = setUnavailableDatesDto.UnavailableDates;
            property.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("host/{hostId}")]
        public async Task<ActionResult<List<PropertyResponseDto>>> GetPropertiesByHost(Guid hostId)
        {
            var properties = await _context.Properties
                .Where(p => p.HostId == hostId)
                .Select(p => new PropertyResponseDto
                {
                    Id = p.Id,
                    HostId = p.HostId,
                    Title = p.Title,
                    Description = p.Description,
                    Address = p.Address,
                    City = p.City,
                    Country = p.Country,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    PropertyType = p.PropertyType,
                    MaxGuests = p.MaxGuests,
                    Bedrooms = p.Bedrooms,
                    Bathrooms = p.Bathrooms,
                    PricePerNight = p.PricePerNight,
                    WeeklyDiscount = p.WeeklyDiscount,
                    MonthlyDiscount = p.MonthlyDiscount,
                    Images = p.Images,
                    Amenities = p.Amenities,
                    IsActive = p.IsActive,
                    InstantBook = p.InstantBook,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    UnavailableDates = p.UnavailableDates,
                    AverageRating = p.AverageRating,
                    ReviewCount = p.ReviewCount,
                    MinNights = p.MinNights,
                    MaxNights = p.MaxNights
                })
                .ToListAsync();

            return Ok(properties);
        }
    }
}