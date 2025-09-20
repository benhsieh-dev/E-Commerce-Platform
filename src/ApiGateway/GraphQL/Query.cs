using GraphQL.Types;
using GraphQL;
using ApiGateway.GraphQL.Types;
using ApiGateway.GraphQL.Resolvers;
using ApiGateway.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ApiGateway.GraphQL
{
    public class Query : ObjectGraphType
    {
        public Query()
        {
            Name = "Query";
            Description = "The root query for the AirBnB platform";

            // User queries
            Field<UserType>("user")
                .Description("Get a user by ID")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The unique identifier of the user")
                .ResolveAsync(async context =>
                {
                    var userResolver = context.RequestServices?.GetService<UserResolver>();
                    var id = context.GetArgument<Guid>("id");
                    return await userResolver!.GetUserById(id);
                });

            Field<ListGraphType<UserType>>("users")
                .Description("Get all users")
                .Argument<IntGraphType>("skip", "Number of users to skip")
                .Argument<IntGraphType>("take", "Number of users to take")
                .ResolveAsync(async context =>
                {
                    var userResolver = context.RequestServices?.GetService<UserResolver>();
                    var skip = context.GetArgument<int>("skip", 0);
                    var take = context.GetArgument<int>("take", 20);
                    return await userResolver!.GetUsers(skip, take);
                });

            Field<UserType>("userByEmail")
                .Description("Get a user by email")
                .Argument<NonNullGraphType<StringGraphType>>("email", "The email of the user")
                .ResolveAsync(async context =>
                {
                    var userResolver = context.RequestServices?.GetService<UserResolver>();
                    var email = context.GetArgument<string>("email");
                    return await userResolver!.GetUserByEmail(email);
                });

            // Property queries
            Field<Types.PropertyType>("property")
                .Description("Get a property by ID")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The unique identifier of the property")
                .ResolveAsync(async context =>
                {
                    var propertyResolver = context.RequestServices?.GetService<PropertyResolver>();
                    var id = context.GetArgument<Guid>("id");
                    return await propertyResolver!.GetPropertyById(id);
                });

            Field<ListGraphType<Types.PropertyType>>("properties")
                .Description("Get all properties")
                .Argument<Types.PropertyFilterInputType>("filter", "Filters for property search")
                .Argument<IntGraphType>("skip", "Number of properties to skip")
                .Argument<IntGraphType>("take", "Number of properties to take")
                .ResolveAsync(async context =>
                {
                    var propertyResolver = context.RequestServices?.GetService<PropertyResolver>();
                    var filterInput = context.GetArgument<IDictionary<string, object?>>("filter");
                    var filter = filterInput != null ? new PropertyFilter
                    {
                        City = filterInput.TryGetValue("city", out var city) ? city?.ToString() : null,
                        State = filterInput.TryGetValue("state", out var state) ? state?.ToString() : null,
                        Country = filterInput.TryGetValue("country", out var country) ? country?.ToString() : null,
                        MinPrice = filterInput.TryGetValue("minPrice", out var minPrice) && minPrice != null ? Convert.ToDecimal(minPrice) : null,
                        MaxPrice = filterInput.TryGetValue("maxPrice", out var maxPrice) && maxPrice != null ? Convert.ToDecimal(maxPrice) : null,
                        MinGuests = filterInput.TryGetValue("minGuests", out var minGuests) && minGuests != null ? Convert.ToInt32(minGuests) : null,
                        MaxGuests = filterInput.TryGetValue("maxGuests", out var maxGuests) && maxGuests != null ? Convert.ToInt32(maxGuests) : null,
                        InstantBookOnly = filterInput.TryGetValue("instantBookOnly", out var instantBook) && instantBook != null ? Convert.ToBoolean(instantBook) : null
                    } : null;
                    var skip = context.GetArgument<int>("skip", 0);
                    var take = context.GetArgument<int>("take", 20);
                    return await propertyResolver!.GetProperties(filter, skip, take);
                });

            Field<ListGraphType<Types.PropertyType>>("propertiesByHost")
                .Description("Get properties by host")
                .Argument<NonNullGraphType<IdGraphType>>("hostId", "The unique identifier of the host")
                .Argument<IntGraphType>("skip", "Number of properties to skip")
                .Argument<IntGraphType>("take", "Number of properties to take")
                .ResolveAsync(async context =>
                {
                    var propertyResolver = context.RequestServices?.GetService<PropertyResolver>();
                    var hostId = context.GetArgument<Guid>("hostId");
                    var skip = context.GetArgument<int>("skip", 0);
                    var take = context.GetArgument<int>("take", 20);
                    return await propertyResolver!.GetPropertiesByHost(hostId, skip, take);
                });

            Field<ListGraphType<Types.PropertyType>>("searchProperties")
                .Description("Search properties")
                .Argument<StringGraphType>("location", "Location to search")
                .Argument<DateTimeGraphType>("checkIn", "Check-in date")
                .Argument<DateTimeGraphType>("checkOut", "Check-out date")
                .Argument<IntGraphType>("guests", "Number of guests")
                .Argument<DecimalGraphType>("minPrice", "Minimum price per night")
                .Argument<DecimalGraphType>("maxPrice", "Maximum price per night")
                .Argument<IntGraphType>("skip", "Number of properties to skip")
                .Argument<IntGraphType>("take", "Number of properties to take")
                .ResolveAsync(async context =>
                {
                    var propertyResolver = context.RequestServices?.GetService<PropertyResolver>();
                    var location = context.GetArgument<string?>("location");
                    var checkIn = context.GetArgument<DateTime?>("checkIn");
                    var checkOut = context.GetArgument<DateTime?>("checkOut");
                    var guests = context.GetArgument<int?>("guests");
                    var minPrice = context.GetArgument<decimal?>("minPrice");
                    var maxPrice = context.GetArgument<decimal?>("maxPrice");
                    var skip = context.GetArgument<int>("skip", 0);
                    var take = context.GetArgument<int>("take", 20);
                    return await propertyResolver!.SearchProperties(location, checkIn, checkOut, guests, minPrice, maxPrice, skip, take);
                });

            Field<BooleanGraphType>("checkPropertyAvailability")
                .Description("Check if a property is available for given dates")
                .Argument<NonNullGraphType<IdGraphType>>("propertyId", "The unique identifier of the property")
                .Argument<NonNullGraphType<DateTimeGraphType>>("checkIn", "Check-in date")
                .Argument<NonNullGraphType<DateTimeGraphType>>("checkOut", "Check-out date")
                .ResolveAsync(async context =>
                {
                    var propertyResolver = context.RequestServices?.GetService<PropertyResolver>();
                    var propertyId = context.GetArgument<Guid>("propertyId");
                    var checkIn = context.GetArgument<DateTime>("checkIn");
                    var checkOut = context.GetArgument<DateTime>("checkOut");
                    return await propertyResolver!.CheckAvailability(propertyId, checkIn, checkOut);
                });

            // Booking queries
            Field<BookingType>("booking")
                .Description("Get a booking by ID")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The unique identifier of the booking")
                .ResolveAsync(async context =>
                {
                    var bookingResolver = context.RequestServices?.GetService<BookingResolver>();
                    var id = context.GetArgument<Guid>("id");
                    return await bookingResolver!.GetBookingById(id);
                });

            Field<ListGraphType<BookingType>>("bookings")
                .Description("Get all bookings")
                .Argument<Types.BookingFilterInputType>("filter", "Filters for booking search")
                .Argument<IntGraphType>("skip", "Number of bookings to skip")
                .Argument<IntGraphType>("take", "Number of bookings to take")
                .ResolveAsync(async context =>
                {
                    var bookingResolver = context.RequestServices?.GetService<BookingResolver>();
                    var filterInput = context.GetArgument<IDictionary<string, object?>>("filter");
                    var filter = filterInput != null ? new BookingFilter
                    {
                        GuestId = filterInput.TryGetValue("guestId", out var guestId) && guestId != null ? Guid.Parse(guestId.ToString()!) : null,
                        HostId = filterInput.TryGetValue("hostId", out var hostId) && hostId != null ? Guid.Parse(hostId.ToString()!) : null,
                        PropertyId = filterInput.TryGetValue("propertyId", out var propertyId) && propertyId != null ? Guid.Parse(propertyId.ToString()!) : null
                    } : null;
                    var skip = context.GetArgument<int>("skip", 0);
                    var take = context.GetArgument<int>("take", 20);
                    return await bookingResolver!.GetBookings(filter, skip, take);
                });

            Field<ListGraphType<BookingType>>("bookingsByGuest")
                .Description("Get bookings by guest")
                .Argument<NonNullGraphType<IdGraphType>>("guestId", "The unique identifier of the guest")
                .Argument<IntGraphType>("skip", "Number of bookings to skip")
                .Argument<IntGraphType>("take", "Number of bookings to take")
                .ResolveAsync(async context =>
                {
                    var bookingResolver = context.RequestServices?.GetService<BookingResolver>();
                    var guestId = context.GetArgument<Guid>("guestId");
                    var skip = context.GetArgument<int>("skip", 0);
                    var take = context.GetArgument<int>("take", 20);
                    return await bookingResolver!.GetBookingsByGuest(guestId, skip, take);
                });

            Field<ListGraphType<BookingType>>("bookingsByHost")
                .Description("Get bookings by host")
                .Argument<NonNullGraphType<IdGraphType>>("hostId", "The unique identifier of the host")
                .Argument<IntGraphType>("skip", "Number of bookings to skip")
                .Argument<IntGraphType>("take", "Number of bookings to take")
                .ResolveAsync(async context =>
                {
                    var bookingResolver = context.RequestServices?.GetService<BookingResolver>();
                    var hostId = context.GetArgument<Guid>("hostId");
                    var skip = context.GetArgument<int>("skip", 0);
                    var take = context.GetArgument<int>("take", 20);
                    return await bookingResolver!.GetBookingsByHost(hostId, skip, take);
                });

            Field<ListGraphType<BookingType>>("bookingsByProperty")
                .Description("Get bookings by property")
                .Argument<NonNullGraphType<IdGraphType>>("propertyId", "The unique identifier of the property")
                .Argument<IntGraphType>("skip", "Number of bookings to skip")
                .Argument<IntGraphType>("take", "Number of bookings to take")
                .ResolveAsync(async context =>
                {
                    var bookingResolver = context.RequestServices?.GetService<BookingResolver>();
                    var propertyId = context.GetArgument<Guid>("propertyId");
                    var skip = context.GetArgument<int>("skip", 0);
                    var take = context.GetArgument<int>("take", 20);
                    return await bookingResolver!.GetBookingsByProperty(propertyId, skip, take);
                });
        }
    }
}