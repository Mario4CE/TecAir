/**
 * mockData.js — Datos de prueba para desarrollo
 *
 * Usado mientras la API real no está disponible.
 * Importa desde aquí las páginas durante desarrollo;
 * cuando el backend esté listo, reemplazar las llamadas
 * por apiFetch() con los endpoints de api.js.
 */

export const MOCK_USUARIO_ADMIN = {
  id_usuario: 1,
  nombre1: "Admin",
  nombre2: "",
  apellido1: "TECAir",
  apellido2: "",
  correo: "admin@tecair.com",
  es_estudiante: false,
};

export const MOCK_VUELOS = [
  {
    id_vuelo: 1,
    fecha_salida: "2026-05-26T08:00:00",
    puerta: "A3",
    estado: "abierto",
    matricula: "TEC-001",
    ruta: "SJO → MIA",
  },
  {
    id_vuelo: 2,
    fecha_salida: "2026-05-26T14:30:00",
    puerta: "B1",
    estado: "pendiente",
    matricula: "TEC-002",
    ruta: "SJO → BOG",
  },
  {
    id_vuelo: 3,
    fecha_salida: "2026-05-25T22:00:00",
    puerta: "C2",
    estado: "cerrado",
    matricula: "TEC-001",
    ruta: "MIA → SJO",
  },
];

export const MOCK_CHECKINS = [
  {
    id_checkin: 1,
    asiento: "12A",
    id_usuario: 2,
    nombre_pasajero: "María González",
    id_vuelo: 1,
    maletas: 1,
  },
  {
    id_checkin: 2,
    asiento: "7C",
    id_usuario: 3,
    nombre_pasajero: "Luis Pérez",
    id_vuelo: 1,
    maletas: 2,
  },
];

export const MOCK_PROMOCIONES = [
  {
    id_promocion: 1,
    precio: 199.99,
    fecha_inicio: "2026-06-01",
    fecha_fin: "2026-06-30",
    imagen: null,
    id_ruta: 1,
    ruta: "SJO → MIA",
  },
  {
    id_promocion: 2,
    precio: 149.5,
    fecha_inicio: "2026-05-20",
    fecha_fin: "2026-05-31",
    imagen: null,
    id_ruta: 2,
    ruta: "SJO → BOG",
  },
];

export const MOCK_STATS = {
  vuelos_hoy: 3,
  pasajeros_hoy: 87,
  checkins_pendientes: 12,
  vuelos_cerrados: 1,
};