using Microsoft.EntityFrameworkCore;
using TecAir.Domain.Models;

namespace TecAir.Infrastructure.Persistence;

/*
Descripción: Contexto de Entity Framework Core para el dominio de TECAir.
Entradas: Opciones de configuración de base de datos.
Salidas: Conjunto de DbSet y mapeos relacionales para las entidades del sistema.
Restricciones: Debe mantenerse coherente con los nombres de tablas y relaciones del modelo.
*/
public sealed class TecAirDb(DbContextOptions<TecAirDb> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Avion> Aviones => Set<Avion>();
    public DbSet<Aeropuerto> Aeropuertos => Set<Aeropuerto>();
    public DbSet<Ruta> Rutas => Set<Ruta>();
    public DbSet<Escala> Escalas => Set<Escala>();
    public DbSet<Vuelo> Vuelos => Set<Vuelo>();
    public DbSet<Reservacion> Reservaciones => Set<Reservacion>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<CheckIn> CheckIns => Set<CheckIn>();
    public DbSet<Maleta> Maletas => Set<Maleta>();
    public DbSet<Promocion> Promociones => Set<Promocion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuario");
            entity.HasKey(x => x.IdUsuario);
            entity.Property(x => x.IdUsuario).HasColumnName("id_usuario");
            entity.Property(x => x.Nombre1).HasColumnName("nombre1").IsRequired();
            entity.Property(x => x.Nombre2).HasColumnName("nombre2");
            entity.Property(x => x.Apellido1).HasColumnName("apellido1");
            entity.Property(x => x.Apellido2).HasColumnName("apellido2");
            entity.Property(x => x.Telefono).HasColumnName("telefono");
            entity.Property(x => x.Correo).HasColumnName("correo").IsRequired();
            entity.HasIndex(x => x.Correo).IsUnique();
            entity.Property(x => x.EsEstudiante).HasColumnName("es_estudiante");
            entity.Property(x => x.Universidad).HasColumnName("universidad");
            entity.Property(x => x.Carnet).HasColumnName("carnet");
            entity.Property(x => x.Millas).HasColumnName("millas");
            entity.Property(x => x.EsAdmin).HasColumnName("es_admin");
        });

        modelBuilder.Entity<Avion>(entity =>
        {
            entity.ToTable("avion");
            entity.HasKey(x => x.Matricula);
            entity.Property(x => x.Matricula).HasColumnName("matricula");
            entity.Property(x => x.Capacidad).HasColumnName("capacidad");
        });

        modelBuilder.Entity<Aeropuerto>(entity =>
        {
            entity.ToTable("aeropuerto");
            entity.HasKey(x => x.IdAeropuerto);
            entity.Property(x => x.IdAeropuerto).HasColumnName("id_aeropuerto");
            entity.Property(x => x.Nombre).HasColumnName("nombre").IsRequired();
            entity.Property(x => x.Ubicacion).HasColumnName("ubicacion");
        });

        modelBuilder.Entity<Ruta>(entity =>
        {
            entity.ToTable("ruta");
            entity.HasKey(x => x.IdRuta);
            entity.Property(x => x.IdRuta).HasColumnName("id_ruta");
        });

        modelBuilder.Entity<Escala>(entity =>
        {
            entity.ToTable("escala");
            entity.HasKey(x => new { x.IdRuta, x.Orden });
            entity.Property(x => x.IdRuta).HasColumnName("id_ruta");
            entity.Property(x => x.Orden).HasColumnName("orden");
            entity.Property(x => x.IdAeropuerto).HasColumnName("id_aeropuerto");
            entity.Property(x => x.Tipo).HasColumnName("tipo").IsRequired();
            entity.HasOne(x => x.Ruta).WithMany(x => x.Escalas).HasForeignKey(x => x.IdRuta).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Aeropuerto).WithMany(x => x.Escalas).HasForeignKey(x => x.IdAeropuerto);
        });

        modelBuilder.Entity<Vuelo>(entity =>
        {
            entity.ToTable("vuelo");
            entity.HasKey(x => x.IdVuelo);
            entity.Property(x => x.IdVuelo).HasColumnName("id_vuelo");
            entity.Property(x => x.FechaSalida).HasColumnName("fecha_salida");
            entity.Property(x => x.HoraSalida).HasColumnName("hora_salida");
            entity.Property(x => x.Puerta).HasColumnName("puerta");
            entity.Property(x => x.Estado).HasColumnName("estado");
            entity.Property(x => x.Matricula).HasColumnName("matricula");
            entity.Property(x => x.IdRuta).HasColumnName("id_ruta");
            entity.Property(x => x.Precio).HasColumnName("precio").HasColumnType("decimal(10,2)");
            entity.HasOne(x => x.Avion).WithMany(x => x.Vuelos).HasForeignKey(x => x.Matricula);
            entity.HasOne(x => x.Ruta).WithMany(x => x.Vuelos).HasForeignKey(x => x.IdRuta);
        });

        modelBuilder.Entity<Reservacion>(entity =>
        {
            entity.ToTable("reservacion");
            entity.HasKey(x => x.IdReservacion);
            entity.Property(x => x.IdReservacion).HasColumnName("id_reservacion");
            entity.Property(x => x.Estado).HasColumnName("estado");
            entity.Property(x => x.FechaReservacion).HasColumnName("fecha_reservacion");
            entity.Property(x => x.IdUsuario).HasColumnName("id_usuario");
            entity.Property(x => x.IdVuelo).HasColumnName("id_vuelo");
            entity.HasOne(x => x.Usuario).WithMany(x => x.Reservaciones).HasForeignKey(x => x.IdUsuario);
            entity.HasOne(x => x.Vuelo).WithMany(x => x.Reservaciones).HasForeignKey(x => x.IdVuelo);
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.ToTable("pago");
            entity.HasKey(x => x.IdPago);
            entity.Property(x => x.IdPago).HasColumnName("id_pago");
            entity.Property(x => x.Monto).HasColumnName("monto").HasColumnType("decimal(10,2)");
            entity.Property(x => x.Metodo).HasColumnName("metodo");
            entity.Property(x => x.IdReservacion).HasColumnName("id_reservacion");
            entity.HasOne(x => x.Reservacion).WithOne(x => x.Pago).HasForeignKey<Pago>(x => x.IdReservacion);
        });

        modelBuilder.Entity<CheckIn>(entity =>
        {
            entity.ToTable("checkin");
            entity.HasKey(x => x.IdCheckin);
            entity.Property(x => x.IdCheckin).HasColumnName("id_checkin");
            entity.Property(x => x.Asiento).HasColumnName("asiento");
            entity.Property(x => x.IdUsuario).HasColumnName("id_usuario");
            entity.Property(x => x.IdVuelo).HasColumnName("id_vuelo");
            entity.HasIndex(x => new { x.IdVuelo, x.Asiento }).IsUnique();
            entity.HasIndex(x => new { x.IdUsuario, x.IdVuelo }).IsUnique();
            entity.HasOne(x => x.Usuario).WithMany(x => x.CheckIns).HasForeignKey(x => x.IdUsuario);
            entity.HasOne(x => x.Vuelo).WithMany(x => x.CheckIns).HasForeignKey(x => x.IdVuelo);
        });

        modelBuilder.Entity<Maleta>(entity =>
        {
            entity.ToTable("maleta");
            entity.HasKey(x => x.NumMaleta);
            entity.Property(x => x.NumMaleta).HasColumnName("num_maleta");
            entity.Property(x => x.Peso).HasColumnName("peso").HasColumnType("decimal(10,2)");
            entity.Property(x => x.Color).HasColumnName("color");
            entity.Property(x => x.IdCheckin).HasColumnName("id_checkin");
            entity.HasOne(x => x.CheckIn).WithMany(x => x.Maletas).HasForeignKey(x => x.IdCheckin);
        });

        modelBuilder.Entity<Promocion>(entity =>
        {
            entity.ToTable("promocion");
            entity.HasKey(x => x.IdPromocion);
            entity.Property(x => x.IdPromocion).HasColumnName("id_promocion");
            entity.Property(x => x.Precio).HasColumnName("precio").HasColumnType("decimal(10,2)");
            entity.Property(x => x.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(x => x.FechaFin).HasColumnName("fecha_fin");
            entity.Property(x => x.Imagen).HasColumnName("imagen");
            entity.Property(x => x.IdRuta).HasColumnName("id_ruta");
            entity.HasOne(x => x.Ruta).WithMany(x => x.Promociones).HasForeignKey(x => x.IdRuta);
        });
    }
}
