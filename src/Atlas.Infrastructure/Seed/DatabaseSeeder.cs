using Atlas.Infrastructure.Identity;
using Atlas.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Infrastructure.Seed;

public static class DatabaseSeeder
{
    private static readonly string[] Roles =
    [
        "SystemAdministrator", "CompanyAdministrator", "ServiceManager", "Dispatcher",
        "Technician", "Warehouse", "Customer"
    ];

    public static async Task MigrateAndSeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AtlasDbContext>();
        await db.Database.MigrateAsync();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        foreach (var role in Roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var email = configuration["SeedAdmin:Email"] ?? "admin@atlas.local";
        var password = configuration["SeedAdmin:Password"] ?? "Atlas.Admin.2026!";
        var admin = await userManager.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new ApplicationUser { UserName = email, Email = email, DisplayName = "Atlas Administrator", EmailConfirmed = true };
            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
        }
        if (!await userManager.IsInRoleAsync(admin, "SystemAdministrator"))
            await userManager.AddToRoleAsync(admin, "SystemAdministrator");
    }
}
