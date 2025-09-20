using ApiGateway.Services;
using ApiGateway.GraphQL;
using ApiGateway.GraphQL.Types;
using ApiGateway.GraphQL.Resolvers;
using GraphQL;
using GraphQL.Types;
using GraphQL.Server.Ui.Playground;
using GraphQL.Server;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Register HTTP service for microservice communication
builder.Services.AddScoped<IHttpService, HttpService>();

// Register GraphQL resolvers
builder.Services.AddScoped<UserResolver>();
builder.Services.AddScoped<PropertyResolver>();
builder.Services.AddScoped<BookingResolver>();

// Register GraphQL types
builder.Services.AddScoped<UserType>();
builder.Services.AddScoped<PropertyType>();
builder.Services.AddScoped<BookingType>();
builder.Services.AddScoped<PaymentType>();
builder.Services.AddScoped<ReviewType>();
builder.Services.AddScoped<NotificationType>();

// Register GraphQL query and mutation
builder.Services.AddScoped<Query>();
builder.Services.AddScoped<Mutation>();

// Register GraphQL schema
builder.Services.AddScoped<ApiGatewaySchema>();

// Add GraphQL services
builder.Services.AddSingleton<ISchema, ApiGatewaySchema>();

builder.Services.AddGraphQL(b => b
    .AddSystemTextJson()
    .AddErrorInfoProvider(opt => opt.ExposeExceptionDetails = builder.Environment.IsDevelopment()));

// Add JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "your-super-secret-jwt-key-that-is-at-least-256-bits-long";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add health checks
builder.Services.AddHealthChecks();

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseGraphQLPlayground("/ui/playground");
}

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Add GraphQL middleware
app.UseGraphQL<ApiGatewaySchema>("/graphql");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new 
{ 
    status = "healthy", 
    service = "ApiGateway",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName
}));

// Service status endpoint
app.MapGet("/status", async (IConfiguration config, IHttpService httpService) =>
{
    var services = new Dictionary<string, object>();
    
    var serviceUrls = new Dictionary<string, string>
    {
        ["UserService"] = config["Services:UserService"] ?? "",
        ["PropertyService"] = config["Services:PropertyService"] ?? "",
        ["BookingService"] = config["Services:BookingService"] ?? "",
        ["PaymentService"] = config["Services:PaymentService"] ?? "",
        ["ReviewService"] = config["Services:ReviewService"] ?? "",
        ["NotificationService"] = config["Services:NotificationService"] ?? ""
    };

    foreach (var service in serviceUrls)
    {
        try
        {
            var healthEndpoint = $"{service.Value}/health";
            var response = await httpService.GetAsync<dynamic>(healthEndpoint);
            services[service.Key] = new { status = "healthy", url = service.Value };
        }
        catch
        {
            services[service.Key] = new { status = "unhealthy", url = service.Value };
        }
    }

    return Results.Ok(new
    {
        gateway = new { status = "healthy", timestamp = DateTime.UtcNow },
        services = services
    });
});

// API documentation endpoint
app.MapGet("/", () => Results.Redirect("/ui/playground"));

app.MapControllers();

app.Run();