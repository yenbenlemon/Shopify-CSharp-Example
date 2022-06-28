namespace ShopifyAppKyle.Models
{
    public class SubscribeViewModel
    {
        public string Error { get; set; }

        public bool ShowError => !string.IsNullOrWhiteSpace(Error);
    }
}
