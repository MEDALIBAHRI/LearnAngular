using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if(await userManager.Users.AnyAsync()) return;
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            List<AppRole> roles = new List<AppRole>{
                new AppRole{Name = "Member"},
                new AppRole{Name = "Moderator"},
                new AppRole{Name = "Admin"}
            };
            foreach(var item in roles)
            {
               await roleManager.CreateAsync(item);
            }
            foreach(var item in users){
                item.UserName = item.UserName.ToLower();
                await userManager.CreateAsync(item, "Pa$$w0rd");
                await userManager.AddToRoleAsync(item, "Member");
            }

            AppUser admin = new AppUser{
                UserName="admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new []{"Admin", "Moderator"});
        }
    }
}