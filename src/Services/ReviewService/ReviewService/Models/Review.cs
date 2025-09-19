using System.ComponentModel.DataAnnotations;

namespace ReviewService.Models
{
    public class Review
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid BookingId { get; set; }
        
        [Required]
        public Guid PropertyId { get; set; }
        
        [Required]
        public Guid ReviewerId { get; set; } // Guest or Host writing the review
        
        [Required]
        public Guid RevieweeId { get; set; } // Host or Guest being reviewed
        
        [Required]
        public ReviewType ReviewType { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int OverallRating { get; set; }
        
        [Required]
        public string ReviewText { get; set; } = string.Empty;
        
        public string? Response { get; set; } // Response from the reviewee
        
        public DateTime? ResponseDate { get; set; }
        
        [Required]
        public ReviewStatus Status { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public DateTime? PublishedAt { get; set; }
        
        public string? ModerationNotes { get; set; }
        
        // Property-specific ratings (for guest reviews of properties)
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
        
        // Cached data for performance
        public string PropertyTitle { get; set; } = string.Empty;
        public string ReviewerName { get; set; } = string.Empty;
        public string RevieweeName { get; set; } = string.Empty;
        
        public List<ReviewHelpfulness> Helpfulness { get; set; } = new();
        public List<ReviewFlag> Flags { get; set; } = new();
    }

    public class ReviewHelpfulness
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid ReviewId { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public bool IsHelpful { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public Review Review { get; set; } = null!;
    }

    public class ReviewFlag
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid ReviewId { get; set; }
        
        [Required]
        public Guid FlaggerId { get; set; }
        
        [Required]
        public FlagReason Reason { get; set; }
        
        public string? Description { get; set; }
        
        [Required]
        public FlagStatus Status { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? ResolvedAt { get; set; }
        
        public string? Resolution { get; set; }
        
        public Review Review { get; set; } = null!;
    }

    public enum ReviewType
    {
        GuestReviewsProperty = 0,   // Guest reviews the property/host
        HostReviewsGuest = 1        // Host reviews the guest
    }

    public enum ReviewStatus
    {
        Draft = 0,          // Review created but not submitted
        Pending = 1,        // Submitted, awaiting moderation
        Published = 2,      // Approved and visible
        Hidden = 3,         // Hidden due to flags/violations
        Rejected = 4        // Rejected by moderation
    }

    public enum FlagReason
    {
        InappropriateContent = 0,
        FakeReview = 1,
        Spam = 2,
        PersonalInfo = 3,
        OffTopic = 4,
        Harassment = 5,
        Other = 6
    }

    public enum FlagStatus
    {
        Pending = 0,
        Reviewed = 1,
        Dismissed = 2,
        Resolved = 3
    }
}