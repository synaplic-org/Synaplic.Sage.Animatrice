using Uni.Scan.Application.Interfaces.Services;
using Uni.Scan.Application.Interfaces.Services.Account;
using Uni.Scan.Application.Interfaces.Services.Identity;
using Uni.Scan.Infrastructure;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Infrastructure.Models.Identity;
using Uni.Scan.Infrastructure.Services;
using Uni.Scan.Infrastructure.Services.Identity;
using Uni.Scan.Server.Localization;
using Uni.Scan.Server.Managers.Preferences;
using Uni.Scan.Server.Permission;
using Uni.Scan.Server.Services;
using Uni.Scan.Server.Settings;
using Uni.Scan.Shared.Constants.Localization;
using Uni.Scan.Shared.Constants.Permission;
using Uni.Scan.Shared.Wrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Uni.Scan.Application.Interfaces.Serialization.Options;
using Uni.Scan.Application.Interfaces.Serialization.Serializers;
using Uni.Scan.Application.Interfaces.Serialization.Settings;
using Uni.Scan.Application.Serialization.JsonConverters;
using Uni.Scan.Application.Serialization.Options;
using Uni.Scan.Application.Serialization.Serializers;
using Uni.Scan.Application.Serialization.Settings;
using Uni.Scan.Infrastructure.Services.Email;
using Uni.Scan.Infrastructure.Services.Inventory;
using Uni.Scan.Infrastructure.Services.Synchronisation;
using Uni.Scan.Infrastructure.Services.v1;
using Uni.Scan.Shared.Configurations;
using Uni.Scan.Infrastructure.Services.Logistics;
using Uni.Scan.Infrastructure.Services.Date;
using Uni.Scan.Infrastructure.Services.Labels;

namespace Uni.Scan.Server.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static async Task<IStringLocalizer> GetRegisteredServerLocalizerAsync<T>(
            this IServiceCollection services) where T : class
        {
            var serviceProvider = services.BuildServiceProvider();
            await SetCultureFromServerPreferenceAsync(serviceProvider);
            var localizer = serviceProvider.GetService<IStringLocalizer<T>>();
            await serviceProvider.DisposeAsync();
            return localizer;
        }

        private static async Task SetCultureFromServerPreferenceAsync(IServiceProvider serviceProvider)
        {
            var storageService = serviceProvider.GetService<ServerPreferenceManager>();
            if (storageService != null)
            {
                // TODO - should implement ServerStorageProvider to work correctly!
                CultureInfo culture;
                var preference = await storageService.GetPreference() as ServerPreference;
                if (preference != null)
                    culture = new CultureInfo(preference.LanguageCode);
                else
                    culture = new CultureInfo(LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ??
                                              LocalizationConstants.DefaultLanguageCode);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
        }

        internal static IServiceCollection AddServerLocalization(this IServiceCollection services)
        {
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(ServerLocalizer<>));
            return services;
        }

        internal static AppConfiguration GetApplicationSettings(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
            services.Configure<AppConfiguration>(applicationSettingsConfiguration);
            return applicationSettingsConfiguration.Get<AppConfiguration>();
        }

        internal static void RegisterSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(async c =>
            {
                //TODO - Lowercase Swagger Documents
                //c.DocumentFilter<LowercaseDocumentFilter>();
                //Refer - https://gist.github.com/rafalkasa/01d5e3b265e5aa075678e0adfd54e23f

                // include all project's xml comments
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!assembly.IsDynamic)
                    {
                        var xmlFile = $"{assembly.GetName().Name}.xml";
                        var xmlPath = Path.Combine(baseDirectory, xmlFile);
                        if (File.Exists(xmlPath))
                        {
                            c.IncludeXmlComments(xmlPath);
                        }
                    }
                }

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "universap",
                    License = new OpenApiLicense
                    {
                        Name = $"Copyright (c) {DateTime.Now.Year} Universap",
                        Url = new Uri("https://universap.fr")
                    }
                });

                var localizer = await GetRegisteredServerLocalizerAsync<ServerCommonResources>(services);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description =
                        localizer[
                            "Input your Bearer token in this format - Bearer {your token here} to access this API"],
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    },
                });
            });
        }

        internal static IServiceCollection AddSerialization(this IServiceCollection services)
        {
            services
                .AddScoped<IJsonSerializerOptions, SystemTextJsonOptions>()
                .Configure<SystemTextJsonOptions>(configureOptions =>
                {
                    if (!configureOptions.JsonSerializerOptions.Converters.Any(c =>
                            c.GetType() == typeof(TimespanJsonConverter)))
                        configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                });
            services.AddScoped<IJsonSerializerSettings, NewtonsoftJsonSettings>();

            services.AddScoped<IJsonSerializer, SystemTextJsonSerializer>(); // you can change it
            return services;
        }

        internal static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
            => services
                .AddDbContext<UniContext>(options => options
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                .AddTransient<IAppInitialiser, DatabaseSeeder>();

        internal static IServiceCollection AddCurrentUserService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }

        internal static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services
                .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
                .AddIdentity<UniUser, UniRole>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<UniContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        internal static IServiceCollection AddSharedInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<IDateTimeService, SystemDateTimeService>();
            services.Configure<MailConfiguration>(configuration.GetSection("MailConfiguration"));
            services.AddTransient<IMailService, SmtpMailService>();
            return services;
        }

        internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<ILogisticTaskLabelService, LogisticTaskLabelService>();
            services.AddTransient<ILogisticAreaService, LogisticAreaService>();
            services.AddTransient<ILogisticTasDetailService, LogisticTasDetailService>();
            services.AddTransient<IMaterialService, MaterialService>();
            services.AddTransient<IRoleClaimService, RoleClaimService>();
            services.AddTransient<ITokenService, IdentityService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUploadService, UploadService>();
            services.AddTransient<IAuditService, AuditService>();

            services.AddScoped<IExcelService, ExcelService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IInventoryService, InventoryService>();
            services.AddTransient<ITemplateService, TemplateService>();
            services.AddTransient<IStockAnomalyService, StockAnomalyService>();
            services.AddTransient<ILabelTemplateService, LabelTemplateService>();
            services.AddTransient<ILogisticParametreService, LogisticParametreService>();
            services.AddTransient<ILogisticLabelService, LogisticLabelService>();


            services.AddTransient<ILogisticTaskService, LogisticTaskService>();

            services.AddTransient<ISyncService, SyncService>();
            services.AddTransient<SyncLogisticTaskService>();
            services.AddTransient<SyncInventoryTaskService>();
            services.AddTransient<SyncMasterDataService>();

            services.AddTransient<ILabelTemplateService, LabelTemplateService>();
            services.AddTransient<ILogisticParametreService, LogisticParametreService>();
            services.AddTransient<IScanningCodeService, ScanningCodeService>();
            return services;
        }

        internal static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services, AppConfiguration config)
        {
            var key = Encoding.ASCII.GetBytes(config.Secret);
            services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(async bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.Zero
                    };

                    var localizer = await GetRegisteredServerLocalizerAsync<ServerCommonResources>(services);

                    bearer.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = c =>
                        {
                            if (c.Exception is SecurityTokenExpiredException)
                            {
                                c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                c.Response.ContentType = "application/json";
                                var result =
                                    JsonConvert.SerializeObject(Result.Fail(localizer["The Token is expired."]));
                                return c.Response.WriteAsync(result);
                            }
                            else
                            {
                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                c.Response.ContentType = "application/json";
                                var result =
                                    JsonConvert.SerializeObject(
                                        Result.Fail(localizer["An unhandled error has occurred."]));
                                return c.Response.WriteAsync(result);
                            }
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";
                                var result =
                                    JsonConvert.SerializeObject(Result.Fail(localizer["You are not Authorized."]));
                                return context.Response.WriteAsync(result);
                            }

                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            context.Response.ContentType = "application/json";
                            var result =
                                JsonConvert.SerializeObject(
                                    Result.Fail(localizer["You are not authorized to access this resource."]));
                            return context.Response.WriteAsync(result);
                        },
                    };
                });
            services.AddAuthorization(options =>
            {
                // Here I stored necessary permissions/roles in a constant
                foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c =>
                             c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    var propertyValue = prop.GetValue(null);
                    if (propertyValue is not null)
                    {
                        options.AddPolicy(propertyValue.ToString(),
                            policy => policy.RequireClaim(Permissions.ClaimType, propertyValue.ToString()));
                    }
                }
            });
            return services;
        }
    }
}