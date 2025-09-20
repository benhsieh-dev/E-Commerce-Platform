using GraphQL.Types;
using ApiGateway.Models;

namespace ApiGateway.GraphQL.Types
{
    public class ReviewType : ObjectGraphType<Review>
    {
        public ReviewType()
        {
            Name = "Review";
            Description = "A review for a property or guest";

            Field(r => r.Id, type: typeof(IdGraphType)).Description("The unique identifier of the review");
            Field(r => r.BookingId, type: typeof(IdGraphType)).Description("The unique identifier of the booking");
            Field(r => r.ReviewerId, type: typeof(IdGraphType)).Description("The unique identifier of the reviewer");
            Field(r => r.RevieweeId, type: typeof(IdGraphType)).Description("The unique identifier of the reviewee");
            Field(r => r.Type, type: typeof(ReviewTypeEnumType)).Description("The type of review");
            Field(r => r.OverallRating).Description("Overall rating (1-5)");
            Field(r => r.CleanlinessRating, nullable: true).Description("Cleanliness rating (1-5)");
            Field(r => r.AccuracyRating, nullable: true).Description("Accuracy rating (1-5)");
            Field(r => r.CheckInRating, nullable: true).Description("Check-in rating (1-5)");
            Field(r => r.CommunicationRating, nullable: true).Description("Communication rating (1-5)");
            Field(r => r.LocationRating, nullable: true).Description("Location rating (1-5)");
            Field(r => r.ValueRating, nullable: true).Description("Value rating (1-5)");
            Field(r => r.GuestCommunicationRating, nullable: true).Description("Guest communication rating (1-5)");
            Field(r => r.GuestCleanlinessRating, nullable: true).Description("Guest cleanliness rating (1-5)");
            Field(r => r.GuestRespectRating, nullable: true).Description("Guest respect rating (1-5)");
            Field(r => r.Comment).Description("Review comment");
            Field(r => r.Status, type: typeof(ReviewStatusType)).Description("Review status");
            Field(r => r.CreatedAt, type: typeof(DateTimeGraphType)).Description("When the review was created");
            Field(r => r.UpdatedAt, type: typeof(DateTimeGraphType)).Description("When the review was last updated");
            Field(r => r.PublishedAt, type: typeof(DateTimeGraphType), nullable: true).Description("When the review was published");
            Field(r => r.IsAnonymous).Description("Whether the review is anonymous");
            Field(r => r.ResponseComment, nullable: true).Description("Response to the review");
            Field(r => r.ResponseDate, type: typeof(DateTimeGraphType), nullable: true).Description("When the response was made");
            Field(r => r.HelpfulVotes).Description("Number of helpful votes");
            Field(r => r.TotalVotes).Description("Total number of votes");
            Field(r => r.IsFlagged).Description("Whether the review is flagged");
            Field(r => r.FlagReason, nullable: true).Description("Reason for flagging");
            Field(r => r.FlaggedAt, type: typeof(DateTimeGraphType), nullable: true).Description("When the review was flagged");
            Field(r => r.FlaggedByUserId, type: typeof(IdGraphType), nullable: true).Description("Who flagged the review");
            Field(r => r.ModerationStatus, type: typeof(ModerationStatusType)).Description("Moderation status");
            Field(r => r.ModerationNotes, nullable: true).Description("Moderation notes");
            Field(r => r.ModeratedAt, type: typeof(DateTimeGraphType), nullable: true).Description("When the review was moderated");
            Field(r => r.ModeratedByUserId, type: typeof(IdGraphType), nullable: true).Description("Who moderated the review");

            Field<BookingType>("booking", resolve: context => context.Source.Booking);
            Field<UserType>("reviewer", resolve: context => context.Source.Reviewer);
            Field<UserType>("reviewee", resolve: context => context.Source.Reviewee);
            Field<PropertyType>("property", resolve: context => context.Source.Property);
        }
    }

    public class ReviewTypeEnumType : EnumerationGraphType<Models.ReviewType>
    {
        public ReviewTypeEnumType()
        {
            Name = "ReviewTypeEnum";
            Description = "The type of review";
        }
    }

    public class ReviewStatusType : EnumerationGraphType<ReviewStatus>
    {
        public ReviewStatusType()
        {
            Name = "ReviewStatus";
            Description = "The status of a review";
        }
    }

    public class ModerationStatusType : EnumerationGraphType<ModerationStatus>
    {
        public ModerationStatusType()
        {
            Name = "ModerationStatus";
            Description = "The moderation status of a review";
        }
    }

    public class ReviewInputType : InputObjectGraphType<Review>
    {
        public ReviewInputType()
        {
            Name = "ReviewInput";
            Description = "Input for creating a review";

            Field(r => r.BookingId, type: typeof(IdGraphType)).Description("The unique identifier of the booking");
            Field(r => r.ReviewerId, type: typeof(IdGraphType)).Description("The unique identifier of the reviewer");
            Field(r => r.RevieweeId, type: typeof(IdGraphType)).Description("The unique identifier of the reviewee");
            Field(r => r.Type, type: typeof(ReviewTypeEnumType)).Description("The type of review");
            Field(r => r.OverallRating).Description("Overall rating (1-5)");
            Field(r => r.CleanlinessRating, nullable: true).Description("Cleanliness rating (1-5)");
            Field(r => r.AccuracyRating, nullable: true).Description("Accuracy rating (1-5)");
            Field(r => r.CheckInRating, nullable: true).Description("Check-in rating (1-5)");
            Field(r => r.CommunicationRating, nullable: true).Description("Communication rating (1-5)");
            Field(r => r.LocationRating, nullable: true).Description("Location rating (1-5)");
            Field(r => r.ValueRating, nullable: true).Description("Value rating (1-5)");
            Field(r => r.GuestCommunicationRating, nullable: true).Description("Guest communication rating (1-5)");
            Field(r => r.GuestCleanlinessRating, nullable: true).Description("Guest cleanliness rating (1-5)");
            Field(r => r.GuestRespectRating, nullable: true).Description("Guest respect rating (1-5)");
            Field(r => r.Comment).Description("Review comment");
            Field(r => r.IsAnonymous).Description("Whether the review is anonymous");
        }
    }

    public class ReviewFilterInputType : InputObjectGraphType
    {
        public ReviewFilterInputType()
        {
            Name = "ReviewFilterInput";
            Description = "Filters for searching reviews";

            Field<IdGraphType>("propertyId");
            Field<IdGraphType>("reviewerId");
            Field<IdGraphType>("revieweeId");
            Field<ReviewTypeEnumType>("type");
            Field<ReviewStatusType>("status");
            Field<IntGraphType>("minRating");
            Field<IntGraphType>("maxRating");
            Field<DateTimeGraphType>("createdFrom");
            Field<DateTimeGraphType>("createdTo");
            Field<BooleanGraphType>("isFlagged");
            Field<ModerationStatusType>("moderationStatus");
        }
    }
}