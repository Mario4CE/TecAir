-- ─────────────────────────────────────────────────────────
-- TECAir — Script de creación de base de datos
-- PostgreSQL
-- ─────────────────────────────────────────────────────────

-- ── Usuario ──
CREATE TABLE Usuario (
    id_usuario      SERIAL PRIMARY KEY,
    nombre1         VARCHAR(50)  NOT NULL,
    nombre2         VARCHAR(50),
    apellido1       VARCHAR(50)  NOT NULL,
    apellido2       VARCHAR(50),
    telefono        VARCHAR(20)  NOT NULL,
    correo          VARCHAR(100) NOT NULL UNIQUE,
    -- La contraseña se guarda como hash, nunca en texto plano
    -- En C# usar BCrypt.Net para hashear antes de guardar
    contrasena      VARCHAR(255) NOT NULL,
    es_estudiante   BOOLEAN      NOT NULL DEFAULT FALSE,
    -- Solo aplican si es_estudiante = true
    universidad     VARCHAR(100),
    carnet          VARCHAR(50),
    -- true = funcionario del aeropuerto, false = cliente normal
    es_admin        BOOLEAN      NOT NULL DEFAULT FALSE
);

-- ── Avion ──
CREATE TABLE Avion (
    matricula   VARCHAR(20) PRIMARY KEY,
    capacidad   INT         NOT NULL
);

-- ── Ruta ──
CREATE TABLE Ruta (
    id_ruta     SERIAL PRIMARY KEY
);

-- ── Aeropuerto ──
CREATE TABLE Aeropuerto (
    id_aeropuerto   SERIAL PRIMARY KEY,
    nombre          VARCHAR(100) NOT NULL,
    ubicacion       VARCHAR(100) NOT NULL
);

-- ── Escala ──
-- Guarda cada aeropuerto de una ruta con su orden
-- tipo puede ser: 'origen', 'escala', 'destino'
CREATE TABLE Escala (
    id_ruta         INT         NOT NULL REFERENCES Ruta(id_ruta),
    orden           INT         NOT NULL,
    id_aeropuerto   INT         NOT NULL REFERENCES Aeropuerto(id_aeropuerto),
    tipo            VARCHAR(20) NOT NULL,
    PRIMARY KEY (id_ruta, orden)
);

-- ── Vuelo ──
-- id_ruta agregado para saber qué ruta sigue el vuelo
CREATE TABLE Vuelo (
    id_vuelo        SERIAL PRIMARY KEY,
    fecha_salida    TIMESTAMP   NOT NULL,
    puerta          VARCHAR(10) NOT NULL,
    -- estado puede ser: 'pendiente', 'abierto', 'cerrado'
    estado          VARCHAR(20) NOT NULL DEFAULT 'pendiente',
    matricula       VARCHAR(20) NOT NULL REFERENCES Avion(matricula),
    id_ruta         INT         NOT NULL REFERENCES Ruta(id_ruta)
);

-- ── Reservacion ──
CREATE TABLE Reservacion (
    id_reservacion      SERIAL PRIMARY KEY,
    -- estado puede ser: 'pendiente', 'confirmada', 'cancelada'
    estado              VARCHAR(20)  NOT NULL DEFAULT 'pendiente',
    fecha_reservacion   TIMESTAMP    NOT NULL DEFAULT NOW(),
    id_usuario          INT          NOT NULL REFERENCES Usuario(id_usuario),
    id_vuelo            INT          NOT NULL REFERENCES Vuelo(id_vuelo)
);

-- ── Pago ──
CREATE TABLE Pago (
    id_pago         SERIAL PRIMARY KEY,
    monto           DECIMAL(10,2)   NOT NULL,
    -- metodo puede ser: 'tarjeta', 'efectivo', etc.
    metodo          VARCHAR(50)     NOT NULL,
    id_reservacion  INT             NOT NULL REFERENCES Reservacion(id_reservacion)
);

-- ── Check-in ──
CREATE TABLE Checkin (
    id_checkin  SERIAL PRIMARY KEY,
    asiento     VARCHAR(10) NOT NULL,
    id_usuario  INT         NOT NULL REFERENCES Usuario(id_usuario),
    id_vuelo    INT         NOT NULL REFERENCES Vuelo(id_vuelo)
);

-- ── Maleta ──
CREATE TABLE Maleta (
    num_maleta  VARCHAR(50)     PRIMARY KEY,
    peso        DECIMAL(5,2)    NOT NULL,
    color       VARCHAR(30)     NOT NULL,
    id_checkin  INT             NOT NULL REFERENCES Checkin(id_checkin)
);

-- ── Promocion ──
CREATE TABLE Promocion (
    id_promocion    SERIAL PRIMARY KEY,
    precio          DECIMAL(10,2)   NOT NULL,
    fecha_inicio    DATE            NOT NULL,
    fecha_fin       DATE            NOT NULL,
    -- imagen guarda la ruta o URL de la imagen (opcional)
    imagen          VARCHAR(255),
    id_ruta         INT             NOT NULL REFERENCES Ruta(id_ruta)
);