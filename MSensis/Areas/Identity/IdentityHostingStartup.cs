using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSensis.Models;

[assembly: HostingStartup(typeof(MSensis.Areas.Identity.IdentityHostingStartup))]
namespace MSensis.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<MSensisContext>(options =>
                     
            options.UseMySql(
                   context.Configuration.GetConnectionString("DefaultConnection2")));

            services.AddIdentity<User, IdentityRole>()
                     .AddEntityFrameworkStores<MSensisContext>(); 

                });
        }
    }
}