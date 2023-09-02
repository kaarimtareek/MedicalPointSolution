using System.Security.Claims;

namespace MedicalPoint.Common
{
    public static class HttpContextHelper
    {
        public static int? GetUserId(this HttpContext context)
        {
          var userIdString =  context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null)
            {
                return null;
            }
            var userId = int.Parse(userIdString);
            return userId;
        }
        public static bool IsUserHasRoles(this HttpContext context, string[] roles)
        {
            return context.User.Claims.Any(x=> roles.Contains(x.Value));
        }
        public static string GetUserClaimValue(this HttpContext context,string claimType)
        {
            var value =  context.User.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;
            return value;
        }
    }
}
