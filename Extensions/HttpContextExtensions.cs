using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using ShopifyAppKyle.Models;

namespace ShopifyAppKyle.Extensions
{
    public static class HttpContextExtensions
    {
        // Sign in via session
        public static async Task SignInAsync(this HttpContext ctx, Session session)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", session.UserId.ToString(), ClaimValueTypes.Integer32),
                new Claim("IsSubscribed", session.IsSubscribed.ToString(), ClaimValueTypes.Boolean)
            };

            var authScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            var identity = new ClaimsIdentity(claims, authScheme);
            var principal = new ClaimsPrincipal(identity);
            
            await ctx.SignInAsync(principal);
        }

        // Sign in via user account, and sessions can be used afterward
        public static async Task SignInAsync(this HttpContext ctx, UserAccount userAccount)
        {
            // Create the session using the user account
            await SignInAsync(ctx, new Session(userAccount));
        }

        public static Session GetUserSession(this ClaimsPrincipal userPrincipal)
        {
            if (!userPrincipal.Identity.IsAuthenticated) { throw new Exception("User is not authenticated, cannot get user session."); }

            /* An inline function that looks for properties on the user principal and converts them 
            to the desired value type (e.g. int, bool, string, etc.) */
            T Find<T>(string claimName, Func<string, T> valueConverter)
            {
                var claim = userPrincipal.Claims.FirstOrDefault(claim => claim.Type == claimName);

                if (claim == null) { throw new NullReferenceException($"Session claim {claimName} was not found.");  }

                return valueConverter(claim.Value);
            }

            var session = new Session
            {    
                // Parse string to int
                UserId = Find("UserId", int.Parse),

                // Parse
                IsSubscribed = Find("IsSubscribed", bool.Parse)
            };

            return session;
        }
    
    }
}

