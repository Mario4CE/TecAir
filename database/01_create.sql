-- =============================================================
-- TECAir — Script de datos iniciales (Initial State)
-- PostgreSQL 18
--
-- Instrucciones:
--   1. Asegurarse de haber corrido primero el 01_create.sql
--   2. Detener el API antes de correr este script para evitar
--      que el seeder inserte datos duplicados
--   3. Abrir el Query Tool en la base "tecair"
--   4. Pegar este script y presionar F5
--   5. Arrancar el API nuevamente con: dotnet run
-- =============================================================

-- Limpia todas las tablas en orden correcto (respetando foreign keys)
TRUNCATE TABLE Maleta      CASCADE;
TRUNCATE TABLE Checkin     CASCADE;
TRUNCATE TABLE Pago        CASCADE;
TRUNCATE TABLE Reservacion CASCADE;
TRUNCATE TABLE Promocion   CASCADE;
TRUNCATE TABLE Escala      CASCADE;
TRUNCATE TABLE Vuelo       CASCADE;
TRUNCATE TABLE Ruta        CASCADE;
TRUNCATE TABLE Aeropuerto  CASCADE;
TRUNCATE TABLE Avion       CASCADE;
TRUNCATE TABLE Usuario     CASCADE;

-- Reinicia los contadores de IDs para que empiecen desde 1
ALTER SEQUENCE usuario_id_usuario_seq     RESTART WITH 1;
ALTER SEQUENCE ruta_id_ruta_seq           RESTART WITH 1;
ALTER SEQUENCE aeropuerto_id_aeropuerto_seq RESTART WITH 1;
ALTER SEQUENCE vuelo_id_vuelo_seq         RESTART WITH 1;
ALTER SEQUENCE reservacion_id_reservacion_seq RESTART WITH 1;
ALTER SEQUENCE pago_id_pago_seq           RESTART WITH 1;
ALTER SEQUENCE checkin_id_checkin_seq     RESTART WITH 1;
ALTER SEQUENCE promocion_id_promocion_seq RESTART WITH 1;

-- Usuarios de prueba
-- es_estudiante = true  → cliente universitario con programa de millas
-- es_estudiante = false → cliente normal
INSERT INTO Usuario (nombre1, nombre2, apellido1, apellido2, telefono, correo, es_estudiante, universidad, carnet, millas)
VALUES
    ('Admin',  '',     'TECAir',   '',      '2222-0000', 'admin@tecair.com',  FALSE, '', '', 0),
    ('María',  'José', 'González', 'Pérez', '8888-1111', 'maria@correo.com',  TRUE,  'Instituto Tecnológico de Costa Rica', '2024001', 100),
    ('Luis',   '',     'Pérez',    'Mora',  '8888-2222', 'luis@correo.com',   FALSE, '', '', 0);

-- Aviones de prueba
INSERT INTO Avion (matricula, capacidad)
VALUES
    ('TI-TEC1', 120),
    ('TI-TEC2', 180);

-- Aeropuertos de prueba
INSERT INTO Aeropuerto (nombre, ubicacion)
VALUES
    ('SJO - Juan Santamaría', 'Alajuela, Costa Rica'),
    ('LIR - Guanacaste',      'Liberia, Costa Rica'),
    ('XQP - Quepos',          'Puntarenas, Costa Rica'),
    ('GLF - Golfito',         'Puntarenas, Costa Rica');

-- Rutas de prueba
-- Ruta 1: SJO → LIR (vuelo directo)
-- Ruta 2: SJO → XQP → GLF (con escala en Quepos)
INSERT INTO Ruta DEFAULT VALUES;
INSERT INTO Ruta DEFAULT VALUES;

-- Escalas de las rutas
INSERT INTO Escala (id_ruta, orden, id_aeropuerto, tipo)
VALUES
    (1, 1, 1, 'origen'),
    (1, 2, 2, 'destino'),
    (2, 1, 1, 'origen'),
    (2, 2, 3, 'escala'),
    (2, 3, 4, 'destino');

-- Vuelos de prueba
INSERT INTO Vuelo (fecha_salida, hora_salida, puerta, estado, matricula, id_ruta, precio)
VALUES
    ('2026-06-01', '08:00:00', 'A3', 'programado', 'TI-TEC1', 1, 105.00),
    ('2026-06-02', '14:30:00', 'B2', 'programado', 'TI-TEC2', 2, 160.00);

-- Reservaciones de prueba
INSERT INTO Reservacion (estado, id_usuario, id_vuelo)
VALUES
    ('pendiente_pago', 2, 1),
    ('pendiente_pago', 3, 1);

-- Pagos de prueba
INSERT INTO Pago (monto, metodo, id_reservacion)
VALUES
    (105.00, 'tarjeta', 1),
    (105.00, 'tarjeta', 2);

-- Check-ins de prueba
INSERT INTO Checkin (asiento, id_usuario, id_vuelo)
VALUES
    ('12A', 2, 1),
    ('7C',  3, 1);

-- Maletas de prueba
INSERT INTO Maleta (num_maleta, peso, color, id_checkin)
VALUES
    ('MAL-001', 23.5, 'Negro', 1),
    ('MAL-002', 18.0, 'Azul',  2);

-- Promociones de prueba
INSERT INTO Promocion (precio, fecha_inicio, fecha_fin, imagen, id_ruta)
VALUES
    (89.99,  '2026-06-01', '2026-06-30', '', 1),
    (129.50, '2026-05-20', '2026-05-31', '', 2);