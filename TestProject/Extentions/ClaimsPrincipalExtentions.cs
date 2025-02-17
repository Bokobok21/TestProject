using System.Security.Claims;

namespace TestProject.Extentions
{
    public static class ClaimsPrincipalExtentions
    {
        public static string Id(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

    }
}
