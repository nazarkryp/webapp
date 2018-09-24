using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WebApp.Web.Infrastructure.Extensions;
using WebApp.Web.Infrastructure.Filters;
using WebApp.Web.Infrastructure.Ioc;

namespace WebApp.Web
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    {
                        var securityKey = Encoding.UTF8.GetBytes(Configuration["SecurityKey"]);

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = "webapp.com",
                            ValidAudience = "webapp.com",
                            IssuerSigningKey = new SymmetricSecurityKey(securityKey)
                        };
                    }
                );

            services.AddMvc(options =>
            {
                options.Filters.Add(new UnauthorizedFilterAttribute());
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //services.AddSpaStaticFiles(configuration =>
            //{
            //    configuration.RootPath = "Client/dist/client";
            //});

            services.AddApiVersioning();

            IocConfig.ConfigureIoc(Configuration, services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                //app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();

            //app.UseStaticFiles();
            //app.UseSpaStaticFiles();

            app.ConfigureExceptionHandler();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //app.UseSpa(spa =>
            //{
            //    spa.Options.SourcePath = "Client";

            //    if (env.IsDevelopment())
            //    {
            //        spa.UseAngularCliServer(npmScript: "start");
            //    }
            //});
        }
    }
}
