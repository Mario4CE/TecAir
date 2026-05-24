namespace TecAir.Api.Config;

/*
Descripción:
Centraliza constantes globales del API para evitar duplicación de valores de configuración y rutas comunes.
Entradas:
No recibe parámetros directos.
Salidas:
Expone constantes de uso transversal para Program y definición de endpoints.
Restricciones:
No debe modificar valores de puertos o hosts externos definidos en configuración del entorno.
*/
public static class ApiGlobals
{
    /*
    Descripción:
    Ruta base de agrupación para los endpoints públicos del API.
    Entradas:
    No recibe parámetros directos.
    Salidas:
    Retorna el prefijo de rutas HTTP del API.
    Restricciones:
    Debe mantenerse compatible con clientes existentes.
    */
    public const string ApiBasePath = "/api";

    /*
    Descripción:
    Nombre de la política de CORS registrada en el contenedor de servicios.
    Entradas:
    No recibe parámetros directos.
    Salidas:
    Retorna el identificador de política CORS.
    Restricciones:
    Debe coincidir con el nombre usado en AddCors y UseCors.
    */
    public const string CorsPolicyName = "TecAirCors";

    /*
    Descripción:
    Origen por defecto para CORS cuando no se define uno en configuración externa.
    Entradas:
    No recibe parámetros directos.
    Salidas:
    Retorna el valor por defecto para orígenes permitidos.
    Restricciones:
    Solo actúa como fallback; la fuente principal es TecAirOptions.
    */
    public const string DefaultCorsOrigin = "*";

    /*
    Descripción:
    Encabezado HTTP usado para resolver el usuario de perfil en endpoints de usuario.
    Entradas:
    No recibe parámetros directos.
    Salidas:
    Retorna el nombre del header esperado.
    Restricciones:
    Debe mantenerse por compatibilidad con clientes existentes.
    */
    public const string UserIdHeaderName = "X-User-Id";
}
