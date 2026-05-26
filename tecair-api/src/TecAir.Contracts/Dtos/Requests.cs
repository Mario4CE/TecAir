namespace TecAir.Contracts.Dtos;

/*
Descripción: Solicitud para crear o actualizar un usuario.
Entradas: NombreCompleto, Nombre1, Nombre2, Apellido1, Apellido2, Telefono, Correo, Email, EsEstudiante, Universidad, Carnet y Millas.
Salidas: Datos normalizados para persistencia en la entidad Usuario.
Restricciones: Debe enviarse al menos un nombre y un correo válido.
*/
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

/*
Descripción: Solicitud para registrar un aeropuerto.
Entradas: Nombre y Ubicacion.
Salidas: Datos listos para crear la entidad Aeropuerto.
Restricciones: El nombre es obligatorio.
*/
public sealed record AeropuertoRequest(string Nombre, string? Ubicacion);

/*
Descripción: Solicitud para registrar un avión.
Entradas: Matricula y Capacidad.
Salidas: Datos listos para crear la entidad Avion.
Restricciones: La matrícula es obligatoria y la capacidad debe ser mayor a cero.
*/
public sealed record AvionRequest(string Matricula, int Capacidad);

/*
Descripción: Solicitud de una escala dentro de una ruta.
Entradas: IdAeropuerto, Orden y Tipo.
Salidas: Datos de la escala para crear la relación ruta-aeropuerto.
Restricciones: Debe apuntar a un aeropuerto existente.
*/
public sealed record EscalaRequest(int IdAeropuerto, int? Orden, string? Tipo);

/*
Descripción: Solicitud para crear una ruta con una o varias escalas.
Entradas: Lista de escalas.
Salidas: Secuencia de escalas asociadas a la nueva ruta.
Restricciones: Debe incluir al menos origen y destino.
*/
public sealed record RutaRequest(List<EscalaRequest> Escalas);

/*
Descripción: Solicitud para registrar un vuelo.
Entradas: IdRuta, Matricula, FechaSalida, HoraSalida, Puerta, Estado y Precio.
Salidas: Datos listos para crear la entidad Vuelo.
Restricciones: Ruta, matrícula y fecha de salida son obligatorias.
*/
public sealed record VueloRequest(
    int IdRuta,
    string Matricula,
    DateOnly? FechaSalida,
    TimeOnly? HoraSalida,
    string? Puerta,
    string? Estado,
    decimal? Precio);

/*
Descripción: Solicitud para crear una reservación.
Entradas: IdUsuario o UsuarioId, IdVuelo o VueloId y Estado.
Salidas: Datos para asociar un usuario con un vuelo.
Restricciones: El usuario y el vuelo son obligatorios.
*/
public sealed record ReservacionRequest(int? IdUsuario, int? UsuarioId, int? IdVuelo, int? VueloId, string? Estado);

/*
Descripción: Solicitud para registrar un pago.
Entradas: IdReservacion o ReservacionId, Monto y Metodo.
Salidas: Datos para crear la entidad Pago y marcar la reservación como pagada.
Restricciones: La reservación debe existir y el monto debe ser mayor a cero.
*/
public sealed record PagoRequest(int? IdReservacion, int? ReservacionId, decimal Monto, string? Metodo);

/*
Descripción: Solicitud para crear una promoción.
Entradas: IdRuta, Precio, FechaInicio, FechaFin e Imagen.
Salidas: Datos para crear la entidad Promocion.
Restricciones: La ruta es obligatoria y el precio promocional debe ser mayor a cero.
*/
public sealed record PromocionRequest(
    int IdRuta,
    decimal Precio,
    DateOnly? FechaInicio,
    DateOnly? FechaFin,
    string? Imagen);

/*
Descripción: Solicitud para hacer check-in de un pasajero.
Entradas: IdUsuario o UsuarioId, IdVuelo o VueloId y Asiento.
Salidas: Datos para registrar el pase de abordaje.
Restricciones: El asiento es obligatorio y no puede repetirse en el mismo vuelo.
*/
public sealed record CheckInRequest(int? IdUsuario, int? UsuarioId, int? IdVuelo, int? VueloId, string Asiento);

/*
Descripción: Solicitud para registrar una maleta asociada a un check-in.
Entradas: NumMaleta, Peso, Color e IdCheckin.
Salidas: Datos para crear la entidad Maleta.
Restricciones: El número de maleta es obligatorio y el check-in debe existir.
*/
public sealed record MaletaRequest(string NumMaleta, decimal Peso, string Color, int IdCheckin);
