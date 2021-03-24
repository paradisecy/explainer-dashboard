using ExplainerDashboard.Model;
using ExplainerDashboard.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;
using MongoDatabaseSettings = ExplainerDashboard.Services.MongoDatabaseSettings;

namespace ExplainerDashboard
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
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });


            services.Configure<MongoDatabaseSettings>(
       Configuration.GetSection(nameof(MongoDatabaseSettings)));

            services.AddSingleton<IMongoDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<MongoDatabaseSettings>>().Value);


            services.AddSingleton<IMongoService, MongoService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            //var m = app.ApplicationServices.GetService<IMongoService>();
            //var c = m.Database.GetCollection<Performance>("performance");

            //var cc = System.IO.File.ReadAllLines("file.csv");

            //var data = cc.ToList().Skip(1).Select(s =>
            //{
            //    var d = s.Split(',');
            //    var p = new Performance()
            //    {
            //        Day = int.Parse(d[0]),
            //        Weight = decimal.Parse(d[1]),
            //        Feed = decimal.Parse(d[2]),
            //        Fcr = decimal.Parse(d[3]),
            //    };
            //    return p;

            //}).ToList();

            //c.InsertManyAsync(data);
            //var m = app.ApplicationServices.GetService<IMongoService>();
            //var c = m.Database.GetCollection<Flock>("flocks");
            //c.Find(f => true).ForEachAsync(f =>
            //{

            //    f.Day = f.GrowingDay;   
            //     c.ReplaceOneAsync(
            //            Builders<Flock>.Filter.Eq(e => e.Id, f.Id), f);


            //});
        }
    }
}
