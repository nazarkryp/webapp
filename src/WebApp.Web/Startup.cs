using System;
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
                ConfigureFilters(options);
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

            app.UseHttpsRedirection();
            app.UseAuthentication();

            //app.UseStaticFiles();
            //app.UseSpaStaticFiles();

            //app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseStatusCodePages(context =>
            {
                Exception exception;
                switch (context.HttpContext.Response.StatusCode)
                {
                    case 401:
                        exception = new Exception("Authorization has been denied for this request");
                        break;
                    case 404:
                        exception = new Exception("No resource was found that matches the request URI");
                        break;
                    default:
                        exception = new Exception("Something went wront. Weird things happen");
                        break;
                }

                context.HttpContext.Response.ContentType = "application/json";

                var errorResult = new ErrorResult((HttpStatusCode)context.HttpContext.Response.StatusCode, exception.Message);
                var jsonResult = JsonConvert.SerializeObject(errorResult);

                return context.HttpContext.Response.WriteAsync(jsonResult);
            });

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

        private static void ConfigureFilters(MvcOptions options)
        {
            options.Filters.Add(new ResourceNotFoundFilterAttribute());
        }
    }
}
