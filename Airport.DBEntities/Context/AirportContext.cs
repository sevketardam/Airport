using Microsoft.EntityFrameworkCore;
using Airport.DBEntities;
using Airport.DBEntities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntities.Context
{
    public class AirportContext : DbContext
    {
        public AirportContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var connectionString = "server=localhost;port=3306;database=wr7076624_;user=globalairport_db;password=Ljfv7789#;Allow Zero Datetime=True;Convert Zero Datetime=True";
            //optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            //var connectionString = "Server=.\\SQLEXPRESS;Database=pairpoX3_db1234;User Id=pairpoX3_db1234;Password=Ur3nedCS; Trusted_Connection = True; MultipleActiveResultSets = true";
            //optionsBuilder.UseSqlServer(connectionString);

            //var connectionString = "Server =.\\SQLEXPRESS01; Database = Airport; Trusted_Connection = True; MultipleActiveResultSets = true";
            //optionsBuilder.UseSqlServer(connectionString);

            var connectionString = "Server =.\\SQLEXPRESS; Database = Airport; Trusted_Connection = True; MultipleActiveResultSets = true";
            optionsBuilder.UseSqlServer(connectionString);

            base.OnConfiguring(optionsBuilder);
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
                .HasOne<CarClasses>(a => a.Class)
                .WithMany(a => a.MyCars)
                .HasForeignKey(a => a.ClassId);

            modelBuilder.Entity<MyCars>()
                .HasOne<CarModels>(a => a.Model)
                .WithMany(a => a.MyCars)
                .HasForeignKey(a => a.ModelId);

            modelBuilder.Entity<MyCars>()
                .HasOne<CarSeries>(a => a.Series)
                .WithMany(a => a.MyCars)
                .HasForeignKey(a => a.SeriesId);

            modelBuilder.Entity<MyCars>()
                .HasOne<CarTrims>(a => a.Trim)
                .WithMany(a => a.MyCars)
                .HasForeignKey(a => a.TrimId);

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

            modelBuilder.Entity<CarTrims>()
                .HasOne<CarModels>(a => a.CarModels)
                .WithMany(a => a.CarTrims)
                .HasForeignKey(a => a.CarModelId);

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

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserDatas> UserDatas { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<ServiceProperties> ServiceProperties { get; set; }
        public DbSet<ServiceCategories> ServiceCategories { get; set; }
        public DbSet<CarBrands> CarBrands { get; set; }
        public DbSet<CarModels> CarModels { get; set; }
        public DbSet<CarSeries> CarSeries { get; set; }
        public DbSet<CarTrims> CarTrims { get; set; }
        public DbSet<CarTypes> CarTypes { get; set; }
        public DbSet<CarClasses> CarClasses { get; set; }
        public DbSet<MyCars> MyCars { get; set; }
        public DbSet<Drivers> Drivers { get; set; }
      
    }
}
