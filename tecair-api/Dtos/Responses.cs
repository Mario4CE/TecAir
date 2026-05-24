namespace TecAir.Api.Dtos;

/*
Descripción:
Representa la salida detallada de una ruta para respuestas del API.
Entradas:
Recibe id de ruta y lista de escalas proyectadas.
Salidas:
Retorna un objeto serializable con formato estable para clientes.
Restricciones:
Debe mantener nombres de campos compatibles con clientes existentes.
*/
public sealed record RutaResponse(int id_ruta, List<object> escalas);


public sealed record EscalaResponse(
    int IdRuta,
    int Orden,
    string Tipo,
    int IdAeropuerto,
    string Nombre,
    string Ubicacion
);

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
    List<EscalaResponse> Escalas
);


public sealed record ReservacionResponse(
    int IdReservacion,
    string Estado,
    DateTime FechaReservacion,
    int IdUsuario,
    int IdVuelo,
    object? Usuario,
    object? Vuelo,
    object? Pago
);

public sealed record PagoResponse(
    int IdPago,
    int IdReservacion,
    decimal Monto,
    string Metodo
);


public sealed record PromocionResponse(int IdPromocion, decimal Precio, DateOnly? FechaInicio, DateOnly? FechaFin, string Imagen, int IdRuta, string origen, string destino);
public sealed record CheckInResponse(int IdCheckin, int IdUsuario, int IdVuelo, string Asiento);
public sealed record PaseAbordarResponse(int IdCheckin, string Asiento, int IdUsuario, string Nombre1, string Apellido1, int IdVuelo, string Puerta, DateOnly FechaSalida, TimeOnly HoraSalida, int TotalMaletas = 0, decimal CostoExtraMaletas = 0);
public sealed record MaletaResponse(string NumMaleta, decimal Peso, string Color, int IdCheckin);
public sealed record ResumenMaletaResponse(int total, decimal costo_extra, List<MaletaResponse> maletas);
public sealed record MaletaRegistroResponse(MaletaResponse maleta, ResumenMaletaResponse resumen);
