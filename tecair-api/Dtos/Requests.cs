namespace TecAir.Api.Dtos;

public sealed record UsuarioRequest(
    string? NombreCompleto,
    string? Nombre1,
    string? Nombre2,
    string? Apellido1,
    string? Apellido2,
    string? Telefono,
    string? Correo,
    string? Email,
    bool? EsEstudiante,
    string? Universidad,
    string? Carnet,
    int? Millas);

public sealed record AeropuertoRequest(string Nombre, string? Ubicacion);

public sealed record AvionRequest(string Matricula, int Capacidad);

public sealed record EscalaRequest(int IdAeropuerto, int? Orden, string? Tipo);

public sealed record RutaRequest(List<EscalaRequest> Escalas);

public sealed record VueloRequest(
    int IdRuta,
    string Matricula,
    DateOnly? FechaSalida,
    TimeOnly? HoraSalida,
    string? Puerta,
    string? Estado,
    decimal? Precio);

public sealed record ReservacionRequest(int? IdUsuario, int? UsuarioId, int? IdVuelo, int? VueloId, string? Estado);

public sealed record PagoRequest(int? IdReservacion, int? ReservacionId, decimal Monto, string? Metodo);

public sealed record PromocionRequest(
    int IdRuta,
    decimal Precio,
    DateOnly? FechaInicio,
    DateOnly? FechaFin,
    string? Imagen);

public sealed record CheckInRequest(int? IdUsuario, int? UsuarioId, int? IdVuelo, int? VueloId, string Asiento);

public sealed record MaletaRequest(string NumMaleta, decimal Peso, string Color, int IdCheckin);
