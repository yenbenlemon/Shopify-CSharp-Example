namespace ShopifyAppKyle.Models
{
    public class Session
    {
        public Session(UserAccount user)
        {
            UserId = user.Id;
            ShopifyChargeId = user.ShopifyChargeId;
            IsSubscribed = user.ShopifyChargeId.HasValue;
        }
        
        public Session()
        {
        }

        public int UserId { get; set; }
        public long? ShopifyChargeId { get; set; }
        public bool IsSubscribed { get; set; }
    }
}