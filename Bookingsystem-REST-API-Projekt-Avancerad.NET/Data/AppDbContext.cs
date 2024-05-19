using Microsoft.EntityFrameworkCore;
using Projekt_API_Models;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Data
{
    public class AppDbContext : DbContext
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

            modelBuilder.Entity<Appointment>()
                .HasMany(a => a.History)
                .WithOne(a => a.Appointment)
                .HasForeignKey(a => a.AppointmentId);


            modelBuilder.Entity<Customer>().HasData(new Customer
            {
                CustomerId = 1,
                CustomerName = "Hannes Dahlberg",
                CustomerEmail = "Hannes@test.se",
                CustomerPhoneNumber = "123456",
                Login = "Hannes",
                Password = "Password"
            });

            modelBuilder.Entity<Customer>().HasData(new Customer
            {
                CustomerId = 2,
                CustomerName = "Börje Svensson",
                CustomerEmail = "Börje@test.se",
                CustomerPhoneNumber = "12356887",
                Login = "Börje",
                Password = "Password"
            });

            modelBuilder.Entity<Customer>().HasData(new Customer
            {
                CustomerId = 3,
                CustomerName = "Twei Twot",
                CustomerEmail = "Twei@test.se",
                CustomerPhoneNumber = "456622345",
                Login = "Twei",
                Password = "Password"
            });

            // Seed companies
            modelBuilder.Entity<Company>().HasData(new Company
            {
                CompanyId = 1,
                CompanyName = "Paintball",
                Login = "Comp1",
                Password = "1234"
            });

            modelBuilder.Entity<Company>().HasData(new Company
            {
                CompanyId = 2,
                CompanyName = "Bowling",
                Login = "Comp2",
                Password = "1234"
            });

            modelBuilder.Entity<Company>().HasData(new Company
            {
                CompanyId = 3,
                CompanyName = "Go Cart",
                Login = "Comp3",
                Password = "1234"
            });

            // Seed appointments
            modelBuilder.Entity<Appointment>().HasData(new Appointment
            {
                AppointmentId = 1,
                AppointmentDiscription = "Just for fun",
                CreatingDateAppointment = new DateTime(2024,05,24),
                CompanyId = 2,
                CustomerId = 1,
            });

            modelBuilder.Entity<Appointment>().HasData(new Appointment
            {
                AppointmentId = 2,
                CompanyId = 1,
                CustomerId = 1,
                AppointmentDiscription = "Bachelorette party",
                CreatingDateAppointment = new DateTime(2024,05,20)
            });

            modelBuilder.Entity<Appointment>().HasData(new Appointment
            {
                AppointmentId = 3,
                CompanyId = 3,
                CustomerId = 2,
                AppointmentDiscription = "Teambuilding",
                CreatingDateAppointment = new DateTime(2024,05,27)
            });

            modelBuilder.Entity<Appointment>().HasData(new Appointment
            {
                AppointmentId = 4,
                CompanyId = 3,
                CustomerId = 1,
                AppointmentDiscription = "Teambuilding",
                CreatingDateAppointment = new DateTime(2024,05,26)
            });
           
        }
    }
}
