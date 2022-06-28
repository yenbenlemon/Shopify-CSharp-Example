using System;
using System.Threading.Tasks;
using ShopifyAppKyle.Attributes;
using ShopifyAppKyle.Data;
using ShopifyAppKyle.Extensions;
using ShopifyAppKyle.Models;
using Microsoft.AspNetCore.Authentication;
using ShopifySharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
//using ShopifyAppKyle.Cache;
using ShopifySharp.Filters;

namespace ShopifyAppKyle.Controllers
{
    public class ShopifyController : Controller
    {
        public ShopifyController(DataContext userDb, ISecrets secrets, IApplicationUrls appUrls)//, IUserCache cache)
        {
            _userDb = userDb;
            _secrets = secrets;
            _appUrls = appUrls;
            //_cache = cache;
        }
        
        private readonly DataContext _userDb;
        private readonly ISecrets _secrets;
        private readonly IApplicationUrls _appUrls;
        //private readonly IUserCache _cache;

        [HttpGet, ValidateShopifyRequest]
        public async Task<ActionResult> Handshake([FromQuery] string shop)
        {
            if (string.IsNullOrEmpty(shop))
            {
                return Problem("Request is missing shop querystring parameter.", statusCode: 422);
            }
            
            // Check if the user is already logged in
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                // Check if the user is logged in as the same shop they're attempting to use
                var session = HttpContext.User.GetUserSession();
                var user = await _userDb.Users.FirstAsync(u => u.Id == session.UserId);
                
                // If the shop domains match, the user is already logged in and can be sent to the home page
                if (user.ShopifyShopDomain == shop)
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                
                // If the shop domains don't match, the user likely owns two or more Shopify shops and they're trying
                // to log in to a separate one. Log them out and let them be redirected to the login page.
                await HttpContext.SignOutAsync();
            }

            // The user has either not yet installed the app, is not logged in, or was logged in to a different shop.
            // Send them to the login page
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet, ValidateShopifyRequest]
        public async Task<ActionResult> AuthResult([FromQuery] string shop, [FromQuery] string code, [FromQuery] string state)
        {
            // Check to make sure the state token has not already been used
            var stateToken = await _userDb.LoginStates.FirstOrDefaultAsync(t => t.Token == state);

            if (stateToken == null)
            {
                // This token has already been used. The user must go through the OAuth process again
                return RedirectToAction("HandleLogin", "Auth");
            }
            
            // Delete the token so it can't be used again
            _userDb.LoginStates.Remove(stateToken);
            await _userDb.SaveChangesAsync();
            
            // Exchange the temporary code for a permanent access token
            string accessToken;

            try
            {
                accessToken = await AuthorizationService.Authorize(code, shop, _secrets.ShopifyPublicKey, _secrets.ShopifySecretKey);
            }
            catch (ShopifyException ex) when ((int) ex.HttpStatusCode == 400)
            {
                // The code has already been used or has expired. The user must go through the OAuth process again. 
                return View("/Views/Auth/Login.cshtml", new LoginViewModel
                {
                    Error = "The temporary Shopify install/login token has expired. Please try again.",
                    ShopDomain = shop
                });
            }
            
            // Get the user's shop data so we can use the shop id 
            var shopData = await new ShopService(shop, accessToken).GetAsync();

            // Check to see if a user account already exists for this shop and needs to be updated, or if it needs to be created
            var user = await _userDb.Users.FirstOrDefaultAsync(u => u.ShopifyShopDomain == shop);

            if (user == null)
            {
                // Create the user's account
                user = new UserAccount
                {
                    ShopifyAccessToken = accessToken,
                    ShopifyShopDomain = shop,
                    ShopifyShopId = shopData.Id.Value
                };

                _userDb.Add(user);
            }
            else
            {
                // Update the user's account
                user.ShopifyAccessToken = accessToken;
                user.ShopifyShopDomain = shop;
                user.ShopifyShopId = shopData.Id.Value;
            }

            await _userDb.SaveChangesAsync();
            
            // Sign the new user in
            await HttpContext.SignInAsync(user);
            
            // Delete the shop's status from cache to force a refresh.
            //_cache.ResetShopStatus(user.Id);

            // Check if the AppUninstalled webhook already exists
            var service = new WebhookService(shop, accessToken);
            var topic = "app/uninstalled";
            var existingHooks = await service.ListAsync(new WebhookFilter
            {
                Topic = topic
            });

            if (!existingHooks.Items.Any())
            {
                // Create the AppUninstalled webhook
                await service.CreateAsync(new Webhook
                {
                    Address = _appUrls.AppUninstalledWebhookUrl,
                    Topic = topic
                });
            }

            // Check if the user needs to activate their subscription charge
            if (!user.ShopifyChargeId.HasValue)
            {
                return RedirectToAction("Start", "Subscription");
            }
            
            // User is subscribed, send them to the home page
            return RedirectToAction("Index", "Home");
        }
    }
}
