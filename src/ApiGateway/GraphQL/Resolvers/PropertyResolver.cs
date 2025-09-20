using ApiGateway.Models;
using ApiGateway.Services;
using ApiGateway.GraphQL.Types;
using ApiGateway.GraphQL.Types;
using GraphQL;
using GraphQL.Types;

namespace ApiGateway.GraphQL.Resolvers
{
    public class PropertyResolver
    {
        private readonly IHttpService _httpService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PropertyResolver> _logger;
        private readonly UserResolver _userResolver;

        public PropertyResolver(IHttpService httpService, IConfiguration configuration, ILogger<PropertyResolver> logger, UserResolver userResolver)
        {
            _httpService = httpService;
            _configuration = configuration;
            _logger = logger;
            _userResolver = userResolver;
        }

        public async Task<Property?> GetPropertyById(Guid id)
        {
            try
            {
                var propertyServiceUrl = _configuration["Services:PropertyService"];
                var endpoint = $"{propertyServiceUrl}/api/properties/{id}";
                
                var property = await _httpService.GetAsync<Property>(endpoint);
                
                if (property != null)
                {
                    // Load host information
                    property.Host = await _userResolver.GetUserById(property.HostId) ?? new User();
                }
                
                return property;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching property {PropertyId}", id);
                return null;
            }
        }

        public async Task<List<Property>> GetProperties(PropertyFilter? filter = null, int skip = 0, int take = 20)
        {
            try
            {
                var propertyServiceUrl = _configuration["Services:PropertyService"];
                var queryParams = BuildQueryString(filter, skip, take);
                var endpoint = $"{propertyServiceUrl}/api/properties{queryParams}";
                
                var properties = await _httpService.GetAsync<List<Property>>(endpoint) ?? new List<Property>();
                
                // Load host information for each property
                foreach (var property in properties)
                {
                    property.Host = await _userResolver.GetUserById(property.HostId) ?? new User();
                }
                
                return properties;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching properties");
                return new List<Property>();
            }
        }

        public async Task<List<Property>> GetPropertiesByHost(Guid hostId, int skip = 0, int take = 20)
        {
            try
            {
                var propertyServiceUrl = _configuration["Services:PropertyService"];
                var endpoint = $"{propertyServiceUrl}/api/properties/host/{hostId}?skip={skip}&take={take}";
                
                var properties = await _httpService.GetAsync<List<Property>>(endpoint) ?? new List<Property>();
                
                // Load host information for each property
                foreach (var property in properties)
                {
                    property.Host = await _userResolver.GetUserById(property.HostId) ?? new User();
                }
                
                return properties;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching properties for host {HostId}", hostId);
                return new List<Property>();
            }
        }

        public async Task<List<Property>> SearchProperties(string? location = null, DateTime? checkIn = null, DateTime? checkOut = null, int? guests = null, decimal? minPrice = null, decimal? maxPrice = null, int skip = 0, int take = 20)
        {
            try
            {
                var propertyServiceUrl = _configuration["Services:PropertyService"];
                var queryParams = BuildSearchQueryString(location, checkIn, checkOut, guests, minPrice, maxPrice, skip, take);
                var endpoint = $"{propertyServiceUrl}/api/properties/search{queryParams}";
                
                var properties = await _httpService.GetAsync<List<Property>>(endpoint) ?? new List<Property>();
                
                // Load host information for each property
                foreach (var property in properties)
                {
                    property.Host = await _userResolver.GetUserById(property.HostId) ?? new User();
                }
                
                return properties;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching properties");
                return new List<Property>();
            }
        }

        public async Task<Property?> CreateProperty(Property propertyInput, string? token = null)
        {
            try
            {
                var propertyServiceUrl = _configuration["Services:PropertyService"];
                var endpoint = $"{propertyServiceUrl}/api/properties";
                
                var property = await _httpService.PostAsync<Property>(endpoint, propertyInput, token);
                
                if (property != null)
                {
                    property.Host = await _userResolver.GetUserById(property.HostId) ?? new User();
                }
                
                return property;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating property");
                return null;
            }
        }

        public async Task<Property?> UpdateProperty(Guid id, Property propertyInput, string? token = null)
        {
            try
            {
                var propertyServiceUrl = _configuration["Services:PropertyService"];
                var endpoint = $"{propertyServiceUrl}/api/properties/{id}";
                
                var property = await _httpService.PutAsync<Property>(endpoint, propertyInput, token);
                
                if (property != null)
                {
                    property.Host = await _userResolver.GetUserById(property.HostId) ?? new User();
                }
                
                return property;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating property {PropertyId}", id);
                return null;
            }
        }

        public async Task<bool> DeleteProperty(Guid id, string? token = null)
        {
            try
            {
                var propertyServiceUrl = _configuration["Services:PropertyService"];
                var endpoint = $"{propertyServiceUrl}/api/properties/{id}";
                
                return await _httpService.DeleteAsync(endpoint, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting property {PropertyId}", id);
                return false;
            }
        }

        public async Task<bool> CheckAvailability(Guid propertyId, DateTime checkIn, DateTime checkOut)
        {
            try
            {
                var propertyServiceUrl = _configuration["Services:PropertyService"];
                var endpoint = $"{propertyServiceUrl}/api/properties/{propertyId}/availability?checkIn={checkIn:yyyy-MM-dd}&checkOut={checkOut:yyyy-MM-dd}";
                
                var result = await _httpService.GetAsync<dynamic>(endpoint);
                return result?.isAvailable ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking availability for property {PropertyId}", propertyId);
                return false;
            }
        }

        private string BuildQueryString(PropertyFilter? filter, int skip, int take)
        {
            var queryParams = new List<string> { $"skip={skip}", $"take={take}" };
            
            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.City))
                    queryParams.Add($"city={Uri.EscapeDataString(filter.City)}");
                
                if (!string.IsNullOrEmpty(filter.State))
                    queryParams.Add($"state={Uri.EscapeDataString(filter.State)}");
                
                if (!string.IsNullOrEmpty(filter.Country))
                    queryParams.Add($"country={Uri.EscapeDataString(filter.Country)}");
                
                if (filter.PropertyType.HasValue)
                    queryParams.Add($"propertyType={filter.PropertyType}");
                
                if (filter.MinPrice.HasValue)
                    queryParams.Add($"minPrice={filter.MinPrice}");
                
                if (filter.MaxPrice.HasValue)
                    queryParams.Add($"maxPrice={filter.MaxPrice}");
                
                if (filter.MinGuests.HasValue)
                    queryParams.Add($"minGuests={filter.MinGuests}");
                
                if (filter.MaxGuests.HasValue)
                    queryParams.Add($"maxGuests={filter.MaxGuests}");
                
                if (filter.InstantBookOnly.HasValue)
                    queryParams.Add($"instantBookOnly={filter.InstantBookOnly}");
            }
            
            return queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        }

        private string BuildSearchQueryString(string? location, DateTime? checkIn, DateTime? checkOut, int? guests, decimal? minPrice, decimal? maxPrice, int skip, int take)
        {
            var queryParams = new List<string> { $"skip={skip}", $"take={take}" };
            
            if (!string.IsNullOrEmpty(location))
                queryParams.Add($"location={Uri.EscapeDataString(location)}");
            
            if (checkIn.HasValue)
                queryParams.Add($"checkIn={checkIn.Value:yyyy-MM-dd}");
            
            if (checkOut.HasValue)
                queryParams.Add($"checkOut={checkOut.Value:yyyy-MM-dd}");
            
            if (guests.HasValue)
                queryParams.Add($"guests={guests}");
            
            if (minPrice.HasValue)
                queryParams.Add($"minPrice={minPrice}");
            
            if (maxPrice.HasValue)
                queryParams.Add($"maxPrice={maxPrice}");
            
            return queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        }
    }

}