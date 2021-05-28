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
//using Microsoft.EntityFrameworkCore.SqlServer; // access the db
//using Microsoft.EntityFrameworkCore.Design; // update, change db
using BookStore_API.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models; // had to add manually to get OpenApiInfo to be in scope
using System.IO; // added to support Path class below
using BookStore_API.Contracts;
using BookStore_API.Services;
using BookStore_API.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BookStore_API {
  public class Startup {
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddDbContextPool<ApplicationDbContext>(options => // note expert says to use addDbContextPool vs addDbContext
        //options.UseSqlite(
        //  Configuration.GetConnectionString("DefaultConnection")));
        options.UseSqlServer(
          Configuration.GetConnectionString("AzureSQLConnection")));

      services.AddDefaultIdentity<IdentityUser>() // include this in parenth's for MFA (options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

      // Enable external servers to access this API service
      services.AddCors(o => {
        o.AddPolicy("CorsPolicy",
          builder => builder.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());
      });

      // Great framework to link names in this project to the database names
      services.AddAutoMapper(typeof(Maps));

      // JWT: JSON Web Token
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o => {
          o.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Configuration["Jwt:Issuer"],
            ValidAudience = Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
          };
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
      services.AddScoped<IAuthorRepository, AuthorRepository>();
      services.AddScoped<IBookRepository, BookRepository>();

      services.AddControllers(); // Add our main controllers after others (above) added
    }


    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app,
      IWebHostEnvironment env,
      UserManager<IdentityUser> userManager,
      RoleManager<IdentityRole> roleManager) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
        // app.UseDatabaseErrorPage(); // commented out due to ASP.NET Core 3.1 to 5.0 issue
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

      // See Policy setting above
      app.UseCors("CorsPolicy");

      // Seed an admin and two customers un/pw's for testing/debugging
      SeedData.Seed(userManager, roleManager).Wait();
      // Note: adding .Wait() to the end instead of "await" at the beginning
      //  allows us to make this one function async without needing to reformat this
      //  class as an async class

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints => {
        endpoints.MapControllers();
      });
    }
  }
}
