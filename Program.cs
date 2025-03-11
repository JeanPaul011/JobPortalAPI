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
using JobPortalAPI.Middleware;
using Microsoft.Extensions.Diagnostics.HealthChecks;

// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Load Configurations Securely
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true) 
    .AddEnvironmentVariables(); // Load ENV variables if available

// Verify Environment Variables in Production
if (builder.Environment.IsProduction())
{
    Console.WriteLine("Running in Production Mode...");
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

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("Connection") ?? throw new Exception("Database connection is missing!");
builder.Services.AddDbContext<JobPortalContext>(options =>
    options.UseSqlite(connectionString), ServiceLifetime.Scoped);

// Identity & Authentication Configuration
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<JobPortalContext>()
    .AddDefaultTokenProviders();

// Register Identity Managers
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<SignInManager<User>>();
builder.Services.AddScoped<RoleManager<IdentityRole>>();

// Load JWT Secret Key Securely
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("JWT Key is missing!");
Console.WriteLine($"Debugging JWT Key: {jwtKey.Substring(0, 5)}******"); // Masked for security

// Configure Authentication using JWT
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
            RoleClaimType = ClaimTypes.Role
        };
    });

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireRecruiterRole", policy => policy.RequireRole("Recruiter"));
    options.AddPolicy("RequireJobSeekerRole", policy => policy.RequireRole("JobSeeker"));
    options.AddPolicy("RequireAdminOrRecruiter", policy => policy.RequireRole("Admin", "Recruiter"));
    options.AddPolicy("RequireAdminOrJobSeeker", policy => policy.RequireRole("Admin", "JobSeeker"));
});


// Application Services & Dependency Injection
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<EmailService>();
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

// Swagger Configuration
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
    // Add this XML documentation code right here
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Rate Limiting Configuration
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

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// Add health checks - place this before "var app = builder.Build();"
builder.Services.AddHealthChecks()
    .AddDbContextCheck<JobPortalContext>("database");
// Build & Configure Application Pipeline
var app = builder.Build();

app.UseHttpsRedirection();
app.UseIpRateLimiting();
app.UseCors("AllowAllOrigins");
app.UseGlobalExceptionMiddleware();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
// Add this after app.MapControllers() but before app.Run()
app.MapHealthChecks("/health");

// Ensure Roles Exist Without Blocking Startup
// Ensure Roles Exist Without Blocking Startup
app.Lifetime.ApplicationStarted.Register(() =>
{
    // Create a new scope inside the event handler
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var roles = new[] { "Admin", "Recruiter", "User" };

        foreach (var role in roles)
        {
            if (!roleManager.RoleExistsAsync(role).Result)
            {
                var result = roleManager.CreateAsync(new IdentityRole(role)).Result;
                Console.WriteLine(result.Succeeded ? $"Created role: {role}" : $"Failed to create role: {role}");
            }
        }
    }
});

// Prevent Sending Email Spam in Production
if (!builder.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var emailService = services.GetRequiredService<EmailService>();

        Console.WriteLine("Testing EmailService Instantiation...");
        await emailService.SendEmailAsync("your-email@example.com", "Test Email", "This is a test email.");
    }
}

// Run the Application
app.Run();
