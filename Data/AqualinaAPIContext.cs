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

        // Nuevo DbSet para la tabla intermedia UserTower
        public DbSet<UserTower> UserTowers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relaciones principales

            // User y Role (Un Usuario tiene un Rol, un Rol tiene muchos Usuarios)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // No eliminar Roles si hay Usuarios asociados

            // Apartment y User (Un Apartamento tiene muchos Usuarios, un Usuario tiene un Apartamento)
            modelBuilder.Entity<Apartment>()
                .HasMany(a => a.Users)
                .WithOne(u => u.Apartment)
                .HasForeignKey(u => u.ApartmentId)
                .OnDelete(DeleteBehavior.SetNull); // Si se elimina un Apartamento, los Usuarios asociados quedan sin ApartmentId

            // Relación de muchos a muchos entre User y Tower a través de UserTower
            modelBuilder.Entity<UserTower>()
                .HasKey(ut => new { ut.UserId, ut.TowerId }); // Clave primaria compuesta

            modelBuilder.Entity<UserTower>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTowers) // Un User tiene muchas UserTower
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina un User, se eliminan sus asociaciones UserTower

            modelBuilder.Entity<UserTower>()
                .HasOne(ut => ut.Tower)
                .WithMany(t => t.UserTowers) // Una Tower tiene muchas UserTower
                .HasForeignKey(ut => ut.TowerId)
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina una Tower, se eliminan sus asociaciones UserTower

            // Reservation y User (Una Reserva pertenece a un Usuario, un Usuario tiene muchas Reservas)
            modelBuilder.Entity<Reservation>()
                .HasOne<User>() // Asumiendo que la propiedad de navegación se llama 'User' en Reservation
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); // No eliminar Usuarios si tienen Reservas activas

            // Reservation y Amenity (Una Reserva es para una Amenidad, una Amenidad tiene muchas Reservas)
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Amenity)
                .WithMany(a => a.Reservations)
                .HasForeignKey(r => r.AmenityId)
                .OnDelete(DeleteBehavior.Restrict); // No eliminar Amenidades si tienen Reservas

            // Request y User (RequestedBy)
            modelBuilder.Entity<Request>()
                .HasOne(r => r.RequestedBy)
                .WithMany(u => u.RequestsMade)
                .HasForeignKey(r => r.RequestedById)
                .OnDelete(DeleteBehavior.Restrict); // No eliminar Usuario si tiene solicitudes hechas

            // Request y User (CompletedBy)
            modelBuilder.Entity<Request>()
                .HasOne(r => r.CompletedBy)
                .WithMany(u => u.RequestsCompleted)
                .HasForeignKey(r => r.CompletedById)
                .OnDelete(DeleteBehavior.Restrict); // No eliminar Usuario si ha completado solicitudes

            // Request y Vehicle (Una Solicitud es para un Vehículo, un Vehículo tiene muchas Solicitudes)
            modelBuilder.Entity<Request>()
                .HasOne(r => r.Vehicle)
                .WithMany(v => v.Requests)
                .HasForeignKey(r => r.VehicleId)
                .OnDelete(DeleteBehavior.Restrict); // No eliminar Vehículo si tiene solicitudes

            // Tower y Apartment (Una Torre tiene muchos Apartamentos, un Apartamento pertenece a una Torre)
            modelBuilder.Entity<Tower>()
                .HasMany(t => t.Apartments)
                .WithOne(a => a.Tower)
                .HasForeignKey(a => a.TowerId)
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina una Torre, se eliminan sus Apartamentos

            // Tower y News (Una Torre tiene muchas Noticias, una Noticia pertenece a una Torre)
            modelBuilder.Entity<Tower>()
                .HasMany(t => t.News)
                .WithOne(n => n.Tower)
                .HasForeignKey(n => n.TowerId)
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina una Torre, se eliminan sus Noticias

            // Tower y Amenity (Una Torre tiene muchas Amenidades, una Amenidad pertenece a una Torre)
            modelBuilder.Entity<Tower>()
                .HasMany(t => t.Amenities)
                .WithOne(a => a.Tower)
                .HasForeignKey(a => a.TowerId)
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina una Torre, se eliminan sus Amenidades

            // Tower y AppSettings (Una Torre tiene una configuración de AppSettings, AppSettings pertenece a una Torre)
            modelBuilder.Entity<Tower>()
                .HasOne(t => t.Settings)
                .WithOne(s => s.Tower)
                .HasForeignKey<AppSettings>(s => s.TowerId) // Clave foránea en AppSettings
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina una Torre, se eliminan sus AppSettings

            // Amenity y AmenityAvailability (Una Amenidad tiene muchas Disponibilidades, una Disponibilidad es para una Amenidad)
            modelBuilder.Entity<Amenity>()
                .HasMany(a => a.Availabilities)
                .WithOne(aa => aa.Amenity)
                .HasForeignKey(aa => aa.AmenityId)
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina una Amenidad, se eliminan sus disponibilidades

            // Vehicle y User (Un Vehículo pertenece a un Usuario, un Usuario tiene muchos Vehículos)
            modelBuilder.Entity<Vehicle>()
                .HasOne<User>() // Asumiendo que la propiedad de navegación se llama 'Owner' en Vehicle
                .WithMany(u => u.OwnedCars)
                .HasForeignKey(v => v.OwnerId)
                .OnDelete(DeleteBehavior.Restrict); // No eliminar Usuario si tiene Vehículos

            // NotificationToken y User (Un Token pertenece a un Usuario, un Usuario tiene muchos Tokens)
            modelBuilder.Entity<NotificationToken>()
                .HasOne<User>() // Asumiendo que la propiedad de navegación se llama 'User' en NotificationToken
                .WithMany(u => u.NotificationTokens)
                .HasForeignKey(nt => nt.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina un Usuario, se eliminan sus Tokens

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

            // Configuración de enums para almacenamiento como string
            modelBuilder.Entity<Reservation>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Request>()
                .Property(r => r.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Role>()
                .Property(r => r.Type)
                .HasConversion<string>();

            // Seed de roles iniciales
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Type = UserRoleEnum.Admin },
                new Role { Id = 2, Type = UserRoleEnum.Security },
                new Role { Id = 3, Type = UserRoleEnum.User }
            );

            // Configuración de tipo poseído para AppSettings.Colors
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