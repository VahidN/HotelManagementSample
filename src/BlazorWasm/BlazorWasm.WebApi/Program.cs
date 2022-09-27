using System.Text;
using BlazorServer.Common;
using BlazorServer.DataAccess;
using BlazorServer.DataAccess.Utils;
using BlazorServer.Entities;
using BlazorServer.Models;
using BlazorServer.Models.Mappings;
using BlazorServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Parbad.Builder;
using Parbad.Gateway.ParbadVirtual;
using Parbad.Storage.EntityFrameworkCore.Builder;

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
    var bearerTokensSection = configuration.GetSection("BearerTokens");
    services.AddOptions<BearerTokensOptions>().Bind(bearerTokensSection);
    services.AddOptions<AdminUserSeed>().Bind(configuration.GetSection("AdminUserSeed"));

    services.AddAutoMapper(typeof(MappingProfile).Assembly);

    services.AddScoped<IHotelRoomService, HotelRoomService>();
    services.AddScoped<IAmenityService, AmenityService>();
    services.AddScoped<IHotelRoomImageService, HotelRoomImageService>();
    services.AddScoped<ITokenFactoryService, TokenFactoryService>();
    services.AddScoped<IIdentityDbInitializer, IdentityDbInitializer>();
    services.AddScoped<IRoomOrderDetailsService, RoomOrderDetailsService>();

    var connectionString = configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

    services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

    services.AddParbad()
            .ConfigureHttpContext(httpContextBuilder => httpContextBuilder.UseDefaultAspNetCore())
            .ConfigureGateways(gatewayBuilder =>
                               {
                                   gatewayBuilder
                                       .AddParbadVirtual()
                                       .WithOptions(gatewayOptions =>
                                                        gatewayOptions.GatewayPath = "/MyVirtualGateway");
                               })
            .ConfigureStorage(storageBuilder =>
                              {
                                  storageBuilder.UseEfCore(efCoreOptions =>
                                                           {
                                                               var assemblyName = typeof(ApplicationDbContext)
                                                                   .Assembly.GetName().Name;
                                                               efCoreOptions.ConfigureDbContext = db =>
                                                                   db.UseSqlServer(
                                                                        connectionString,
                                                                        sqlOptions =>
                                                                            sqlOptions
                                                                                .MigrationsAssembly(assemblyName)
                                                                       );
                                                           });
                              })
            //.ConfigureAutoTrackingNumber(opt => opt.MinimumValue = 1)
            .ConfigureOptions(parbadOptions =>
                              {
                                  // parbadOptions.Messages.PaymentSucceed = "YOUR MESSAGE"
                              });

    services.AddCors(o => o.AddPolicy("HotelManagement",
                                      builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }));

    var apiSettings = bearerTokensSection.Get<BearerTokensOptions>();
    var apiSettingsKey = apiSettings.Key;
    if (string.IsNullOrWhiteSpace(apiSettingsKey))
    {
        throw new InvalidOperationException("apiSettingsKey is null");
    }

    var key = Encoding.UTF8.GetBytes(apiSettingsKey);
    services.AddAuthentication(opt =>
                               {
                                   opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                   opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                   opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                               })
            .AddJwtBearer(cfg =>
                          {
                              cfg.RequireHttpsMetadata = false;
                              cfg.SaveToken = true;
                              cfg.TokenValidationParameters = new TokenValidationParameters
                                                              {
                                                                  ValidateIssuerSigningKey = true,
                                                                  IssuerSigningKey = new SymmetricSecurityKey(key),
                                                                  ValidateAudience = true,
                                                                  ValidateIssuer = true,
                                                                  ValidAudience = apiSettings.Audience,
                                                                  ValidIssuer = apiSettings.Issuer,
                                                                  ClockSkew = TimeSpan.Zero,
                                                                  ValidateLifetime = true,
                                                              };
                          });

    services.AddAuthorization(options => options.AddAppPolicies());

    services.AddRouting(option => option.LowercaseUrls = true);

    services.AddControllers()
            .AddJsonOptions(options =>
                            {
                                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                                // To avoid `JsonSerializationException: Self referencing loop detected error`
                                // options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve
                            });
    services.AddSwaggerGen(c =>
                           {
                               c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlazorWasm.WebApi", Version = "v1" });
                               c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                                                                 {
                                                                     In = ParameterLocation.Header,
                                                                     Description =
                                                                         "Please enter the token in the field",
                                                                     Name = "Authorization",
                                                                     Type = SecuritySchemeType.ApiKey,
                                                                 });
                               c.AddSecurityRequirement(new OpenApiSecurityRequirement
                                                        {
                                                            {
                                                                new OpenApiSecurityScheme
                                                                {
                                                                    Reference = new OpenApiReference
                                                                        {
                                                                            Type = ReferenceType.SecurityScheme,
                                                                            Id = "Bearer",
                                                                        },
                                                                },
                                                                Array.Empty<string>()
                                                            },
                                                        });
                           });
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
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlazorWasm.WebApi v1"));
    }

    app.UseHttpsRedirection();

    app.UseCors("HotelManagement");
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    if (env.IsDevelopment())
    {
        app.UseParbadVirtualGatewayWhenDeveloping();
    }
    else
    {
        app.UseParbadVirtualGateway();
    }
}

void ConfigureEndpoints(IApplicationBuilder app)
{
    app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
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