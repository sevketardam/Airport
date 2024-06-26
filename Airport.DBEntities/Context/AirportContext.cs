using Microsoft.EntityFrameworkCore;
using Airport.DBEntities.Entities;

namespace Airport.DBEntities.Context;

public class AirportContext : DbContext
{
    public AirportContext()
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Services>()
            .HasOne<UserDatas>(a => a.User)
            .WithMany(a => a.Services)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<ServiceItems>()
            .HasOne<Services>(a => a.Service)
            .WithMany(a => a.ServiceItems)
            .HasForeignKey(a => a.ServiceId);

        modelBuilder.Entity<ServiceItems>()
            .HasOne<ServiceProperties>(a => a.ServiceProperty)
            .WithMany(a => a.ServiceItems)
            .HasForeignKey(a => a.ServicePropertyId);

        modelBuilder.Entity<ServiceProperties>()
            .HasOne<ServiceCategories>(a => a.ServiceCategory)
            .WithMany(a => a.ServiceProperties)
            .HasForeignKey(a => a.ServiceCategoryId);

        modelBuilder.Entity<MyCars>()
            .HasOne<CarBrands>(a => a.Brand)
            .WithMany(a => a.MyCars)
            .HasForeignKey(a => a.BrandId);

        modelBuilder.Entity<MyCars>()
            .HasOne<CarModels>(a => a.Model)
            .WithMany(a => a.MyCars)
            .HasForeignKey(a => a.ModelId);

        modelBuilder.Entity<MyCars>()
            .HasOne<CarSeries>(a => a.Series)
            .WithMany(a => a.MyCars)
            .HasForeignKey(a => a.SeriesId);

        modelBuilder.Entity<MyCars>()
            .HasOne<CarTypes>(a => a.Type)
            .WithMany(a => a.MyCars)
            .HasForeignKey(a => a.TypeId);

        modelBuilder.Entity<MyCars>()
            .HasOne<Services>(a => a.Service)
            .WithMany(a => a.MyCars)
            .HasForeignKey(a => a.ServiceId);

        modelBuilder.Entity<MyCars>()
            .HasOne<UserDatas>(a => a.User)
            .WithMany(a => a.MyCars)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<Drivers>()
            .HasOne<UserDatas>(a => a.User)
            .WithMany(a => a.Drivers)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<Locations>()
            .HasOne<UserDatas>(a => a.User)
            .WithMany(a => a.Locations)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<LocationCars>()
            .HasOne<Locations>(a => a.Location)
            .WithMany(a => a.LocationCars)
            .HasForeignKey(a => a.LocationId);

        modelBuilder.Entity<LocationCars>()
            .HasOne<MyCars>(a => a.Car)
            .WithMany(a => a.LocationCars)
            .HasForeignKey(a => a.CarId);

        modelBuilder.Entity<LocationCarsFare>()
            .HasOne<LocationCars>(a => a.LocationCar)
            .WithMany(a => a.LocationCarsFares)
            .HasForeignKey(a => a.LocationCarId);

        modelBuilder.Entity<Reservations>()
            .HasOne<LocationCars>(a => a.LocationCars)
            .WithMany(a => a.Reservations)
            .HasForeignKey(a => a.LocationCarId);

        modelBuilder.Entity<ReservationPeople>()
            .HasOne<Reservations>(a => a.Reservation)
            .WithMany(a => a.ReservationPeoples)
            .HasForeignKey(a => a.ReservationId);

        modelBuilder.Entity<MyCars>()
            .HasOne<Drivers>(a => a.Driver)
            .WithMany(a => a.MyCars)
            .HasForeignKey(a => a.DriverId);

        modelBuilder.Entity<ServiceCategories>()
            .HasOne<UserDatas>(a => a.User)
            .WithMany(a => a.ServiceCategories)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<Reservations>()
            .HasOne<UserDatas>(a => a.User)
            .WithMany(a => a.Reservations)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<Reservations>()
            .HasOne<Drivers>(a => a.Driver)
            .WithMany(a => a.Reservations)
            .HasForeignKey(a => a.DriverId);

        modelBuilder.Entity<ReservationServicesTable>()
            .HasOne<Reservations>(a => a.Reservation)
            .WithMany(a => a.ReservationServicesTables)
            .HasForeignKey(a => a.ReservationId);

        modelBuilder.Entity<ReservationServicesTable>()
            .HasOne<ServiceItems>(a => a.ServiceItem)
            .WithMany(a => a.ReservationServicesTables)
            .HasForeignKey(a => a.ServiceItemId);

        modelBuilder.Entity<Coupons>()
            .HasOne<UserDatas>(a => a.User)
            .WithMany(a => a.Coupons)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<Reservations>()
            .HasOne<Coupons>(a => a.Coupons)
            .WithMany(a => a.Reservations)
            .HasForeignKey(a => a.Coupon);

        modelBuilder.Entity<LoginAuth>()
            .HasOne<UserDatas>(a => a.User)
            .WithOne(a => a.LoginAuth);

        modelBuilder.Entity<LoginAuth>()
            .HasOne<Drivers>(a => a.Driver)
            .WithOne(a => a.LoginAuth);


        modelBuilder.Entity<UserDocs>()
            .HasOne<UserDatas>(a => a.User)
            .WithOne(a => a.Docs);

        modelBuilder.Entity<WithdrawalRequest>()
            .HasOne<UserDatas>(a => a.User)
            .WithMany(a => a.WithdrawalRequests)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<PaymentDetail>()
            .HasOne<Reservations>(a => a.Reservation)
            .WithMany(a => a.PaymentDetails)
            .HasForeignKey(a => a.ReservationId);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<UserDatas> UserDatas { get; set; }
    public DbSet<Services> Services { get; set; }
    public DbSet<ServiceProperties> ServiceProperties { get; set; }
    public DbSet<ServiceCategories> ServiceCategories { get; set; }
    public DbSet<CarBrands> CarBrands { get; set; }
    public DbSet<CarModels> CarModels { get; set; }
    public DbSet<CarSeries> CarSeries { get; set; }
    public DbSet<CarTypes> CarTypes { get; set; }
    public DbSet<MyCars> MyCars { get; set; }
    public DbSet<Drivers> Drivers { get; set; }
    public DbSet<GlobalSettings> GlobalSettings { get; set; }
    public DbSet<WithdrawalRequest> WithdrawalRequest { get; set; }
  
}
