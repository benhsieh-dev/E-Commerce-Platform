using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReviewService.Data;
using ReviewService.DTOs;
using ReviewService.Models;

namespace ReviewService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(
            ReviewDbContext context, 
            IHttpClientFactory httpClientFactory,
            ILogger<ReviewsController> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ReviewListResponseDto>> GetReviews(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? propertyId = null,
            [FromQuery] Guid? reviewerId = null,
            [FromQuery] Guid? revieweeId = null,
            [FromQuery] ReviewType? reviewType = null,
            [FromQuery] ReviewStatus? status = null,
            [FromQuery] int? minRating = null,
            [FromQuery] int? maxRating = null,
            [FromQuery] string? searchText = null,
            [FromQuery] bool publishedOnly = true)
        {
            var query = _context.Reviews
                .Include(r => r.Helpfulness)
                .Include(r => r.Flags)
                .AsQueryable();

            if (publishedOnly)
                query = query.Where(r => r.Status == ReviewStatus.Published);

            if (propertyId.HasValue)
                query = query.Where(r => r.PropertyId == propertyId.Value);

            if (reviewerId.HasValue)
                query = query.Where(r => r.ReviewerId == reviewerId.Value);

            if (revieweeId.HasValue)
                query = query.Where(r => r.RevieweeId == revieweeId.Value);

            if (reviewType.HasValue)
                query = query.Where(r => r.ReviewType == reviewType.Value);

            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);

            if (minRating.HasValue)
                query = query.Where(r => r.OverallRating >= minRating.Value);

            if (maxRating.HasValue)
                query = query.Where(r => r.OverallRating <= maxRating.Value);

            if (!string.IsNullOrEmpty(searchText))
                query = query.Where(r => r.ReviewText.Contains(searchText) || r.PropertyTitle.Contains(searchText));

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var reviews = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReviewResponseDto
                {
                    Id = r.Id,
                    BookingId = r.BookingId,
                    PropertyId = r.PropertyId,
                    ReviewerId = r.ReviewerId,
                    RevieweeId = r.RevieweeId,
                    ReviewType = r.ReviewType,
                    OverallRating = r.OverallRating,
                    ReviewText = r.ReviewText,
                    Response = r.Response,
                    ResponseDate = r.ResponseDate,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    PublishedAt = r.PublishedAt,
                    CleanlinessRating = r.CleanlinessRating,
                    AccuracyRating = r.AccuracyRating,
                    CheckInRating = r.CheckInRating,
                    CommunicationRating = r.CommunicationRating,
                    LocationRating = r.LocationRating,
                    ValueRating = r.ValueRating,
                    GuestCommunicationRating = r.GuestCommunicationRating,
                    GuestCleanlinessRating = r.GuestCleanlinessRating,
                    GuestRespectRating = r.GuestRespectRating,
                    PropertyTitle = r.PropertyTitle,
                    ReviewerName = r.ReviewerName,
                    RevieweeName = r.RevieweeName,
                    HelpfulCount = r.Helpfulness.Count(h => h.IsHelpful),
                    NotHelpfulCount = r.Helpfulness.Count(h => !h.IsHelpful),
                    FlagCount = r.Flags.Count(f => f.Status == FlagStatus.Pending)
                })
                .ToListAsync();

            // Calculate stats
            var allReviews = await _context.Reviews
                .Where(r => r.Status == ReviewStatus.Published && 
                           (!propertyId.HasValue || r.PropertyId == propertyId.Value))
                .ToListAsync();

            var stats = CalculateReviewStats(allReviews);

            return Ok(new ReviewListResponseDto
            {
                Reviews = reviews,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                Stats = stats
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewResponseDto>> GetReview(Guid id, [FromQuery] Guid? currentUserId = null)
        {
            var review = await _context.Reviews
                .Include(r => r.Helpfulness)
                .Include(r => r.Flags)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return NotFound();

            bool? currentUserFoundHelpful = null;
            if (currentUserId.HasValue)
            {
                var helpfulness = review.Helpfulness.FirstOrDefault(h => h.UserId == currentUserId.Value);
                currentUserFoundHelpful = helpfulness?.IsHelpful;
            }

            return Ok(new ReviewResponseDto
            {
                Id = review.Id,
                BookingId = review.BookingId,
                PropertyId = review.PropertyId,
                ReviewerId = review.ReviewerId,
                RevieweeId = review.RevieweeId,
                ReviewType = review.ReviewType,
                OverallRating = review.OverallRating,
                ReviewText = review.ReviewText,
                Response = review.Response,
                ResponseDate = review.ResponseDate,
                Status = review.Status,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                PublishedAt = review.PublishedAt,
                CleanlinessRating = review.CleanlinessRating,
                AccuracyRating = review.AccuracyRating,
                CheckInRating = review.CheckInRating,
                CommunicationRating = review.CommunicationRating,
                LocationRating = review.LocationRating,
                ValueRating = review.ValueRating,
                GuestCommunicationRating = review.GuestCommunicationRating,
                GuestCleanlinessRating = review.GuestCleanlinessRating,
                GuestRespectRating = review.GuestRespectRating,
                PropertyTitle = review.PropertyTitle,
                ReviewerName = review.ReviewerName,
                RevieweeName = review.RevieweeName,
                HelpfulCount = review.Helpfulness.Count(h => h.IsHelpful),
                NotHelpfulCount = review.Helpfulness.Count(h => !h.IsHelpful),
                CurrentUserFoundHelpful = currentUserFoundHelpful,
                FlagCount = review.Flags.Count(f => f.Status == FlagStatus.Pending)
            });
        }

        [HttpPost]
        public async Task<ActionResult<ReviewResponseDto>> CreateReview(CreateReviewDto createReviewDto)
        {
            // Check if review already exists for this booking and reviewer
            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.BookingId == createReviewDto.BookingId && 
                                         r.ReviewerId == createReviewDto.ReviewerId);

            if (existingReview != null)
                return BadRequest("Review already exists for this booking.");

            // Get booking details to validate and fetch property/host info
            var bookingClient = _httpClientFactory.CreateClient();
            var bookingResponse = await bookingClient.GetAsync($"http://localhost:5003/api/bookings/{createReviewDto.BookingId}");
            
            if (!bookingResponse.IsSuccessStatusCode)
                return BadRequest("Booking not found.");

            // Validate that the booking is completed
            // In a real implementation, parse the booking response and check status

            var review = new Review
            {
                Id = Guid.NewGuid(),
                BookingId = createReviewDto.BookingId,
                PropertyId = Guid.NewGuid(), // Should come from booking
                ReviewerId = createReviewDto.ReviewerId,
                RevieweeId = Guid.NewGuid(), // Should come from booking (host or guest)
                ReviewType = createReviewDto.ReviewType,
                OverallRating = createReviewDto.OverallRating,
                ReviewText = createReviewDto.ReviewText,
                Status = ReviewStatus.Pending, // Auto-moderate in production
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CleanlinessRating = createReviewDto.CleanlinessRating,
                AccuracyRating = createReviewDto.AccuracyRating,
                CheckInRating = createReviewDto.CheckInRating,
                CommunicationRating = createReviewDto.CommunicationRating,
                LocationRating = createReviewDto.LocationRating,
                ValueRating = createReviewDto.ValueRating,
                GuestCommunicationRating = createReviewDto.GuestCommunicationRating,
                GuestCleanlinessRating = createReviewDto.GuestCleanlinessRating,
                GuestRespectRating = createReviewDto.GuestRespectRating,
                PropertyTitle = "Sample Property", // Should come from property service
                ReviewerName = "Sample Reviewer", // Should come from user service
                RevieweeName = "Sample Reviewee" // Should come from user service
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Review {ReviewId} created for booking {BookingId} by user {ReviewerId}", 
                review.Id, review.BookingId, review.ReviewerId);

            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, new ReviewResponseDto
            {
                Id = review.Id,
                BookingId = review.BookingId,
                PropertyId = review.PropertyId,
                ReviewerId = review.ReviewerId,
                RevieweeId = review.RevieweeId,
                ReviewType = review.ReviewType,
                OverallRating = review.OverallRating,
                ReviewText = review.ReviewText,
                Status = review.Status,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                CleanlinessRating = review.CleanlinessRating,
                AccuracyRating = review.AccuracyRating,
                CheckInRating = review.CheckInRating,
                CommunicationRating = review.CommunicationRating,
                LocationRating = review.LocationRating,
                ValueRating = review.ValueRating,
                GuestCommunicationRating = review.GuestCommunicationRating,
                GuestCleanlinessRating = review.GuestCleanlinessRating,
                GuestRespectRating = review.GuestRespectRating,
                PropertyTitle = review.PropertyTitle,
                ReviewerName = review.ReviewerName,
                RevieweeName = review.RevieweeName,
                HelpfulCount = 0,
                NotHelpfulCount = 0,
                FlagCount = 0
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(Guid id, UpdateReviewDto updateReviewDto)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return NotFound();

            if (review.Status != ReviewStatus.Draft && review.Status != ReviewStatus.Pending)
                return BadRequest("Only draft or pending reviews can be updated.");

            if (updateReviewDto.OverallRating.HasValue)
                review.OverallRating = updateReviewDto.OverallRating.Value;

            if (!string.IsNullOrEmpty(updateReviewDto.ReviewText))
                review.ReviewText = updateReviewDto.ReviewText;

            // Update property-specific ratings
            if (updateReviewDto.CleanlinessRating.HasValue)
                review.CleanlinessRating = updateReviewDto.CleanlinessRating.Value;
            
            if (updateReviewDto.AccuracyRating.HasValue)
                review.AccuracyRating = updateReviewDto.AccuracyRating.Value;
            
            if (updateReviewDto.CheckInRating.HasValue)
                review.CheckInRating = updateReviewDto.CheckInRating.Value;
            
            if (updateReviewDto.CommunicationRating.HasValue)
                review.CommunicationRating = updateReviewDto.CommunicationRating.Value;
            
            if (updateReviewDto.LocationRating.HasValue)
                review.LocationRating = updateReviewDto.LocationRating.Value;
            
            if (updateReviewDto.ValueRating.HasValue)
                review.ValueRating = updateReviewDto.ValueRating.Value;

            // Update host-specific ratings
            if (updateReviewDto.GuestCommunicationRating.HasValue)
                review.GuestCommunicationRating = updateReviewDto.GuestCommunicationRating.Value;
            
            if (updateReviewDto.GuestCleanlinessRating.HasValue)
                review.GuestCleanlinessRating = updateReviewDto.GuestCleanlinessRating.Value;
            
            if (updateReviewDto.GuestRespectRating.HasValue)
                review.GuestRespectRating = updateReviewDto.GuestRespectRating.Value;

            review.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/respond")]
        public async Task<IActionResult> RespondToReview(Guid id, RespondToReviewDto respondDto)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return NotFound();

            if (review.Status != ReviewStatus.Published)
                return BadRequest("Only published reviews can be responded to.");

            if (!string.IsNullOrEmpty(review.Response))
                return BadRequest("Review already has a response.");

            review.Response = respondDto.Response;
            review.ResponseDate = DateTime.UtcNow;
            review.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Response added to review {ReviewId}", review.Id);

            return NoContent();
        }

        [HttpPost("{id}/helpful")]
        public async Task<IActionResult> MarkHelpful(Guid id, MarkHelpfulDto helpfulDto, [FromQuery] Guid userId)
        {
            var review = await _context.Reviews
                .Include(r => r.Helpfulness)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return NotFound();

            var existingHelpfulness = review.Helpfulness.FirstOrDefault(h => h.UserId == userId);

            if (existingHelpfulness != null)
            {
                existingHelpfulness.IsHelpful = helpfulDto.IsHelpful;
            }
            else
            {
                var helpfulness = new ReviewHelpfulness
                {
                    Id = Guid.NewGuid(),
                    ReviewId = id,
                    UserId = userId,
                    IsHelpful = helpfulDto.IsHelpful,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ReviewHelpfulness.Add(helpfulness);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/flag")]
        public async Task<IActionResult> FlagReview(Guid id, FlagReviewDto flagDto, [FromQuery] Guid flaggerId)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return NotFound();

            // Check if user already flagged this review
            var existingFlag = await _context.ReviewFlags
                .FirstOrDefaultAsync(f => f.ReviewId == id && f.FlaggerId == flaggerId);

            if (existingFlag != null)
                return BadRequest("You have already flagged this review.");

            var flag = new ReviewFlag
            {
                Id = Guid.NewGuid(),
                ReviewId = id,
                FlaggerId = flaggerId,
                Reason = flagDto.Reason,
                Description = flagDto.Description,
                Status = FlagStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.ReviewFlags.Add(flag);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Review {ReviewId} flagged by user {FlaggerId} for reason {Reason}", 
                id, flaggerId, flagDto.Reason);

            return NoContent();
        }

        [HttpGet("stats/property/{propertyId}")]
        public async Task<ActionResult<ReviewStatsDto>> GetPropertyStats(Guid propertyId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.PropertyId == propertyId && r.Status == ReviewStatus.Published)
                .ToListAsync();

            var stats = CalculateReviewStats(reviews);
            return Ok(stats);
        }

        private static ReviewStatsDto CalculateReviewStats(List<Review> reviews)
        {
            if (!reviews.Any())
            {
                return new ReviewStatsDto
                {
                    RatingDistribution = new Dictionary<int, int> { {1, 0}, {2, 0}, {3, 0}, {4, 0}, {5, 0} }
                };
            }

            var stats = new ReviewStatsDto
            {
                AverageRating = reviews.Average(r => r.OverallRating),
                TotalReviews = reviews.Count,
                RatingDistribution = reviews
                    .GroupBy(r => r.OverallRating)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            // Calculate property-specific averages
            var propertyReviews = reviews.Where(r => r.ReviewType == ReviewType.GuestReviewsProperty).ToList();
            if (propertyReviews.Any())
            {
                stats.AverageCleanlinessRating = propertyReviews.Where(r => r.CleanlinessRating.HasValue).Average(r => r.CleanlinessRating!.Value);
                stats.AverageAccuracyRating = propertyReviews.Where(r => r.AccuracyRating.HasValue).Average(r => r.AccuracyRating!.Value);
                stats.AverageCheckInRating = propertyReviews.Where(r => r.CheckInRating.HasValue).Average(r => r.CheckInRating!.Value);
                stats.AverageCommunicationRating = propertyReviews.Where(r => r.CommunicationRating.HasValue).Average(r => r.CommunicationRating!.Value);
                stats.AverageLocationRating = propertyReviews.Where(r => r.LocationRating.HasValue).Average(r => r.LocationRating!.Value);
                stats.AverageValueRating = propertyReviews.Where(r => r.ValueRating.HasValue).Average(r => r.ValueRating!.Value);
            }

            // Ensure all rating levels are represented
            for (int i = 1; i <= 5; i++)
            {
                if (!stats.RatingDistribution.ContainsKey(i))
                    stats.RatingDistribution[i] = 0;
            }

            return stats;
        }
    }
}