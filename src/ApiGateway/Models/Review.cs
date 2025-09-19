using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models
{
    public class Review
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid ReviewerId { get; set; }
        public Guid RevieweeId { get; set; }
        public ReviewType Type { get; set; }
        public int OverallRating { get; set; }
        public int? CleanlinessRating { get; set; }
        public int? AccuracyRating { get; set; }
        public int? CheckInRating { get; set; }
        public int? CommunicationRating { get; set; }
        public int? LocationRating { get; set; }
        public int? ValueRating { get; set; }
        public int? GuestCommunicationRating { get; set; }
        public int? GuestCleanlinessRating { get; set; }
        public int? GuestRespectRating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public ReviewStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public bool IsAnonymous { get; set; }
        public string? ResponseComment { get; set; }
        public DateTime? ResponseDate { get; set; }
        public int HelpfulVotes { get; set; }
        public int TotalVotes { get; set; }
        public bool IsFlagged { get; set; }
        public string? FlagReason { get; set; }
        public DateTime? FlaggedAt { get; set; }
        public Guid? FlaggedByUserId { get; set; }
        public ModerationStatus ModerationStatus { get; set; }
        public string? ModerationNotes { get; set; }
        public DateTime? ModeratedAt { get; set; }
        public Guid? ModeratedByUserId { get; set; }
        
        // Navigation properties
        public Booking Booking { get; set; } = null!;
        public User Reviewer { get; set; } = null!;
        public User Reviewee { get; set; } = null!;
        public Property? Property { get; set; }
    }

    public enum ReviewType
    {
        PropertyReview = 0,
        GuestReview = 1
    }

    public enum ReviewStatus
    {
        Draft = 0,
        Published = 1,
        Hidden = 2,
        Removed = 3
    }

    public enum ModerationStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Flagged = 3,
        UnderReview = 4
    }
}