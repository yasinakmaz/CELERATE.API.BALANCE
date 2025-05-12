using CELERATE.API.API.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

// AutoMapper i�in sadece temel namespace
using AutoMapper;

// Projenizdeki di�er �nemli namespace'ler
using CELERATE.API.Application.Commands;
using CELERATE.API.Infrastructure.Firebase;
using CELERATE.API.Infrastructure.Firebase.Logging;
using CELERATE.API.CORE.Interfaces;
using CELERATE.API.Application.Mappings;
using Google.Api;
using Google.Cloud.Firestore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger'� JWT authorization deste�i ile yap�land�rma
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "G�rdes Belediyesi NFC Kart API",
        Version = "v1",
        Description = "NFC kartl� bakiye i�lemleri yapan API sistemi",
        Contact = new OpenApiContact
        {
            Name = "G�rdes Belediyesi",
            Email = "bilgi@gordes.bel.tr"
        }
    });

    // JWT yetkilendirme deste�i
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header - Bearer �emas� kullan�l�r. �rnek: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

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

    // SignalR i�in JWT token y�netimi
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
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

// Add MediatR - AddBalanceCommand tipini do�ru �ekilde belirtin
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CELERATE.API.Application.Commands.AddBalanceCommand).Assembly);
});

// AutoMapper'� manuel olarak yap�land�rma (�ak��malar� �nlemek i�in)
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

// Add JWT Token Generator - Tam namespace kullan�n
builder.Services.AddScoped<CELERATE.API.CORE.Interfaces.IJwtTokenGenerator, CELERATE.API.Infrastructure.Firebase.JwtTokenGenerator>();

// Add Logging Service - Tam namespace kullan�n
builder.Services.AddScoped<CELERATE.API.Infrastructure.Firebase.Logging.LoggingService>();

// Add INotificationService ve Hub implementasyonu - �NEML�: NotificationHub singleton olarak kay�tl� olmal�d�r!
builder.Services.AddSingleton<INotificationService, NotificationHub>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "G�rdes Belediyesi NFC API v1");
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.RoutePrefix = "swagger";
        c.DefaultModelsExpandDepth(-1);
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");

app.UseMiddleware<FirebaseAuthMiddleware>();

app.UseMiddleware<SecurityMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

app.MapDefaultEndpoints();

app.Run();