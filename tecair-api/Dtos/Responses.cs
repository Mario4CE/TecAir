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
