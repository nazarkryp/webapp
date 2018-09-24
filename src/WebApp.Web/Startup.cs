using System.Net;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebApp.Web.Infrastructure.Extensions;
using WebApp.Web.Infrastructure.Filters;
using WebApp.Web.Infrastructure.Ioc;
using WebApp.Web.Infrastructure.Middlewares;
using WebApp.Web.Models;

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
                        var securityKey = Configuration["SecurityKey"];
                        var securityKeyBytes = Encoding.UTF8.GetBytes(securityKey);

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = "webapp.com",
                            ValidAudience = "webapp.com",
                            IssuerSigningKey = new SymmetricSecurityKey(securityKeyBytes)
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
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Error");
            //    //app.UseHsts();
            //}

            app.UseExceptionHandler("/Error");

            app.UseHttpsRedirection();
            app.UseAuthentication();

            //app.UseStaticFiles();
            //app.UseSpaStaticFiles();

            app.ConfigureExceptionHandler();

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            //app.UseStatusCodePages(context =>
            //{
            //    context.HttpContext.Response.ContentType = "application/json";

            //    var body = context.HttpContext.Response.Body;

            //    var errorResult = new ErrorResult((HttpStatusCode)context.HttpContext.Response.StatusCode, string.Empty);
            //    var jsonResult = JsonConvert.SerializeObject(errorResult);

            //    return context.HttpContext.Response.WriteAsync(jsonResult);
            //});

            app.UseMvc(routes =>
            {
                //routes.MapRoute(
                //    name: "default",
                //    template: "{controller=Home}/{action=Index}/{id?}");
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
