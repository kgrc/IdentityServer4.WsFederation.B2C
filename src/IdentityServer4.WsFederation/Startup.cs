using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Security.Cryptography.X509Certificates;


namespace IdentityServer4.WsFederation
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var cert = new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "idsrvtest.pfx"), "idsrv3test");

            //Ratheesh: In services we add the service middleware IdentityServer 4 and configure it via DI
            //we also add services from Google & Azure B2C

            services.AddIdentityServer()
                .AddSigningCredential(cert)
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(TestUsers.Users)
                .AddWsFederation()
                .AddInMemoryRelyingParties(Config.GetRelyingParties());


            services.AddAuthentication().
                AddGoogle("Google", "Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = "708996912208-9m4dkjb5hscn7cjrn5u0r4tbgkbj1fko.apps.googleusercontent.com";
                    options.ClientSecret = "wdfPY6t8H8cecgjlxud__4Gh";                   
                });

            //Ratheesh: Here we are setting the OIDC authentication schema and adding additionally
            //AzureB2C. This is because B2C has more query params than OIDC and standard OIDC will not suffice.
            services.AddAuthentication (sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
           .AddAzureAdB2C(options =>
               {
                   Configuration.Bind("Authentication:AzureAdB2C", options);
                   options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                   options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                   options.ResponseType = "id_token";
                   options.CallbackPath = "/signin-oidc";

               }
           )
           .AddCookie();

           
            // claims transformation is run after every Authenticate call
            //Ratheesh: adding this here, just in case the app needs to add more custom claims before passing it back to relying party
            services.AddTransient<IClaimsTransformation, ClaimsTransformer>();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();
            
            app.UseStaticFiles();

            //Ratheesh: Adding Identity Server and MVC in that order so ID4 handles OIDC and WSFED 
            //before routing decides what to do

            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();       

        }
    }
}