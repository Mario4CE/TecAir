# REVISION_REFACTORIZACION_API

## Checklist por módulo
- [x] Usuarios
- [x] Aeropuertos
- [x] Aviones
- [x] Rutas
- [x] Vuelos
- [x] Reservaciones
- [x] Pagos
- [x] Promociones
- [x] Check-ins
- [x] Maletas

## Estado de Services y Repositories
Todos los módulos funcionales principales ya cuentan con interfaces, servicios y repositorios registrados en DI.

## Estado de DTOs
- Requests centralizados en `Dtos/Requests.cs`.
- Responses centralizados en `Dtos/Responses.cs`.
- Sin DTOs definidos dentro de `ApiEndpoints.cs`.

## Estado de DI
`Program.cs` registra repositories, services y la estrategia `ICalculoCobroMaletaStrategy`.

## Estado de ApiEndpoints.cs
- Endpoints delegan en servicios por módulo.
- Sin acceso directo a `TecAirDb` en endpoints públicos.
- Se removieron helpers legacy no usados en esta iteración final.

## Patrones aplicados
- Repository Pattern
- Service Layer
- DTO Pattern
- Dependency Injection
- Strategy Pattern (maletas)

## Facade Pattern
Evaluado para check-in, no aplicado para evitar sobrecomplejidad en esta versión.

## Estado PostgreSQL/InMemory
- Runtime actual: `UseInMemoryDatabase("TecAirDb")`.
- PostgreSQL pendiente de activación real en entorno objetivo.

## Riesgos restantes
- Falta validar compilación/ejecución en este entorno por ausencia de SDK .NET.
- Falta activación operativa de PostgreSQL y validación integral con frontend real.

## Recomendaciones finales
1. Ejecutar `dotnet build` y pruebas en entorno con SDK .NET.
2. Activar PostgreSQL con Npgsql en ambiente de integración, manteniendo fallback documentado.
3. Agregar pruebas de integración por endpoint crítico.


## Estado de tests
- Carpeta creada: `tests/TecAir.Api.Tests/`.
- Cobertura inicial: estrategia de maletas y pruebas smoke de servicios (aeropuertos, aviones, reservaciones/pagos).
- Pendiente: ampliar pruebas de integración con `WebApplicationFactory` cuando SDK y dependencias estén disponibles.


## InMemory vs PostgreSQL (riesgos detectados)
- Los tests actuales usan EF Core InMemory y **no validan** traducción SQL real de PostgreSQL.
- Diferencia detectada entre modelo EF y SQL:
  - EF usa `Vuelo.FechaSalida` (`DateOnly`) + `HoraSalida` (`TimeOnly`).
  - SQL define `Vuelo.fecha_salida` como `TIMESTAMP`.
  - Se requiere una decisión de mapeo para mantener consistencia al activar PostgreSQL.
- El script SQL incluye campos como `Usuario.contrasena` y `Usuario.es_admin` que no están reflejados en el modelo API actual, por lo que se debe alinear antes de migración productiva.
- Pendiente ejecutar pruebas reales PostgreSQL para validar PK/FK, UNIQUE, NOT NULL y consultas LINQ traducidas.
