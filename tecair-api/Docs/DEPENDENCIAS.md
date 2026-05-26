# Dependencias y ejecución

## Requisitos

- .NET 8 SDK.
- Un editor compatible con proyectos ASP.NET Core.
- No se requieren paquetes externos manuales adicionales para ejecutar el API en modo actual.

## Paquetes del proyecto

El archivo `TecAir.Api.csproj` incluye estas dependencias:

- Microsoft.EntityFrameworkCore.InMemory.
- Microsoft.EntityFrameworkCore.Sqlite.

## Instalación

Desde la carpeta `tecair-api`, restaura las dependencias con:

```bash
dotnet restore
```

## Ejecución

Para levantar el API en desarrollo:

```bash
dotnet run
```

## Notas

- La base activa actual es en memoria, así que los datos de prueba se cargan al iniciar la aplicación.
- SQLite está listo en el proyecto, pero la línea de configuración todavía está comentada en `Program.cs`.
- Si más adelante se cambia a una base persistente, habrá que completar la cadena de conexión y descomentar el proveedor correspondiente.
