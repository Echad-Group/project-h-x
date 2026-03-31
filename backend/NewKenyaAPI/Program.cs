using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using NewKenyaAPI.Services;
using ProjectHX.Contexts.MDB;
using System.Text;
using AspNetCoreRateLimit;
using ThirdPartyServices.Interfaces;
using ThirdPartyServices;
using ThirdPartyServices.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Email Service
builder.Services.AddScoped<IEmailService, EmailService>();

// Register third-party services (MailerSend, Twilio, etc.)
builder.Services.RegisterThridParties(builder.Configuration);
builder.Services.AddScoped<CampaignHierarchyService>();
builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<CampaignMessagingService>();
builder.Services.AddScoped<PushNotificationDispatcher>();
builder.Services.AddScoped<LeaderboardService>();
builder.Services.AddScoped<CampaignBootstrapService>();
builder.Services.AddScoped<FaceMatchService>();
builder.Services.AddScoped<AuditLogService>();
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<WarRoomMongoStore>();
builder.Services.AddSingleton<WarRoomCommandService>();
builder.Services.AddSingleton<VerificationReviewService>();
builder.Services.AddHttpClient("twilio-whatsapp");

// Add Role Initialization Service
builder.Services.AddScoped<RoleInitializationService>();
builder.Services.AddHostedService<MessageDeliveryWorker>();
builder.Services.AddHostedService<ComplianceReminderScheduler>();
builder.Services.AddHostedService<WeeklyCommandReportingService>();

// Configure rate limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173", 
                             "http://localhost:5174", "http://127.0.0.1:5174")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite($"{connectionString};Cache=Shared");
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:SecretKey"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "NewKenyaAPI",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "NewKenyaApp",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure HTTP logging (similar to EF Core logging for database operations)
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("Authorization");
    logging.ResponseHeaders.Add("Content-Type");
    logging.ResponseHeaders.Add("Content-Length");
    logging.MediaTypeOptions.AddText("application/json");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

var app = builder.Build();

// Initialize roles
using (var scope = app.Services.CreateScope())
{
    var roleService = scope.ServiceProvider.GetRequiredService<RoleInitializationService>();
    await roleService.InitializeRolesAsync();
    
    // Create default admin (optional - comment out in production)
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var adminEmail = config["DefaultAdmin:Email"] ?? "admin@newkenya.org";
    var adminPassword = config["DefaultAdmin:Password"] ?? "Admin@123456";
    await roleService.CreateDefaultAdminAsync(adminEmail, adminPassword);

    var bootstrapService = scope.ServiceProvider.GetRequiredService<CampaignBootstrapService>();
    await bootstrapService.SeedInitialHierarchyAsync(adminPassword);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Add HTTP logging middleware (logs all endpoint hits with request/response details)
    app.UseHttpLogging();
}
app.UseHttpsRedirection();

app.UseStaticFiles(); // Enable serving static files from wwwroot

app.UseCors("AllowFrontend");

// Add rate limiting middleware
app.UseIpRateLimiting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
