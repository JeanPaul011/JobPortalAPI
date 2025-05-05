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
using JobPortalAPI.Services.TokenServices;




// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);
// ===== ADD THIS BLOCK HERE =====
// For Azure deployment
// Database Configuration - REPLACE your existing code with this:
var homeDirectory = Environment.GetEnvironmentVariable("HOME");
var dbPath = homeDirectory != null 
    ? Path.Combine(homeDirectory, "site", "wwwroot", "jobportal.db")
    : "jobportal.db";

builder.Services.AddDbContext<JobPortalContext>(options =>
    options.UseSqlite($"Data Source={dbPath};"), ServiceLifetime.Scoped);


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


// Configure Authentication using JWT
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new Exception("JWT Issuer is missing!");
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
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    
        };
        options.MapInboundClaims = false;
        options.IncludeErrorDetails = true;
        options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            // Transform array claims to single values if needed
            var roleClaims = context.Principal.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").ToList();
            if (roleClaims.Count > 1)
            {
                var identity = (ClaimsIdentity)context.Principal.Identity;
                identity.RemoveClaim(identity.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role"));
                identity.AddClaim(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Recruiter"));
            }
            return Task.CompletedTask;
        }
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
builder.Services.AddScoped<ITokenService, TokenService>();


// Register Repositories
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IAdminRequestRepository, AdminRequestRepository>();


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
    options.AddPolicy("ReactApp", policy => 
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173", "http://localhost:5174", // ✅ ADD THIS
        "https://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});

// Add health checks - place this before "var app = builder.Build();"
builder.Services.AddHealthChecks()
    .AddDbContextCheck<JobPortalContext>("database");
// Build & Configure Application Pipeline
var app = builder.Build();
// ===== ADD THIS BLOCK HERE =====
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else 
{
    app.UseExceptionHandler("/error");
    // Remove the AddAzureWebAppDiagnostics line from here
}

// Add this INSTEAD at the TOP of your services configuration (around line ~15)
builder.Logging.AddAzureWebAppDiagnostics();
// ===== END OF ADDED BLOCK =====
app.UseHttpsRedirection();
app.UseIpRateLimiting();
app.UseRouting();
app.UseCors("ReactApp");
app.UseGlobalExceptionMiddleware();
app.UseStaticFiles();
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
        var roles = new[] { "Admin", "Recruiter", "User", "Jobseeker" };

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
if (builder.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var emailService = services.GetRequiredService<EmailService>();

        await emailService.SendEmailAsync("your-email@example.com", "Test Email", "This is a test email.");
    }
}
// ===== ADD THIS BLOCK HERE =====
// Initialize database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<JobPortalContext>();
    db.Database.EnsureCreated(); // For SQLite (simple creation)
    // OR for migrations:
    // db.Database.Migrate();
}
// ===== END OF ADDED BLOCK =====
// In your Program.cs, update the database initialization:
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<JobPortalContext>();
    db.Database.Migrate(); // Use this instead of EnsureCreated()
    
    // Seed roles if they don't exist
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in new[] { "Admin", "Recruiter", "JobSeeker" })
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
// Run the Application
app.Run();
