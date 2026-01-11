using ECommerce.Application.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Application.DependencyInjection
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            return services;
        }
    }
}
