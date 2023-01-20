using Microsoft.Extensions.DependencyInjection;
using Uni.Scan.Infrastructure.ByDesign.Service;

namespace Uni.Scan.Infrastructure.ByDesign.Extentions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBydesignServices(this IServiceCollection services)
        {
            services.AddScoped<BydIdentifiedStockService>();
            services.AddScoped<BydInventoryService>();
            services.AddScoped<BydLogisticTaskService>();
            services.AddScoped<BydMaterialService>();
            services.AddScoped<BydStockService>();

            return services;
        }
    }
}