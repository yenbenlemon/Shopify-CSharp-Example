using System.Text.RegularExpressions;

namespace ShopifyAppKyle.Models
{
    public class ApplicationUrls : IApplicationUrls
    {
        public ApplicationUrls(ISecrets secrets)
        {
            // These are our redirection URLs
            OauthRedirectUrl = JoinUrls(secrets.HostDomain, "/shopify/authresult");
            SubscriptionRedirectUrl = JoinUrls(secrets.HostDomain, "/subscription/chargeresult");
            AppUninstalledWebhookUrl = JoinUrls(secrets.HostDomain, "/webhooks/appuninstalled");
        }

        string JoinUrls(string left, string right)
        {
            // Helper class that will remove double slashes and clean url text
            var trimTrailingSlash = new Regex("/+$");
            var trimLeadingSlash = new Regex("^/+");

            return trimTrailingSlash.Replace(left, "") + "/" + trimLeadingSlash.Replace(right, "");
        }

        // Getters
        public string OauthRedirectUrl { get; }
        public string SubscriptionRedirectUrl { get; }
        public string AppUninstalledWebhookUrl { get; }
    }
}