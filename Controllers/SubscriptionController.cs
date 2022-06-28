using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ShopifyAppKyle.Attributes;
//using ShopifyAppKyle.Cache;
using ShopifyAppKyle.Data;
using ShopifyAppKyle.Extensions;
using ShopifyAppKyle.Models;
using ShopifySharp;

namespace ShopifyAppKyle.Controllers
{
    [Authorize]
    public class SubscriptionController : Controller
    {
        public SubscriptionController(DataContext userContext, IHostEnvironment environment, IApplicationUrls urls)//, IUserCache cache)
        {
            _dataContext = userContext;
            _environment = environment;
            _urls = urls;
            //_cache = cache;
        }
        
        private readonly DataContext _dataContext;
        private readonly IHostEnvironment _environment;
        private readonly IApplicationUrls _urls;
        //private readonly IUserCache _cache;

        [HttpGet]
        public async Task<IActionResult> Start()
        {
            // Make sure the user isn't already subscribed
            var userSession = HttpContext.User.GetUserSession();

            // We are already subscribed
            if (userSession.IsSubscribed) return RedirectToAction("Index", "Home");
            
            // Lets see the Subscribe View Model
            return View(new SubscribeViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> HandleStartSubscription()
        {
            // Make sure the user isn't already subscribed
            var userSession = HttpContext.User.GetUserSession();
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == userSession.UserId);
            
            if (user.ShopifyChargeId.HasValue)
            {
                // Update the user's session to prevent redirect loops
                await HttpContext.SignInAsync(user);

                // Send them to the home page
                return RedirectToAction("Index", "Home");
            }

            var service = new RecurringChargeService(
                user.ShopifyShopDomain,
                user.ShopifyAccessToken
            );

            var charge = await service.CreateAsync(
                new RecurringCharge
                {
                    TrialDays = 7,
                    Name = "Kyle's Big Susbcription Plan",
                    Price = 9.99M,
                    ReturnUrl = _urls.SubscriptionRedirectUrl,
                    // If the app is running in development mode, make this a test charge
                    Test = _environment.IsDevelopment()
                }
            );

            // Send the user to the charge's confirmation URL
            return Redirect(charge.ConfirmationUrl);
        }

        [HttpGet]
        public async Task<IActionResult> ChargeResult([FromQuery] string shop, [FromQuery] long charge_id)
        {
            // Grab the user from the database
            var userSession = HttpContext.User.GetUserSession();
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == userSession.UserId);
            
            if (userSession.IsSubscribed)
            {
                // The user has already subscribed
                return RedirectToAction("Index", "Home");
            }

            // Get the charge via Shopify API and check its status
            var service = new RecurringChargeService(
                user.ShopifyShopDomain,
                user.ShopifyAccessToken
            );
            var charge = await service.GetAsync(charge_id);
            switch (charge.Status)
            {
                case "pending":
                    // User has not accepted or declined the charge
                    return Redirect(charge.ConfirmationUrl);
                case "expired":
                case "declined":
                    // Send the user back to start a subscription again
                    return RedirectToAction("Start");
                case "active":
                    // User has activated the charge, update their account and session
                    user.ShopifyChargeId = charge_id;
                    user.BillingOn = charge.BillingOn;
                    await _dataContext.SaveChangesAsync();
                    await HttpContext.SignInAsync(user);
                    return RedirectToAction("Index", "Home");
                default:
                    var message = $"Unhandled charge status of {charge.Status}";
                    throw new ArgumentOutOfRangeException(nameof(charge.Status), message);
            }
        }

        [AuthorizeWithActiveSubscription]
        public async Task<IActionResult> Index()
        {
            var userSession = HttpContext.User.GetUserSession();
            var user = await _dataContext.Users.SingleAsync(u => u.Id == userSession.UserId);

            if (userSession.IsSubscribed == false)
            {
                return RedirectToAction("Start");
            }
            
            // Pull in the user's subscription data from Shopify
            var chargeService = new RecurringChargeService(user.ShopifyShopDomain, user.ShopifyAccessToken);
            RecurringCharge charge;

            try
            {
                charge = await chargeService.GetAsync(user.ShopifyChargeId.Value);
            }
            catch (ShopifyException e) when (e.HttpStatusCode == HttpStatusCode.NotFound)
            {
                // The user's subscription no longer exists. Update their user model to delete their charge ID
                user.ShopifyChargeId = null;

                await _dataContext.SaveChangesAsync();
                
                // Update the user's session, then redirect them to the subscription page to accept a new charge
                await HttpContext.SignInAsync(user);

                return RedirectToAction("Start");
            }
            
            return View(new SubscriptionViewModel(charge));
        }
    }
}
