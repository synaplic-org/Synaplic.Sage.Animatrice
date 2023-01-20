using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Uni.Scan.Application.Interfaces.Repositories;
using Uni.Scan.Application.Interfaces.Services.Storage;
using Uni.Scan.Application.Interfaces.Services.Storage.Provider;
using Uni.Scan.Application.Interfaces.Serialization.Serializers;
using Uni.Scan.Application.Serialization.JsonConverters;
using Uni.Scan.Infrastructure.Repositories;
using Uni.Scan.Infrastructure.Services.Storage;
using Uni.Scan.Application.Serialization.Options;
using Uni.Scan.Infrastructure.Services.Storage.Provider;
using Uni.Scan.Application.Serialization.Serializers;

namespace Uni.Scan.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructureMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // TODO : Add Repositories
            return services
                .AddTransient(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>))
                .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        }

     

        public static IServiceCollection AddServerStorage(this IServiceCollection services)
            => AddServerStorage(services, null);

        public static IServiceCollection AddServerStorage(this IServiceCollection services, Action<SystemTextJsonOptions> configure)
        {
            return services
                .AddScoped<IJsonSerializer, SystemTextJsonSerializer>()
                .AddScoped<IStorageProvider, ServerStorageProvider>()
                .AddScoped<IServerStorageService, ServerStorageService>()
                .AddScoped<ISyncServerStorageService, ServerStorageService>()
                .Configure<SystemTextJsonOptions>(configureOptions =>
                {
                    configure?.Invoke(configureOptions);
                    if (!configureOptions.JsonSerializerOptions.Converters.Any(c => c.GetType() == typeof(TimespanJsonConverter)))
                        configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                });
        }
    }
}