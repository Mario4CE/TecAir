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

        /*
        Descripción: Endpoint de autenticación de usuarios.
        Entradas: LoginRequest con Correo y Contraseña.
        Salidas: LoginResponse con datos del usuario.
        Restricciones: El usuario debe existir en la BD.
        */
        api.MapPost("/auth/login", async (LoginRequest datos, IUsuarioService usuarioService) =>
        {
            try
            {
                // Normalizar entrada (soportar 'correo' o 'email')
                var correo = datos.Correo ?? datos.Email;

                if (string.IsNullOrWhiteSpace(correo))
                {
                    return Results.BadRequest(new { mensaje = "El correo es requerido." });
                }

                // Buscar usuario por correo
                var usuarios = await usuarioService.GetUsuariosAsync();
                var usuario = usuarios.FirstOrDefault(u => 
                    u.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase));

                if (usuario == null)
                {
                    return Results.NotFound(new { mensaje = "Usuario no encontrado." });
                }

                // Retornar datos del usuario (sin contraseña)
                var respuesta = new LoginResponse(
                    Token: null, // Token será null por ahora (implementar JWT después)
                    IdUsuario: usuario.IdUsuario,
                    Nombre1: usuario.Nombre1,
                    Nombre2: usuario.Nombre2,
                    Apellido1: usuario.Apellido1,
                    Apellido2: usuario.Apellido2,
                    Correo: usuario.Correo,
                    Telefono: usuario.Telefono,
                    EsEstudiante: usuario.EsEstudiante,
                    Universidad: usuario.Universidad,
                    Carnet: usuario.Carnet,
                    Millas: usuario.Millas,
                    EsAdmin: usuario.EsAdmin
                );

                return Results.Ok(respuesta);
            }
            catch (Exception ex)
            {
                return Results.InternalServerError();
            }
        });

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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al crear usuario: {ex.GetType().Name} - {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                return Results.BadRequest(new { mensaje = $"Error al crear usuario: {ex.Message}" });
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

        api.MapGet("/vuelos", async (HttpRequest request, IVueloService vueloService) =>
        {
            var origen =
                ObtenerTextoQuery(request, "origen") ??
                ObtenerTextoQuery(request, "origin") ??
                ObtenerTextoQuery(request, "origenCodigo") ??
                ObtenerTextoQuery(request, "codigo_origen");

            var destino =
                ObtenerTextoQuery(request, "destino") ??
                ObtenerTextoQuery(request, "destination") ??
                ObtenerTextoQuery(request, "destinoCodigo") ??
                ObtenerTextoQuery(request, "codigo_destino");

            return Results.Ok(new { vuelos = await vueloService.GetVuelosAsync(origen, destino) });
        });

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
        var idUsuarioHeader = ObtenerHeaderTexto(
            request,
            ApiGlobals.UserIdHeaderName,
            "X-Usuario-Id",
            "X-Id-Usuario");

        return int.TryParse(idUsuarioHeader, out var idUsuario)
            ? idUsuario
            : ApiGlobals.DefaultUserId;
    }

    /*
    Descripción: Obtiene un texto de header HTTP considerando aliases.
    Entradas: Request HTTP y encabezados candidatos ordenados por prioridad.
    Salidas: Texto limpiado o null si no existe.
    Restricciones: No transforma mayúsculas/minúsculas del valor recibido.
    */
    private static string? ObtenerHeaderTexto(HttpRequest request, params string[] nombres)
    {
        foreach (var nombre in nombres)
        {
            var valor = request.Headers[nombre].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(valor))
            {
                return valor.Trim();
            }
        }

        return null;
    }

    /*
    Descripción: Obtiene un texto de query string y normaliza vacíos a null.
    Entradas: Request HTTP y nombre del parámetro.
    Salidas: Texto limpiado o null si no existe.
    Restricciones: No transforma mayúsculas/minúsculas del valor recibido.
    */
    private static string? ObtenerTextoQuery(HttpRequest request, string nombre)
    {
        var valor = request.Query[nombre].FirstOrDefault();
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
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
