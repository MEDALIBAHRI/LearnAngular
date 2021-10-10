using API.Data;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
       
        public BuggyController(DataContext dataContext): base(dataContext)
        {}

        [HttpGet("server-error")]
        public  ActionResult<string> GetServerError()
        {
            var thing = this._context.Users.Find(-1);
            var thingToReturn = thing.ToString();
            return thingToReturn;
        }
    }
}