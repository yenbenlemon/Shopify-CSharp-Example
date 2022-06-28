/*
* Interface for getting our URLs to use in the testing service
*/

namespace ShopifyAppKyle.Models
{
    public interface IApplicationUrls
    {
        string OauthRedirectUrl { get; }
        string SubscriptionRedirectUrl { get; }
        string AppUninstalledWebhookUrl { get; }
    }
}