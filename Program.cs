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
Env.Load(); // ✅ Loads environment variables from .env file

var builder = WebApplication.CreateBuilder(args);

// ✅ Load Configurations Securely
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true) // 🔒 Production settings
    .AddEnvironmentVariables(); // 🔒 Load ENV variables if available

// ✅ Register Database Context
var connectionString = builder.Configuration["ConnectionStrings:Connection"] ?? throw new Exception("Database connection is missing!");
builder.Services.AddDbContext<JobPortalContext>(options => options.UseSqlite(connectionString));

// ✅ Register Identity & Authentication
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<JobPortalContext>()
    .AddSignInManager<SignInManager<User>>()
    .AddRoleManager<RoleManager<IdentityRole>>()
    .AddDefaultTokenProviders();

// ✅ Configure JWT Authentication (Now Reads from ENV)
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

// ✅ Register Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireRecruiterRole", policy => policy.RequireRole("Recruiter"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("JobSeeker"));
});

// ✅ Register Mail Service (Hides Email Password)
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<EmailService>();

// ✅ Register Application Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();

// ✅ Register Repositories
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// ✅ Register Rate Limiting services
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

// ✅ Register CORS Policy
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

// ✅ Debugging - Print Environment Variables
Console.WriteLine("🔍 Checking Environment Variables:");
Console.WriteLine($"🔹 ConnectionString: {connectionString}");
Console.WriteLine($"🔹 JWT Key: {(string.IsNullOrEmpty(jwtKey) ? "❌ MISSING" : "✅ OK")}");
Console.WriteLine($"🔹 JWT Issuer: {jwtIssuer}");
Console.WriteLine($"🔹 JWT Expiry: {builder.Configuration["Jwt:ExpireHours"]}");

// ✅ Ensure Roles Exist Before Running the App
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Recruiter", "JobSeeker" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            Console.WriteLine($"✅ Created role: {role}");
        }
    }
}

app.UseHttpsRedirection(); // Enforce HTTPS
app.UseIpRateLimiting();   // Enable Rate Limiting
app.UseCors("AllowAllOrigins"); // Apply CORS Policy

// Enable Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
