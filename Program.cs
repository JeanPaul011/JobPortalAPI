using JobPortalAPI.Models;
using JobPortalAPI.Services;
using JobPortalAPI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using AspNetCoreRateLimit;
using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Load Configurations Securely
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true) 
    .AddEnvironmentVariables(); // Load ENV variables if available

// Check if running in Production and verify environment variables
if (builder.Environment.IsProduction())
{
    Console.WriteLine("Running in Production Mode...");

    // Ensure required environment variables are set
    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SMTP_EMAIL")) ||
        string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SMTP_PASSWORD")) ||
        string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_SECRET")))
    {
        throw new Exception("Critical environment variables are missing! Check SMTP_EMAIL, SMTP_PASSWORD, and JWT_SECRET.");
    }
}
else
{
    Console.WriteLine("Running in Development Mode...");
}

// Register Database Context
var connectionString = builder.Configuration.GetConnectionString("Connection") ?? throw new Exception("Database connection is missing!");
builder.Services.AddDbContext<JobPortalContext>(options =>
    options.UseSqlite(connectionString), ServiceLifetime.Scoped);

// Register Identity & Authentication with the correct User model
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<JobPortalContext>()
    .AddDefaultTokenProviders();

// Register Identity Managers


// Register Authentication using JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("JWT Key is missing!");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "https://localhost:5276";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = "role"
        };
    });

// Register Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireRecruiterRole", policy => policy.RequireRole("Recruiter"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User")); // Fixed: Matching "User" role
});

// Register Mail Service using MailKit
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<EmailService>();

// Register Application Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();



// Register Repositories
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Register Rate Limiting Services
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*",
            Period = "10s",
            Limit = 5
        }
    };
});
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// Register CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Debugging - Print Environment Variables
Console.WriteLine(" Checking Environment Variables:");
Console.WriteLine($" ConnectionString: {connectionString}");
Console.WriteLine($" JWT Key: {(string.IsNullOrEmpty(jwtKey) ? " MISSING" : " OK")}");
Console.WriteLine($" JWT Issuer: {jwtIssuer}");
Console.WriteLine($" JWT Expiry: {builder.Configuration["Jwt:ExpireHours"]}");

//  Ensure Roles Exist Before Running the App
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

    var roles = new[] { "Admin", "Recruiter", "User" }; //  Fixed: "JobSeeker" -> "User"
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(role));
            if (result.Succeeded)
            {
                Console.WriteLine($" Created role: {role}");
            }
            else
            {
                Console.WriteLine($" Failed to create role: {role}");
            }
        }
    }
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var emailService = services.GetRequiredService<EmailService>();

    Console.WriteLine("📩 Testing EmailService Instantiation...");
    await emailService.SendEmailAsync("test@example.com", "Test Email", "This is a test email.");
}

// Enable Middleware (Ensuring Proper Order)
app.UseHttpsRedirection();
app.UseIpRateLimiting();
app.UseCors("AllowAllOrigins");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
