# Orden de proyectos TecAir

## Estructura recomendada

- `tecair-api/` → Host HTTP (solo composición de API)
  - `Program.cs`
  - `Endpoints/`
  - `Configuration/`
  - `Properties/`
  - `Docs/`
- `src/TecAir.Contracts/` → DTOs y contratos compartidos
- `src/TecAir.Domain/` → Entidades y reglas de dominio
- `src/TecAir.Application/` → Servicios, interfaces de casos de uso y configuración de aplicación
- `src/TecAir.Infrastructure/` → Repositorios, EF Core, persistencia
- `tests/TecAir.Api.Tests/` → Pruebas

## Regla práctica

- Todo lo HTTP/API vive en `tecair-api`.
- Todo lo reusable entre apps vive en `src/*`.
