# Orden de proyectos TecAir

## Estructura recomendada

- `tecair-api/` → Host HTTP (solo composición de API)
  - `Program.cs`
  - `Endpoints/`
  - `Configuration/`
  - `Properties/`
  - `Docs/`
- `tecair-api/TecAir.Contracts/` → DTOs y contratos compartidos
- `tecair-api/TecAir.Domain/` → Entidades y reglas de dominio
- `tecair-api/TecAir.Application/` → Servicios, interfaces de casos de uso y configuración de aplicación
- `tecair-api/TecAir.Infrastructure/` → Repositorios, EF Core, persistencia
- `tests/TecAir.Api.Tests/` → Pruebas

## Regla práctica

- Todo lo HTTP/API vive en `tecair-api`.
- Todo lo reusable entre apps vive en `tecair-api/*`.
