// Copyright © 2018 Damien Mayance
// This file is subject to the terms and conditions defined in
// file 'LICENSE.md', which is part of this source code package
using System.IO;
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

      app.UseStaticFiles(); // wwwroot

      app.UseMvc(routes =>
      {
        routes.MapRoute("name", "name/{*name}",
            defaults: new { controller = "Home", action = "Name" });

        routes.MapRoute("date", "date/{*dateString}",
            defaults: new { controller = "Home", action = "CustomDate" });

        routes.MapRoute("search", "search",
            defaults: new { controller = "Home", action = "Search" });

        routes.MapRoute("about", "about",
            defaults: new { controller = "Home", action = "About" });


        routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");
      });

      // Load data once on startup
      var dataFile = Path.Combine(hostingEnvironment.WebRootPath, "data", "nat2018.csv");
      string cacheFile = Path.Combine(hostingEnvironment.ContentRootPath, ".", "cache2018.bin");
      PrenomsService.Initialize(dataFile, cacheFile);
    }
  }
}
