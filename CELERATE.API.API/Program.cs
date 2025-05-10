using CELERATE.API.API.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;

// AutoMapper için sadece temel namespace
using AutoMapper;

// Projenizdeki diðer önemli namespace'ler
using CELERATE.API.Application.Commands;
using CELERATE.API.Infrastructure.Firebase;
using CELERATE.API.Infrastructure.Firebase.Logging;
using CELERATE.API.CORE.Interfaces;
using CELERATE.API.Application.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add aspire service defaults
builder.AddServiceDefaults();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins",
        policy => policy.WithOrigins(builder.Configuration["AllowedOrigins"]?.Split(',') ?? new string[0])
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
});

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CreateCard", policy => policy.RequireClaim("Permission", "CreateCard"));
    options.AddPolicy("CreateAuthorizedCard", policy => policy.RequireClaim("Permission", "CreateAuthorizedCard"));
    options.AddPolicy("ViewDashboard", policy => policy.RequireClaim("Permission", "ViewDashboard"));
    options.AddPolicy("ViewLogs", policy => policy.RequireClaim("Permission", "ViewLogs"));
    options.AddPolicy("ViewReports", policy => policy.RequireClaim("Permission", "ViewReports"));
    options.AddPolicy("CreateBranch", policy => policy.RequireClaim("Permission", "CreateBranch"));
    options.AddPolicy("AddBalance", policy => policy.RequireClaim("Permission", "AddBalance"));
    options.AddPolicy("SpendBalance", policy => policy.RequireClaim("Permission", "SpendBalance"));
});

// Add MediatR - AddBalanceCommand tipini doðru þekilde belirtin
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CELERATE.API.Application.Commands.AddBalanceCommand).Assembly);
});

// AutoMapper'ý manuel olarak yapýlandýrma (çakýþmalarý önlemek için)
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Add SignalR
builder.Services.AddSignalR();

// Add Firebase services
builder.Services.AddFirebaseServices(builder.Configuration);

// Add JWT Token Generator - Tam namespace kullanýn
builder.Services.AddScoped<CELERATE.API.CORE.Interfaces.IJwtTokenGenerator, CELERATE.API.Infrastructure.Firebase.JwtTokenGenerator>();

// Add Logging Service - Tam namespace kullanýn
builder.Services.AddScoped<CELERATE.API.Infrastructure.Firebase.Logging.LoggingService>();

// Add INotificationService ve Hub implementasyonu
builder.Services.AddScoped<INotificationService, NotificationHub>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");

// Add Firebase Auth Middleware
app.UseMiddleware<FirebaseAuthMiddleware>();

// Add Security Middleware
app.UseMiddleware<SecurityMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

// Map default health check endpoints
app.MapDefaultEndpoints();

app.Run();