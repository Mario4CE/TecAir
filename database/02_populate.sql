-- ─────────────────────────────────────────────────────────
-- TECAir — Script de datos iniciales (Initial State)
--
-- IMPORTANTE: las contraseñas aquí son hashes de BCrypt
-- La contraseña real de todos es: admin123
-- En producción cada usuario tendrá su propia contraseña
-- ─────────────────────────────────────────────────────────

-- ── Usuarios ──
-- es_admin = true  → funcionario del aeropuerto
-- es_admin = false → cliente normal
INSERT INTO Usuario (nombre1, nombre2, apellido1, apellido2, telefono, correo, contrasena, es_estudiante, universidad, carnet, es_admin)
VALUES
    -- Admin principal del sistema
    ('Admin',  '',      'TECAir',   '',         '2222-0000', 'admin@tecair.com',
     '$2a$11$hashed_password_here',
     FALSE, NULL, NULL, TRUE),

    -- Cliente estudiante
    ('María',  'José',  'González', 'Pérez',    '8888-1111', 'maria@correo.com',
     '$2a$11$hashed_password_here',
     TRUE, 'Instituto Tecnológico de Costa Rica', '2024001', FALSE),

    -- Cliente normal
    ('Luis',   '',      'Pérez',    'Mora',     '8888-2222', 'luis@correo.com',
     '$2a$11$hashed_password_here',
     FALSE, NULL, NULL, FALSE);

-- ── Aviones ──
INSERT INTO Avion (matricula, capacidad)
VALUES
    ('TEC-001', 150),
    ('TEC-002', 180);

-- ── Aeropuertos ──
INSERT INTO Aeropuerto (nombre, ubicacion)
VALUES
    ('Juan Santamaría',     'San José, Costa Rica'),
    ('Miami International', 'Miami, Estados Unidos'),
    ('El Dorado',           'Bogotá, Colombia');

-- ── Rutas ──
INSERT INTO Ruta DEFAULT VALUES; -- id_ruta = 1 (SJO → MIA)
INSERT INTO Ruta DEFAULT VALUES; -- id_ruta = 2 (SJO → BOG)

-- ── Escalas ──
-- Ruta 1: SJO → MIA
INSERT INTO Escala (id_ruta, orden, id_aeropuerto, tipo)
VALUES
    (1, 1, 1, 'origen'),
    (1, 2, 2, 'destino');

-- Ruta 2: SJO → BOG
INSERT INTO Escala (id_ruta, orden, id_aeropuerto, tipo)
VALUES
    (2, 1, 1, 'origen'),
    (2, 2, 3, 'destino');

-- ── Vuelos ──
INSERT INTO Vuelo (fecha_salida, puerta, estado, matricula, id_ruta)
VALUES
    ('2026-05-26 08:00:00', 'A3', 'abierto',   'TEC-001', 1),
    ('2026-05-26 14:30:00', 'B1', 'pendiente', 'TEC-002', 2),
    ('2026-05-25 22:00:00', 'C2', 'cerrado',   'TEC-001', 1);

-- ── Reservaciones ──
INSERT INTO Reservacion (estado, id_usuario, id_vuelo)
VALUES
    ('confirmada', 2, 1),
    ('confirmada', 3, 1);

-- ── Pagos ──
INSERT INTO Pago (monto, metodo, id_reservacion)
VALUES
    (350.00, 'tarjeta', 1),
    (350.00, 'tarjeta', 2);

-- ── Check-ins ──
INSERT INTO Checkin (asiento, id_usuario, id_vuelo)
VALUES
    ('12A', 2, 1),
    ('7C',  3, 1);

-- ── Maletas ──
INSERT INTO Maleta (num_maleta, peso, color, id_checkin)
VALUES
    ('MAL-001', 23.5, 'Negro', 1),
    ('MAL-002', 18.0, 'Azul',  2);

-- ── Promociones ──
INSERT INTO Promocion (precio, fecha_inicio, fecha_fin, imagen, id_ruta)
VALUES
    (199.99, '2026-06-01', '2026-06-30', NULL, 1),
    (149.50, '2026-05-20', '2026-05-31', NULL, 2);