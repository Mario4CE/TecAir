# Estructura de reutilización en TecAir

Este repositorio ya fue separado en capas para compartir contratos, dominio, lógica de aplicación e infraestructura entre múltiples proyectos.

## Capas y responsabilidades

- `src/TecAir.Contracts`
  - DTOs, Requests/Responses, enums y modelos comunes para intercambio entre capas/consumidores.
- `src/TecAir.Domain`
  - Entidades y reglas de negocio puras.
- `src/TecAir.Application`
  - Interfaces de caso de uso, servicios de aplicación y configuración compartida (`TecAirOptions`).
- `src/TecAir.Infrastructure`
  - Persistencia (`TecAirDb`, `DatabaseSeeder`) y repositorios.
- `tecair-api`
  - Host HTTP/composición: `Program.cs`, `Endpoints/`, `Configuration/`, `Properties/` y `Docs/`.

## Dependencias permitidas

- `Domain` -> sin dependencias
- `Contracts` -> sin dependencias
- `Application` -> `Domain` + `Contracts`
- `Infrastructure` -> `Application` + `Domain`
- `API` -> `Application` + `Infrastructure` + `Contracts`

## Estado actual aplicado

- DTOs migrados de API a `tecair-api/src/TecAir.Contracts/Dtos`.
- Entidades migradas de API a `tecair-api/src/TecAir.Domain/Models`.
- Interfaces y servicios migrados a `src/TecAir.Application`.
- Repositorios, `TecAirDb` y `DatabaseSeeder` migrados a `src/TecAir.Infrastructure`.
- Registro de DI centralizado con:
  - `AddTecAirApplication()`
  - `AddTecAirInfrastructure()`
- `TecAirOptions` desacoplado de `TecAir.Api.Config`.

## Uso en Program.cs (API host)

```csharp
builder.Services
    .AddTecAirApplication()
    .AddTecAirInfrastructure();
```

## Pendientes recomendados

1. Crear una solución `TecAir.sln` en la raíz e incluir los proyectos (script listo: `scripts/setup_solution.sh`).
2. Configurar CI para ejecutar `restore`, `build` y `test` en cada PR. ✅ (workflow: `.github/workflows/dotnet-ci.yml`)
3. Publicar `TecAir.Contracts` (y opcionalmente `TecAir.Domain`) como paquete NuGet privado para reutilización entre repos.
