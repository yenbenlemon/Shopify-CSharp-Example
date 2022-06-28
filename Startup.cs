using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using ShopifyAppKyle.Data;
using ShopifyAppKyle.Models;

namespace ShopifyAppKyle
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add MVC
            services.AddControllersWithViews();

            // Add cookie authentication
            var authScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            
            services
                .AddAuthentication(authScheme)
                .AddCookie(ConfigureCookieAuthentication);
                
            // Add Entity Framework and the DataContext class
            services.AddDbContext<DataContext>(options =>options.UseSqlServer(GetSqlConnectionString()));

            // Add ISecrets and Secrets to Dependency Injection
            services.AddSingleton<ISecrets, Secrets>();

            // Add IApplicationUrls and ApplicationUrls to Dependency Injection
            services.AddSingleton<IApplicationUrls, ApplicationUrls>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Add status and authentication functionality (404, etc)
            app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>{
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }

        private void ConfigureCookieAuthentication(CookieAuthenticationOptions options)
        {
            options.Cookie.HttpOnly = true;
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(1);
            options.LogoutPath = "/Auth/Logout";
            options.LoginPath = "/Auth/Login";
            options.AccessDeniedPath = "/Auth/Login";

            options.Validate();
        }

        private string GetSqlConnectionString()
        {
            var partialConnectionString = Configuration.GetConnectionString("DefaultConnection");
            var password = Configuration.GetValue<string>("SQL_PASSWORD");

            var connStr = new SqlConnectionStringBuilder(partialConnectionString)
            {
                Password = password,
                Authentication = SqlAuthenticationMethod.SqlPassword
            };

            return connStr.ToString();
        }
    }
}