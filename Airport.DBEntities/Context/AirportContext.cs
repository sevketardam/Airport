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
            //var connectionString = "server=94.73.149.221;port=3306;database=db55A;user=user55A;password=:3:_ejQz@Q6RfK43;Allow Zero Datetime=True;Convert Zero Datetime=True";
            //optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            //var connectionString = "Server =.\\SQLEXPRESS; Database = spor_sepeti; Trusted_Connection = True; MultipleActiveResultSets = true";
            //optionsBuilder.UseSqlServer(connectionString);

            var connectionString = "Server =.\\SQLEXPRESS01; Database = Airport; Trusted_Connection = True; MultipleActiveResultSets = true";
            optionsBuilder.UseSqlServer(connectionString);


            base.OnConfiguring(optionsBuilder);
        }
        /*
         Foreign Key İçin Gün2 part2 17:00 dk*/
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<Members>()
            //    .HasOne<Users>(a => a.User)
            //    .WithMany(a => a.Members)
            //    .HasForeignKey(a => a.Adminid);


            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserDatas> UserDatas { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<ServiceProperties> ServiceProperties { get; set; }
      
    }
}
