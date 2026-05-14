# TECAir API

API REST mínima para el sistema TECAir. Expone recursos para usuarios, aeropuertos, aviones, rutas, vuelos, reservaciones, pagos, promociones, check-ins y maletas.

## Estado actual

El API ya tiene implementados los flujos principales de prueba y desarrollo:

- Registro y consulta de usuarios.
- Perfil de usuario con lectura y actualización.
- Registro y consulta de aeropuertos y aviones.
- Creación y consulta de rutas con escalas.
- Creación, consulta y cambio de estado de vuelos.
- Creación y consulta de reservaciones.
- Registro y consulta de pagos.
- Creación y consulta de promociones.
- Check-in y generación de pase de abordar.
- Registro y consulta de maletas.
- Semilla automática de datos de prueba al iniciar la app.

## Persistencia actual

La configuración activa usa base de datos en memoria para pruebas locales. La opción de SQLite está preparada en el proyecto, pero actualmente está comentada en `Program.cs`.

## Endpoints principales

### Salud del servicio

- `GET /api`.
- Devuelve un mensaje de estado y el catálogo de recursos disponibles.

### Usuarios

- `GET /api/usuarios`.
- `GET /api/usuarios/{idUsuario}`.
- `GET /api/usuarios/perfil`.
- `PUT /api/usuarios/perfil`.
- `POST /api/usuarios`.

Restricciones principales:

- El usuario requiere nombre y correo válidos.
- El perfil se identifica por el encabezado `X-User-Id`.
- Si el encabezado no existe, el sistema usa el usuario `1` como valor por defecto.

### Aeropuertos

- `GET /api/aeropuertos`.
- `POST /api/aeropuertos`.

Restricciones principales:

- El nombre del aeropuerto es obligatorio.

### Aviones

- `GET /api/aviones`.
- `POST /api/aviones`.

Restricciones principales:

- La matrícula es obligatoria.
- La capacidad debe ser mayor a cero.

### Rutas

- `GET /api/rutas`.
- `POST /api/rutas`.

Restricciones principales:

- Una ruta debe incluir al menos origen y destino.
- Las escalas se crean en orden y se asocian a aeropuertos existentes.

### Vuelos

- `GET /api/vuelos`.
- `GET /api/vuelos/{idVuelo}`.
- `POST /api/vuelos`.
- `PATCH /api/vuelos/{idVuelo}/abrir`.
- `PATCH /api/vuelos/{idVuelo}/cerrar`.

Restricciones principales:

- La ruta, la matrícula y la fecha de salida son obligatorias.
- El filtro por origen y destino acepta id o nombre del aeropuerto.
- El estado del vuelo solo puede cambiar si el vuelo existe.

### Reservaciones

- `GET /api/reservaciones`.
- `GET /api/reservaciones/{idReservacion}`.
- `POST /api/reservaciones`.
- `PATCH /api/reservaciones/{idReservacion}/cancelar`.

Restricciones principales:

- El usuario y el vuelo son obligatorios.
- La consulta puede filtrar por `id_usuario`, `usuario_id`, `idUsuario` o `usuarioId`.
- Al crear una reservación se agregan millas al usuario según la configuración.

### Pagos

- `GET /api/pagos`.
- `POST /api/pagos`.

Restricciones principales:

- La reservación debe existir.
- El monto debe ser mayor a cero.
- Al registrar el pago, la reservación pasa a estado `pagada`.

### Promociones

- `GET /api/promociones`.
- `POST /api/promociones`.

Restricciones principales:

- La ruta es obligatoria.
- El precio promocional debe ser mayor a cero.

### Check-ins

- `GET /api/checkins`.
- `GET /api/checkins/{idCheckin}/pase-abordar`.
- `POST /api/checkins`.

Restricciones principales:

- El usuario, el vuelo y el asiento son obligatorios.
- Un asiento no puede repetirse dentro del mismo vuelo.

### Maletas

- `GET /api/maletas`.
- `POST /api/maletas`.

Restricciones principales:

- El número de maleta es obligatorio.
- La maleta debe asociarse a un check-in existente.

## Respuestas comunes

El API devuelve objetos con una estructura simple de mensajes y recursos. Algunos ejemplos:

- `mensaje`: texto de confirmación o error.
- `usuario`, `aeropuerto`, `avion`, `ruta`, `vuelo`, `reservacion`, `pago`, `promocion`, `checkin`, `maleta`: entidad creada o consultada.
- `pase_abordar`: información del check-in.
- `resumen`: detalle de maletas.

## Próximos pasos sugeridos

- Activar persistencia con SQLite o PostgreSQL.
- Añadir autenticación real.
- Separar contratos de entrada y salida en archivos dedicados.
- Agregar pruebas automáticas de los endpoints principales.
