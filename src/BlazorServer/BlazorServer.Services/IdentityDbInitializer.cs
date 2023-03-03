using BlazorServer.Common;
using BlazorServer.DataAccess;
using BlazorServer.Entities;
using BlazorServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BlazorServer.Services;

public class IdentityDbInitializer : IIdentityDbInitializer
{
    private readonly IOptions<AdminUserSeed> _adminUserSeedOptions;
    private readonly ApplicationDbContext _dbContext;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityDbInitializer(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<AdminUserSeed> adminUserSeedOptions)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _adminUserSeedOptions = adminUserSeedOptions ?? throw new ArgumentNullException(nameof(adminUserSeedOptions));
    }

    public async Task SeedDatabaseWithAdminUserAsync()
    {
        if (_dbContext.Roles.Any(role => role.Name == ConstantRoles.Admin))
        {
            return;
        }

        await _roleManager.CreateAsync(new IdentityRole(ConstantRoles.Admin));
        await _roleManager.CreateAsync(new IdentityRole(ConstantRoles.Customer));
        await _roleManager.CreateAsync(new IdentityRole(ConstantRoles.Employee));

        var password = _adminUserSeedOptions.Value.Password ??
                       throw new InvalidOperationException("_adminUserSeedOptions.Value.Password is null");
        await _userManager.CreateAsync(new ApplicationUser
                                       {
                                           UserName = _adminUserSeedOptions.Value.UserName,
                                           Email = _adminUserSeedOptions.Value.Email,
                                           EmailConfirmed = true,
                                       },
                                       password);

        var user = await _dbContext.Users.FirstAsync(u => u.Email == _adminUserSeedOptions.Value.Email);
        await _userManager.AddToRoleAsync(user, ConstantRoles.Admin);
        await _userManager.AddToRoleAsync(user, ConstantRoles.Employee);
    }
}