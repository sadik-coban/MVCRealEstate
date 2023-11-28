using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVCRealEstateData;
using System.Security.Claims;

namespace MVCRealEstate;

public static class AppExtensions
{
    public static IApplicationBuilder UseMVCRealEstate(this IApplicationBuilder builder)
    {

        using var scope = builder.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        context.Database.Migrate();

        roleManager.CreateAsync(new Role { Name = "Administrators" }).Wait();
        roleManager.CreateAsync(new Role { Name = "Members" }).Wait();

        var user = new User
        {
            UserName = configuration.GetValue<string>("Security:DefaultUser:UserName"),
            Email = configuration.GetValue<string>("Security:DefaultUser:UserName"),
            Name = configuration.GetValue<string>("Security:DefaultUser:Name"),
            EmailConfirmed = true
        };

        var claims = new[]
        {
            new Claim(ClaimTypes.GivenName, user.Name),
        };



        userManager.CreateAsync(user, configuration.GetValue<string>("Security:DefaultUser:Password")).Wait(); ;
        userManager.AddToRoleAsync(user, "Administrators").Wait();
        userManager.AddClaimsAsync(user, claims).Wait();



        return builder;
    }
}
