using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(option=>{
                option.Password.RequireNonAlphanumeric = false;
            })
                    .AddRoles<AppRole>()
                    .AddRoleManager<RoleManager<AppRole>>()
                    .AddSignInManager<SignInManager<AppUser>>()
                    .AddRoleValidator<RoleValidator<AppRole>>()
                    .AddEntityFrameworkStores<DataContext>();
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                options.Events = new JwtBearerEvents{
                    OnMessageReceived = conetxt =>{
                        var accessToken = conetxt.Request.Query["access_token"];
                        var path = conetxt.Request.Path;
                        if(!string.IsNullOrEmpty(accessToken) 
                            && path.StartsWithSegments("/hubs"))
                        {
                            conetxt.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(option=>{
                    option.AddPolicy("RequireAdminRole", policy=>policy.RequireRole("Admin"));
                    option.AddPolicy("ModeratorPhotosRole", policy=>policy.RequireRole("Admin", "Moderator"));
            });
            return services;
        }
    }
}