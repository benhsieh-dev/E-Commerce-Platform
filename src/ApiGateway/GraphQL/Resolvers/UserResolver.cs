using ApiGateway.Models;
using ApiGateway.Services;
using ApiGateway.GraphQL.Types;
using GraphQL;
using GraphQL.Types;

namespace ApiGateway.GraphQL.Resolvers
{
    public class UserResolver
    {
        private readonly IHttpService _httpService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserResolver> _logger;

        public UserResolver(IHttpService httpService, IConfiguration configuration, ILogger<UserResolver> logger)
        {
            _httpService = httpService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<User?> GetUserById(Guid id)
        {
            try
            {
                var userServiceUrl = _configuration["Services:UserService"];
                var endpoint = $"{userServiceUrl}/api/users/{id}";
                
                var user = await _httpService.GetAsync<User>(endpoint);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user {UserId}", id);
                return null;
            }
        }

        public async Task<List<User>> GetUsers(int skip = 0, int take = 20)
        {
            try
            {
                var userServiceUrl = _configuration["Services:UserService"];
                var endpoint = $"{userServiceUrl}/api/users?skip={skip}&take={take}";
                
                var users = await _httpService.GetAsync<List<User>>(endpoint);
                return users ?? new List<User>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users");
                return new List<User>();
            }
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            try
            {
                var userServiceUrl = _configuration["Services:UserService"];
                var endpoint = $"{userServiceUrl}/api/users/email/{email}";
                
                var user = await _httpService.GetAsync<User>(endpoint);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by email {Email}", email);
                return null;
            }
        }

        public async Task<User?> CreateUser(User userInput)
        {
            try
            {
                var userServiceUrl = _configuration["Services:UserService"];
                var endpoint = $"{userServiceUrl}/api/users";
                
                var user = await _httpService.PostAsync<User>(endpoint, userInput);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return null;
            }
        }

        public async Task<User?> UpdateUser(Guid id, User userInput)
        {
            try
            {
                var userServiceUrl = _configuration["Services:UserService"];
                var endpoint = $"{userServiceUrl}/api/users/{id}";
                
                var user = await _httpService.PutAsync<User>(endpoint, userInput);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                return null;
            }
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            try
            {
                var userServiceUrl = _configuration["Services:UserService"];
                var endpoint = $"{userServiceUrl}/api/users/{id}";
                
                return await _httpService.DeleteAsync(endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return false;
            }
        }

        public async Task<string?> AuthenticateUser(string email, string password)
        {
            try
            {
                var userServiceUrl = _configuration["Services:UserService"];
                var endpoint = $"{userServiceUrl}/api/users/login";
                
                var loginRequest = new { Email = email, Password = password };
                var response = await _httpService.PostAsync<dynamic>(endpoint, loginRequest);
                
                return response?.token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating user {Email}", email);
                return null;
            }
        }

        public async Task<User?> RegisterUser(string email, string password, string firstName, string lastName)
        {
            try
            {
                var userServiceUrl = _configuration["Services:UserService"];
                var endpoint = $"{userServiceUrl}/api/users/register";
                
                var registerRequest = new 
                { 
                    Email = email, 
                    Password = password, 
                    FirstName = firstName, 
                    LastName = lastName 
                };
                
                var user = await _httpService.PostAsync<User>(endpoint, registerRequest);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user {Email}", email);
                return null;
            }
        }
    }
}