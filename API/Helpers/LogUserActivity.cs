using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using API.IServices;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context,
         ActionExecutionDelegate next)
        {
            var resultContext = await next();
            if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var username = resultContext.HttpContext.User.GetUsername();
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await repo.GetUserByUsernameAsync(username);
            user.LastActive = System.DateTime.Now;
            repo.Update(user);
            await repo.SaveAllAsync();
        }
    }
}