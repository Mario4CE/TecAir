/*
 * Este es el ejecutable del API
 */

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TecAir.Api.Data;
using TecAir.Api.Dtos;
using TecAir.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TecAirOptions>(builder.Configuration.GetSection("TecAir"));
var tecAirOptions = builder.Configuration.GetSection("TecAir").Get<TecAirOptions>() ?? new TecAirOptions();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("TecAirCors", policy =>
    {
        if (tecAirOptions.CorsOrigin == "*")
        {
            policy.AllowAnyOrigin();
        }
        else
        {
            policy.WithOrigins(tecAirOptions.CorsOrigin.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        }

        policy.AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddDbContext<TecAirDb>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("TecAirDb") ?? "Data Source=data/tecair.sqlite";
    options.UseSqlite(connectionString);
});

var app = builder.Build();
app.UseCors("TecAirCors");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TecAirDb>();
    await DatabaseSeeder.SeedAsync(db);
}

var api = app.MapGroup("/api");

api.MapGet("/", () => Results.Ok(new
{
    mensaje = "API TECAir activa",
    recursos = new[] { "usuarios", "aeropuertos", "aviones", "rutas", "vuelos", "reservaciones", "pagos", "promociones", "checkins", "maletas" }
}));

api.MapGet("/usuarios", async (TecAirDb db) =>
    Results.Ok(new { usuarios = await db.Usuarios.AsNoTracking().OrderBy(x => x.IdUsuario).ToListAsync() }));

api.MapGet("/usuarios/perfil", async (HttpRequest request, TecAirDb db) =>
{
    var idUsuario = ObtenerIdUsuarioHeader(request);
    var usuario = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);
    return usuario is null ? Results.NotFound(new { mensaje = "Usuario no encontrado." }) : Results.Ok(new { usuario });
});

api.MapPut("/usuarios/perfil", async (HttpRequest request, UsuarioRequest datos, TecAirDb db) =>
{
    var idUsuario = ObtenerIdUsuarioHeader(request);
    var usuario = await db.Usuarios.FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);
    if (usuario is null) return Results.NotFound(new { mensaje = "Usuario no encontrado." });

    ActualizarUsuario(usuario, datos);
    await db.SaveChangesAsync();
    return Results.Ok(new { mensaje = "Perfil actualizado.", usuario });
});

api.MapGet("/usuarios/{idUsuario:int}", async (int idUsuario, TecAirDb db) =>
{
    var usuario = await db.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);
    return usuario is null ? Results.NotFound(new { mensaje = "Usuario no encontrado." }) : Results.Ok(new { usuario });
});

api.MapPost("/usuarios", async (UsuarioRequest datos, TecAirDb db) =>
{
    var correo = datos.Correo ?? datos.Email;
    if (string.IsNullOrWhiteSpace(datos.Nombre1) && string.IsNullOrWhiteSpace(datos.NombreCompleto))
        return Results.BadRequest(new { mensaje = "El nombre es obligatorio." });
    if (string.IsNullOrWhiteSpace(correo))
        return Results.BadRequest(new { mensaje = "El correo es obligatorio." });

    var nombre = SepararNombre(datos.NombreCompleto);
    var usuario = new Usuario
    {
        Nombre1 = datos.Nombre1 ?? nombre.Nombre1,
        Nombre2 = datos.Nombre2 ?? nombre.Nombre2,
        Apellido1 = datos.Apellido1 ?? nombre.Apellido1,
        Apellido2 = datos.Apellido2 ?? nombre.Apellido2,
        Telefono = datos.Telefono ?? string.Empty,
        Correo = correo,
        EsEstudiante = datos.EsEstudiante ?? false,
        Universidad = datos.Universidad ?? string.Empty,
        Carnet = datos.Carnet ?? string.Empty,
        Millas = datos.Millas ?? 0
    };

    db.Usuarios.Add(usuario);
    await db.SaveChangesAsync();
    return Results.Created($"/api/usuarios/{usuario.IdUsuario}", new { mensaje = "Usuario creado.", usuario });
});

api.MapGet("/aeropuertos", async (TecAirDb db) =>
    Results.Ok(new { aeropuertos = await db.Aeropuertos.AsNoTracking().OrderBy(x => x.Nombre).ToListAsync() }));

api.MapPost("/aeropuertos", async (AeropuertoRequest datos, TecAirDb db) =>
{
    if (string.IsNullOrWhiteSpace(datos.Nombre)) return Results.BadRequest(new { mensaje = "El nombre del aeropuerto es obligatorio." });
    var aeropuerto = new Aeropuerto { Nombre = datos.Nombre, Ubicacion = datos.Ubicacion ?? string.Empty };
    db.Aeropuertos.Add(aeropuerto);
    await db.SaveChangesAsync();
    return Results.Created($"/api/aeropuertos/{aeropuerto.IdAeropuerto}", new { mensaje = "Aeropuerto creado.", aeropuerto });
});

api.MapGet("/aviones", async (TecAirDb db) =>
    Results.Ok(new { aviones = await db.Aviones.AsNoTracking().OrderBy(x => x.Matricula).ToListAsync() }));

api.MapPost("/aviones", async (AvionRequest datos, TecAirDb db) =>
{
    if (string.IsNullOrWhiteSpace(datos.Matricula)) return Results.BadRequest(new { mensaje = "La matrícula es obligatoria." });
    if (datos.Capacidad <= 0) return Results.BadRequest(new { mensaje = "La capacidad debe ser mayor a cero." });
    var avion = new Avion { Matricula = datos.Matricula, Capacidad = datos.Capacidad };
    db.Aviones.Add(avion);
    await db.SaveChangesAsync();
    return Results.Created($"/api/aviones/{avion.Matricula}", new { mensaje = "Avión creado.", avion });
});

api.MapGet("/rutas", async (TecAirDb db) =>
    Results.Ok(new { rutas = await ObtenerRutasAsync(db) }));

api.MapPost("/rutas", async (RutaRequest datos, TecAirDb db) =>
{
    if (datos.Escalas.Count < 2) return Results.BadRequest(new { mensaje = "Debe indicar al menos origen y destino en escalas." });

    var ruta = new Ruta();
    db.Rutas.Add(ruta);
    await db.SaveChangesAsync();

    for (var index = 0; index < datos.Escalas.Count; index++)
    {
        var escala = datos.Escalas[index];
        db.Escalas.Add(new Escala
        {
            IdRuta = ruta.IdRuta,
            Orden = escala.Orden ?? index + 1,
            IdAeropuerto = escala.IdAeropuerto,
            Tipo = escala.Tipo ?? (index == 0 ? "origen" : index == datos.Escalas.Count - 1 ? "destino" : "escala")
        });
    }

    await db.SaveChangesAsync();
    return Results.Created($"/api/rutas/{ruta.IdRuta}", new { mensaje = "Ruta creada.", ruta = await ObtenerRutaAsync(db, ruta.IdRuta) });
});

api.MapGet("/vuelos", async (string? origen, string? destino, TecAirDb db) =>
    Results.Ok(new { vuelos = await ObtenerVuelosAsync(db, origen, destino) }));

api.MapGet("/vuelos/{idVuelo:int}", async (int idVuelo, TecAirDb db) =>
{
    var vuelo = (await ObtenerVuelosAsync(db, null, null)).FirstOrDefault(x => x.IdVuelo == idVuelo);
    return vuelo is null ? Results.NotFound(new { mensaje = "Vuelo no encontrado." }) : Results.Ok(new { vuelo });
});

api.MapPost("/vuelos", async (VueloRequest datos, TecAirDb db, IConfiguration configuration) =>
{
    var options = configuration.GetSection("TecAir").Get<TecAirOptions>() ?? new TecAirOptions();
    if (datos.IdRuta <= 0) return Results.BadRequest(new { mensaje = "La ruta es obligatoria." });
    if (string.IsNullOrWhiteSpace(datos.Matricula)) return Results.BadRequest(new { mensaje = "La matrícula del avión es obligatoria." });
    if (datos.FechaSalida is null) return Results.BadRequest(new { mensaje = "La fecha de salida es obligatoria." });

    var vuelo = new Vuelo
    {
        IdRuta = datos.IdRuta,
        Matricula = datos.Matricula,
        FechaSalida = datos.FechaSalida.Value,
        HoraSalida = datos.HoraSalida ?? new TimeOnly(8, 0),
        Puerta = datos.Puerta ?? options.DefaultGate,
        Estado = datos.Estado ?? "programado",
        Precio = datos.Precio ?? options.DefaultFlightPrice
    };

    db.Vuelos.Add(vuelo);
    await db.SaveChangesAsync();
    var respuesta = (await ObtenerVuelosAsync(db, null, null)).First(x => x.IdVuelo == vuelo.IdVuelo);
    return Results.Created($"/api/vuelos/{vuelo.IdVuelo}", new { mensaje = "Vuelo creado.", vuelo = respuesta });
});

api.MapPatch("/vuelos/{idVuelo:int}/abrir", async (int idVuelo, TecAirDb db) => await CambiarEstadoVueloAsync(idVuelo, "abierto", db));
api.MapPatch("/vuelos/{idVuelo:int}/cerrar", async (int idVuelo, TecAirDb db) => await CambiarEstadoVueloAsync(idVuelo, "cerrado", db));

api.MapGet("/reservaciones", async (HttpRequest request, TecAirDb db) =>
{
    var filtroUsuario = ObtenerEnteroQuery(request, "id_usuario")
        ?? ObtenerEnteroQuery(request, "usuario_id")
        ?? ObtenerEnteroQuery(request, "idUsuario")
        ?? ObtenerEnteroQuery(request, "usuarioId");
    var query = db.Reservaciones.AsNoTracking().OrderByDescending(x => x.FechaReservacion).AsQueryable();
    if (filtroUsuario.HasValue) query = query.Where(x => x.IdUsuario == filtroUsuario.Value);
    return Results.Ok(new { reservaciones = await query.ToListAsync() });
});

api.MapGet("/reservaciones/{idReservacion:int}", async (int idReservacion, TecAirDb db) =>
{
    var reservacion = await ObtenerReservacionAsync(db, idReservacion);
    return reservacion is null ? Results.NotFound(new { mensaje = "Reservación no encontrada." }) : Results.Ok(new { reservacion });
});

api.MapPost("/reservaciones", async (ReservacionRequest datos, TecAirDb db, IConfiguration configuration) =>
{
    var idUsuario = datos.IdUsuario ?? datos.UsuarioId;
    var idVuelo = datos.IdVuelo ?? datos.VueloId;
    if (idUsuario is null) return Results.BadRequest(new { mensaje = "El usuario es obligatorio." });
    if (idVuelo is null) return Results.BadRequest(new { mensaje = "El vuelo es obligatorio." });

    var usuario = await db.Usuarios.FirstOrDefaultAsync(x => x.IdUsuario == idUsuario.Value);
    if (usuario is null) return Results.NotFound(new { mensaje = "Usuario no encontrado." });
    if (!await db.Vuelos.AnyAsync(x => x.IdVuelo == idVuelo.Value)) return Results.NotFound(new { mensaje = "Vuelo no encontrado." });

    var options = configuration.GetSection("TecAir").Get<TecAirOptions>() ?? new TecAirOptions();
    var reservacion = new Reservacion
    {
        Estado = datos.Estado ?? "pendiente_pago",
        FechaReservacion = DateTime.UtcNow,
        IdUsuario = idUsuario.Value,
        IdVuelo = idVuelo.Value
    };

    usuario.Millas += options.LoyaltyMilesPerReservation;
    db.Reservaciones.Add(reservacion);
    await db.SaveChangesAsync();
    return Results.Created($"/api/reservaciones/{reservacion.IdReservacion}", new { mensaje = "Reservación creada.", reservacion = await ObtenerReservacionAsync(db, reservacion.IdReservacion) });
});

api.MapPatch("/reservaciones/{idReservacion:int}/cancelar", async (int idReservacion, TecAirDb db) =>
{
    var reservacion = await db.Reservaciones.FirstOrDefaultAsync(x => x.IdReservacion == idReservacion);
    if (reservacion is null) return Results.NotFound(new { mensaje = "Reservación no encontrada." });
    reservacion.Estado = "cancelada";
    await db.SaveChangesAsync();
    return Results.Ok(new { mensaje = "Reservación cancelada.", reservacion = await ObtenerReservacionAsync(db, idReservacion) });
});

api.MapGet("/pagos", async (TecAirDb db) =>
    Results.Ok(new { pagos = await db.Pagos.AsNoTracking().OrderByDescending(x => x.IdPago).ToListAsync() }));

api.MapPost("/pagos", async (PagoRequest datos, TecAirDb db) =>
{
    var idReservacion = datos.IdReservacion ?? datos.ReservacionId;
    if (idReservacion is null) return Results.BadRequest(new { mensaje = "La reservación es obligatoria." });
    if (datos.Monto <= 0) return Results.BadRequest(new { mensaje = "El monto debe ser mayor a cero." });

    var reservacion = await db.Reservaciones.FirstOrDefaultAsync(x => x.IdReservacion == idReservacion.Value);
    if (reservacion is null) return Results.NotFound(new { mensaje = "Reservación no encontrada." });

    var pago = new Pago { IdReservacion = idReservacion.Value, Monto = datos.Monto, Metodo = datos.Metodo ?? "tarjeta" };
    reservacion.Estado = "pagada";
    db.Pagos.Add(pago);
    await db.SaveChangesAsync();
    return Results.Created($"/api/pagos/{pago.IdPago}", new { mensaje = "Pago registrado.", pago });
});

api.MapGet("/promociones", async (TecAirDb db) =>
    Results.Ok(new { promociones = await ObtenerPromocionesAsync(db) }));

api.MapPost("/promociones", async (PromocionRequest datos, TecAirDb db) =>
{
    if (datos.IdRuta <= 0) return Results.BadRequest(new { mensaje = "La ruta es obligatoria." });
    if (datos.Precio <= 0) return Results.BadRequest(new { mensaje = "El precio promocional debe ser mayor a cero." });

    var promocion = new Promocion
    {
        IdRuta = datos.IdRuta,
        Precio = datos.Precio,
        FechaInicio = datos.FechaInicio,
        FechaFin = datos.FechaFin,
        Imagen = datos.Imagen ?? string.Empty
    };
    db.Promociones.Add(promocion);
    await db.SaveChangesAsync();
    return Results.Created($"/api/promociones/{promocion.IdPromocion}", new { mensaje = "Promoción creada.", promocion });
});

api.MapGet("/checkins", async (TecAirDb db) =>
    Results.Ok(new { checkins = await db.CheckIns.AsNoTracking().OrderByDescending(x => x.IdCheckin).ToListAsync() }));

api.MapGet("/checkins/{idCheckin:int}/pase-abordar", async (int idCheckin, TecAirDb db) =>
{
    var pase = await ObtenerPaseAbordarAsync(db, idCheckin);
    return pase is null ? Results.NotFound(new { mensaje = "Check-in no encontrado." }) : Results.Ok(new { pase_abordar = pase });
});

api.MapPost("/checkins", async (CheckInRequest datos, TecAirDb db) =>
{
    var idUsuario = datos.IdUsuario ?? datos.UsuarioId;
    var idVuelo = datos.IdVuelo ?? datos.VueloId;
    if (idUsuario is null) return Results.BadRequest(new { mensaje = "El usuario es obligatorio." });
    if (idVuelo is null) return Results.BadRequest(new { mensaje = "El vuelo es obligatorio." });
    if (string.IsNullOrWhiteSpace(datos.Asiento)) return Results.BadRequest(new { mensaje = "El asiento es obligatorio." });

    var asientoOcupado = await db.CheckIns.AnyAsync(x => x.IdVuelo == idVuelo.Value && x.Asiento == datos.Asiento);
    if (asientoOcupado) return Results.Conflict(new { mensaje = "El asiento ya está ocupado para este vuelo." });

    var checkIn = new CheckIn { IdUsuario = idUsuario.Value, IdVuelo = idVuelo.Value, Asiento = datos.Asiento };
    db.CheckIns.Add(checkIn);
    await db.SaveChangesAsync();
    return Results.Created($"/api/checkins/{checkIn.IdCheckin}", new { mensaje = "Check-in realizado.", pase_abordar = await ObtenerPaseAbordarAsync(db, checkIn.IdCheckin) });
});

api.MapGet("/maletas", async (TecAirDb db) =>
    Results.Ok(new { maletas = await db.Maletas.AsNoTracking().OrderBy(x => x.IdCheckin).ThenBy(x => x.NumMaleta).ToListAsync() }));

api.MapPost("/maletas", async (MaletaRequest datos, TecAirDb db) =>
{
    if (string.IsNullOrWhiteSpace(datos.NumMaleta)) return Results.BadRequest(new { mensaje = "El número de maleta es obligatorio." });
    if (!await db.CheckIns.AnyAsync(x => x.IdCheckin == datos.IdCheckin)) return Results.NotFound(new { mensaje = "Check-in no encontrado." });

    var maleta = new Maleta { NumMaleta = datos.NumMaleta, Peso = datos.Peso, Color = datos.Color, IdCheckin = datos.IdCheckin };
    db.Maletas.Add(maleta);
    await db.SaveChangesAsync();
    return Results.Created($"/api/maletas/{maleta.NumMaleta}", new { mensaje = "Maleta asignada.", resumen = await ResumenMaletasAsync(db, datos.IdCheckin) });
});

await app.RunAsync();

static int ObtenerIdUsuarioHeader(HttpRequest request)
{
    return int.TryParse(request.Headers["X-User-Id"].FirstOrDefault(), out var idUsuario) ? idUsuario : 1;
}


static int? ObtenerEnteroQuery(HttpRequest request, string nombre)
{
    return int.TryParse(request.Query[nombre].FirstOrDefault(), out var valor) ? valor : null;
}

static void ActualizarUsuario(Usuario usuario, UsuarioRequest datos)
{
    var nombre = SepararNombre(datos.NombreCompleto);
    usuario.Nombre1 = datos.Nombre1 ?? (string.IsNullOrWhiteSpace(nombre.Nombre1) ? usuario.Nombre1 : nombre.Nombre1);
    usuario.Nombre2 = datos.Nombre2 ?? (string.IsNullOrWhiteSpace(nombre.Nombre2) ? usuario.Nombre2 : nombre.Nombre2);
    usuario.Apellido1 = datos.Apellido1 ?? (string.IsNullOrWhiteSpace(nombre.Apellido1) ? usuario.Apellido1 : nombre.Apellido1);
    usuario.Apellido2 = datos.Apellido2 ?? (string.IsNullOrWhiteSpace(nombre.Apellido2) ? usuario.Apellido2 : nombre.Apellido2);
    usuario.Telefono = datos.Telefono ?? usuario.Telefono;
    usuario.Correo = datos.Correo ?? datos.Email ?? usuario.Correo;
    usuario.EsEstudiante = datos.EsEstudiante ?? usuario.EsEstudiante;
    usuario.Universidad = datos.Universidad ?? usuario.Universidad;
    usuario.Carnet = datos.Carnet ?? usuario.Carnet;
}

static (string Nombre1, string Nombre2, string Apellido1, string Apellido2) SepararNombre(string? nombreCompleto)
{
    var partes = (nombreCompleto ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    return (
        partes.ElementAtOrDefault(0) ?? string.Empty,
        partes.ElementAtOrDefault(1) ?? string.Empty,
        partes.ElementAtOrDefault(2) ?? string.Empty,
        string.Join(' ', partes.Skip(3)));
}

static async Task<List<object>> ObtenerRutasAsync(TecAirDb db)
{
    var rutas = await db.Rutas.AsNoTracking().OrderBy(x => x.IdRuta).ToListAsync();
    var resultado = new List<object>();
    foreach (var ruta in rutas)
    {
        resultado.Add(await ObtenerRutaAsync(db, ruta.IdRuta));
    }

    return resultado;
}

static async Task<object> ObtenerRutaAsync(TecAirDb db, int idRuta)
{
    var escalas = await db.Escalas.AsNoTracking()
        .Where(x => x.IdRuta == idRuta)
        .OrderBy(x => x.Orden)
        .Join(db.Aeropuertos.AsNoTracking(), escala => escala.IdAeropuerto, aeropuerto => aeropuerto.IdAeropuerto,
            (escala, aeropuerto) => new
            {
                escala.IdRuta,
                escala.Orden,
                escala.Tipo,
                aeropuerto.IdAeropuerto,
                aeropuerto.Nombre,
                aeropuerto.Ubicacion
            })
        .ToListAsync();

    return new { id_ruta = idRuta, escalas };
}

static async Task<List<VueloResponse>> ObtenerVuelosAsync(TecAirDb db, string? origen, string? destino)
{
    var vuelos = await db.Vuelos.AsNoTracking()
        .Include(x => x.Avion)
        .Include(x => x.Reservaciones)
        .OrderBy(x => x.FechaSalida)
        .ThenBy(x => x.HoraSalida)
        .ToListAsync();

    var respuestas = new List<VueloResponse>();
    foreach (var vuelo in vuelos)
    {
        var escalas = await db.Escalas.AsNoTracking()
            .Where(x => x.IdRuta == vuelo.IdRuta)
            .OrderBy(x => x.Orden)
            .Join(db.Aeropuertos.AsNoTracking(), escala => escala.IdAeropuerto, aeropuerto => aeropuerto.IdAeropuerto,
                (escala, aeropuerto) => new EscalaResponse(escala.IdRuta, escala.Orden, escala.Tipo, aeropuerto.IdAeropuerto, aeropuerto.Nombre, aeropuerto.Ubicacion))
            .ToListAsync();

        var escalaOrigen = escalas.FirstOrDefault(x => x.Tipo == "origen");
        var escalaDestino = escalas.FirstOrDefault(x => x.Tipo == "destino");
        if (!CoincideAeropuerto(escalaOrigen, origen) || !CoincideAeropuerto(escalaDestino, destino)) continue;

        var reservacionesActivas = vuelo.Reservaciones.Count(x => x.Estado != "cancelada");
        respuestas.Add(new VueloResponse(
            vuelo.IdVuelo,
            vuelo.FechaSalida,
            vuelo.HoraSalida,
            vuelo.Puerta,
            vuelo.Estado,
            vuelo.Matricula,
            vuelo.IdRuta,
            vuelo.Precio,
            vuelo.Avion?.Capacidad ?? 0,
            escalaOrigen?.Nombre ?? string.Empty,
            escalaDestino?.Nombre ?? string.Empty,
            escalaOrigen?.IdAeropuerto ?? 0,
            escalaDestino?.IdAeropuerto ?? 0,
            (vuelo.Avion?.Capacidad ?? 0) - reservacionesActivas,
            escalas));
    }

    return respuestas;
}

static bool CoincideAeropuerto(EscalaResponse? escala, string? filtro)
{
    if (string.IsNullOrWhiteSpace(filtro)) return true;
    if (escala is null) return false;
    return escala.IdAeropuerto.ToString() == filtro || escala.Nombre.Contains(filtro, StringComparison.OrdinalIgnoreCase);
}

static async Task<object?> ObtenerReservacionAsync(TecAirDb db, int idReservacion)
{
    var reservacion = await db.Reservaciones.AsNoTracking()
        .Include(x => x.Usuario)
        .Include(x => x.Pago)
        .FirstOrDefaultAsync(x => x.IdReservacion == idReservacion);
    if (reservacion is null) return null;

    var vuelo = (await ObtenerVuelosAsync(db, null, null)).FirstOrDefault(x => x.IdVuelo == reservacion.IdVuelo);
    return new { reservacion.IdReservacion, reservacion.Estado, reservacion.FechaReservacion, reservacion.IdUsuario, reservacion.IdVuelo, reservacion.Usuario, vuelo, reservacion.Pago };
}

static async Task<IResult> CambiarEstadoVueloAsync(int idVuelo, string estado, TecAirDb db)
{
    var vuelo = await db.Vuelos.FirstOrDefaultAsync(x => x.IdVuelo == idVuelo);
    if (vuelo is null) return Results.NotFound(new { mensaje = "Vuelo no encontrado." });
    vuelo.Estado = estado;
    await db.SaveChangesAsync();
    var respuesta = (await ObtenerVuelosAsync(db, null, null)).First(x => x.IdVuelo == idVuelo);
    return Results.Ok(new { mensaje = $"Vuelo {estado}.", vuelo = respuesta });
}

static async Task<List<object>> ObtenerPromocionesAsync(TecAirDb db)
{
    var promociones = await db.Promociones.AsNoTracking().OrderByDescending(x => x.FechaInicio).ToListAsync();
    var resultado = new List<object>();
    foreach (var promocion in promociones)
    {
        var escalas = await db.Escalas.AsNoTracking()
            .Where(x => x.IdRuta == promocion.IdRuta)
            .Join(db.Aeropuertos.AsNoTracking(), escala => escala.IdAeropuerto, aeropuerto => aeropuerto.IdAeropuerto,
                (escala, aeropuerto) => new { escala.Tipo, aeropuerto.Nombre })
            .ToListAsync();
        resultado.Add(new
        {
            promocion.IdPromocion,
            promocion.Precio,
            promocion.FechaInicio,
            promocion.FechaFin,
            promocion.Imagen,
            promocion.IdRuta,
            origen = escalas.FirstOrDefault(x => x.Tipo == "origen")?.Nombre ?? string.Empty,
            destino = escalas.FirstOrDefault(x => x.Tipo == "destino")?.Nombre ?? string.Empty
        });
    }

    return resultado;
}

static async Task<object?> ObtenerPaseAbordarAsync(TecAirDb db, int idCheckin)
{
    var checkIn = await db.CheckIns.AsNoTracking()
        .Include(x => x.Usuario)
        .Include(x => x.Vuelo)
        .FirstOrDefaultAsync(x => x.IdCheckin == idCheckin);
    if (checkIn?.Usuario is null || checkIn.Vuelo is null) return null;

    var vuelo = (await ObtenerVuelosAsync(db, null, null)).First(x => x.IdVuelo == checkIn.IdVuelo);
    return new
    {
        checkIn.IdCheckin,
        checkIn.Asiento,
        checkIn.Usuario.IdUsuario,
        checkIn.Usuario.Nombre1,
        checkIn.Usuario.Apellido1,
        checkIn.Vuelo.IdVuelo,
        checkIn.Vuelo.Puerta,
        checkIn.Vuelo.FechaSalida,
        checkIn.Vuelo.HoraSalida,
        vuelo.Origen,
        vuelo.Destino,
        maletas = await ResumenMaletasAsync(db, idCheckin)
    };
}

static async Task<object> ResumenMaletasAsync(TecAirDb db, int idCheckin)
{
    var maletas = await db.Maletas.AsNoTracking().Where(x => x.IdCheckin == idCheckin).OrderBy(x => x.NumMaleta).ToListAsync();
    var total = maletas.Count;
    var costoExtra = total <= 1 ? 0 : 50 + Math.Max(total - 2, 0) * 75;
    return new { total, costo_extra = costoExtra, maletas };
}

public sealed record EscalaResponse(int IdRuta, int Orden, string Tipo, int IdAeropuerto, string Nombre, string Ubicacion);

public sealed record VueloResponse(
    int IdVuelo,
    DateOnly FechaSalida,
    TimeOnly HoraSalida,
    string Puerta,
    string Estado,
    string Matricula,
    int IdRuta,
    decimal Precio,
    int Capacidad,
    string Origen,
    string Destino,
    int IdOrigen,
    int IdDestino,
    int AsientosDisponibles,
    List<EscalaResponse> Escalas);

