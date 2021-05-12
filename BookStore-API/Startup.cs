using System;
using System.Reflection; // added for Assembly class below
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using BookStore_API.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models; // had to add manually to get OpenApiInfo to be in scope
using System.IO; // added to support Path class below
using BookStore_API.Contracts;
using BookStore_API.Services;

namespace BookStore_API {
  public class Startup {
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlite(
              Configuration.GetConnectionString("DefaultConnection")));
      services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
          .AddEntityFrameworkStores<ApplicationDbContext>();

      // Enable external servers to access this API service
      services.AddCors(o => {
        o.AddPolicy("CorsPolicy",
          builder => builder.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());
      });

      // Add swagger controls (via swashbuckler NuGet packages)
      services.AddSwaggerGen(c => { // lamda expression token => expression
        c.SwaggerDoc("v1", new OpenApiInfo {
          Title = "Book Store API",
          Version = "v1",
          Description = "This an educational API for a book store"
        });
        var xmlfile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; // project name, append .xml
        var xmlpath = Path.Combine(AppContext.BaseDirectory, xmlfile);
        c.IncludeXmlComments(xmlpath);
      });

      services.AddSingleton<ILoggerService, LoggerService>();

      services.AddControllers(); // Add our main controllers after others (above) added
    }


    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      else {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      // Add Swagger documentation and UI
      app.UseSwagger();
      app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Book Store API");
        c.RoutePrefix = "";
      });

      app.UseHttpsRedirection();

      app.UseCors("CorsPolicy");

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints => {
        endpoints.MapControllers();
      });
    }
  }
}
