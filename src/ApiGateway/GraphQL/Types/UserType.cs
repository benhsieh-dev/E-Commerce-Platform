using GraphQL.Types;
using ApiGateway.Models;

namespace ApiGateway.GraphQL.Types
{
    public class UserType : ObjectGraphType<User>
    {
        public UserType()
        {
            Name = "User";
            Description = "A user in the AirBnB platform";

            Field(u => u.Id, type: typeof(IdGraphType)).Description("The unique identifier of the user");
            Field(u => u.FirstName).Description("The user's first name");
            Field(u => u.LastName).Description("The user's last name");
            Field(u => u.Email).Description("The user's email address");
            Field(u => u.Phone, nullable: true).Description("The user's phone number");
            Field(u => u.ProfilePictureUrl, nullable: true).Description("URL to the user's profile picture");
            Field(u => u.DateOfBirth, type: typeof(DateTimeGraphType)).Description("The user's date of birth");
            Field(u => u.Role, type: typeof(UserRoleType)).Description("The user's role in the platform");
            Field(u => u.IsEmailVerified).Description("Whether the user's email is verified");
            Field(u => u.IsPhoneVerified).Description("Whether the user's phone is verified");
            Field(u => u.IsActive).Description("Whether the user account is active");
            Field(u => u.CreatedAt, type: typeof(DateTimeGraphType)).Description("When the user account was created");
            Field(u => u.UpdatedAt, type: typeof(DateTimeGraphType)).Description("When the user account was last updated");
            Field(u => u.LastLoginAt, type: typeof(DateTimeGraphType), nullable: true).Description("When the user last logged in");
        }
    }

    public class UserRoleType : EnumerationGraphType<UserRole>
    {
        public UserRoleType()
        {
            Name = "UserRole";
            Description = "The role of a user in the platform";
        }
    }

    public class UserInputType : InputObjectGraphType<User>
    {
        public UserInputType()
        {
            Name = "UserInput";
            Description = "Input for creating or updating a user";

            Field(u => u.FirstName).Description("The user's first name");
            Field(u => u.LastName).Description("The user's last name");
            Field(u => u.Email).Description("The user's email address");
            Field(u => u.Phone, nullable: true).Description("The user's phone number");
            Field(u => u.ProfilePictureUrl, nullable: true).Description("URL to the user's profile picture");
            Field(u => u.DateOfBirth, type: typeof(DateTimeGraphType)).Description("The user's date of birth");
            Field(u => u.Role, type: typeof(UserRoleType)).Description("The user's role in the platform");
        }
    }
}