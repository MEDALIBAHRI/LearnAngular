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
            var unitOfWork = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            user.LastActive = System.DateTime.Now;
            unitOfWork.UserRepository.Update(user);
            await unitOfWork.Complete();
        }
    }
}