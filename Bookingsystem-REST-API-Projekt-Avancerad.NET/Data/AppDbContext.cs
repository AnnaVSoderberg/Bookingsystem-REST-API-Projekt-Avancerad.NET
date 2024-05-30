using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Projekt_API_Models;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<History> History { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Appointments)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId);


            modelBuilder.Entity<Company>()
                .HasMany(c => c.Appointments)
                .WithOne(a => a.Company)
                .HasForeignKey(a => a.CompanyId);



            modelBuilder.Entity<Customer>().HasData(new Customer
            {
                CustomerId = 1,
                CustomerName = "Hannes Dahlberg",
                CustomerEmail = "Hannes@test.se",
                CustomerPhoneNumber = "123456",

            });

            modelBuilder.Entity<Customer>().HasData(new Customer
            {
                CustomerId = 2,
                CustomerName = "Börje Svensson",
                CustomerEmail = "Börje@test.se",
                CustomerPhoneNumber = "12356887",

            });

            modelBuilder.Entity<Customer>().HasData(new Customer
            {
                CustomerId = 3,
                CustomerName = "Twei Twot",
                CustomerEmail = "Twei@test.se",
                CustomerPhoneNumber = "456622345"
            });

            modelBuilder.Entity<Company>().HasData(new Company
            {
                CompanyId = 1,
                CompanyName = "Paintball",

            });

            modelBuilder.Entity<Company>().HasData(new Company
            {
                CompanyId = 2,
                CompanyName = "Bowling",

            });

            modelBuilder.Entity<Company>().HasData(new Company
            {
                CompanyId = 3,
                CompanyName = "Go Cart",

            });

            // Seed appointments
            modelBuilder.Entity<Appointment>().HasData(new Appointment
            {
                AppointmentId = 1,
                AppointmentTime = DateTime.Now.AddDays(1),
                CompanyId = 1,
                CustomerId = 1,
            },
            new Appointment
            {
                AppointmentId = 2,
                AppointmentTime = DateTime.Now.AddDays(12),
                CompanyId = 2,
                CustomerId = 2,
            },
            new Appointment
            {
                AppointmentId = 3,
                AppointmentTime = DateTime.Now.AddDays(3),
                CompanyId = 3,
                CustomerId = 3,
            },
            new Appointment
            {
                AppointmentId = 4,
                AppointmentTime = DateTime.Now.AddDays(15),
                CompanyId = 1,
                CustomerId = 2,
            },
            new Appointment
            {
                AppointmentId = 5,
                AppointmentTime = DateTime.Now.AddDays(5),
                CompanyId = 2,
                CustomerId = 3,
            },
            new Appointment
            {
                AppointmentId = 6,
                AppointmentTime = DateTime.Now.AddDays(6),
                CompanyId = 3,
                CustomerId = 1,
            },
            new Appointment
            {
                AppointmentId = 7,
                AppointmentTime = DateTime.Now.AddDays(12),
                CompanyId = 1,
                CustomerId = 3,
            },
            new Appointment
            {
                AppointmentId = 8,
                AppointmentTime = DateTime.Now.AddDays(8),
                CompanyId = 2,
                CustomerId = 1,
            },
            new Appointment
            {
                AppointmentId = 9,
                AppointmentTime = DateTime.Now.AddDays(9),
                CompanyId = 3,
                CustomerId = 2,
            },
            new Appointment
            {
                AppointmentId = 10,
                AppointmentTime = DateTime.Now.AddDays(17),
                CompanyId = 1,
                CustomerId = 1,
            });

        }
    }
}
