using System;

namespace ShopifyAppKyle.Models
{
    public class OauthState
    {
        public int Id { get; set; }
        public string ShopifyShopDomain { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public string Token { get; set; }
    }
}
