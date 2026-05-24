# REVISION_REFACTORIZACION_API

## Módulos refactorizados
- Usuarios
- Aeropuertos
- Aviones
- Rutas
- Vuelos
- Reservaciones (iteración actual)
- Pagos (iteración actual)

## Módulos pendientes
- Promociones
- Check-ins
- Maletas
- Apertura/Cierre de vuelos (actualmente en endpoints de vuelos)

## Patrones aplicados
- Repository Pattern: usuarios, aeropuertos, aviones, rutas.
- Service Layer: usuarios, aeropuertos, aviones, rutas.
- DTO Pattern: requests existentes y response de rutas (`RutaResponse`).
- Dependency Injection: servicios y repositorios registrados en `Program.cs` para módulos refactorizados.

## Estado de DI
DI activo para:
- Usuario
- Aeropuerto
- Avion
- Ruta

## Estado de ApiEndpoints.cs
- Se redujo responsabilidad en usuarios, aeropuertos, aviones y rutas.
- Siguen pendientes de extraer a servicios/repositorios: vuelos, reservaciones, pagos, promociones, check-ins y maletas.

## Estado PostgreSQL/InMemory
- Runtime actual: `UseInMemoryDatabase("TecAirDb")`.
- PostgreSQL aún no activado en runtime de `Program.cs`.

## Riesgos
- La lógica de negocio de módulos pendientes sigue en `ApiEndpoints.cs`.
- No se puede validar compilación en este entorno por falta de CLI `dotnet`.

## Archivos eliminados
- Ninguno en esta iteración.

## Recomendaciones
1. Migrar módulo Vuelos a capas en la próxima iteración.
2. Implementar Strategy Pattern para cobro de maletas.
3. Evaluar Facade en check-in luego de extraer servicios base.
4. Activar PostgreSQL con configuración segura y fallback controlado.
