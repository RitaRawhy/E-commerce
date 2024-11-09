using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace APIDemo.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string RetrieveEmailFromPrincipal(this ClaimsPrincipal user)
            => user.FindFirstValue(ClaimTypes.Email);
    }
}
