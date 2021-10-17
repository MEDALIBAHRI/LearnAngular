using API.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;
       
        public BuggyController(DataContext context)
        {
            this._context = context;
        }
            

        [HttpGet("server-error")]
        public  ActionResult<string> GetServerError()
        {
            var thing = this._context.Users.Find(-1);
            var thingToReturn = thing.ToString();
            return thingToReturn;
        }
    }
}