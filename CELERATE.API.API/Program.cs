// CELERATE.API.API/Program.cs
using CELERATE.API.API.Hubs;
using CELERATE.API.Infrastructure.Firebase;
using CELERATE.API.Application.Mappings;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Microsoft.OpenApi.Models;
using CELERATE.API.API.Models;
using CELERATE.API.Application.Commands;
using CELERATE.API.Infrastructure.Firebase.Services;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Serilog konfigürasyonu
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Servislerin eklenmesi
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateCardCommandValidator>());

// CORS politikasý
string[] allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string>()?.Split(',')
                          ?? new[] { "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Authorization policies
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

// Firebase entegrasyonu
builder.Services.AddFirebaseServices(builder.Configuration);

// SignalR
builder.Services.AddSignalR();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateCardCommand).Assembly));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Gördes Belediyesi NFC API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// DI Container kayýtlarý
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddHostedService<FirebaseRealtimeService>();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

// Güvenlik middleware'leri
app.UseMiddleware<SecurityMiddleware>();
app.UseMiddleware<FirebaseAuthMiddleware>();

// Exception handling
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            Log.Error($"Something went wrong: {contextFeature.Error}");
            await context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error."
            }.ToString());
        }
    });
});

// SignalR hub endpoint
app.MapHub<NotificationHub>("/hubs/notifications");

app.MapControllers();

app.Run();