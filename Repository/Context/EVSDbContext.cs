using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Context
{
    public class EVSDbContext : DbContext
    {
        public EVSDbContext() { }
        public EVSDbContext(DbContextOptions<EVSDbContext> options) : base(options) { }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarDeliveryHistory> CarDeliveryHistories { get; set; }
        public DbSet<CarReturnHistory> CarReturnHistories { get; set; }
        public DbSet<CarRentalLocation> CarRentalLocations { get; set; }
        public DbSet<CitizenId> CitizenIds { get; set; }
        public DbSet<DriverLicense> DriverLicenses { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RentalOrder> RentalOrders { get; set; }
        public DbSet<RentalContact> RentalContacts { get; set; }
        public DbSet<RentalLocation> RentalLocations { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Car
            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasMany(e => e.CarRentalLocations)
                    .WithOne(e => e.Car)
                    .HasForeignKey(e => e.CarId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.RentalOrders)
                    .WithOne(e => e.Car)
                    .HasForeignKey(e => e.CarId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasData(
                    new Car
                    {
                        Id = 1,
                        Model = "Tesla Model 3",
                        Name = "Model 3",
                        Seats = 5,
                        SizeType = "Sedan",
                        TrunkCapacity = 425,
                        BatteryType = "Lithium-Ion",
                        BatteryDuration = 350,
                        RentPricePerDay = 1000000,
                        RentPricePerHour = 45000,
                        RentPricePerDayWithDriver = 1400000,
                        RentPricePerHourWithDriver = 60000,
                        ImageUrl = "https://example.com/tesla_model_3.jpg",
                        ImageUrl2 = "https://example.com/tesla_model_3.jpg",
                        ImageUrl3 = "https://example.com/tesla_model_3.jpg",
                        Status = 1,
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false
                    },
                    new Car
                    {
                        Id = 2,
                        Model = "Nissan Leaf",
                        Name = "Leaf",
                        Seats = 5,
                        SizeType = "Hatchback",
                        TrunkCapacity = 435,
                        BatteryType = "Lithium-Ion",
                        BatteryDuration = 240,
                        RentPricePerDay = 800000,
                        RentPricePerHour = 35000,
                        RentPricePerDayWithDriver = 1200000,
                        RentPricePerHourWithDriver = 50000,
                        ImageUrl = "https://example.com/nissan_leaf.jpg",
                        ImageUrl2 = "https://example.com/nissan_leaf.jpg",
                        ImageUrl3 = "https://example.com/nissan_leaf.jpg",
                        Status = 1,
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false
                    },
                    new Car
                    {
                        Id = 3,
                        Model = "Chevrolet Bolt EV",
                        Name = "Bolt EV",
                        Seats = 5,
                        SizeType = "Hatchback",
                        TrunkCapacity = 478,
                        BatteryType = "Lithium-Ion",
                        BatteryDuration = 259,
                        RentPricePerDay = 900000,
                        RentPricePerHour = 40000,
                        RentPricePerDayWithDriver = 1300000,
                        RentPricePerHourWithDriver = 55000,
                        ImageUrl = "https://example.com/chevrolet_bolt_ev.jpg",
                        ImageUrl2 = "https://example.com/chevrolet_bolt_ev.jpg",
                        ImageUrl3 = "https://example.com/chevrolet_bolt_ev.jpg",
                        Status = 1,
                        CreatedAt = new DateTime(2025, 10, 11),
                        UpdatedAt = null,
                        IsActive = true,
                        IsDeleted = false
                    }
                );
            });

            // Configure CarDeliveryHistory
            modelBuilder.Entity<CarDeliveryHistory>(entity =>
            {
                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Customer)
                    .WithMany(e => e.CarDeliveryHistories)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Staff)
                    .WithMany()
                    .HasForeignKey(e => e.StaffId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Car)
                    .WithMany()
                    .HasForeignKey(e => e.CarId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure CarRentalLocation
            modelBuilder.Entity<CarRentalLocation>(entity =>
            {
                entity.HasOne(e => e.Car)
                    .WithMany(e => e.CarRentalLocations)
                    .HasForeignKey(e => e.CarId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Location)
                    .WithMany(e => e.CarRentalLocations)
                    .HasForeignKey(e => e.LocationId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasData(
                    new CarRentalLocation
                    {
                        Id = 1,
                        CarId = 1,
                        LocationId = 1,
                        Quantity = 5
                    });
            });

            // Configure CarReturnHistory
            modelBuilder.Entity<CarReturnHistory>(entity =>
            {
                entity.HasOne(e => e.Order)
                    .WithMany()
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Staff)
                    .WithMany()
                    .HasForeignKey(e => e.StaffId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Car)
                    .WithMany()
                    .HasForeignKey(e => e.CarId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure CitizenId
            modelBuilder.Entity<CitizenId>(entity =>
            {
                entity.HasOne(e => e.RentalOrder)
                    .WithOne(e => e.CitizenIdNavigation)
                    .HasForeignKey<CitizenId>(e => e.RentalOrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            // Configure DriverLicense
            modelBuilder.Entity<DriverLicense>(entity =>
                {
                    entity.HasOne(e => e.RentalOrder)
                        .WithOne(e => e.DriverLicense)
                        .HasForeignKey<DriverLicense>(e => e.RentalOrderId)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                // Configure Feedback
                modelBuilder.Entity<Feedback>(entity =>
                {
                    entity.HasOne(e => e.User)
                        .WithMany(e => e.Feedback)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(e => e.RentalOrder)
                        .WithMany()
                        .HasForeignKey(e => e.RentalOrderId)
                        .OnDelete(DeleteBehavior.Restrict);
                });

                // Configure Payment
                modelBuilder.Entity<Payment>(entity =>
                {
                    entity.HasOne(e => e.User)
                        .WithMany(e => e.Payments)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.Cascade);
                    entity.HasOne(e => e.RentalOrder)
                        .WithOne(e => e.Payment)
                        .HasForeignKey<Payment>(e => e.RentalOrderId)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                // Configure RentalContact
                modelBuilder.Entity<RentalContact>(entity =>
                {
                    entity.HasOne(e => e.RentalOrder)
                        .WithOne(e => e.RentalContact)
                        .HasForeignKey<RentalOrder>(e => e.RentalContactId)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                // Configure RentalLocation
                modelBuilder.Entity<RentalLocation>(entity =>
                {
                    entity.HasMany(e => e.CarRentalLocations)
                        .WithOne(e => e.Location)
                        .HasForeignKey(e => e.LocationId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasMany(e => e.RentalOrders)
                        .WithOne(e => e.RentalLocation)
                        .HasForeignKey(e => e.RentalLocationId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasMany(e => e.Users)
                        .WithOne(e => e.RentalLocation)
                        .HasForeignKey(e => e.RentalLocationId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasData(
                        new RentalLocation
                        {
                            Id = 1,
                            Name = "Downtown Rental Location",
                            Address = "123 Tran Hung Dao St, Ho Chi Minh City",
                            Coordinates = "10.7769,106.7009",
                            CreatedAt = new DateTime(2025, 10, 11),
                            UpdatedAt = null,
                            IsActive = true,
                            IsDeleted = false
                        },
                        new RentalLocation
                        {
                            Id = 2,
                            Name = "Airport Rental Location",
                            Address = "456 Nguyen Cuu Phuc St, Ho Chi Minh City",
                            Coordinates = "10.7950,106.6540",
                            CreatedAt = new DateTime(2025, 10, 11),
                            UpdatedAt = null,
                            IsActive = true,
                            IsDeleted = false
                        });
                });

                // Configure RentalOrder
                modelBuilder.Entity<RentalOrder>(entity =>
                {
                    entity.HasOne(e => e.User)
                        .WithMany(e => e.RentalOrders)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(e => e.Car)
                        .WithMany(e => e.RentalOrders)
                        .HasForeignKey(e => e.CarId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(e => e.RentalContact)
                        .WithOne(e => e.RentalOrder)
                        .HasForeignKey<RentalOrder>(e => e.RentalContactId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(e => e.Payment)
                        .WithOne(e => e.RentalOrder)
                        .HasForeignKey<RentalOrder>(e => e.PaymentId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(e => e.RentalLocation)
                        .WithMany(e => e.RentalOrders)
                        .HasForeignKey(e => e.RentalLocationId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(e => e.CitizenIdNavigation)
                        .WithOne(e => e.RentalOrder)
                        .HasForeignKey<RentalOrder>(e => e.CitizenId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(e => e.DriverLicense)
                        .WithOne(e => e.RentalOrder)
                        .HasForeignKey<RentalOrder>(e => e.DriverLicenseId)
                        .OnDelete(DeleteBehavior.Cascade);
                });

                // Configure User
                modelBuilder.Entity<User>(entity =>
                {
                    entity.HasOne(e => e.RentalLocation)
                        .WithMany(e => e.Users)
                        .HasForeignKey(e => e.RentalLocationId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasMany(e => e.Feedback)
                        .WithOne(e => e.User)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasMany(e => e.RentalOrders)
                        .WithOne(e => e.User)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.Restrict);

                    entity.HasMany(e => e.Payments)
                        .WithOne(e => e.User)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasMany(e => e.CarDeliveryHistories)
                        .WithOne(e => e.Customer)
                        .HasForeignKey(e => e.CustomerId)
                        .OnDelete(DeleteBehavior.Restrict);

                    // Note: Ignored ICollection<CarRentalLocation> as no corresponding FK in CarRentalLocation
                    entity.HasData(
                new User
                {
                    Id = 1,
                    Email = "admin@gmail.com",
                    Password = "1",
                    PasswordHash = "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm",
                    FullName = "Admin User",
                    Role = "Admin",
                    ConfirmEmailToken = null,
                    IsEmailConfirmed = true,
                    CreatedAt = new DateTime(2025, 10, 11),
                    UpdatedAt = null,
                    IsActive = true,
                    RentalLocationId = null
                },
                new User
                {
                    Id = 2,
                    Email = "staff@gmail.com",
                    Password = "1",
                    PasswordHash = "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm",
                    FullName = "Staff User",
                    Role = "Staff",
                    ConfirmEmailToken = null,
                    IsEmailConfirmed = true,
                    CreatedAt = new DateTime(2025, 10, 11),
                    UpdatedAt = null,
                    IsActive = true,
                    RentalLocationId = 1
                },
                new User
                {
                    Id = 3,
                    Email = "customer@gmail.com",
                    Password = "1",
                    PasswordHash = "$2a$12$z.y2vdQFkt/drkj6yzAXm.6v/rirvWIaw1tXyIgvR7dki1roEfLXm",
                    FullName = "Customer User",
                    Role = "Customer",
                    ConfirmEmailToken = null,
                    IsEmailConfirmed = true,
                    CreatedAt = new DateTime(2025, 10, 11),
                    UpdatedAt = null,
                    IsActive = true,
                    RentalLocationId = null
                }
                    );
                });

            base.OnModelCreating(modelBuilder);
         
        }
    }
}
//Add-Migration InitMigration -Context EVSDbContext -Project Repository -StartupProject EVStation-basedRentalSystem -OutputDir Context/Migrations