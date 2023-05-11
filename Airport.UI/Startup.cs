using Airport.DBEntitiesDAL.Concrete;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.MessageExtension.Interfaces;
using Airport.MessageExtension.Repos;
using Airport.MessageExtensions.Interfaces;
using Airport.MessageExtensions.Repos;
using Airport.UI.Models.Interface;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;


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

            services.AddSession(a =>
            {
                a.IdleTimeout = TimeSpan.FromHours(2);
                a.Cookie.Name = "user";
                a.Cookie.IsEssential = true;
            });

            services.AddScoped<IUserDatasDAL, UserDatasDAL>();
            services.AddScoped<IServicesDAL, ServicesDAL>();
            services.AddScoped<IServicePropertiesDAL, ServicePropertiesDAL>();
            services.AddScoped<IServiceItemsDAL, ServiceItemsDAL>();
            services.AddScoped<IServiceCategoriesDAL, ServiceCategoriesDAL>();
            services.AddScoped<ICarBrandsDAL, CarBrandsDAL>();
            services.AddScoped<ICarModelsDAL, CarModelDAL>();
            services.AddScoped<ICarSeriesDAL, CarSeriesDAL>();
            services.AddScoped<ICarTypesDAL, CarTypesDAL>();
            services.AddScoped<IMyCarsDAL, MyCarsDAL>();
            services.AddScoped<IDriversDAL, DriversDAL>();
            services.AddScoped<ILocationsDAL, LocationsDAL>();
            services.AddScoped<ILocationCarsDAL, LocationCarsDAL>();
            services.AddScoped<IGetCarDetail, GetCarDetail>();
            services.AddScoped<ILocationCarsFareDAL, LocationCarsFareDAL>();
            services.AddScoped<IReservationsDAL, ReservationsDAL>();
            services.AddScoped<IReservationPeopleDAL, ReservationPeopleDAL>();
            services.AddScoped<IReservationServicesTableDAL, ReservationServicesTableDAL>();
            services.AddScoped<IMail, MailRepo>();
            services.AddScoped<ISMS, SMSRepo>();

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

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
