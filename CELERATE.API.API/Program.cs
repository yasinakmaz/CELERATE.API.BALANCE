using CELERATE.API.API.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

// Application katman� i�in gerekli namespace'ler
using AutoMapper;
using CELERATE.API.Application.Commands;
using CELERATE.API.Application.Mappings;
using CELERATE.API.Application.Handlers;

// Infrastructure katman� i�in gerekli namespace'ler
using CELERATE.API.Infrastructure.Firebase;
using CELERATE.API.Infrastructure.Firebase.Logging;
using CELERATE.API.Infrastructure.Firebase.Services;
using CELERATE.API.Infrastructure.Firebase.Repositories;

// Core katman� i�in gerekli namespace'ler
using CELERATE.API.CORE.Interfaces;
using CELERATE.API.CORE.Entities;

// MediatR i�in namespace
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Loglama yap�land�rmas�
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

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

// MediatR'� do�ru �ekilde yap�land�rma (t�m handler'lar� bulabilmesi i�in)
builder.Services.AddMediatR(cfg =>
{
    // Application assembly'sini tarayarak t�m handler'lar� otomatik olarak kaydet
    cfg.RegisterServicesFromAssembly(typeof(CreateCardCommand).Assembly);
});

// �NEML�: MediatR handler'lar�n� manuel olarak kaydetme (sorun ya�anan handler i�in)
builder.Services.AddTransient<IRequestHandler<CreateAuthorizedCardCommand, string>, CreateAuthorizedCardHandler>();
builder.Services.AddTransient<IRequestHandler<CreateCardCommand, string>, CreateCardHandler>();
builder.Services.AddTransient<IRequestHandler<AddBalanceCommand, decimal>, AddBalanceHandler>();
builder.Services.AddTransient<IRequestHandler<AuthenticateByCardCommand, CELERATE.API.Application.Models.AuthenticationResult>, AuthenticateByCardHandler>();

// AutoMapper'� yap�land�rma
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "G�rdes Belediyesi NFC API v1");
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.RoutePrefix = "swagger";
        c.DefaultModelsExpandDepth(-1);
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowedOrigins");

// �NEML�: Middleware s�ralamas� d�zeltildi
// �nce standart Authentication middleware �al��mal�
app.UseAuthentication();

// Sonra �zel middlewares
app.UseMiddleware<FirebaseAuthMiddleware>();
app.UseMiddleware<SecurityMiddleware>();

// Son olarak Authorization middleware
app.UseAuthorization();

// Controller ve SignalR yap�land�rmas�
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

// Aspire service defaults endpoints
app.MapDefaultEndpoints();

app.Run();