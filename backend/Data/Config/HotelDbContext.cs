using Microsoft.EntityFrameworkCore;
using HotelManagement.Models;

namespace HotelManagement.Datos.Config
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<DetalleReserva> DetalleReservas { get; set; }
        public DbSet<Habitacion> Habitaciones { get; set; }
        public DbSet<Huesped> Huespedes { get; set; }
        public DbSet<TipoHabitacion> TipoHabitaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuario");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Usuario1).HasColumnName("Usuario").IsRequired();
                entity.HasIndex(e => e.Usuario1).IsUnique();
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Cliente");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Email).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                
                entity.HasOne(e => e.UsuarioCreacion)
                      .WithMany(u => u.ClientesCreados)
                      .HasForeignKey(e => e.Usuario_Creacion_ID)
                      .OnDelete(DeleteBehavior.SetNull);
                      
                entity.HasOne(e => e.UsuarioActualizacion)
                      .WithMany(u => u.ClientesActualizados)
                      .HasForeignKey(e => e.Usuario_Actualizacion_ID)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<TipoHabitacion>(entity =>
            {
                entity.ToTable("Tipo_Habitacion");
                entity.HasKey(e => e.ID);
                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            modelBuilder.Entity<Reserva>(entity =>
            {
                entity.ToTable("Reserva");
                entity.HasKey(e => e.ID);
                entity.HasOne(e => e.Cliente)
                      .WithMany(c => c.Reservas)
                      .HasForeignKey(e => e.Cliente_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DetalleReserva>(entity =>
            {
                entity.ToTable("Detalle_Reserva");
                entity.HasKey(e => e.ID);
                entity.HasOne(e => e.Reserva)
                      .WithMany(r => r.DetalleReservas)
                      .HasForeignKey(e => e.Reserva_ID)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Habitacion)
                      .WithMany(h => h.DetalleReservas)
                      .HasForeignKey(e => e.Habitacion_ID)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Huesped)
                      .WithMany(hu => hu.DetalleReservas)
                      .HasForeignKey(e => e.Huesped_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Habitacion>(entity =>
            {
                entity.ToTable("Habitacion");
                entity.HasKey(e => e.ID);
                entity.HasIndex(e => e.Numero_Habitacion).IsUnique();
                entity.HasOne(e => e.TipoHabitacion)
                      .WithMany(t => t.Habitaciones)
                      .HasForeignKey(e => e.Tipo_Habitacion_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Huesped>(entity =>
            {
                entity.ToTable("Huesped");
                entity.HasKey(e => e.ID);
            });
        }
    }
}
