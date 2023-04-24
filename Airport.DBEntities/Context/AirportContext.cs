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
            //var connectionString = "server=93.89.225.59;database=pairpoX3_db1234;user=pairpoX3_db1234;password=Zw6Gzx9B;Allow Zero Datetime=True;Convert Zero Datetime=True";
            //optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            //var connectionString = "Server=.\\SQLEXPRESS;Database=pairpoX3_db1234;User Id=pairpoX3_db1234;Password=Ur3nedCS; Trusted_Connection = True; MultipleActiveResultSets = true";
            //optionsBuilder.UseSqlServer(connectionString);

            var connectionString = "Server =.\\SQLEXPRESS01; Database = Airport; Trusted_Connection = True; MultipleActiveResultSets = true";
            optionsBuilder.UseSqlServer(connectionString);


            base.OnConfiguring(optionsBuilder);
        }
        /*
         Foreign Key İçin Gün2 part2 17:00 dk*/
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
      
    }
}
