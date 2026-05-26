using Microsoft.EntityFrameworkCore;
using TecAir.Domain.Models;

namespace TecAir.Infrastructure.Persistence;

/*
Descripción:
Inicializa la base de datos con datos de ejemplo cuando no existen registros.
Entradas:
No recibe parámetros directos.
Salidas:
No retorna valor.
Restricciones:
Solo debe ejecutarse sobre una base vacía o recién creada.
*/
public static class DatabaseSeeder
{
    /*
    Descripción:
    Inicializa la base de la aplicación con datos predeterminados cuando está vacía.
    Entradas:
    Recibe una instancia de TecAirDb para verificar y cargar datos.
    Salidas:
    No retorna valor.
    Restricciones:
    Si la base de datos no está configurada, el seed se omite para evitar que la aplicación falle.
    */
    public static async Task SeedAsync(TecAirDb db)
    {
        try
        {
            if (await db.Usuarios.AnyAsync())
            {
                return;
            }

            db.Usuarios.Add(new Usuario
            {
                Nombre1 = "Ana",
                Apellido1 = "Mora",
                Telefono = "+506 8888-0000",
                Correo = "ana@tecair.cr",
                EsEstudiante = true,
                Universidad = "TEC",
                Carnet = "2026001",
                Millas = 500
            });

            db.Aviones.AddRange(
                new Avion { Matricula = "TI-TEC1", Capacidad = 120 },
                new Avion { Matricula = "TI-TEC2", Capacidad = 180 }
            );

            db.Aeropuertos.AddRange(
                new Aeropuerto
                {
                    IdAeropuerto = 1,
                    Nombre = "SJO - Juan Santamaría",
                    Ubicacion = "Alajuela, Costa Rica"
                },
                new Aeropuerto
                {
                    IdAeropuerto = 2,
                    Nombre = "LIR - Guanacaste",
                    Ubicacion = "Liberia, Costa Rica"
                },
                new Aeropuerto
                {
                    IdAeropuerto = 3,
                    Nombre = "XQP - Quepos",
                    Ubicacion = "Puntarenas, Costa Rica"
                },
                new Aeropuerto
                {
                    IdAeropuerto = 4,
                    Nombre = "GLF - Golfito",
                    Ubicacion = "Puntarenas, Costa Rica"
                }
            );

            db.Rutas.AddRange(
                new Ruta { IdRuta = 1 },
                new Ruta { IdRuta = 2 }
            );

            await db.SaveChangesAsync();

            db.Escalas.AddRange(
                new Escala
                {
                    IdRuta = 1,
                    Orden = 1,
                    IdAeropuerto = 1,
                    Tipo = "origen"
                },
                new Escala
                {
                    IdRuta = 1,
                    Orden = 2,
                    IdAeropuerto = 2,
                    Tipo = "destino"
                },
                new Escala
                {
                    IdRuta = 2,
                    Orden = 1,
                    IdAeropuerto = 1,
                    Tipo = "origen"
                },
                new Escala
                {
                    IdRuta = 2,
                    Orden = 2,
                    IdAeropuerto = 3,
                    Tipo = "escala"
                },
                new Escala
                {
                    IdRuta = 2,
                    Orden = 3,
                    IdAeropuerto = 4,
                    Tipo = "destino"
                }
            );

            db.Vuelos.AddRange(
                new Vuelo
                {
                    FechaSalida = new DateOnly(2026, 6, 1),
                    HoraSalida = new TimeOnly(8, 0),
                    Puerta = "A3",
                    Estado = "programado",
                    Matricula = "TI-TEC1",
                    IdRuta = 1,
                    Precio = 105m
                },
                new Vuelo
                {
                    FechaSalida = new DateOnly(2026, 6, 2),
                    HoraSalida = new TimeOnly(14, 30),
                    Puerta = "B2",
                    Estado = "programado",
                    Matricula = "TI-TEC2",
                    IdRuta = 2,
                    Precio = 160m
                }
            );

            db.Promociones.Add(new Promocion
            {
                Precio = 89m,
                FechaInicio = new DateOnly(2026, 5, 1),
                FechaFin = new DateOnly(2026, 6, 30),
                Imagen = "promo-sjo-lir.jpg",
                IdRuta = 1
            });

            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Seed omitido porque la base de datos aún no está configurada.");
            Console.WriteLine($"Detalle: {ex.Message}");
        }
    }
}
