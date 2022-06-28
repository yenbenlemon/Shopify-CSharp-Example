using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShopifyAppKyle.Extensions;
using Microsoft.AspNetCore.Routing;
//using ShopifyAppKyle.Cache;

namespace ShopifyAppKyle.Attributes
{
    public class AuthorizeWithActiveSubscriptionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext ctx)
        {
            // Check if the user is authenticated first
            if (!ctx.HttpContext.User.Identity.IsAuthenticated)
            {
                // The base class will handle basic authentication
                return;
            }
            
            // Get the user's session and check if they're subscribed
            var session = ctx.HttpContext.User.GetUserSession();
            //var cache = (IUserCache) ctx.HttpContext.RequestServices.GetService(typeof(IUserCache));
            //var status = cache.GetShopStatus(session.UserId);

            if (!session.IsSubscribed)
            {
                // Redirect the user to /Subscription/Start where they can start a subscription
                ctx.Result = new RedirectToActionResult("Start", "Subscription", null);
            }
        }
    }
}
