using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShopifyAppKyle.Models;
using ShopifyAppKyle.Extensions;
using ShopifySharp;
using System.IO;
using System.Text;
using System.Web;
using System.Net;
using Newtonsoft.Json;

namespace ShopifyAppKyle.Attributes
{
    public class ValidateShopifyWebhookAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync( ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // This might be bugged
            var rawBody = await context.HttpContext.Request.ReadRawBodyAsync();

            // Get an instance of the ISecrets interface from Dependency Injection
            var secrets = (ISecrets)context.HttpContext.RequestServices.GetService(typeof(ISecrets));

            var isAuthentic = AuthorizationService.IsAuthenticWebhook(
                context.HttpContext.Request.Headers,
                rawBody,
                secrets.ShopifySecretKey
            );
            if (isAuthentic)
            {
                // Request passed validation, let it continue on to the controller
                await next();
            }
            else
            {
                // Request did not pass validation. Return a JSON error message
                context.HttpContext.Response.ContentType = "application/json";
                var body = JsonConvert.SerializeObject(
                    new { message = "Webhook did not pass validation.", ok = false }
                );
                // Copy the JSON error message to the response body
                using (var buffer = new MemoryStream(Encoding.UTF8.GetBytes(body)))
                {
                    context.HttpContext.Response.StatusCode = 401;
                    await buffer.CopyToAsync(context.HttpContext.Response.Body);
                }
            }
        }
    }
}
