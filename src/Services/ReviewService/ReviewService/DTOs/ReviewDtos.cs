using System.ComponentModel.DataAnnotations;
using ReviewService.Models;

namespace ReviewService.DTOs
{
    public class CreateReviewDto
    {
        [Required]
        public Guid BookingId { get; set; }
        
        [Required]
        public Guid ReviewerId { get; set; }
        
        [Required]
        public ReviewType ReviewType { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int OverallRating { get; set; }
        
        [Required]
        [StringLength(2000, MinimumLength = 10)]
        public string ReviewText { get; set; } = string.Empty;
        
        // Property-specific ratings (for guest reviews)
        [Range(1, 5)]
        public int? CleanlinessRating { get; set; }
        
        [Range(1, 5)]
        public int? AccuracyRating { get; set; }
        
        [Range(1, 5)]
        public int? CheckInRating { get; set; }
        
        [Range(1, 5)]
        public int? CommunicationRating { get; set; }
        
        [Range(1, 5)]
        public int? LocationRating { get; set; }
        
        [Range(1, 5)]
        public int? ValueRating { get; set; }
        
        // Host-specific ratings (for host reviews of guests)
        [Range(1, 5)]
        public int? GuestCommunicationRating { get; set; }
        
        [Range(1, 5)]
        public int? GuestCleanlinessRating { get; set; }
        
        [Range(1, 5)]
        public int? GuestRespectRating { get; set; }
    }

    public class UpdateReviewDto
    {
        [Range(1, 5)]
        public int? OverallRating { get; set; }
        
        [StringLength(2000, MinimumLength = 10)]
        public string? ReviewText { get; set; }
        
        // Property-specific ratings
        [Range(1, 5)]
        public int? CleanlinessRating { get; set; }
        
        [Range(1, 5)]
        public int? AccuracyRating { get; set; }
        
        [Range(1, 5)]
        public int? CheckInRating { get; set; }
        
        [Range(1, 5)]
        public int? CommunicationRating { get; set; }
        
        [Range(1, 5)]
        public int? LocationRating { get; set; }
        
        [Range(1, 5)]
        public int? ValueRating { get; set; }
        
        // Host-specific ratings
        [Range(1, 5)]
        public int? GuestCommunicationRating { get; set; }
        
        [Range(1, 5)]
        public int? GuestCleanlinessRating { get; set; }
        
        [Range(1, 5)]
        public int? GuestRespectRating { get; set; }
    }

    public class ReviewResponseDto
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid ReviewerId { get; set; }
        public Guid RevieweeId { get; set; }
        public ReviewType ReviewType { get; set; }
        public int OverallRating { get; set; }
        public string ReviewText { get; set; } = string.Empty;
        public string? Response { get; set; }
        public DateTime? ResponseDate { get; set; }
        public ReviewStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        
        // Property-specific ratings
        public int? CleanlinessRating { get; set; }
        public int? AccuracyRating { get; set; }
        public int? CheckInRating { get; set; }
        public int? CommunicationRating { get; set; }
        public int? LocationRating { get; set; }
        public int? ValueRating { get; set; }
        
        // Host-specific ratings
        public int? GuestCommunicationRating { get; set; }
        public int? GuestCleanlinessRating { get; set; }
        public int? GuestRespectRating { get; set; }
        
        // Cached data
        public string PropertyTitle { get; set; } = string.Empty;
        public string ReviewerName { get; set; } = string.Empty;
        public string RevieweeName { get; set; } = string.Empty;
        
        // Engagement metrics
        public int HelpfulCount { get; set; }
        public int NotHelpfulCount { get; set; }
        public bool? CurrentUserFoundHelpful { get; set; }
        public int FlagCount { get; set; }
    }

    public class ReviewListResponseDto
    {
        public List<ReviewResponseDto> Reviews { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public ReviewStatsDto Stats { get; set; } = new();
    }

    public class ReviewStatsDto
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
        
        // Property-specific averages
        public double? AverageCleanlinessRating { get; set; }
        public double? AverageAccuracyRating { get; set; }
        public double? AverageCheckInRating { get; set; }
        public double? AverageCommunicationRating { get; set; }
        public double? AverageLocationRating { get; set; }
        public double? AverageValueRating { get; set; }
    }

    public class RespondToReviewDto
    {
        [Required]
        [StringLength(1000, MinimumLength = 10)]
        public string Response { get; set; } = string.Empty;
    }

    public class MarkHelpfulDto
    {
        [Required]
        public bool IsHelpful { get; set; }
    }

    public class FlagReviewDto
    {
        [Required]
        public FlagReason Reason { get; set; }
        
        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class ModerateReviewDto
    {
        [Required]
        public ReviewStatus Status { get; set; }
        
        [StringLength(500)]
        public string? ModerationNotes { get; set; }
    }

    public class ReviewSearchDto
    {
        public Guid? PropertyId { get; set; }
        public Guid? ReviewerId { get; set; }
        public Guid? RevieweeId { get; set; }
        public ReviewType? ReviewType { get; set; }
        public ReviewStatus? Status { get; set; }
        public int? MinRating { get; set; }
        public int? MaxRating { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public string? SearchText { get; set; }
        public bool PublishedOnly { get; set; } = true;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAt";
        public string SortOrder { get; set; } = "Desc";
    }

    public class ReviewFlagResponseDto
    {
        public Guid Id { get; set; }
        public Guid ReviewId { get; set; }
        public Guid FlaggerId { get; set; }
        public FlagReason Reason { get; set; }
        public string? Description { get; set; }
        public FlagStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? Resolution { get; set; }
    }

    public class ResolveFlagDto
    {
        [Required]
        public FlagStatus Status { get; set; }
        
        [StringLength(500)]
        public string? Resolution { get; set; }
    }
}