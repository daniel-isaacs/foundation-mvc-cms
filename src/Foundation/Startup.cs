﻿using EPiServer.Cms.TinyMce;
using EPiServer.Framework.Web.Resources;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Foundation.Infrastructure;
using Foundation.Infrastructure.Cms.Users;
using Foundation.Infrastructure.Display;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;

namespace Foundation
{
    public class Startup
    {
        private readonly IWebHostEnvironment _webHostingEnvironment;

        public Startup(IWebHostEnvironment webHostingEnvironment)
        {
            _webHostingEnvironment = webHostingEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
			services.AddRouting(options => options.LowercaseUrls = true);
            services.AddCmsAspNetIdentity<SiteUser>();
            services.AddMvc(o => o.Conventions.Add(new FeatureConvention()))
                .AddRazorOptions(ro => ro.ViewLocationExpanders.Add(new FeatureViewLocationExpander()));

            if (_webHostingEnvironment.IsDevelopment())
            {
                services.Configure<ClientResourceOptions>(uiOptions => uiOptions.Debug = true);
            }

            services.AddCms();
            services.AddDisplay();
            services.AddTinyMce();
            services.AddFind();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/util/Login";
                options.ExpireTimeSpan = new TimeSpan(0, 20, 0);
                options.SlidingExpiration = true;
            });
            services.TryAddEnumerable(Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Singleton(typeof(IFirstRequestInitializer), typeof(ContentInstaller)));
            services.AddDetection();
			
			// Increase allowed upload file size
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = 5000; // Limit on individual form values
                x.MultipartBodyLengthLimit = 268435456; // Limit on form body size
                x.MultipartHeadersLengthLimit = 268435456; // Limit on form header size
            });
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 837280000; // Limit on request body size
            });
			
			// Disable websockets
			services.Configure<UIOptions>(options =>
			{
				options.WebSocketEnabled = false;
			});
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapContent();
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}