using GraphQL.Types;
using ApiGateway.GraphQL.Types;
using ApiGateway.GraphQL.Resolvers;
using ApiGateway.Models;

namespace ApiGateway.GraphQL
{
    public class Mutation : ObjectGraphType
    {
        public Mutation()
        {
            Name = "Mutation";
            Description = "The root mutation for the AirBnB platform";

            // User mutations
            Field<UserType>("createUser")
                .Description("Create a new user")
                .Argument<NonNullGraphType<UserInputType>>("user", "The user input")
                .ResolveAsync(async context =>
                {
                    var userResolver = context.RequestServices?.GetService<UserResolver>();
                    var userInput = context.GetArgument<User>("user");
                    return await userResolver!.CreateUser(userInput);
                });

            Field<UserType>("updateUser")
                .Description("Update an existing user")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The unique identifier of the user")
                .Argument<NonNullGraphType<UserInputType>>("user", "The user input")
                .ResolveAsync(async context =>
                {
                    var userResolver = context.RequestServices?.GetService<UserResolver>();
                    var id = context.GetArgument<Guid>("id");
                    var userInput = context.GetArgument<User>("user");
                    var token = context.UserContext as string;
                    return await userResolver!.UpdateUser(id, userInput);
                });

            Field<BooleanGraphType>("deleteUser")
                .Description("Delete a user")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The unique identifier of the user")
                .ResolveAsync(async context =>
                {
                    var userResolver = context.RequestServices?.GetService<UserResolver>();
                    var id = context.GetArgument<Guid>("id");
                    return await userResolver!.DeleteUser(id);
                });

            Field<StringGraphType>("login")
                .Description("Authenticate a user and return a token")
                .Argument<NonNullGraphType<StringGraphType>>("email", "User email")
                .Argument<NonNullGraphType<StringGraphType>>("password", "User password")
                .ResolveAsync(async context =>
                {
                    var userResolver = context.RequestServices?.GetService<UserResolver>();
                    var email = context.GetArgument<string>("email");
                    var password = context.GetArgument<string>("password");
                    return await userResolver!.AuthenticateUser(email, password);
                });

            Field<UserType>("register")
                .Description("Register a new user")
                .Argument<NonNullGraphType<StringGraphType>>("email", "User email")
                .Argument<NonNullGraphType<StringGraphType>>("password", "User password")
                .Argument<NonNullGraphType<StringGraphType>>("firstName", "User first name")
                .Argument<NonNullGraphType<StringGraphType>>("lastName", "User last name")
                .ResolveAsync(async context =>
                {
                    var userResolver = context.RequestServices?.GetService<UserResolver>();
                    var email = context.GetArgument<string>("email");
                    var password = context.GetArgument<string>("password");
                    var firstName = context.GetArgument<string>("firstName");
                    var lastName = context.GetArgument<string>("lastName");
                    return await userResolver!.RegisterUser(email, password, firstName, lastName);
                });

            // Property mutations
            Field<PropertyType>("createProperty")
                .Description("Create a new property")
                .Argument<NonNullGraphType<PropertyInputType>>("property", "The property input")
                .ResolveAsync(async context =>
                {
                    var propertyResolver = context.RequestServices?.GetService<PropertyResolver>();
                    var propertyInput = context.GetArgument<Property>("property");
                    var token = context.UserContext as string;
                    return await propertyResolver!.CreateProperty(propertyInput, token);
                });

            Field<PropertyType>("updateProperty")
                .Description("Update an existing property")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The unique identifier of the property")
                .Argument<NonNullGraphType<PropertyInputType>>("property", "The property input")
                .ResolveAsync(async context =>
                {
                    var propertyResolver = context.RequestServices?.GetService<PropertyResolver>();
                    var id = context.GetArgument<Guid>("id");
                    var propertyInput = context.GetArgument<Property>("property");
                    var token = context.UserContext as string;
                    return await propertyResolver!.UpdateProperty(id, propertyInput, token);
                });

            Field<BooleanGraphType>("deleteProperty")
                .Description("Delete a property")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The unique identifier of the property")
                .ResolveAsync(async context =>
                {
                    var propertyResolver = context.RequestServices?.GetService<PropertyResolver>();
                    var id = context.GetArgument<Guid>("id");
                    var token = context.UserContext as string;
                    return await propertyResolver!.DeleteProperty(id, token);
                });

            // Booking mutations
            Field<BookingType>("createBooking")
                .Description("Create a new booking")
                .Argument<NonNullGraphType<BookingInputType>>("booking", "The booking input")
                .ResolveAsync(async context =>
                {
                    var bookingResolver = context.RequestServices?.GetService<BookingResolver>();
                    var bookingInput = context.GetArgument<Booking>("booking");
                    var token = context.UserContext as string;
                    return await bookingResolver!.CreateBooking(bookingInput, token);
                });

            Field<BookingType>("updateBooking")
                .Description("Update an existing booking")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The unique identifier of the booking")
                .Argument<NonNullGraphType<BookingInputType>>("booking", "The booking input")
                .ResolveAsync(async context =>
                {
                    var bookingResolver = context.RequestServices?.GetService<BookingResolver>();
                    var id = context.GetArgument<Guid>("id");
                    var bookingInput = context.GetArgument<Booking>("booking");
                    var token = context.UserContext as string;
                    return await bookingResolver!.UpdateBooking(id, bookingInput, token);
                });

            Field<BookingType>("updateBookingStatus")
                .Description("Update booking status")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The unique identifier of the booking")
                .Argument<NonNullGraphType<BookingStatusType>>("status", "The new booking status")
                .ResolveAsync(async context =>
                {
                    var bookingResolver = context.RequestServices?.GetService<BookingResolver>();
                    var id = context.GetArgument<Guid>("id");
                    var status = context.GetArgument<BookingStatus>("status");
                    var token = context.UserContext as string;
                    return await bookingResolver!.UpdateBookingStatus(id, status, token);
                });

            Field<BookingType>("cancelBooking")
                .Description("Cancel a booking")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The unique identifier of the booking")
                .Argument<NonNullGraphType<StringGraphType>>("reason", "Cancellation reason")
                .ResolveAsync(async context =>
                {
                    var bookingResolver = context.RequestServices?.GetService<BookingResolver>();
                    var id = context.GetArgument<Guid>("id");
                    var reason = context.GetArgument<string>("reason");
                    var token = context.UserContext as string;
                    return await bookingResolver!.CancelBooking(id, reason, token);
                });

            Field<BooleanGraphType>("checkInBooking")
                .Description("Check in a booking")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The unique identifier of the booking")
                .ResolveAsync(async context =>
                {
                    var bookingResolver = context.RequestServices?.GetService<BookingResolver>();
                    var id = context.GetArgument<Guid>("id");
                    var token = context.UserContext as string;
                    return await bookingResolver!.CheckIn(id, token);
                });

            Field<BooleanGraphType>("checkOutBooking")
                .Description("Check out a booking")
                .Argument<NonNullGraphType<IdGraphType>>("id", "The unique identifier of the booking")
                .ResolveAsync(async context =>
                {
                    var bookingResolver = context.RequestServices?.GetService<BookingResolver>();
                    var id = context.GetArgument<Guid>("id");
                    var token = context.UserContext as string;
                    return await bookingResolver!.CheckOut(id, token);
                });
        }
    }
}