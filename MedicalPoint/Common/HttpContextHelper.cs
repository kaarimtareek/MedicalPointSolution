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
    }
}
