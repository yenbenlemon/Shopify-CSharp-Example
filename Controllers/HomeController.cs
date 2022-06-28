using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopifyAppKyle.Attributes;
using ShopifyAppKyle.Data;
using ShopifyAppKyle.Extensions;
using ShopifyAppKyle.Models;
using ShopifySharp;
using ShopifySharp.Filters;

namespace ShopifyAppKyle.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _dataContext;

        public HomeController(ILogger<HomeController> logger, DataContext userContext)
        {
            _logger = logger;
            _dataContext = userContext;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var userRequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            return View(new ErrorViewModel { RequestId = userRequestId });
        }

        [AuthorizeWithActiveSubscription]
        public async Task<IActionResult> Index([FromQuery] string pageInfo = null)
        {
            var userSession = HttpContext.User.GetUserSession();
            var user = await _dataContext.Users.FirstAsync(u => u.Id == userSession.UserId);
            var service = new OrderService(user.ShopifyShopDomain, user.ShopifyAccessToken);

            // Build a list filter to get the requested page of orders
            var limit = 25;

            // Only get the fields we'll use in the OrderSummary model
            var orderFields = "name,id,customer,line_items,created_at";

            //Grab the orders
            var allOrders = new List<Order>();
            var orders = await service.ListAsync(new OrderListFilter 
            { 
                Limit = limit,
                Fields = orderFields
            });

            while (true)
            {
                allOrders.AddRange(orders.Items);

                if (!orders.HasNextPage) break;

                orders = await service.ListAsync(orders.GetNextPageFilter());
            }

            return View(
                new HomeViewModel
                {
                    Orders = orders.Items.Select(o => new OrderSummary(o)),
                    NextPage = orders.GetNextPageFilter()?.PageInfo,
                    PreviousPage = orders.GetPreviousPageFilter()?.PageInfo
                }
            );
        }
    }
}
