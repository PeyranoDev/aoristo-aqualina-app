using Data.Entities;
using Data.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class AqualinaAPIContext : DbContext
    {
        public AqualinaAPIContext(DbContextOptions<AqualinaAPIContext> options): base(options)
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Apartment>()
                .HasMany(a => a.Users)
                .WithOne(u => u.Apartment)
                .HasForeignKey(u => u.ApartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
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
                .WithMany(u => u.Requests)
                .HasForeignKey(r => r.RequestedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.Vehicle)
                .WithMany(v => v.Requests)
                .HasForeignKey(r => r.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasData(
                    new Role
                    {
                        Id = 1,
                        
                        Type = UserRoleEnum.Admin
                    },
                    new Role
                    {
                        Id = 2,
                        Type = UserRoleEnum.Security
                    },
                    new Role
                    {
                        Id = 3,
                        Type = UserRoleEnum.User
                    }
            
            );
        }
    }
}

   

