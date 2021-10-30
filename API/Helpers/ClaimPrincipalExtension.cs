using System.Security.Claims;

namespace API.Helpers
{
    public static class ClaimPrincipalExtension
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
             return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}