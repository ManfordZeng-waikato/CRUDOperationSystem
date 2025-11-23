using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDOperationSystem.Filters.AuthorizationFilters
{
    public class TokenAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Request.Cookies.ContainsKey("Auth-Key") == false)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                return;
            }

            string? authKey = context.HttpContext.Request.Cookies["Auth-Key"];
            if (authKey != "My-AuthKey-1234")
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }

        }
    }
}
