﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IdentityServer4;

namespace web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            //services.AddIdentityServer().AddDeveloperSigningCredential();
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(Config.GetUsers());

            services.AddAuthentication().AddGoogle("Google", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                options.ClientId = Configuration["Google:clientId"];
                options.ClientSecret = Configuration["Google:clientSecret"];
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }

    // public class Startup
    // {
    //     public Startup(IConfiguration configuration)
    //     {
    //         Configuration = configuration;
    //     }

    //     public IConfiguration Configuration { get; }

    //     // This method gets called by the runtime. Use this method to add services to the container.
    //     public void ConfigureServices(IServiceCollection services)
    //     {
    //         services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
    //     }

    //     // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    //     public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    //     {
    //         if (env.IsDevelopment())
    //         {
    //             app.UseDeveloperExceptionPage();
    //         }
    //         else
    //         {
    //             // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //             app.UseHsts();
    //         }

    //         app.UseHttpsRedirection();
    //         app.UseMvc();
    //     }
    // }
}
