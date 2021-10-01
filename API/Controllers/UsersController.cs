using Microsoft.AspNetCore.Mvc;
using API.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.Entities;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    public class UsersController: BaseApiController
    {

        public UsersController(DataContext context):base(context)
        {
            
        }
     
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
            return await _context.Users.ToListAsync();
        }
        [HttpGet("{id}")]
        [Authorize]
         public async Task<ActionResult<AppUser>> GetUser(int id){
            return await _context.Users.FindAsync(id);
        }
    }
}