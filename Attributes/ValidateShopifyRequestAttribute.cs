using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShopifyAppKyle.Models;
using ShopifySharp;

namespace ShopifyAppKyle.Attributes
{
    public class ValidateShopifyRequestAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
        {
            var secrets = (ISecrets)ctx.HttpContext.RequestServices.GetService(typeof(ISecrets));

            var querystring = ctx.HttpContext.Request.Query;
            var isAuthentic = AuthorizationService.IsAuthenticRequest(querystring, secrets.ShopifySecretKey);

            if (isAuthentic)
            {
                // Call the delegate to let the request go through to the next action
                await next();
            }
            else
            {
                // Forbid the request, showing a login screen
                ctx.Result = new ForbidResult();
            }
        }
    }
}