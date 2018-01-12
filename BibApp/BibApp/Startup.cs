using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BibApp.Models.Benutzer;
using System;
using BibApp.Data;
using NToastNotify;

namespace BibApp
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
            services.AddNToastNotify();

            services.AddDbContext<BibContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("BibContextConnection")));

            services.AddIdentity<Benutzer, IdentityRole>()
             .AddEntityFrameworkStores<BibContext>();

            services.Configure<IdentityOptions>(options =>
            {
                //Passwort Einstellungen
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 2;

            });

            services.AddScoped<DbInitializer>();
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie Einstellungen
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(150);
                options.LoginPath = "/Benutzer/Login";
                options.LogoutPath = "/Benutzer/Login";
                options.AccessDeniedPath = "/Benutzer/AccessDenied";
                options.SlidingExpiration = true;
            });
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, BibContext context, IServiceProvider service, DbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Benutzer}/{action=Login}/{id?}");
            });
            context.Database.EnsureCreated();
            ((DbInitializer)dbInitializer).Initialize().Wait();
        }
    }
}
