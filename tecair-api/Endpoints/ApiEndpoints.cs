using TecAir.Api.Config;
using TecAir.Contracts.Dtos;
using TecAir.Application.Interfaces;

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

            return vuelo is null
                ? Results.NotFound(new { mensaje = "Vuelo no encontrado." })
                : Results.Ok(new { vuelo });
        });

        api.MapPost("/vuelos", async (VueloRequest datos, IVueloService vueloService) =>
        {
            try
            {
                var vuelo = await vueloService.CrearVueloAsync(datos);

                return Results.Created(
                    $"{ApiGlobals.ApiBasePath}/vuelos/{vuelo.IdVuelo}",
                    new { mensaje = "Vuelo creado.", vuelo }
                );
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { mensaje = ex.Message });
            }
        });

        api.MapPatch("/vuelos/{idVuelo:int}/abrir", async (int idVuelo, IVueloService vueloService) =>
        {
            var vuelo = await vueloService.CambiarEstadoAsync(idVuelo, "abierto");

            return vuelo is null
                ? Results.NotFound(new { mensaje = "Vuelo no encontrado." })
                : Results.Ok(new { mensaje = "Vuelo abierto.", vuelo });
        });

        api.MapPatch("/vuelos/{idVuelo:int}/cerrar", async (int idVuelo, IVueloService vueloService) =>
        {
            var vuelo = await vueloService.CambiarEstadoAsync(idVuelo, "cerrado");

            return vuelo is null
                ? Results.NotFound(new { mensaje = "Vuelo no encontrado." })
                : Results.Ok(new { mensaje = "Vuelo cerrado.", vuelo });
        });

        api.MapGet("/reservaciones", async (HttpRequest request, IReservacionService reservacionService) =>
        {
            var filtroUsuario =
                ObtenerEnteroQuery(request, "id_usuario") ??
                ObtenerEnteroQuery(request, "usuario_id") ??
                ObtenerEnteroQuery(request, "idUsuario") ??
                ObtenerEnteroQuery(request, "usuarioId");

            return Results.Ok(new
            {
                reservaciones = await reservacionService.GetReservacionesAsync(filtroUsuario)
            });
        });

        api.MapGet("/reservaciones/{idReservacion:int}", async (int idReservacion, IReservacionService reservacionService) =>
        {
            var reservacion = await reservacionService.GetReservacionByIdAsync(idReservacion);

            return reservacion is null
                ? Results.NotFound(new { mensaje = "Reservación no encontrada." })
                : Results.Ok(new { reservacion });
        });

        api.MapPost("/reservaciones", async (ReservacionRequest datos, IReservacionService reservacionService) =>
        {
            try
            {
                var reservacion = await reservacionService.CrearReservacionAsync(datos);

                return Results.Created(
                    $"{ApiGlobals.ApiBasePath}/reservaciones/{reservacion.IdReservacion}",
                    new { id_reservacion = reservacion.IdReservacion }
                );
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { mensaje = ex.Message });
            }
        });

        api.MapPatch("/reservaciones/{idReservacion:int}/cancelar", async (int idReservacion, IReservacionService reservacionService) =>
        {
            var reservacion = await reservacionService.CancelarReservacionAsync(idReservacion);

            return reservacion is null
                ? Results.NotFound(new { mensaje = "Reservación no encontrada." })
                : Results.Ok(new { mensaje = "Reservación cancelada.", reservacion });
        });

        api.MapGet("/pagos", async (IPagoService pagoService) =>
            Results.Ok(new { pagos = await pagoService.GetPagosAsync() }));

        api.MapPost("/pagos", async (PagoRequest datos, IPagoService pagoService) =>
        {
            try
            {
                var pago = await pagoService.CrearPagoAsync(datos);

                return Results.Created(
                    $"{ApiGlobals.ApiBasePath}/pagos/{pago.IdPago}",
                    new { mensaje = "Pago registrado.", pago }
                );
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { mensaje = ex.Message });
            }
        });

        api.MapGet("/promociones", async (IPromocionService promocionService) =>
            Results.Ok(new { promociones = await promocionService.GetPromocionesAsync() }));

        api.MapPost("/promociones", async (PromocionRequest datos, IPromocionService promocionService) =>
        {
            try
            {
                var promocion = await promocionService.CrearPromocionAsync(datos);

                return Results.Created(
                    $"{ApiGlobals.ApiBasePath}/promociones/{promocion.IdPromocion}",
                    new { mensaje = "Promoción creada.", promocion }
                );
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { mensaje = ex.Message });
            }
        });

        api.MapGet("/checkins", async (ICheckInService checkInService) =>
            Results.Ok(new { checkins = await checkInService.GetCheckInsAsync() }));

        api.MapGet("/checkins/{idCheckin:int}/pase-abordar", async (int idCheckin, ICheckInService checkInService) =>
        {
            var pase = await checkInService.GetPaseAbordarAsync(idCheckin);

            return pase is null
                ? Results.NotFound(new { mensaje = "Check-in no encontrado." })
                : Results.Ok(new { pase_abordar = pase });
        });

        api.MapPost("/checkins", async (CheckInRequest datos, ICheckInService checkInService) =>
        {
            try
            {
                var checkIn = await checkInService.CrearCheckInAsync(datos);
                var pase = await checkInService.GetPaseAbordarAsync(checkIn.IdCheckin);

                return Results.Created(
                    $"{ApiGlobals.ApiBasePath}/checkins/{checkIn.IdCheckin}",
                    new { mensaje = "Check-in realizado.", pase_abordar = pase }
                );
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { mensaje = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return Results.Conflict(new { mensaje = ex.Message });
            }
        });

        api.MapGet("/maletas", async (IMaletaService maletaService) =>
            Results.Ok(new { maletas = await maletaService.GetMaletasAsync() }));

        api.MapPost("/maletas", async (MaletaRequest datos, IMaletaService maletaService) =>
        {
            try
            {
                var resultado = await maletaService.CrearMaletaAsync(datos);

                return Results.Created(
                    $"{ApiGlobals.ApiBasePath}/maletas/{resultado.maleta.NumMaleta}",
                    new { mensaje = "Maleta asignada.", resumen = resultado.resumen }
                );
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { mensaje = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { mensaje = ex.Message });
            }
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
        return int.TryParse(request.Query[nombre].FirstOrDefault(), out var valor)
            ? valor
            : null;
    }
}

