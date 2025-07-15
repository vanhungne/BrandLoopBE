using BrandLoop.Application;
using BrandLoop.Application.Background;
using BrandLoop.Application.Hubs;
using BrandLoop.Infratructure;
using BrandLoop.Infratructure.Config;
using BrandLoop.Infratructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

public class Program
{
    public static void Main(string[] args)
    {
        // Set global culture
        var globalCulture = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = globalCulture;
        CultureInfo.DefaultThreadCurrentUICulture = globalCulture;

        var builder = WebApplication.CreateBuilder(args);

        // Configuration
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Services.Configure<GroqOptions>(
        builder.Configuration.GetSection(GroqOptions.SectionName));
        // Logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        // Cấu hình SignalR
        builder.Services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
            options.HandshakeTimeout = TimeSpan.FromSeconds(30);
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
        });
        // Services
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        // Update the following line to use the correct reference to the configuration object
        builder.Services.Configure<AccountCleanupOptions>(builder.Configuration.GetSection(AccountCleanupOptions.SectionName));

        // Add custom services
        builder.Services    
            .AddDatabaseServices(builder.Configuration)
            .AddInfratructure().AddApplication();

        // JWT Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
                ),
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine("Token validated successfully");
                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    // 1. Ưu tiên lấy từ query string (dành cho SignalR)
                    var accessToken = context.Request.Query["access_token"];

                    // 2. Nếu không có, lấy từ cookie (cho các API HTTP bình thường)
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        accessToken = context.Request.Cookies["accessToken"];
                    }

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }

            };
        });

        builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

        // CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", policy =>
            {
                policy.WithOrigins("http://localhost:8880", "https://128.199.174.77:2053", "https://localhost:2053"
                    , "https://brandloop.io.vn", "https://www.brandloop.io.vn", "https://139.59.226.2:2053", "http://localhost:5173"
                    , "https://brandloop.pages.dev")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        // Swagger
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SmokeFreeHub",
                Version = "v1.0"
            });

            //
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            c.AddSecurityDefinition("Cookie", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Cookie,
                Description = "Please enter a valid cookie",
                Name = "accessToken",
                Type = SecuritySchemeType.ApiKey
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
                    Array.Empty<string>()
                },
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Cookie"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        //config ure Kestrel server to listen on specific ports

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(8880); // HTTP
            options.ListenAnyIP(2053, listenOptions =>
            {
                listenOptions.UseHttps("/app/https/brandloop.io.vn.pfx", "12345");
            });
        });


        // Set default time zone
        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        var app = builder.Build();

        // Middleware pipeline
        if (app.Environment.IsDevelopment())
        {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            c.RoutePrefix = string.Empty;
            c.InjectJavascript("/swagger/custom-swagger.js");
        });
        }
        app.UseStaticFiles();

        // Use middleware for Swagger
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        });
        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseCors("AllowSpecificOrigin");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapHub<ChatAiHub>("/ChatAiHub");
        app.MapHub<ChatHub>("/chathub");
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.Run();
    }
}
