using BlazorServer.App.Areas.Identity;

[assembly: HostingStartup(typeof(IdentityHostingStartup))]

namespace BlazorServer.App.Areas.Identity;

public class IdentityHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.ConfigureServices((context, services) => { });
    }
}