namespace ShopifyAppKyle.Models
{
    /// <summary>
    /// Used to store information about a user's shop status. This information can be safely stored in cache, assuming that you don't 
    /// write code that will let an unauthorized user access data about another user's shop.
    /// </summary>
    public class CachedShopStatus
    {
        public CachedShopStatus(UserAccount user)
        {
            ShopifyShopId = user.ShopifyShopId;
            ShopifyShopDomain = user.ShopifyShopDomain;
            ShopifyAccessToken = user.ShopifyAccessToken;
            ShopifyChargeId = user.ShopifyChargeId;
        }
        
        /// <summary>
        /// The user's Shopify shop id. 
        /// </summary>
        public long ShopifyShopId { get; set; }
        
        ///<summary>
        /// The user's ShopifyAccessToken, received from Shopify's OAuth integration flow.
        ///</summary>
        public string ShopifyAccessToken { get; set; }

        ///<summary>
        /// The user's *.myshopify.com domain.
        ///</summary>
        public string ShopifyShopDomain { get; set; }

        ///<summary>
        ///The id of the user's Shopify subscription charge.
        ///</summary>
        public long? ShopifyChargeId { get; set; }

        //The shop is connected if the access token exists.
        public bool ShopIsConnected => 
            !string.IsNullOrEmpty(ShopifyAccessToken);

        //Billing is connected if the charge id has a value.
        public bool BillingIsConnected =>
            ShopifyChargeId.HasValue;
    }
}
