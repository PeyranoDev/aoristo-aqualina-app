using Data.Entities;
using Data.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class AqualinaAPIContext : DbContext
    {
        public AqualinaAPIContext(DbContextOptions<AqualinaAPIContext> options) : base(options)
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<NotificationToken> NotificationTokens { get; set; }
        public DbSet<Tower> Towers { get; set; }
        public DbSet<AppSettings> AppSettings { get; set; }
        public DbSet<AmenityAvailability> AmenityAvailabilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de relaciones principales
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Apartment>()
                .HasMany(a => a.Users)
                .WithOne(u => u.Apartment)
                .HasForeignKey(u => u.ApartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Reservation>()
                .HasOne<User>()
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Amenity)
                .WithMany(a => a.Reservations)
                .HasForeignKey(r => r.AmenityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.RequestedBy) 
                .WithMany(u => u.RequestsMade) 
                .HasForeignKey(r => r.RequestedById) 
                .OnDelete(DeleteBehavior.Restrict); 

      
            modelBuilder.Entity<Request>()
                .HasOne(r => r.CompletedBy) 
                .WithMany(u => u.RequestsCompleted)
                .HasForeignKey(r => r.CompletedById) 
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Request>()
                .HasOne(r => r.Vehicle)
                .WithMany(v => v.Requests)
                .HasForeignKey(r => r.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Tower>()
                .HasMany(t => t.Apartments)
                .WithOne(a => a.Tower)
                .HasForeignKey(a => a.TowerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tower>()
                .HasMany(t => t.News)
                .WithOne(n => n.Tower)
                .HasForeignKey(n => n.TowerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tower>()
                .HasMany(t => t.Amenities)
                .WithOne(a => a.Tower)
                .HasForeignKey(a => a.TowerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tower>()
                .HasOne(t => t.Settings)
                .WithOne(s => s.Tower)
                .HasForeignKey<AppSettings>(s => s.TowerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Amenity>()
                .HasMany(a => a.Availabilities)
                .WithOne(aa => aa.Amenity)
                .HasForeignKey(aa => aa.AmenityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Vehicle>()
                .HasOne<User>()
                .WithMany(u => u.OwnedCars)
                .HasForeignKey(v => v.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NotificationToken>()
                .HasOne<User>()
                .WithMany(u => u.NotificationTokens)
                .HasForeignKey(nt => nt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices para optimización
            modelBuilder.Entity<Reservation>()
                .HasIndex(r => new { r.AmenityId, r.ReservationDate, r.Status });

            modelBuilder.Entity<Request>()
                .HasIndex(r => new { r.VehicleId, r.Status })
                .IncludeProperties(r => new { r.CompletedAt, r.RequestedAt });

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.Plate)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<AmenityAvailability>()
                .HasIndex(a => new { a.AmenityId, a.DayOfWeek });

            // Configuración de enums
            modelBuilder.Entity<Reservation>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Request>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Role>()
                .Property(r => r.Type)
                .HasConversion<string>();

            // Seed de roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Type = UserRoleEnum.Admin },
                new Role { Id = 2, Type = UserRoleEnum.Security },
                new Role { Id = 3, Type = UserRoleEnum.User }
            );

            modelBuilder.Entity<AppSettings>()
                .OwnsOne(a => a.Colors, c =>
                {
                    c.Property(p => p.Primary).HasColumnName("PrimaryColor");
                    c.Property(p => p.Secondary).HasColumnName("SecondaryColor");
                    c.Property(p => p.Accent).HasColumnName("AccentColor");
                    c.Property(p => p.Background).HasColumnName("BackgroundColor");
                });
        }
    }
}