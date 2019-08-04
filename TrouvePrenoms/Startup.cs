// Copyright © 2018 Damien Mayance
// This file is subject to the terms and conditions defined in
// file 'LICENSE.md', which is part of this source code package
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrouvePrenoms.Models;

namespace TrouvePrenoms
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();
      services.AddRouting(options => options.LowercaseUrls = true);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment hostingEnvironment)
    {
      if (hostingEnvironment.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseBrowserLink();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseStaticFiles();

      app.UseMvc(routes =>
      {
        routes.MapRoute("name", "name/{*name}",
            defaults: new { controller = "Home", action = "Name" });

        routes.MapRoute("date", "date/{*dateString}",
            defaults: new { controller = "Home", action = "CustomDate" });

        routes.MapRoute("search", "search",
            defaults: new { controller = "Home", action = "Search" });


        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });

      // Load data once on startup
      string dataFile = Path.Combine(hostingEnvironment.ContentRootPath, "Data", "nat2018.txt");
      string cacheFile = Path.Combine(hostingEnvironment.ContentRootPath, "Data", "cache2018.bin");
      PrenomsService.Initialize(dataFile, cacheFile);
    }
  }
}
