using CELERATE.API.API.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

// Application katmaný için gerekli namespace'ler
using AutoMapper;
using CELERATE.API.Application.Commands;
using CELERATE.API.Application.Mappings;
using CELERATE.API.Application.Handlers;

// Infrastructure katmaný için gerekli namespace'ler
using CELERATE.API.Infrastructure.Firebase;
using CELERATE.API.Infrastructure.Firebase.Logging;
using CELERATE.API.Infrastructure.Firebase.Services;
using CELERATE.API.Infrastructure.Firebase.Repositories;

// Core katmaný için gerekli namespace'ler
using CELERATE.API.CORE.Interfaces;
using CELERATE.API.CORE.Entities;

// MediatR için namespace
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Loglama yapýlandýrmasý
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger'ý JWT authorization desteði ile yapýlandýrma
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gördes Belediyesi NFC Kart API",
        Version = "v1",
        Description = "NFC kartlý bakiye iþlemleri yapan API sistemi",
        Contact = new OpenApiContact
        {
            Name = "Gördes Belediyesi",
            Email = "bilgi@gordes.bel.tr"
        }
    });

    // JWT yetkilendirme desteði
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header - Bearer þemasý kullanýlýr. Örnek: \"Authorization: Bearer {token}\"",
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

    // SignalR için JWT token yönetimi
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

// MediatR'ý doðru þekilde yapýlandýrma (tüm handler'larý bulabilmesi için)
builder.Services.AddMediatR(cfg =>
{
    // Application assembly'sini tarayarak tüm handler'larý otomatik olarak kaydet
    cfg.RegisterServicesFromAssembly(typeof(CreateCardCommand).Assembly);
});

// ÖNEMLÝ: MediatR handler'larýný manuel olarak kaydetme (sorun yaþanan handler için)
builder.Services.AddTransient<IRequestHandler<CreateAuthorizedCardCommand, string>, CreateAuthorizedCardHandler>();
builder.Services.AddTransient<IRequestHandler<CreateCardCommand, string>, CreateCardHandler>();
builder.Services.AddTransient<IRequestHandler<AddBalanceCommand, decimal>, AddBalanceHandler>();
builder.Services.AddTransient<IRequestHandler<AuthenticateByCardCommand, CELERATE.API.Application.Models.AuthenticationResult>, AuthenticateByCardHandler>();

// AutoMapper'ý yapýlandýrma
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

// Add JWT Token Generator
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Add Logging Service
builder.Services.AddScoped<LoggingService>();

// Add INotificationService ve Hub implementasyonu
builder.Services.AddSingleton<INotificationService, NotificationHub>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gördes Belediyesi NFC API v1");
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.RoutePrefix = "swagger";
        c.DefaultModelsExpandDepth(-1);
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");

// ÖNEMLÝ: Middleware sýralamasý düzeltildi
// Önce standart Authentication middleware çalýþmalý
app.UseAuthentication();

// Sonra özel middlewares
app.UseMiddleware<FirebaseAuthMiddleware>();
app.UseMiddleware<SecurityMiddleware>();

// Son olarak Authorization middleware
app.UseAuthorization();

// Controller ve SignalR yapýlandýrmasý
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

// Aspire service defaults endpoints
app.MapDefaultEndpoints();

app.Run();