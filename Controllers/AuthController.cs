using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ShopifyAppKyle.Data;
using ShopifyAppKyle.Models;
using ShopifySharp;

namespace ShopifyAppKyle.Controllers
{
    public class AuthController : Controller
    {
        public AuthController(DataContext userContext, IApplicationUrls appUrls, ISecrets secrets)
        {
            _dataContext = userContext;
            _appUrls = appUrls;
            _secrets = secrets;
        }

        private readonly DataContext _dataContext;
        private readonly IApplicationUrls _appUrls;
        private readonly ISecrets _secrets;

        [HttpGet]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Login([FromQuery] string shop = null)
        {
            return View(new LoginViewModel
            {
                ShopDomain = shop
            });
        }

        [HttpPost]
        public async Task<ActionResult> HandleLogin([FromForm] string shop)
        {
            // Check to make sure the domain they entered is a real Shopify store. This will prevent accidentally redirecting
            // the user away to a bad website. 
            if (!await AuthorizationService.IsValidShopDomainAsync(shop))
            {
                return View("Login", new LoginViewModel
                {
                    ShopDomain = shop,
                    Error = $"It looks like {shop} is not a valid Shopify shop domain."
                });
            }

            // This user account doesn't exist. Send them to Shopify to start the OAuth installation process
            // 1. Create a list of permissions to request from them when installing
            // 2. Create an OauthState record to ensure the login request can only be used once
            // 3. Save the new oauth state record
            // 4. Redirect them to the OAuth URL
            var requiredPermissions = new [] { "read_orders" };
            var oauthState = await _dataContext.LoginStates.AddAsync(new OauthState
            {
                DateCreated = DateTimeOffset.Now,
                Token = Guid.NewGuid().ToString()
            });

            await _dataContext.SaveChangesAsync();
            
            var oauthUrl = AuthorizationService.BuildAuthorizationUrl(
                requiredPermissions, 
                shop,
                _secrets.ShopifyPublicKey, 
                _appUrls.OauthRedirectUrl, 
                oauthState.Entity.Token);

            return Redirect(oauthUrl.ToString());
        }
    }
}
