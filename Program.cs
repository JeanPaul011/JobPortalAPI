using JobPortalAPI.Models;
using JobPortalAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using AspNetCoreRateLimit;
using System.Collections.Generic;






var builder = WebApplication.CreateBuilder(args);

// Register Database Context
builder.Services.AddDbContext<JobPortalContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Connection")));

// Register Identity & Authentication
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<JobPortalContext>()
    .AddSignInManager<SignInManager<User>>() // FIXED: Added SignInManager.
    .AddRoleManager<RoleManager<IdentityRole>>() // Add RoleManager
    
    .AddDefaultTokenProviders();

// Configure JWT Authentication
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        };
    });

// Register Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireRecruiterRole", policy => policy.RequireRole("Recruiter"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("JobSeeker"));
});


// Register Mail Service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<EmailService>();

// Register Services (Dependency Injection)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IJobApplicationService, JobApplicationService>();



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

var app = builder.Build();

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "*", // Apply to all endpoints
            Period = "1m",  // 1 minute
            Limit = 100     // Max 100 requests per minute
        }
    };
});
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


// ✅ Configure CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
{
    policy.WithOrigins("http://localhost:5276")
          .AllowAnyMethod()
          .AllowAnyHeader();
});


    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});



//Debugging - Print Environment Variables
Console.WriteLine("🔍 Checking Environment Variables:");
Console.WriteLine($"🔹 ConnectionString: {builder.Configuration["ConnectionStrings:Connection"]}");
Console.WriteLine($"🔹 JWT Key: {builder.Configuration["Jwt:Key"]}");
Console.WriteLine($"🔹 JWT Issuer: {builder.Configuration["Jwt:Issuer"]}");
Console.WriteLine($"🔹 JWT Expiry: {builder.Configuration["Jwt:ExpireHours"]}");


// Ensure Roles Exist Before Running the App
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Recruiter", "JobSeeker" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            Console.WriteLine($" Created role: {role}"); //  Debugging Log
        }
    }
}
app.UseHttpsRedirection(); // Enforce HTTPS
app.UseIpRateLimiting();   // Enable Rate Limiting

app.UseCors("AllowLocalhost"); // Apply CORS Policy (before authentication)

// Enable Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();



