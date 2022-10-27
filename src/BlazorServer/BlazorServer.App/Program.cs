using BlazorServer.App.Areas.Identity;
using BlazorServer.DataAccess;
using BlazorServer.DataAccess.Utils;
using BlazorServer.Entities;
using BlazorServer.Models;
using BlazorServer.Models.Mappings;
using BlazorServer.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
ConfigureLogging(builder.Logging, builder.Environment, builder.Configuration);
ConfigureServices(builder.Services, builder.Configuration);
var webApp = builder.Build();
ConfigureMiddlewares(webApp, webApp.Environment);
ConfigureEndpoints(webApp);
ConfigureDatabase(webApp);
webApp.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddOptions<AdminUserSeed>().Bind(configuration.GetSection("AdminUserSeed"));

    services.AddAutoMapper(typeof(MappingProfile).Assembly);

    var connectionString = configuration.GetConnectionString("DefaultConnection");

    // NOTE! this method of registering the db-context doesn't work with blazor-server apps!
    //services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString))

    services.AddDbContextFactory<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
    services.AddScoped(serviceProvider =>
                           serviceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
                                          .CreateDbContext()
                      );

    services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

    services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
    services.AddScoped<IUserInfoService, UserInfoService>();

    services.AddScoped<IHotelRoomService, HotelRoomService>();
    services.AddScoped<IIdentityDbInitializer, IdentityDbInitializer>();
    services.AddScoped<IHotelRoomImageService, HotelRoomImageService>();
    services.AddScoped<IFileUploadService, FileUploadService>();
    services.AddScoped<IAmenityService, AmenityService>();

    services.AddRazorPages();
    services.AddServerSideBlazor();
}

void ConfigureLogging(ILoggingBuilder logging, IHostEnvironment env, IConfiguration configuration)
{
    logging.ClearProviders();

    logging.AddDebug();

    if (env.IsDevelopment())
    {
        logging.AddConsole();
    }

    logging.AddConfiguration(configuration.GetSection("Logging"));
}

void ConfigureMiddlewares(IApplicationBuilder app, IHostEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();
}

void ConfigureEndpoints(IApplicationBuilder app)
{
    app.UseEndpoints(endpoints =>
                     {
                         endpoints.MapRazorPages();
                         endpoints.MapBlazorHub();
                         endpoints.MapFallbackToPage("/_Host");
                     });
}

void ConfigureDatabase(IApplicationBuilder app)
{
    app.ApplicationServices.MigrateDbContext<ApplicationDbContext>(
                                                                   scopedServiceProvider =>
                                                                       scopedServiceProvider
                                                                           .GetRequiredService<IIdentityDbInitializer>()
                                                                           .SeedDatabaseWithAdminUserAsync()
                                                                           .GetAwaiter()
                                                                           .GetResult()
                                                                  );
}