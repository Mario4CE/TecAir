# TECAir API en C#

API REST en **C# / ASP.NET Core** para conectar el cascarón web de TECAir con una base de datos SQLite basada en el diagrama entidad-relación del proyecto.

## Requisitos

- .NET SDK 8 o superior.
- No hace falta instalar una base de datos aparte: SQLite se crea automáticamente en `tecair-api/data/tecair.sqlite`.

## Configuración global

Cambie IP, puerto, CORS, ruta de base de datos y valores globales en:

```text
tecair-api/appsettings.json
```

Valores principales:

```json
{
  "TecAir": {
    "CorsOrigin": "*",
    "DefaultGate": "A1",
    "DefaultFlightPrice": 120,
    "LoyaltyMilesPerReservation": 100
  },
  "ConnectionStrings": {
    "TecAirDb": "Data Source=data/tecair.sqlite"
  },
  "Urls": "http://0.0.0.0:3000"
}
```

También puede cambiar puerto/IP con variables de ambiente de ASP.NET Core:

```bash
ASPNETCORE_URLS=http://0.0.0.0:3000 dotnet run
```

En el cliente web, cambie la URL de la API en:

```text
tecair-cliente/js/config.js
```

## Ejecutar

```bash
cd tecair-api
dotnet restore
dotnet run
```

La API queda disponible por defecto en:

```text
http://localhost:3000/api
```

## Endpoints principales

### Usuarios

- `GET /api/usuarios`
- `POST /api/usuarios`
- `GET /api/usuarios/:id`
- `GET /api/usuarios/perfil` usando header `X-User-Id`
- `PUT /api/usuarios/perfil` usando header `X-User-Id`

Ejemplo:

```bash
curl -X POST http://localhost:3000/api/usuarios \
  -H "Content-Type: application/json" \
  -d '{"nombre_completo":"María López","correo":"maria@correo.com","telefono":"8888-1111","es_estudiante":true,"universidad":"TEC","carnet":"20261234"}'
```

### Aeropuertos, aviones y rutas

- `GET /api/aeropuertos`
- `POST /api/aeropuertos`
- `GET /api/aviones`
- `POST /api/aviones`
- `GET /api/rutas`
- `POST /api/rutas`

Crear ruta:

```bash
curl -X POST http://localhost:3000/api/rutas \
  -H "Content-Type: application/json" \
  -d '{"escalas":[{"id_aeropuerto":1,"tipo":"origen"},{"id_aeropuerto":2,"tipo":"destino"}]}'
```

### Vuelos

- `GET /api/vuelos`
- `GET /api/vuelos?origen=SJO&destino=LIR`
- `GET /api/vuelos/:id`
- `POST /api/vuelos`
- `PATCH /api/vuelos/:id/abrir`
- `PATCH /api/vuelos/:id/cerrar`

Crear vuelo:

```bash
curl -X POST http://localhost:3000/api/vuelos \
  -H "Content-Type: application/json" \
  -d '{"id_ruta":1,"matricula":"TI-TEC1","fecha_salida":"2026-06-15","hora_salida":"09:30:00","puerta":"A5","precio":115}'
```

### Reservaciones y pagos

- `GET /api/reservaciones`
- `GET /api/reservaciones?id_usuario=1`
- `GET /api/reservaciones/:id`
- `POST /api/reservaciones`
- `PATCH /api/reservaciones/:id/cancelar`
- `GET /api/pagos`
- `POST /api/pagos`

```bash
curl -X POST http://localhost:3000/api/reservaciones \
  -H "Content-Type: application/json" \
  -d '{"id_usuario":1,"id_vuelo":1}'

curl -X POST http://localhost:3000/api/pagos \
  -H "Content-Type: application/json" \
  -d '{"id_reservacion":1,"monto":105,"metodo":"tarjeta"}'
```

### Promociones

- `GET /api/promociones`
- `POST /api/promociones`

```bash
curl -X POST http://localhost:3000/api/promociones \
  -H "Content-Type: application/json" \
  -d '{"id_ruta":1,"precio":89,"fecha_inicio":"2026-05-01","fecha_fin":"2026-06-30","imagen":"promo.jpg"}'
```

### Check-in, pase de abordar y maletas

- `GET /api/checkins`
- `POST /api/checkins`
- `GET /api/checkins/:id/pase-abordar`
- `GET /api/maletas`
- `POST /api/maletas`

Regla de cobro de maletas implementada:

- Primera maleta: $0
- Segunda maleta: $50
- Tercera y siguientes: $75 cada una

```bash
curl -X POST http://localhost:3000/api/checkins \
  -H "Content-Type: application/json" \
  -d '{"id_usuario":1,"id_vuelo":1,"asiento":"12A"}'

curl -X POST http://localhost:3000/api/maletas \
  -H "Content-Type: application/json" \
  -d '{"num_maleta":"MAL-001","peso":22.5,"color":"azul","id_checkin":1}'
```
