using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Trackily.Areas.Identity.IdentityHostingStartup))]
namespace Trackily.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}