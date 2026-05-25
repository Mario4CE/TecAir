using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TecAir.Api.Data;
using TecAir.Api.Dtos;
using TecAir.Api.Repositories;
using TecAir.Api.Services;
using Xunit;

namespace TecAir.Api.Tests;

public class ServiceSmokeTests
{
    private static TecAirDb CrearDb(string nombre)
    {
        var options = new DbContextOptionsBuilder<TecAirDb>().UseInMemoryDatabase(nombre).Options;
        return new TecAirDb(options);
    }

    [Fact]
    public async Task AeropuertoService_CrearYListar_DebeFuncionar()
    {
        await using var db = CrearDb(Guid.NewGuid().ToString());
        var repo = new AeropuertoRepository(db);
        var service = new AeropuertoService(repo);

        await service.CrearAeropuertoAsync(new AeropuertoRequest("SJO", "CR"));
        var lista = await service.GetAeropuertosAsync();

        Assert.Single(lista);
    }

    [Fact]
    public async Task AvionService_CrearYListar_DebeFuncionar()
    {
        await using var db = CrearDb(Guid.NewGuid().ToString());
        var repo = new AvionRepository(db);
        var service = new AvionService(repo);

        await service.CrearAvionAsync(new AvionRequest("TI-123", 100));
        var lista = await service.GetAvionesAsync();

        Assert.Single(lista);
    }

    [Fact]
    public async Task ReservacionYPago_DebeCrearYActualizarEstado()
    {
        await using var db = CrearDb(Guid.NewGuid().ToString());
        db.Usuarios.Add(new() { IdUsuario = 1, Nombre1 = "A", Apellido1 = "B", Correo = "a@a.com", Telefono = "1" });
        db.Vuelos.Add(new() { IdVuelo = 1, IdRuta = 1, Matricula = "TI-1", FechaSalida = new DateOnly(2026,1,1), HoraSalida = new TimeOnly(8,0), Puerta = "A1", Estado = "programado", Precio = 100 });
        await db.SaveChangesAsync();

        var reservRepo = new ReservacionRepository(db);
        var reservSvc = new ReservacionService(reservRepo, Options.Create(new TecAirOptions()));
        var reserv = await reservSvc.CrearReservacionAsync(new ReservacionRequest(1, null, 1, null, null));

        var pagoRepo = new PagoRepository(db);
        var pagoSvc = new PagoService(pagoRepo);
        var pago = await pagoSvc.CrearPagoAsync(new PagoRequest(reserv.IdReservacion, null, 10, "tarjeta"));

        Assert.True(pago.IdPago > 0);
    }
}
