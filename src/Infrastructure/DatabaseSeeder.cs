using Uni.Scan.Application.Interfaces.Services;
using Uni.Scan.Infrastructure.Contexts;
using Uni.Scan.Infrastructure.Helpers;
using Uni.Scan.Infrastructure.Models.Identity;
using Uni.Scan.Shared.Constants.Permission;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Uni.Scan.Shared.Constants.Identity;

namespace Uni.Scan.Infrastructure
{
    public class DatabaseSeeder : IAppInitialiser
    {
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly IStringLocalizer<DatabaseSeeder> _localizer;
        private readonly UniContext _db;
        private readonly UserManager<UniUser> _userManager;
        private readonly RoleManager<UniRole> _roleManager;

        public DatabaseSeeder(
            UserManager<UniUser> userManager,
            RoleManager<UniRole> roleManager,
            UniContext db,
            ILogger<DatabaseSeeder> logger,
            IStringLocalizer<DatabaseSeeder> localizer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _logger = logger;
            _localizer = localizer;
        }

        public void Initialize()
        {
            AddAdministrator();
            AddBasicUser();
            _db.SaveChanges();
        }

        private void AddAdministrator()
        {
            Task.Run(async () =>
            {
                //Check if Role Exists
                var adminRole = new UniRole(RoleConstants.AdministratorRole, _localizer["Administrator role with full permissions"]);
                var adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
                if (adminRoleInDb == null)
                {
                    await _roleManager.CreateAsync(adminRole);
                    adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
                    _logger.LogInformation(_localizer["Seeded Administrator Role."]);
                }
                //Check if User Exists
                var superUser = new UniUser
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    Email = UserConstants.SuperAdminEmail,
                    UserName = "SuperAdmin",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    EmployeeID = "7000013",
                    SiteID="*",
                    CreatedOn = DateTime.Now,
                    IsActive = true
                };
                var superUserInDb = await _userManager.FindByEmailAsync(superUser.Email);
                if (superUserInDb == null)
                {
                    await _userManager.CreateAsync(superUser, UserConstants.DefaultPassword);
                    var result = await _userManager.AddToRoleAsync(superUser, RoleConstants.AdministratorRole);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation(_localizer["Seeded Default SuperAdmin User."]);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError(error.Description);
                        }
                    }
                }
                foreach (var permission in Permissions.GetRegisteredPermissions())
                {
                    if (!permission.Contains("Logistics.Home"))
                    {
                          await _roleManager.AddPermissionClaim(adminRoleInDb, permission);
                    }
                  
                }
            }).GetAwaiter().GetResult();
        }

        private void AddBasicUser()
        {
            Task.Run(async () =>
            {
                //Check if Role Exists
                var basicRole = new UniRole(RoleConstants.BasicRole, _localizer["Basic role with default permissions"]);
                var basicRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.BasicRole);
                if (basicRoleInDb == null)
                {
                    await _roleManager.CreateAsync(basicRole);
                    _logger.LogInformation(_localizer["Seeded Basic Role."]);
                }
                //Check if User Exists
                var basicUser = new UniUser
                {
                    FirstName = "Logistics",
                    LastName = "User",
                    Email = UserConstants.BasicUserEmail,
                    UserName = "Universap",
                    EmployeeID = "7000013",
                    SiteID ="*",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true
                };
                var basicUserInDb = await _userManager.FindByEmailAsync(basicUser.Email);
                if (basicUserInDb == null)
                {
                    await _userManager.CreateAsync(basicUser, UserConstants.BasicPassword);
                    await _userManager.AddToRoleAsync(basicUser, RoleConstants.BasicRole);
                    _logger.LogInformation(_localizer["Seeded User with Basic Role."]);
                }

                //foreach (var permission in Permissions.GetRegisteredPermissions())
                //{
                //    if (permission.Contains("Logistics."))
                //    {
                //        await _roleManager.AddPermissionClaim(basicRoleInDb, permission);
                //    }

                //}
            }).GetAwaiter().GetResult();
        }
    }
}