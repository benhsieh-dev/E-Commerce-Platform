using System.ComponentModel.DataAnnotations;

namespace ApiGateway.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public UserRole Role { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    public enum UserRole
    {
        Guest = 0,
        Host = 1,
        Admin = 2
    }
}