/*
* An interface for getting our secrets from our ASP.NET projects secrets file
*/

namespace ShopifyAppKyle
{
    public interface ISecrets
    {
        // Get our API keys for Shopify
        public string ShopifySecretKey { get; }
        public string ShopifyPublicKey { get; }
        public string HostDomain { get; }
    }
}
