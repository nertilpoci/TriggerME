using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApplicationBasic.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WebApplicationBasic.Services;
using Microsoft.EntityFrameworkCore;
using TriggerMe.Model;
using System.IdentityModel.Tokens.Jwt;
using IdentityServer4.AccessTokenValidation;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Threading.Tasks;
using TriggerMe;
using TriggerMe.Models;
using Microsoft.AspNetCore.Authentication.LinkedIn;
using Microsoft.AspNetCore.Http;
using System.IO;
using IdentityServer4.Configuration;
using System.Linq;
using TriggerMe.DAL;

namespace WebApplicationBasic
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IISOptions>(options => { });
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddMultitenancy<AppTenant, AppTenantResolver>();
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;

                options.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
                {

                    OnRedirectToLogin = ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") || ctx.Request.Path.StartsWithSegments("/connect"))
                        {
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        return Task.FromResult(0);
                    }
                };
            })
              .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new SignalRContractResolver();

            var serializer = JsonSerializer.Create(settings);
            services.Add(new ServiceDescriptor(typeof(JsonSerializer),
                         provider => serializer,
                         ServiceLifetime.Transient));

            services.AddSignalR(options =>
            {
                options.Hubs.EnableJavaScriptProxies = true;
                options.Hubs.EnableDetailedErrors = true;
                options.EnableJSONP = true;
            });
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            // Adds IdentityServer
            services.AddIdentityServer()
                .AddTemporarySigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<ApplicationUser>();

            services.Configure<AuthMessageSenderOptions>(Configuration);
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.Use(async (context, next) =>
            {
                if (string.IsNullOrWhiteSpace(context.Request.Headers["Authorization"]))
                {
                    if (context.Request.QueryString.HasValue)
                    {
                        var token = context.Request.QueryString.Value.Split('&').SingleOrDefault(x => x.Contains("authorization"))?.Split('=')[1];
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            context.Request.Headers.Add("Authorization", new[] { $"Bearer {token}" });
                        }
                    }
                }
                await next.Invoke();
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseExceptionHandler("/Home/Error");
            app.UseStaticFiles(); // For the wwwroot folder


            app.UseIdentity();

            IdentityServerAuthenticationOptions identityServerValidationOptions = new IdentityServerAuthenticationOptions
            {
                Authority = Config.HOST_URL,
                ApiName = "api1",
                RequireHttpsMetadata = false,
                SupportedTokens = SupportedTokens.Both,
            };

            app.UseIdentityServer();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseIdentityServerAuthentication(identityServerValidationOptions);
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            app.UseMultitenancy<AppTenant>();
            app.UseSignalR();

            // external authentication data is a placholder for security reasons
            app.UseGoogleAuthentication(new GoogleOptions
            {
                AuthenticationScheme = "Google",
                SignInScheme = "Identity.External", // this is the name of the cookie middleware registered by UseIdentity()
                ClientId = "xxxxxxxxxxxxxxx.apps.googleusercontent.com",
                ClientSecret = "xxxxxxxxxxxxxxxxxxxxxxx",
            });
            app.UseTwitterAuthentication(new TwitterOptions()
            {
                ConsumerKey = "xxxxxxxxxxxxxxxxxx",
                ConsumerSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            });
            app.UseFacebookAuthentication(new FacebookOptions()
            {
                AppId = "xxxxxxxxxxxxxxxxxx",
                AppSecret = "xxxxxxxxxxxxxxxxxxxxxx"
            });
            app.UseMicrosoftAccountAuthentication(new MicrosoftAccountOptions()
            {
                ClientId = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                ClientSecret = "xxxxxxxxxxxxxxxxxxxxxxxx"
            });
            app.UseLinkedInAuthentication(new LinkedInOptions()
            {
                AppId = "xxxxxxxxxxxxxxxxxxxxxxxx",
                AppSecret = "xxxxxxxxxxxxxxxxxxxxxxxxx",
                ProfileScheme = LinkedInDefaults.ProfileLoadFormat.AppDefined
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                   name: "spa-fallback",
                   defaults: new { controller = "Home", action = "Index" });
            });

        }
    }
}
