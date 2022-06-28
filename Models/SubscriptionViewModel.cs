using System;
using ShopifySharp;

namespace ShopifyAppKyle.Models
{
    public class SubscriptionViewModel
    {
        public SubscriptionViewModel(RecurringCharge charge)
        {
            ChargeName = charge.Name;
            Price = charge.Price.Value;
            TestMode = charge.Test == true;
            DateCreated = charge.CreatedAt.Value;
            TrialEndsOn = charge.TrialEndsOn;
        }

        public string ChargeName { get; }
        public decimal Price { get; }
        public bool TestMode { get; }
        
        public DateTimeOffset DateCreated { get; }
        public DateTimeOffset? TrialEndsOn { get; }

        public bool IsTrialing => TrialEndsOn.HasValue;
    }
}

