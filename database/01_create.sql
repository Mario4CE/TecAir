-- =============================================================
-- TECAir — Script de creación de base de datos
-- PostgreSQL 18
--
-- Instrucciones:
--   1. Abrir pgAdmin y conectarse al servidor
--   2. Crear una base de datos llamada "tecair"
--   3. Abrir el Query Tool en la base "tecair"
--   4. Pegar este script y presionar F5
--   5. Correr luego el script 02_populate.sql
-- =============================================================

-- Usuario
-- es_admin = true  → funcionario del aeropuerto (accede al portal admin)
-- es_admin = false → cliente normal (accede al portal de reservaciones)
CREATE TABLE Usuario (
    id_usuario      SERIAL PRIMARY KEY,
    nombre1         VARCHAR(50)  NOT NULL,
    nombre2         VARCHAR(50)  NOT NULL DEFAULT '',
    apellido1       VARCHAR(50)  NOT NULL,
    apellido2       VARCHAR(50)  NOT NULL DEFAULT '',
    telefono        VARCHAR(20)  NOT NULL,
    correo          VARCHAR(100) NOT NULL UNIQUE,
    es_estudiante   BOOLEAN      NOT NULL DEFAULT FALSE,
    universidad     VARCHAR(100) NOT NULL DEFAULT '',
    carnet          VARCHAR(50)  NOT NULL DEFAULT '',
    millas          INT          NOT NULL DEFAULT 0,
    es_admin        BOOLEAN      NOT NULL DEFAULT FALSE
);

-- Avion
-- La matrícula funciona como clave primaria
CREATE TABLE Avion (
    matricula   VARCHAR(20) PRIMARY KEY,
    capacidad   INT         NOT NULL
);

-- Ruta
-- Una ruta queda definida por sus escalas en la tabla Escala
CREATE TABLE Ruta (
    id_ruta     SERIAL PRIMARY KEY
);

-- Aeropuerto
CREATE TABLE Aeropuerto (
    id_aeropuerto   SERIAL PRIMARY KEY,
    nombre          VARCHAR(100) NOT NULL,
    ubicacion       VARCHAR(100) NOT NULL
);

-- Escala
-- Guarda cada aeropuerto de una ruta con su orden
-- tipo puede ser: 'origen', 'escala', 'destino'
CREATE TABLE Escala (
    id_ruta         INT         NOT NULL REFERENCES Ruta(id_ruta),
    orden           INT         NOT NULL,
    id_aeropuerto   INT         NOT NULL REFERENCES Aeropuerto(id_aeropuerto),
    tipo            VARCHAR(20) NOT NULL,
    PRIMARY KEY (id_ruta, orden)
);

-- Vuelo
-- fecha_salida y hora_salida están separadas según el modelo del API
-- estado puede ser: 'programado', 'abierto', 'cerrado'
CREATE TABLE Vuelo (
    id_vuelo        SERIAL PRIMARY KEY,
    fecha_salida    DATE          NOT NULL,
    hora_salida     TIME          NOT NULL,
    puerta          VARCHAR(10)   NOT NULL,
    estado          VARCHAR(20)   NOT NULL DEFAULT 'programado',
    matricula       VARCHAR(20)   NOT NULL REFERENCES Avion(matricula),
    id_ruta         INT           NOT NULL REFERENCES Ruta(id_ruta),
    precio          DECIMAL(10,2) NOT NULL DEFAULT 0
);

-- Reservacion
-- estado puede ser: 'pendiente_pago', 'confirmada', 'cancelada'
CREATE TABLE Reservacion (
    id_reservacion      SERIAL PRIMARY KEY,
    estado              VARCHAR(20)  NOT NULL DEFAULT 'pendiente_pago',
    fecha_reservacion   TIMESTAMP    NOT NULL DEFAULT NOW(),
    id_usuario          INT          NOT NULL REFERENCES Usuario(id_usuario),
    id_vuelo            INT          NOT NULL REFERENCES Vuelo(id_vuelo)
);

-- Pago
-- Solo puede existir un pago por reservación (UNIQUE en id_reservacion)
CREATE TABLE Pago (
    id_pago         SERIAL PRIMARY KEY,
    monto           DECIMAL(10,2) NOT NULL,
    metodo          VARCHAR(50)   NOT NULL DEFAULT 'tarjeta',
    id_reservacion  INT           NOT NULL UNIQUE REFERENCES Reservacion(id_reservacion)
);

-- Checkin
-- No puede repetirse el mismo asiento en el mismo vuelo
CREATE TABLE Checkin (
    id_checkin  SERIAL PRIMARY KEY,
    asiento     VARCHAR(10) NOT NULL,
    id_usuario  INT         NOT NULL REFERENCES Usuario(id_usuario),
    id_vuelo    INT         NOT NULL REFERENCES Vuelo(id_vuelo),
    UNIQUE (asiento, id_vuelo)
);

-- Maleta
-- num_maleta es único y lo asigna el funcionario del aeropuerto
CREATE TABLE Maleta (
    num_maleta  VARCHAR(50)   PRIMARY KEY,
    peso        DECIMAL(5,2)  NOT NULL,
    color       VARCHAR(30)   NOT NULL,
    id_checkin  INT           NOT NULL REFERENCES Checkin(id_checkin)
);

-- Promocion
-- fecha_inicio y fecha_fin son opcionales
CREATE TABLE Promocion (
    id_promocion    SERIAL PRIMARY KEY,
    precio          DECIMAL(10,2) NOT NULL,
    fecha_inicio    DATE,
    fecha_fin       DATE,
    imagen          VARCHAR(255)  NOT NULL DEFAULT '',
    id_ruta         INT           NOT NULL REFERENCES Ruta(id_ruta)
);