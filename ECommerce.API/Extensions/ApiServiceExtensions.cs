using System.Reflection;
using Microsoft.OpenApi.Models;

namespace ECommerce.API.Extensions
{
    public static class ApiServiceExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddControllers();
            services.AddSwaggerDocumentation();
            services.AddCorsPolicy(config);
            return services;
        }

        private static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "E-Commerce API",
                    Version = "v1",
                    Description = "E-Commerce Application API"
                });

                var applicationXmlFile = $"{Assembly.Load("ECommerce.Application").GetName().Name}.xml";
                var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXmlFile);
                if (File.Exists(applicationXmlPath))
                    options.IncludeXmlComments(applicationXmlPath);

                var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);
                if (File.Exists(apiXmlPath))
                    options.IncludeXmlComments(apiXmlPath);

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token in the format: Bearer {token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        private static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration config)
        {
            var allowedOrigins = config.GetSection("CorsSettings:AllowedOrigins").Get<string[]>()
                ?? new[] { "http://localhost:3000" };

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            return services;
        }
    }
}
