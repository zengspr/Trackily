using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trackily.Areas.Identity.Data;
using Trackily.Data;

[assembly: HostingStartup(typeof(Trackily.Areas.Identity.IdentityHostingStartup))]
namespace Trackily.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<TrackilyContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("TrackilyContextConnection")));

                services.AddDefaultIdentity<TrackilyUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<TrackilyContext>();
            });
        }
    }
}