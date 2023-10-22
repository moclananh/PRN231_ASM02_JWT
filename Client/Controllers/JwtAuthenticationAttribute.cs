using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Client.Controllers
{

    public class JwtAuthenticationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = new RedirectResult("/Users/Login"); // Redirect to login if token is missing
            }

         
            base.OnActionExecuting(context);
        }
    }
}
