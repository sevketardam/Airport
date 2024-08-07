using Airport.DBEntities.Context;
using Airport.DBEntitiesDAL.Concrete;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.MessageExtension.Interfaces;
using Airport.MessageExtension.Repos;
using Airport.MessageExtensions.Interfaces;
using Airport.MessageExtensions.Repos;
using Airport.UI.Models.Extendions;
using Airport.UI.Models.Interface;
using Airport.UI.Models.ITransactions;
using Airport.UI.Models.VM;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Airport.UI;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        _env = env;
    }

    public IConfiguration Configuration { get; }

    private readonly IWebHostEnvironment _env;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {

        services.Configure<GoogleAPIKeys>(Configuration.GetSection("APIKeys"));

        services.AddSession(a =>
        {
            a.IdleTimeout = TimeSpan.FromHours(2);
            a.Cookie.Name = "user";
            a.Cookie.IsEssential = true;
        });

        services.AddDbContext<AirportContext>(options =>
        {
            options.UseSqlServer(Configuration.GetConnectionString("MSSQL"));
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
        services.AddScoped<ICouponsDAL, CouponsDAL>();
        services.AddScoped<ILoginAuthDAL, LoginAuthDAL>();
        services.AddScoped<IUserDocsDAL, UserDocsDAL>();
        services.AddScoped<IGlobalSettingsDAL, GlobalSettingsDAL>();
        services.AddScoped<IWithdrawalRequestDAL, WithdrawalRequestDAL>();
        services.AddScoped<IPaymentDetailDAL, PaymentDetailDAL>();


        services.AddScoped<IMail, MailRepo>();
        services.AddScoped<ISMS, SMSRepo>();
        services.AddScoped<IPayment, PaymentMethods>();
        services.AddScoped<ITReservations, TReservations>();
        services.AddScoped<ITReservationHelpers, TReservationHelpers>();
        services.AddScoped<IFileOperation, FileOperation>();
        services.AddScoped<IApiResult, ApiResults>();
        services.AddScoped<IGlobalSettings, TGlobalSettings>();


        services.AddHttpContextAccessor();
        services.AddControllersWithViews();
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opt =>
        {
            opt.LoginPath = "/";
            opt.SlidingExpiration = false;
            opt.AccessDeniedPath = "/NotFound";
        });


    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            //app.UseHsts();
        }
        else
        {
            app.UseExceptionHandler("/error");
        }

        //app.UseHttpsRedirection();

        app.UseStatusCodePagesWithReExecute("/Error/Index", "?code={0}");

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseCookiePolicy();

        app.UseSession();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
            name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}
