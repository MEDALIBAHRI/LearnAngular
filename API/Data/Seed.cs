using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if(await context.Users.AnyAsync()) return;
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            foreach(var item in users){
                var hmac = new HMACSHA512();
                item.UserName = item.UserName.ToLower();
                item.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password"));
                item.PasswordSalt = hmac.Key;
                context.Users.Add(item); 
            }
            await context.SaveChangesAsync();
        }
    }
}