using GraphQL.Types;
using ApiGateway.Models;

namespace ApiGateway.GraphQL.Types
{
    public class BookingType : ObjectGraphType<Booking>
    {
        public BookingType()
        {
            Name = "Booking";
            Description = "A booking for a rental property";

            Field(b => b.Id, type: typeof(IdGraphType)).Description("The unique identifier of the booking");
            Field(b => b.PropertyId, type: typeof(IdGraphType)).Description("The unique identifier of the property");
            Field(b => b.GuestId, type: typeof(IdGraphType)).Description("The unique identifier of the guest");
            Field(b => b.HostId, type: typeof(IdGraphType)).Description("The unique identifier of the host");
            Field(b => b.CheckInDate, type: typeof(DateTimeGraphType)).Description("Check-in date");
            Field(b => b.CheckOutDate, type: typeof(DateTimeGraphType)).Description("Check-out date");
            Field(b => b.NumberOfGuests).Description("Number of guests");
            Field(b => b.NumberOfNights).Description("Number of nights");
            Field(b => b.PropertyPricePerNight, type: typeof(DecimalGraphType)).Description("Property price per night");
            Field(b => b.SubTotal, type: typeof(DecimalGraphType)).Description("Subtotal amount");
            Field(b => b.CleaningFee, type: typeof(DecimalGraphType)).Description("Cleaning fee");
            Field(b => b.ServiceFee, type: typeof(DecimalGraphType)).Description("Service fee");
            Field(b => b.TaxAmount, type: typeof(DecimalGraphType)).Description("Tax amount");
            Field(b => b.TotalAmount, type: typeof(DecimalGraphType)).Description("Total amount");
            Field(b => b.Status, type: typeof(BookingStatusType)).Description("Booking status");
            Field(b => b.SpecialRequests, nullable: true).Description("Special requests from guest");
            Field(b => b.CancellationPolicy, type: typeof(CancellationPolicyType)).Description("Cancellation policy");
            Field(b => b.CancelledAt, type: typeof(DateTimeGraphType), nullable: true).Description("When the booking was cancelled");
            Field(b => b.CancellationReason, nullable: true).Description("Reason for cancellation");
            Field(b => b.RefundAmount, type: typeof(DecimalGraphType)).Description("Refund amount");
            Field(b => b.CreatedAt, type: typeof(DateTimeGraphType)).Description("When the booking was created");
            Field(b => b.UpdatedAt, type: typeof(DateTimeGraphType)).Description("When the booking was last updated");
            Field(b => b.CheckedInAt, type: typeof(DateTimeGraphType), nullable: true).Description("When the guest checked in");
            Field(b => b.CheckedOutAt, type: typeof(DateTimeGraphType), nullable: true).Description("When the guest checked out");

            Field<PropertyType>("property", resolve: context => context.Source.Property);
            Field<UserType>("guest", resolve: context => context.Source.Guest);
            Field<UserType>("host", resolve: context => context.Source.Host);
            Field<ListGraphType<PaymentType>>("payments", resolve: context => context.Source.Payments);
            Field<ListGraphType<ReviewType>>("reviews", resolve: context => context.Source.Reviews);
        }
    }

    public class BookingStatusType : EnumerationGraphType<BookingStatus>
    {
        public BookingStatusType()
        {
            Name = "BookingStatus";
            Description = "The status of a booking";
        }
    }

    public class CancellationPolicyType : EnumerationGraphType<CancellationPolicy>
    {
        public CancellationPolicyType()
        {
            Name = "CancellationPolicy";
            Description = "The cancellation policy for a booking";
        }
    }

    public class BookingInputType : InputObjectGraphType<Booking>
    {
        public BookingInputType()
        {
            Name = "BookingInput";
            Description = "Input for creating a booking";

            Field(b => b.PropertyId, type: typeof(IdGraphType)).Description("The unique identifier of the property");
            Field(b => b.GuestId, type: typeof(IdGraphType)).Description("The unique identifier of the guest");
            Field(b => b.CheckInDate, type: typeof(DateTimeGraphType)).Description("Check-in date");
            Field(b => b.CheckOutDate, type: typeof(DateTimeGraphType)).Description("Check-out date");
            Field(b => b.NumberOfGuests).Description("Number of guests");
            Field(b => b.SpecialRequests, nullable: true).Description("Special requests from guest");
        }
    }

    public class BookingFilterInputType : InputObjectGraphType
    {
        public BookingFilterInputType()
        {
            Name = "BookingFilterInput";
            Description = "Filters for searching bookings";

            Field<IdGraphType>("guestId");
            Field<IdGraphType>("hostId");
            Field<IdGraphType>("propertyId");
            Field<BookingStatusType>("status");
            Field<DateTimeGraphType>("checkInDateFrom");
            Field<DateTimeGraphType>("checkInDateTo");
            Field<DateTimeGraphType>("checkOutDateFrom");
            Field<DateTimeGraphType>("checkOutDateTo");
            Field<DateTimeGraphType>("createdFrom");
            Field<DateTimeGraphType>("createdTo");
        }
    }
}