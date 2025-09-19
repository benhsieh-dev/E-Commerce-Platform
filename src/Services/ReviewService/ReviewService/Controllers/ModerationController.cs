using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReviewService.Data;
using ReviewService.DTOs;
using ReviewService.Models;

namespace ReviewService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModerationController : ControllerBase
    {
        private readonly ReviewDbContext _context;
        private readonly ILogger<ModerationController> _logger;

        public ModerationController(ReviewDbContext context, ILogger<ModerationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("pending-reviews")]
        public async Task<ActionResult<ReviewListResponseDto>> GetPendingReviews(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Reviews
                .Include(r => r.Flags)
                .Where(r => r.Status == ReviewStatus.Pending)
                .OrderBy(r => r.CreatedAt);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var reviews = await query
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
                    Status = r.Status,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    PropertyTitle = r.PropertyTitle,
                    ReviewerName = r.ReviewerName,
                    RevieweeName = r.RevieweeName,
                    FlagCount = r.Flags.Count(f => f.Status == FlagStatus.Pending)
                })
                .ToListAsync();

            return Ok(new ReviewListResponseDto
            {
                Reviews = reviews,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            });
        }

        [HttpPost("reviews/{id}/moderate")]
        public async Task<IActionResult> ModerateReview(Guid id, ModerateReviewDto moderateDto)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return NotFound();

            review.Status = moderateDto.Status;
            review.ModerationNotes = moderateDto.ModerationNotes;
            review.UpdatedAt = DateTime.UtcNow;

            if (moderateDto.Status == ReviewStatus.Published)
            {
                review.PublishedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Review {ReviewId} moderated with status {Status}", id, moderateDto.Status);

            return NoContent();
        }

        [HttpGet("flagged-reviews")]
        public async Task<ActionResult<List<ReviewResponseDto>>> GetFlaggedReviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Flags)
                .Where(r => r.Flags.Any(f => f.Status == FlagStatus.Pending))
                .OrderByDescending(r => r.Flags.Max(f => f.CreatedAt))
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
                    Status = r.Status,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    PropertyTitle = r.PropertyTitle,
                    ReviewerName = r.ReviewerName,
                    RevieweeName = r.RevieweeName,
                    FlagCount = r.Flags.Count(f => f.Status == FlagStatus.Pending)
                })
                .ToListAsync();

            return Ok(reviews);
        }

        [HttpGet("flags/{reviewId}")]
        public async Task<ActionResult<List<ReviewFlagResponseDto>>> GetReviewFlags(Guid reviewId)
        {
            var flags = await _context.ReviewFlags
                .Where(f => f.ReviewId == reviewId)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new ReviewFlagResponseDto
                {
                    Id = f.Id,
                    ReviewId = f.ReviewId,
                    FlaggerId = f.FlaggerId,
                    Reason = f.Reason,
                    Description = f.Description,
                    Status = f.Status,
                    CreatedAt = f.CreatedAt,
                    ResolvedAt = f.ResolvedAt,
                    Resolution = f.Resolution
                })
                .ToListAsync();

            return Ok(flags);
        }

        [HttpPost("flags/{flagId}/resolve")]
        public async Task<IActionResult> ResolveFlag(Guid flagId, ResolveFlagDto resolveDto)
        {
            var flag = await _context.ReviewFlags.FindAsync(flagId);
            if (flag == null)
                return NotFound();

            flag.Status = resolveDto.Status;
            flag.Resolution = resolveDto.Resolution;
            flag.ResolvedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Flag {FlagId} resolved with status {Status}", flagId, resolveDto.Status);

            return NoContent();
        }

        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetModerationStats()
        {
            var stats = new
            {
                PendingReviews = await _context.Reviews.CountAsync(r => r.Status == ReviewStatus.Pending),
                PublishedReviews = await _context.Reviews.CountAsync(r => r.Status == ReviewStatus.Published),
                RejectedReviews = await _context.Reviews.CountAsync(r => r.Status == ReviewStatus.Rejected),
                HiddenReviews = await _context.Reviews.CountAsync(r => r.Status == ReviewStatus.Hidden),
                PendingFlags = await _context.ReviewFlags.CountAsync(f => f.Status == FlagStatus.Pending),
                ResolvedFlags = await _context.ReviewFlags.CountAsync(f => f.Status == FlagStatus.Resolved),
                DismissedFlags = await _context.ReviewFlags.CountAsync(f => f.Status == FlagStatus.Dismissed),
                TotalReviews = await _context.Reviews.CountAsync(),
                TotalFlags = await _context.ReviewFlags.CountAsync()
            };

            return Ok(stats);
        }

        [HttpPost("auto-moderate")]
        public async Task<ActionResult<object>> AutoModerate()
        {
            var pendingReviews = await _context.Reviews
                .Where(r => r.Status == ReviewStatus.Pending)
                .ToListAsync();

            int approvedCount = 0;
            int rejectedCount = 0;

            foreach (var review in pendingReviews)
            {
                // Simple auto-moderation rules
                bool shouldApprove = true;

                // Check for inappropriate content (basic)
                var inappropriateWords = new[] { "spam", "fake", "terrible", "awful" };
                if (inappropriateWords.Any(word => review.ReviewText.ToLower().Contains(word)))
                {
                    shouldApprove = false;
                }

                // Check review length
                if (review.ReviewText.Length < 10)
                {
                    shouldApprove = false;
                }

                // Update status
                if (shouldApprove)
                {
                    review.Status = ReviewStatus.Published;
                    review.PublishedAt = DateTime.UtcNow;
                    review.ModerationNotes = "Auto-approved by system";
                    approvedCount++;
                }
                else
                {
                    review.Status = ReviewStatus.Rejected;
                    review.ModerationNotes = "Auto-rejected: Content policy violation";
                    rejectedCount++;
                }

                review.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Auto-moderation completed: {ApprovedCount} approved, {RejectedCount} rejected", 
                approvedCount, rejectedCount);

            return Ok(new { ApprovedCount = approvedCount, RejectedCount = rejectedCount });
        }
    }
}