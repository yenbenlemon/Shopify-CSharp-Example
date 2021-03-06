#pragma checksum "F:\Shopify-Projects\ShopifyAppKyle\Views\Subscription\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1caaaa55e2706d9c1bdeba1618b6836bd345b274"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Subscription_Index), @"mvc.1.0.view", @"/Views/Subscription/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "F:\Shopify-Projects\ShopifyAppKyle\Views\_ViewImports.cshtml"
using ShopifyAppKyle;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "F:\Shopify-Projects\ShopifyAppKyle\Views\_ViewImports.cshtml"
using ShopifyAppKyle.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1caaaa55e2706d9c1bdeba1618b6836bd345b274", @"/Views/Subscription/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"d58a655211ab37cf09b14b038e3081c117265fdb", @"/Views/_ViewImports.cshtml")]
    public class Views_Subscription_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<SubscriptionViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "F:\Shopify-Projects\ShopifyAppKyle\Views\Subscription\Index.cshtml"
  
    ViewData["Title"] = "Your Subscription";
    // Format the decimal price to two places, e.g. "14" becomes "14.00"
    var formattedPrice = "$" + Model.Price.ToString("f2");

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div class=\"text-center\">\r\n    <h2>Your Subscription Details</h2>\r\n    <h5>\r\n");
#nullable restore
#line 11 "F:\Shopify-Projects\ShopifyAppKyle\Views\Subscription\Index.cshtml"
         if (Model.IsTrialing)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <span class=\"badge badge-pill badge-primary\">Free Trial</span>\r\n");
#nullable restore
#line 14 "F:\Shopify-Projects\ShopifyAppKyle\Views\Subscription\Index.cshtml"
        }

#line default
#line hidden
#nullable disable
#nullable restore
#line 15 "F:\Shopify-Projects\ShopifyAppKyle\Views\Subscription\Index.cshtml"
         if (Model.TestMode)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <span class=\"badge badge-pill badge-secondary\">Test Mode</span>\r\n");
#nullable restore
#line 18 "F:\Shopify-Projects\ShopifyAppKyle\Views\Subscription\Index.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </h5>\r\n    <hr />\r\n</div>\r\n\r\n<div>\r\n    <ul>\r\n        <li>\r\n            Plan: ");
#nullable restore
#line 26 "F:\Shopify-Projects\ShopifyAppKyle\Views\Subscription\Index.cshtml"
             Write(Model.ChargeName);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </li>\r\n        <li>\r\n            Price: ");
#nullable restore
#line 29 "F:\Shopify-Projects\ShopifyAppKyle\Views\Subscription\Index.cshtml"
              Write(formattedPrice);

#line default
#line hidden
#nullable disable
            WriteLiteral(" / month\r\n        </li>\r\n        <li>\r\n            Subscribed on: ");
#nullable restore
#line 32 "F:\Shopify-Projects\ShopifyAppKyle\Views\Subscription\Index.cshtml"
                      Write(Model.DateCreated.ToString());

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </li>\r\n");
#nullable restore
#line 34 "F:\Shopify-Projects\ShopifyAppKyle\Views\Subscription\Index.cshtml"
         if (Model.IsTrialing)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <li>\r\n            Trial ends on: ");
#nullable restore
#line 37 "F:\Shopify-Projects\ShopifyAppKyle\Views\Subscription\Index.cshtml"
                      Write(Model.TrialEndsOn.ToString());

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </li>\r\n");
#nullable restore
#line 39 "F:\Shopify-Projects\ShopifyAppKyle\Views\Subscription\Index.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </ul>\r\n</div>\r\n\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<SubscriptionViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
