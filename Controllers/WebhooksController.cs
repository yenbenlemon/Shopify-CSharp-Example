using ShopifySharp;
using System;
using System.Threading.Tasks;
//using ShopifyAppKyle.Cache;
using ShopifyAppKyle.Data;
using ShopifyAppKyle.Models;
using ShopifyAppKyle.Extensions;
using ShopifyAppKyle.Attributes;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ShopifyAppKyle.Controllers
{
    [ValidateShopifyWebhook]
    public class WebhooksController : Controller
    {
        public WebhooksController(
            ISecrets secrets,
            DataContext userContext,
            ILogger<WebhooksController> logger
        )
        {
            _secrets = secrets;
            _dataContext = userContext;
            _logger = logger;
        }

        private readonly ISecrets _secrets;
        private readonly DataContext _dataContext;
        private readonly ILogger<WebhooksController> _logger;

        [HttpPost]
        public async Task<StatusCodeResult> AppUninstalled([FromQuery] string shop)
        {
            if (string.IsNullOrWhiteSpace(shop))
            {
                _logger.LogWarning(
                    "Received AppUninstalled webhook but the shop value was null or empty."
                );
                return Ok();
            }
            // Pull in the user
            var user = await _dataContext.Users.FirstOrDefaultAsync(
                u => u.ShopifyShopDomain == shop
            );
            if (user == null)
            {
                _logger.LogWarning(
                    "Received AppUninstalled webhook for shop {shop}, but a user with that shop value could not be found.",
                    shop
                );
                // User does not exist or may have already been deleted
                return Ok();
            }

            _logger.LogInformation("Handling 'app/uninstalled' webhook for shop {shop}", shop);
            // Delete the user's subscription and Shopify details, leaving their ShopifyShopId intact
            user.ShopifyChargeId = null;
            user.ShopifyAccessToken = null;
            user.ShopifyShopDomain = null;
            user.BillingOn = null;

            // Save the changes and return 200 OK
            await _dataContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<StatusCodeResult> GdprCustomerDataRequest()
        {
            // Deserialize the request body into an object containing the customer's ID
            var data =
                await Request.DeserializeBodyAsync<ShopifySharp.CustomerDataRequestWebhook>();

            // Log the customer ID, the shop ID, and the orders that were made by the customer
            var requestedOrders = string.Join(
                ", ",
                data.OrdersRequested ?? Enumerable.Empty<long>()
            );
            var message =
                "Customer {0} has requested their data via shop {1} ({2}). Orders requested: {3}";
            _logger.LogCritical(
                message,
                data.Customer.Id,
                data.ShopId,
                data.ShopDomain,
                requestedOrders
            );
            return Ok();
        }

        [HttpPost]
        public async Task<StatusCodeResult> GdprCustomerRedacted()
        {
            var data = await Request.DeserializeBodyAsync<ShopifySharp.CustomerRedactedWebhook>();
            var message = "Customer {0} has been redacted via shop {1} ({2})";
            _logger.LogWarning(message, data.Customer.Id, data.ShopId, data.ShopDomain);

            // This app does not currently log customer data, nothing to do here
            return Ok();
        }

        [HttpPost]
        public async Task<StatusCodeResult> GdprShopRedacted()
        {
            var data = await Request.DeserializeBodyAsync<ShopifySharp.ShopRedactedWebhook>();
            var message = "Shop {0} ({1}) has been redacted";
            _logger.LogWarning(message, data.ShopId, data.ShopDomain);

            // Pull in the user and delete their data
            var user = await _dataContext.Users.FirstOrDefaultAsync(
                u => u.ShopifyShopId == data.ShopId
            );
            if (user != null)
            {
                // Delete the user's oauth states and their user record
                var oauthStates = await _dataContext.LoginStates
                    .Where(state => state.ShopifyShopDomain == user.ShopifyShopDomain)
                    .ToListAsync();
                    
                _dataContext.LoginStates.RemoveRange(oauthStates);
                _dataContext.Users.Remove(user);
                await _dataContext.SaveChangesAsync();
            }
            return Ok();
        }
    }
}
