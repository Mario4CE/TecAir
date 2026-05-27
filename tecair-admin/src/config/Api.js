/**
 * api.js — Configuración central de la API de TECAir
 *
 * Cambia BASE_URL para apuntar al servidor real.
 * Todos los demás archivos importan desde aquí; no escribir nunca
 * la URL directamente en los componentes.
 */

// URL base del API. Cambiar esto cuando el backend esté listo.
export const BASE_URL = "http://localhost:5000/api";

/**
 * Endpoints agrupados por recurso.
 * Uso: import { ENDPOINTS } from '../config/api';
 *      fetch(ENDPOINTS.auth.login)
 */
export const ENDPOINTS = {
  auth: {
    login:    `${BASE_URL}/auth/login`,
    register: `${BASE_URL}/auth/register`,
    logout:   `${BASE_URL}/auth/logout`,
  },
  vuelos: {
    list:   `${BASE_URL}/vuelos`,
    create: `${BASE_URL}/vuelos`,
    byId:   (id) => `${BASE_URL}/vuelos/${id}`,
    abrir:  (id) => `${BASE_URL}/vuelos/${id}/abrir`,
    cerrar: (id) => `${BASE_URL}/vuelos/${id}/cerrar`,
  },
  checkins: {
    list:        `${BASE_URL}/checkins`,
    create:      `${BASE_URL}/checkins`,
    paseAbordar: (id) => `${BASE_URL}/checkins/${id}/pase-abordar`,
  },
  promociones: {
    list:   `${BASE_URL}/promociones`,
    create: `${BASE_URL}/promociones`,
    delete: (id) => `${BASE_URL}/promociones/${id}`,
  },
  usuarios: {
    list:   `${BASE_URL}/usuarios`,
    create: `${BASE_URL}/usuarios`,
    byId:   (id) => `${BASE_URL}/usuarios/${id}`,
    update: (id) => `${BASE_URL}/usuarios/${id}`,
    delete: (id) => `${BASE_URL}/usuarios/${id}`,
  },
  aeropuertos: {
    list:   `${BASE_URL}/aeropuertos`,
    create: `${BASE_URL}/aeropuertos`,
    byId:   (id) => `${BASE_URL}/aeropuertos/${id}`,
  },
  rutas: {
    list:   `${BASE_URL}/rutas`,
    create: `${BASE_URL}/rutas`,
    byId:   (id) => `${BASE_URL}/rutas/${id}`,
  },
  aviones: {
    list:   `${BASE_URL}/aviones`,
    create: `${BASE_URL}/aviones`,
    byId:   (id) => `${BASE_URL}/aviones/${id}`,
  },
  maletas: {
    list:   `${BASE_URL}/maletas`,
    create: `${BASE_URL}/maletas`,
  },
  reservaciones: {
    list:     `${BASE_URL}/reservaciones`,
    create:   `${BASE_URL}/reservaciones`,
    cancelar: (id) => `${BASE_URL}/reservaciones/${id}/cancelar`,
  },
};

/**
 * Helper genérico para llamadas fetch con JSON.
 * Lanza un Error si el servidor responde con status >= 400.
 *
 * @param {string} url        - URL completa del endpoint
 * @param {string} method     - Método HTTP (GET, POST, PUT, DELETE)
 * @param {object} [body]     - Cuerpo de la petición (opcional)
 * @param {string} [token]    - JWT del usuario autenticado (opcional)
 * @returns {Promise<any>}    - JSON de la respuesta
 */
export async function apiFetch(url, method = "GET", body = null, token = null) {
  const headers = { "Content-Type": "application/json" };
  if (token) headers["Authorization"] = `Bearer ${token}`;

  const options = { method, headers };
  if (body) options.body = JSON.stringify(body);

  const response = await fetch(url, options);
  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    // Muestra el mensaje completo del API en consola
    console.log("API Error Response:", errorData);
    throw new Error(errorData.mensaje || errorData.message || `Error ${response.status}`);
  }
  return response.json();
}