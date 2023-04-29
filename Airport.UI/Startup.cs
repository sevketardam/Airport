using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Concrete;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.UI.Models.Interface;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airport.UI
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

            services.AddScoped<IUserDatasDAL, UserDatasDAL>();
            services.AddScoped<IServicesDAL, ServicesDAL>();
            services.AddScoped<IServicePropertiesDAL, ServicePropertiesDAL>();
            services.AddScoped<IServiceItemsDAL, ServiceItemsDAL>();
            services.AddScoped<IServiceCategoriesDAL, ServiceCategoriesDAL>();
            services.AddScoped<ICarBrandsDAL, CarBrandsDAL>();
            services.AddScoped<ICarModelsDAL, CarModelDAL>();
            services.AddScoped<ICarSeriesDAL, CarSeriesDAL>();
            services.AddScoped<ICarTrimsDAL, CarTrimsDAL>();
            services.AddScoped<ICarTypesDAL, CarTypesDAL>();
            services.AddScoped<ICarClassesDAL, CarClassesDAL>();
            services.AddScoped<IMyCarsDAL, MyCarsDAL>();
            services.AddScoped<IDriversDAL, DriversDAL>();
            services.AddScoped<ILocationsDAL, LocationsDAL>();
            services.AddScoped<ILocationCarsDAL, LocationCarsDAL>();
            services.AddScoped<IGetCarDetail, GetCarDetail>();
            services.AddScoped<ILocationCarsFareDAL, LocationCarsFareDAL>();

            services.AddControllersWithViews();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opt =>
            {
                opt.LoginPath = "/";
            });

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
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();
            //app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
