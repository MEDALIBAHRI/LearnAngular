using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        public AdminController(UserManager<AppUser> userManager)
        {
            this._userManager = userManager;
            
        }
            [Authorize(Policy = "RequireAdminRole")]
            [HttpGet("users-with-roles")]
            public async Task<ActionResult> GeUsersWithRoles()
            {
                var users = await _userManager.Users
                            //.Include(x=>x.UserRoles)
                            .OrderBy(x=> x.UserName)
                            .Select(x=> new{
                                Id = x.Id,
                                Username = x.UserName,
                                Roles = x.UserRoles.Select(y=>y.Role.Name)
                            })
                            .ToListAsync();
                return Ok(users);
            }

            [Authorize(Policy = "ModeratorPhotosRole")]
            [HttpGet("photos-to-moderate")]
            public ActionResult GeUsersWithPhotos()
            {
                return Ok("only admin or moderator");
            }

            [HttpPost("edit-roles/{username}")]
            public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
            {
                var user = await _userManager.FindByNameAsync(username);

                if(user == null)
                  return NotFound("Could find user");

                 var oldRoles = await _userManager.GetRolesAsync(user);
                 var newRoles = roles.Split(',').ToArray();

                var result = await  _userManager.AddToRolesAsync(user, newRoles.Except(oldRoles));

                if(!result.Succeeded)
                return BadRequest("Failed to add roles");

                result = await _userManager.RemoveFromRolesAsync(user, oldRoles.Except(newRoles));

                if(!result.Succeeded)
                return BadRequest("Failed to remove roles");

                return Ok(await _userManager.GetRolesAsync(user));
            }
    }
}