using Microsoft.EntityFrameworkCore;
using TecAir.Api.Config;
using TecAir.Api.Data;
using TecAir.Api.Dtos;
using TecAir.Api.Interfaces;
using TecAir.Api.Models;

namespace TecAir.Api;

public static class ApiEndpoints
{
    public static WebApplication MapTecAirApiEndpoints(this WebApplication app)
    {
        var api = app.MapGroup(ApiGlobals.ApiBasePath);

        /*
        Descripción: Punto de entrada informativo del API.
        Entradas: Ninguna.
        Salidas: Mensaje de estado y catálogo de recursos disponibles.
        Restricciones: Solo expone un resumen; no modifica datos.
        */
        api.MapGet("/", () => Results.Ok(new
        {
            mensaje = "API TECAir activa",
            recursos = new[]
            {
                "usuarios",
                "aeropuertos",
                "aviones",
                "rutas",
                "vuelos",
                "reservaciones",
                "pagos",
                "promociones",
                "checkins",
                "maletas"
            }
        }));

        api.MapGet("/usuarios", async (IUsuarioService usuarioService) =>
            Results.Ok(new { usuarios = await usuarioService.GetUsuariosAsync() }));

        api.MapGet("/usuarios/perfil", async (HttpRequest request, IUsuarioService usuarioService) =>
        {
            var idUsuario = ObtenerIdUsuarioHeader(request);
            var usuario = await usuarioService.GetPerfilAsync(idUsuario);

            return usuario is null
                ? Results.NotFound(new { mensaje = "Usuario no encontrado." })
                : Results.Ok(new { usuario });
        });

        api.MapPut("/usuarios/perfil", async (HttpRequest request, UsuarioRequest datos, IUsuarioService usuarioService) =>
        {
            var idUsuario = ObtenerIdUsuarioHeader(request);
            var usuario = await usuarioService.ActualizarPerfilAsync(idUsuario, datos);

            return usuario is null
                ? Results.NotFound(new { mensaje = "Usuario no encontrado." })
                : Results.Ok(new { mensaje = "Perfil actualizado.", usuario });
        });

        api.MapGet("/usuarios/{idUsuario:int}", async (int idUsuario, IUsuarioService usuarioService) =>
        {
            var usuario = await usuarioService.GetUsuarioByIdAsync(idUsuario);

            return usuario is null
                ? Results.NotFound(new { mensaje = "Usuario no encontrado." })
                : Results.Ok(new { usuario });
        });

        api.MapPost("/usuarios", async (UsuarioRequest datos, IUsuarioService usuarioService) =>
        {
            try
            {
                var usuario = await usuarioService.CrearUsuarioAsync(datos);

                return Results.Created(
                    $"{ApiGlobals.ApiBasePath}/usuarios/{usuario.IdUsuario}",
                    new { mensaje = "Usuario creado.", usuario }
                );
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { mensaje = ex.Message });
            }
        });

        api.MapGet("/aeropuertos", async (IAeropuertoService aeropuertoService) =>
            Results.Ok(new { aeropuertos = await aeropuertoService.GetAeropuertosAsync() }));

        api.MapPost("/aeropuertos", async (AeropuertoRequest datos, IAeropuertoService aeropuertoService) =>
        {
            try
            {
                var aeropuerto = await aeropuertoService.CrearAeropuertoAsync(datos);

                return Results.Created(
                    $"{ApiGlobals.ApiBasePath}/aeropuertos/{aeropuerto.IdAeropuerto}",
                    new { mensaje = "Aeropuerto creado.", aeropuerto }
                );
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { mensaje = ex.Message });
            }
        });

        api.MapGet("/aviones", async (IAvionService avionService) =>
            Results.Ok(new { aviones = await avionService.GetAvionesAsync() }));

        api.MapPost("/aviones", async (AvionRequest datos, IAvionService avionService) =>
        {
            try
            {
                var avion = await avionService.CrearAvionAsync(datos);

                return Results.Created(
                    $"{ApiGlobals.ApiBasePath}/aviones/{avion.Matricula}",
                    new { mensaje = "Avión creado.", avion }
                );
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { mensaje = ex.Message });
            }
        });

        api.MapGet("/rutas", async (IRutaService rutaService) =>
            Results.Ok(new { rutas = await rutaService.GetRutasAsync() }));

        api.MapPost("/rutas", async (RutaRequest datos, IRutaService rutaService) =>
        {
            try
            {
                var ruta = await rutaService.CrearRutaAsync(datos);

                return Results.Created(
                    $"{ApiGlobals.ApiBasePath}/rutas/{ruta.id_ruta}",
                    new
                    {
                        mensaje = "Ruta creada.",
                        ruta
                    }
                );
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { mensaje = ex.Message });
            }
        });

        api.MapGet("/vuelos", async (string? origen, string? destino, IVueloService vueloService) =>
            Results.Ok(new { vuelos = await vueloService.GetVuelosAsync(origen, destino) }));

        api.MapGet("/vuelos/{idVuelo:int}", async (int idVuelo, IVueloService vueloService) =>
        {
            var vuelo = await vueloService.GetVueloByIdAsync(idVuelo);
            return vuelo is null ? Results.NotFound(new { mensaje = "Vuelo no encontrado." }) : Results.Ok(new { vuelo });
        });

        api.MapPost("/vuelos", async (VueloRequest datos, IVueloService vueloService) =>
        {
            try
            {
                var vuelo = await vueloService.CrearVueloAsync(datos);
                return Results.Created($"{ApiGlobals.ApiBasePath}/vuelos/{vuelo.IdVuelo}", new { mensaje = "Vuelo creado.", vuelo });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { mensaje = ex.Message });
            }
        });

        api.MapPatch("/vuelos/{idVuelo:int}/abrir", async (int idVuelo, IVueloService vueloService) =>
        {
            var vuelo = await vueloService.CambiarEstadoAsync(idVuelo, "abierto");
            return vuelo is null ? Results.NotFound(new { mensaje = "Vuelo no encontrado." }) : Results.Ok(new { mensaje = "Vuelo abierto.", vuelo });
        });

        api.MapPatch("/vuelos/{idVuelo:int}/cerrar", async (int idVuelo, IVueloService vueloService) =>
        {
            var vuelo = await vueloService.CambiarEstadoAsync(idVuelo, "cerrado");
            return vuelo is null ? Results.NotFound(new { mensaje = "Vuelo no encontrado." }) : Results.Ok(new { mensaje = "Vuelo cerrado.", vuelo });
        });

        api.MapGet("/reservaciones", async (HttpRequest request, TecAirDb db) =>
        {
            var filtroUsuario = ObtenerEnteroQuery(request, "id_usuario")
                ?? ObtenerEnteroQuery(request, "usuario_id")
                ?? ObtenerEnteroQuery(request, "idUsuario")
                ?? ObtenerEnteroQuery(request, "usuarioId");

            var query = db.Reservaciones
                .AsNoTracking()
                .OrderByDescending(x => x.FechaReservacion)
                .AsQueryable();

            if (filtroUsuario.HasValue)
                query = query.Where(x => x.IdUsuario == filtroUsuario.Value);

            return Results.Ok(new { reservaciones = await query.ToListAsync() });
        });

        api.MapGet("/reservaciones/{idReservacion:int}", async (int idReservacion, TecAirDb db) =>
        {
            var reservacion = await ObtenerReservacionAsync(db, idReservacion);

            return reservacion is null
                ? Results.NotFound(new { mensaje = "Reservación no encontrada." })
                : Results.Ok(new { reservacion });
        });

        api.MapPost("/reservaciones", async (ReservacionRequest datos, TecAirDb db, IConfiguration configuration) =>
        {
            var idUsuario = datos.IdUsuario ?? datos.UsuarioId;
            var idVuelo = datos.IdVuelo ?? datos.VueloId;

            if (idUsuario is null)
                return Results.BadRequest(new { mensaje = "El usuario es obligatorio." });

            if (idVuelo is null)
                return Results.BadRequest(new { mensaje = "El vuelo es obligatorio." });

            var usuario = await db.Usuarios.FirstOrDefaultAsync(x => x.IdUsuario == idUsuario.Value);

            if (usuario is null)
                return Results.NotFound(new { mensaje = "Usuario no encontrado." });

            if (!await db.Vuelos.AnyAsync(x => x.IdVuelo == idVuelo.Value))
                return Results.NotFound(new { mensaje = "Vuelo no encontrado." });

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

            return Results.Created(
                $"{ApiGlobals.ApiBasePath}/reservaciones/{reservacion.IdReservacion}",
                new
                {
                    mensaje = "Reservación creada.",
                    reservacion = await ObtenerReservacionAsync(db, reservacion.IdReservacion)
                }
            );
        });

        api.MapPatch("/reservaciones/{idReservacion:int}/cancelar", async (int idReservacion, TecAirDb db) =>
        {
            var reservacion = await db.Reservaciones.FirstOrDefaultAsync(x => x.IdReservacion == idReservacion);

            if (reservacion is null)
                return Results.NotFound(new { mensaje = "Reservación no encontrada." });

            reservacion.Estado = "cancelada";

            await db.SaveChangesAsync();

            return Results.Ok(new
            {
                mensaje = "Reservación cancelada.",
                reservacion = await ObtenerReservacionAsync(db, idReservacion)
            });
        });

        api.MapGet("/pagos", async (TecAirDb db) =>
            Results.Ok(new
            {
                pagos = await db.Pagos
                    .AsNoTracking()
                    .OrderByDescending(x => x.IdPago)
                    .ToListAsync()
            }));

        api.MapPost("/pagos", async (PagoRequest datos, TecAirDb db) =>
        {
            var idReservacion = datos.IdReservacion ?? datos.ReservacionId;

            if (idReservacion is null)
                return Results.BadRequest(new { mensaje = "La reservación es obligatoria." });

            if (datos.Monto <= 0)
                return Results.BadRequest(new { mensaje = "El monto debe ser mayor a cero." });

            var reservacion = await db.Reservaciones.FirstOrDefaultAsync(x => x.IdReservacion == idReservacion.Value);

            if (reservacion is null)
                return Results.NotFound(new { mensaje = "Reservación no encontrada." });

            var pago = new Pago
            {
                IdReservacion = idReservacion.Value,
                Monto = datos.Monto,
                Metodo = datos.Metodo ?? "tarjeta"
            };

            reservacion.Estado = "pagada";

            db.Pagos.Add(pago);
            await db.SaveChangesAsync();

            return Results.Created(
                $"{ApiGlobals.ApiBasePath}/pagos/{pago.IdPago}",
                new
                {
                    mensaje = "Pago registrado.",
                    pago
                }
            );
        });

        api.MapGet("/promociones", async (TecAirDb db) =>
            Results.Ok(new { promociones = await ObtenerPromocionesAsync(db) }));

        api.MapPost("/promociones", async (PromocionRequest datos, TecAirDb db) =>
        {
            if (datos.IdRuta <= 0)
                return Results.BadRequest(new { mensaje = "La ruta es obligatoria." });

            if (datos.Precio <= 0)
                return Results.BadRequest(new { mensaje = "El precio promocional debe ser mayor a cero." });

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

            return Results.Created(
                $"{ApiGlobals.ApiBasePath}/promociones/{promocion.IdPromocion}",
                new
                {
                    mensaje = "Promoción creada.",
                    promocion
                }
            );
        });

        api.MapGet("/checkins", async (TecAirDb db) =>
            Results.Ok(new
            {
                checkins = await db.CheckIns
                    .AsNoTracking()
                    .OrderByDescending(x => x.IdCheckin)
                    .ToListAsync()
            }));

        api.MapGet("/checkins/{idCheckin:int}/pase-abordar", async (int idCheckin, TecAirDb db) =>
        {
            var pase = await ObtenerPaseAbordarAsync(db, idCheckin);

            return pase is null
                ? Results.NotFound(new { mensaje = "Check-in no encontrado." })
                : Results.Ok(new { pase_abordar = pase });
        });

        api.MapPost("/checkins", async (CheckInRequest datos, TecAirDb db) =>
        {
            var idUsuario = datos.IdUsuario ?? datos.UsuarioId;
            var idVuelo = datos.IdVuelo ?? datos.VueloId;

            if (idUsuario is null)
                return Results.BadRequest(new { mensaje = "El usuario es obligatorio." });

            if (idVuelo is null)
                return Results.BadRequest(new { mensaje = "El vuelo es obligatorio." });

            if (string.IsNullOrWhiteSpace(datos.Asiento))
                return Results.BadRequest(new { mensaje = "El asiento es obligatorio." });

            var asientoOcupado = await db.CheckIns.AnyAsync(x => x.IdVuelo == idVuelo.Value && x.Asiento == datos.Asiento);

            if (asientoOcupado)
                return Results.Conflict(new { mensaje = "El asiento ya está ocupado para este vuelo." });

            var checkIn = new CheckIn
            {
                IdUsuario = idUsuario.Value,
                IdVuelo = idVuelo.Value,
                Asiento = datos.Asiento
            };

            db.CheckIns.Add(checkIn);
            await db.SaveChangesAsync();

            return Results.Created(
                $"{ApiGlobals.ApiBasePath}/checkins/{checkIn.IdCheckin}",
                new
                {
                    mensaje = "Check-in realizado.",
                    pase_abordar = await ObtenerPaseAbordarAsync(db, checkIn.IdCheckin)
                }
            );
        });

        api.MapGet("/maletas", async (TecAirDb db) =>
            Results.Ok(new
            {
                maletas = await db.Maletas
                    .AsNoTracking()
                    .OrderBy(x => x.IdCheckin)
                    .ThenBy(x => x.NumMaleta)
                    .ToListAsync()
            }));

        api.MapPost("/maletas", async (MaletaRequest datos, TecAirDb db) =>
        {
            if (string.IsNullOrWhiteSpace(datos.NumMaleta))
                return Results.BadRequest(new { mensaje = "El número de maleta es obligatorio." });

            if (!await db.CheckIns.AnyAsync(x => x.IdCheckin == datos.IdCheckin))
                return Results.NotFound(new { mensaje = "Check-in no encontrado." });

            var maleta = new Maleta
            {
                NumMaleta = datos.NumMaleta,
                Peso = datos.Peso,
                Color = datos.Color,
                IdCheckin = datos.IdCheckin
            };

            db.Maletas.Add(maleta);
            await db.SaveChangesAsync();

            return Results.Created(
                $"{ApiGlobals.ApiBasePath}/maletas/{maleta.NumMaleta}",
                new
                {
                    mensaje = "Maleta asignada.",
                    resumen = await ResumenMaletasAsync(db, datos.IdCheckin)
                }
            );
        });

        return app;
    }

    /*
    Descripción: Lee el identificador de usuario desde el encabezado de la petición.
    Entradas: Request HTTP con encabezado definido en ApiGlobals.UserIdHeaderName.
    Salidas: Id numérico del usuario.
    Restricciones: Si el encabezado no existe o es inválido, se usa ApiGlobals.DefaultUserId.
    */
    private static int ObtenerIdUsuarioHeader(HttpRequest request)
    {
        return int.TryParse(request.Headers[ApiGlobals.UserIdHeaderName].FirstOrDefault(), out var idUsuario)
            ? idUsuario
            : ApiGlobals.DefaultUserId;
    }

    /*
    Descripción: Convierte un parámetro de consulta en entero nullable.
    Entradas: Request HTTP y nombre del parámetro.
    Salidas: Valor entero si existe y es válido.
    Restricciones: Devuelve null cuando el parámetro no está presente o no es numérico.
    */
    private static int? ObtenerEnteroQuery(HttpRequest request, string nombre)
    {
        return int.TryParse(request.Query[nombre].FirstOrDefault(), out var valor) ? valor : null;
    }


    /*
    Descripción: Construye el listado de vuelos para procesos internos dependientes.
    Entradas: Contexto de base de datos y filtros opcionales de origen/destino.
    Salidas: Lista de vuelos en DTO de respuesta.
    Restricciones: No presenta restricciones adicionales.
    */
    private static async Task<List<VueloResponse>> ObtenerVuelosAsync(TecAirDb db, string? origen, string? destino)
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
                .Join(db.Aeropuertos.AsNoTracking(),
                    escala => escala.IdAeropuerto,
                    aeropuerto => aeropuerto.IdAeropuerto,
                    (escala, aeropuerto) => new EscalaResponse(
                        escala.IdRuta,
                        escala.Orden,
                        escala.Tipo,
                        aeropuerto.IdAeropuerto,
                        aeropuerto.Nombre,
                        aeropuerto.Ubicacion))
                .ToListAsync();

            var escalaOrigen = escalas.FirstOrDefault(x => x.Tipo == "origen");
            var escalaDestino = escalas.FirstOrDefault(x => x.Tipo == "destino");

            if (!CoincideAeropuerto(escalaOrigen, origen) || !CoincideAeropuerto(escalaDestino, destino))
                continue;

            var reservacionesActivas = vuelo.Reservaciones.Count(x => x.Estado != "cancelada");

            respuestas.Add(new VueloResponse(
                vuelo.IdVuelo, vuelo.FechaSalida, vuelo.HoraSalida, vuelo.Puerta, vuelo.Estado,
                vuelo.Matricula, vuelo.IdRuta, vuelo.Precio, vuelo.Avion?.Capacidad ?? 0,
                escalaOrigen?.Nombre ?? string.Empty, escalaDestino?.Nombre ?? string.Empty,
                escalaOrigen?.IdAeropuerto ?? 0, escalaDestino?.IdAeropuerto ?? 0,
                (vuelo.Avion?.Capacidad ?? 0) - reservacionesActivas, escalas));
        }

        return respuestas;
    }

    private static bool CoincideAeropuerto(EscalaResponse? escala, string? filtro)
    {
        if (string.IsNullOrWhiteSpace(filtro)) return true;
        if (escala is null) return false;
        return escala.IdAeropuerto.ToString() == filtro || escala.Nombre.Contains(filtro, StringComparison.OrdinalIgnoreCase);
    }

    /*
    Descripción: Obtiene una reservación con usuario, vuelo y pago asociado.
    Entradas: Contexto de base de datos e id de reservación.
    Salidas: Objeto serializable con el detalle completo de la reservación.
    Restricciones: Devuelve null si la reservación no existe.
    */
    private static async Task<object?> ObtenerReservacionAsync(TecAirDb db, int idReservacion)
    {
        var reservacion = await db.Reservaciones.AsNoTracking()
            .Include(x => x.Usuario)
            .Include(x => x.Pago)
            .FirstOrDefaultAsync(x => x.IdReservacion == idReservacion);

        if (reservacion is null)
            return null;

        var vuelo = (await ObtenerVuelosAsync(db, null, null))
            .FirstOrDefault(x => x.IdVuelo == reservacion.IdVuelo);

        return new
        {
            reservacion.IdReservacion,
            reservacion.Estado,
            reservacion.FechaReservacion,
            reservacion.IdUsuario,
            reservacion.IdVuelo,
            reservacion.Usuario,
            vuelo,
            reservacion.Pago
        };
    }

    /*
    Descripción: Construye el listado de promociones con origen y destino visibles.
    Entradas: Contexto de base de datos.
    Salidas: Lista serializable de promociones enriquecidas.
    Restricciones: Solo incluye promociones persistidas en base.
    */
    private static async Task<List<object>> ObtenerPromocionesAsync(TecAirDb db)
    {
        var promociones = await db.Promociones
            .AsNoTracking()
            .OrderByDescending(x => x.FechaInicio)
            .ToListAsync();

        var resultado = new List<object>();

        foreach (var promocion in promociones)
        {
            var escalas = await db.Escalas.AsNoTracking()
                .Where(x => x.IdRuta == promocion.IdRuta)
                .Join(
                    db.Aeropuertos.AsNoTracking(),
                    escala => escala.IdAeropuerto,
                    aeropuerto => aeropuerto.IdAeropuerto,
                    (escala, aeropuerto) => new
                    {
                        escala.Tipo,
                        aeropuerto.Nombre
                    })
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

    /*
    Descripción: Genera el pase de abordar con los datos del check-in.
    Entradas: Contexto de base de datos e id de check-in.
    Salidas: Objeto serializable con datos del pasajero, vuelo y maletas.
    Restricciones: Devuelve null si faltan el check-in, el usuario o el vuelo.
    */
    private static async Task<object?> ObtenerPaseAbordarAsync(TecAirDb db, int idCheckin)
    {
        var checkIn = await db.CheckIns.AsNoTracking()
            .Include(x => x.Usuario)
            .Include(x => x.Vuelo)
            .FirstOrDefaultAsync(x => x.IdCheckin == idCheckin);

        if (checkIn?.Usuario is null || checkIn.Vuelo is null)
            return null;

        var vuelo = (await ObtenerVuelosAsync(db, null, null))
            .First(x => x.IdVuelo == checkIn.IdVuelo);

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

    /*
    Descripción: Resume la cantidad de maletas y el costo extra asociado.
    Entradas: Contexto de base de datos e id de check-in.
    Salidas: Objeto con total, costo_extra y lista de maletas.
    Restricciones: Aplica la regla de costo extra definida por cantidad de maletas.
    */
    private static async Task<object> ResumenMaletasAsync(TecAirDb db, int idCheckin)
    {
        var maletas = await db.Maletas.AsNoTracking()
            .Where(x => x.IdCheckin == idCheckin)
            .OrderBy(x => x.NumMaleta)
            .ToListAsync();

        var total = maletas.Count;
        var costoExtra = total <= 1 ? 0 : 50 + Math.Max(total - 2, 0) * 75;

        return new
        {
            total,
            costo_extra = costoExtra,
            maletas
        };
    }
}

