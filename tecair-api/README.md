# TECAir API

## Descripción general

TECAir API es un backend Web API en C# para gestionar usuarios, aeropuertos, aviones, rutas, vuelos, reservaciones, pagos, promociones, check-ins y maletas. La API es consumida por las vistas web del proyecto y por clientes externos (incluida la app móvil cuando sincroniza con el backend).

## Arquitectura aplicada

Se mantiene una arquitectura por capas (no MVC tradicional con vistas renderizadas):

- Capa de entrada HTTP: endpoints en `ApiEndpoints.cs`.
- Capa de servicios: reglas de negocio en `Services/`.
- Capa de repositorios: acceso a datos en `Repositories/`.
- Capa de contratos: DTOs en `Dtos/`.
- Capa de datos: `TecAirDb`, configuración y seeding en `Data/`.
- Capa de dominio: entidades en `Models/`.

Motivo de no usar MVC tradicional: este proyecto expone una API y las vistas web/app móvil viven por separado y consumen HTTP.

## Estructura de carpetas del API

- `Data/`: contexto EF Core, opciones y seeding.
- `Dtos/`: contratos de entrada de requests.
- `Interfaces/`: contratos de servicios y repositorios.
- `Repositories/`: implementación del acceso a datos.
- `Services/`: reglas de negocio.
- `Models/`: entidades del dominio persistente.
- `ApiEndpoints.cs`: definición de endpoints públicos.
- `Program.cs`: configuración, DI, CORS y arranque.

## Patrones de diseño aplicados

- Repository Pattern: `IUsuarioRepository`/`UsuarioRepository`, `IAeropuertoRepository`/`AeropuertoRepository`, `IAvionRepository`/`AvionRepository`, `IRutaRepository`/`RutaRepository`, `IVueloRepository`/`VueloRepository`.
- Service Layer: `IUsuarioService`/`UsuarioService`, `IAeropuertoService`/`AeropuertoService`, `IAvionService`/`AvionService`, `IRutaService`/`RutaService`, `IVueloService`/`VueloService`.
- DTO Pattern: contratos de entrada en `Dtos/Requests.cs`.
- Dependency Injection: registro de servicios y repositorios en `Program.cs`.

## PostgreSQL y capa de datos

La fuente de verdad de esquema y datos iniciales está en scripts SQL del repositorio:

- `database/01_create.sql` (DDL).
- `database/02_populate.sql` (datos iniciales).

Actualmente el API está configurado para base en memoria para pruebas locales. La estructura del dominio se mantiene alineada para uso con base relacional. No se usan procedimientos almacenados, vistas de base de datos ni triggers.

## Endpoints principales

Se mantienen las rutas y métodos existentes:

- Usuarios: `GET/POST /api/usuarios`, `GET /api/usuarios/{idUsuario}`, `GET/PUT /api/usuarios/perfil`.
- Aeropuertos: `GET/POST /api/aeropuertos`.
- Aviones: `GET/POST /api/aviones`.
- Rutas: `GET/POST /api/rutas`.
- Vuelos: `GET /api/vuelos`, `GET /api/vuelos/{idVuelo}`, `POST /api/vuelos`, `PATCH /api/vuelos/{idVuelo}/abrir|cerrar`.
- Reservaciones: `GET/POST /api/reservaciones`, `GET /api/reservaciones/{idReservacion}`, `PATCH /api/reservaciones/{idReservacion}/cancelar`.
- Pagos: `GET/POST /api/pagos`.
- Promociones: `GET/POST /api/promociones`.
- Check-ins: `GET/POST /api/checkins`, `GET /api/checkins/{idCheckin}/pase-abordar`.
- Maletas: `GET/POST /api/maletas`.

## Ejecución básica

1. Restaurar dependencias del proyecto.
2. Ejecutar el proyecto `TecAir.Api.csproj`.
3. Abrir `GET /api` para validar disponibilidad.

## Conexión con vistas web y app móvil

Se detectan clientes web con base URL a `http://localhost:5000/api` en:

- `tecair-admin/src/config/Api.js`.
- `tecair-cliente/js/api.js`.

Por compatibilidad, durante esta refactorización no se cambian rutas HTTP, métodos, ni forma general de payload JSON existente.

## Cambios realizados en esta refactorización

- Se extrajo la lógica del módulo de usuarios a capas de servicio y repositorio.
- Se registró DI para `IUsuarioRepository` e `IUsuarioService`.
- Los endpoints de usuarios ahora dependen de servicios, evitando acceso directo al contexto en esas operaciones.
- Se documentaron internamente las nuevas clases y métodos con formato estándar.

## Recomendaciones pendientes

- Ejecutar compilación y pruebas automáticas en entorno con SDK .NET disponible.
- Incorporar pruebas automáticas de integración por endpoint.
- Activar y validar conexión PostgreSQL en ambiente de integración (actualmente runtime en InMemory).

## Estado de app móvil y SQLite

En esta revisión no se encontró un proyecto móvil dedicado dentro del repositorio (carpetas típicas como `mobile/`, `app/`, `android/`, `ios/` o .NET MAUI).
Sí existe un archivo local `tecair-api/Data/tecair.sqlite`, pero no evidencia integración móvil ni sincronización implementada con la base principal.
