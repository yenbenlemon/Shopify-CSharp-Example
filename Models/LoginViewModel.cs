namespace ShopifyAppKyle.Models
{
    public class LoginViewModel
    {
        public string ShopDomain { get; set; }
        public string Error { get; set; }
        
        public bool ShowError => !string.IsNullOrEmpty(Error);
    }
}
