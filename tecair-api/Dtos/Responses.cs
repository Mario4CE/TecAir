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
