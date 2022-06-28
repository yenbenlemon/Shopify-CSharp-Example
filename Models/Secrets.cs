using System;
using Microsoft.Extensions.Configuration;

namespace ShopifyAppKyle
{
    public class Secrets : ISecrets
    {
        public Secrets(IConfiguration config)
        {
            // Try to find the key
            string Find(string key)
            {
                var value = config.GetValue<string>(key);

                if (string.IsNullOrWhiteSpace(value)) { throw new NullReferenceException(key); }

                return value;
            }

            // Gets our keys saved in our secret file
            ShopifySecretKey = Find("SHOPIFY_SECRET_KEY");
            ShopifyPublicKey = Find("SHOPIFY_PUBLIC_KEY");
            HostDomain = Find("HOST_DOMAIN");
        }

        // Getters
        public string ShopifySecretKey { get; }
        public string ShopifyPublicKey { get; }
        public string HostDomain { get; }
    }
}
