using Blazored.LocalStorage;
using Uni.Scan.Client.Infrastructure.Authentication;
using Uni.Scan.Client.Infrastructure.Managers;
using Uni.Scan.Client.Infrastructure.Managers.Preferences;
using Uni.Scan.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using BlazorDB;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Uni.Scan.Client.Infrastructure.ApiClients;
using Uni.Scan.Client.Infrastructure.Settings;
using Uni.Scan.Transfer.DataModel;
using Uni.Scan.Transfer.DataModel.LogisticTask;

namespace Uni.Scan.Client.Extensions
{
    public static class WebAssemblyHostBuilderExtensions
    {
        private const string ClientName = "UNISCAN";

        public static WebAssemblyHostBuilder AddRootComponents(this WebAssemblyHostBuilder builder)
        {
            builder.RootComponents.Add<App>("#app");

            return builder;
        }

        public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
        {
            builder
                .Services
                .AddLocalization(options => { options.ResourcesPath = "Resources"; })
                .AddAuthorizationCore(options => { RegisterPermissionClaims(options); })
                .AddBlazoredLocalStorage()
                .AddMudServices(configuration =>
                {
                    configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                    configuration.SnackbarConfiguration.HideTransitionDuration = 100;
                    configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
                    configuration.SnackbarConfiguration.VisibleStateDuration = 5000;
                    configuration.SnackbarConfiguration.ShowCloseIcon = true;
                })
                //.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
                .AddScoped<ClientPreferenceManager>()
                .AddScoped<UniStateProvider>()
                .AddScoped<AuthenticationStateProvider, UniStateProvider>()
                .AutoRegisterInterfaces<IApiClient>()
                .AutoRegisterInterfaces<IManager>()
                .AddSingleton<GlobalVariables>()
                .AddLocalDB("UniScan", 1)
                .AddTransient<AuthenticationHeaderHandler>()
                .AddScoped(sp => sp
                    .GetRequiredService<IHttpClientFactory>()
                    .CreateClient(ClientName).EnableIntercept(sp))
                .AddHttpClient(ClientName, client =>
                {
                    client.DefaultRequestHeaders.AcceptLanguage.Clear();
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture
                        ?.TwoLetterISOLanguageName);
                    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                })
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();
            builder.Services.AddHttpClientInterceptor();


            return builder;
        }


        public static IServiceCollection AutoRegisterInterfaces<T>(this IServiceCollection services)
        {
            var @interface = typeof(T);

            var types = @interface
                .Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Service = t.GetInterface($"I{t.Name}"),
                    Implementation = t
                })
                .Where(t => t.Service != null);

            foreach (var type in types)
            {
                if (@interface.IsAssignableFrom(type.Service))
                {
                    services.AddTransient(type.Service, type.Implementation);
                }
            }

            return services;
        }

        private static void RegisterPermissionClaims(AuthorizationOptions options)
        {
            foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c =>
                         c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                {
                    options.AddPolicy(propertyValue.ToString() ?? string.Empty,
                        policy => policy.RequireClaim(Permissions.ClaimType, propertyValue.ToString()));
                }
            }
        }

        public static IServiceCollection AddLocalDB(this IServiceCollection services, string name, int version)
        {
            return services.AddBlazorDB(options =>
            {
                options.Name = name;
                options.Version = version;
                options.StoreSchemas = new List<StoreSchema>()
                {
                    new StoreSchema()
                    {
                        Name = nameof(InventoryTaskDTO),
                        PrimaryKey = "objectID"
                    },
                    new StoreSchema()
                    {
                        Name = nameof(StockAnomalyDTO),
                        PrimaryKey = "objectID"
                    },
                    new StoreSchema()
                    {
                        Name = nameof(LogisticTaskDTO2),
                        PrimaryKey = "objectID"
                    }
                };
            });
        }
    }
}