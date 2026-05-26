/**
 * api.js — Funciones para comunicación con la API de TECAir
 *
 * URL base centralizada — cambiar aquí si el servidor cambia de dirección.
 * Todos los demás archivos usan estas funciones, nunca llaman a fetch directamente.
 */

// URL base del API
const API_BASE_URL = 'http://localhost:5000/api';

/**
 * Realiza una petición GET al API
 * @param {string} endpoint - Endpoint sin la base URL
 * @returns {Promise<object>} Respuesta del API en JSON
 */
async function apiGet(endpoint) {
  try {
    const respuesta = await fetch(`${API_BASE_URL}/${endpoint}`, {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' }
    });
    if (!respuesta.ok) throw new Error(`Error ${respuesta.status}: ${respuesta.statusText}`);
    return await respuesta.json();
  } catch (error) {
    console.error('Error en apiGet:', error);
    throw error;
  }
}

/**
 * Realiza una petición POST al API
 * @param {string} endpoint
 * @param {object} datos - Cuerpo de la petición
 * @returns {Promise<object>}
 */
async function apiPost(endpoint, datos) {
  try {
    const respuesta = await fetch(`${API_BASE_URL}/${endpoint}`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(datos)
    });
    if (!respuesta.ok) {
      const error = await respuesta.json().catch(() => ({}));
      throw new Error(error.mensaje || `Error ${respuesta.status}`);
    }
    return await respuesta.json();
  } catch (error) {
    console.error('Error en apiPost:', error);
    throw error;
  }
}

/**
 * Realiza una petición PUT al API
 * @param {string} endpoint
 * @param {object} datos
 * @returns {Promise<object>}
 */
async function apiPut(endpoint, datos) {
  try {
    const respuesta = await fetch(`${API_BASE_URL}/${endpoint}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(datos)
    });
    if (!respuesta.ok) {
      const error = await respuesta.json().catch(() => ({}));
      throw new Error(error.mensaje || `Error ${respuesta.status}`);
    }
    return await respuesta.json();
  } catch (error) {
    console.error('Error en apiPut:', error);
    throw error;
  }
}

/**
 * Realiza una petición DELETE al API
 * @param {string} endpoint
 * @returns {Promise<object>}
 */
async function apiDelete(endpoint) {
  try {
    const respuesta = await fetch(`${API_BASE_URL}/${endpoint}`, {
      method: 'DELETE',
      headers: { 'Content-Type': 'application/json' }
    });
    if (!respuesta.ok) throw new Error(`Error ${respuesta.status}: ${respuesta.statusText}`);
    return await respuesta.json();
  } catch (error) {
    console.error('Error en apiDelete:', error);
    throw error;
  }
}

/**
 * Obtiene vuelos disponibles, con filtro opcional de origen y destino
 * @param {object} parametros - { origen, destino }
 * @returns {Promise<object>}
 */
async function obtenerVuelos(parametros = {}) {
  const query = new URLSearchParams(parametros).toString();
  return await apiGet(`vuelos${query ? '?' + query : ''}`);
}

/**
 * Obtiene las reservaciones de un usuario
 * @param {number} idUsuario
 * @returns {Promise<object>}
 */
async function obtenerReservaciones(idUsuario) {
  return await apiGet(`reservaciones?id_usuario=${idUsuario}`);
}

/**
 * Crea una nueva reservación
 * @param {object} datosReservacion
 * @returns {Promise<object>}
 */
async function crearReservacion(datosReservacion) {
  return await apiPost('reservaciones', datosReservacion);
}

/**
 * Cancela una reservación
 * @param {number} idReservacion
 * @returns {Promise<object>}
 */
async function cancelarReservacion(idReservacion) {
  return await apiPost(`reservaciones/${idReservacion}/cancelar`, {});
}

/**
 * Obtiene las promociones disponibles
 * @returns {Promise<object>}
 */
async function obtenerPromociones() {
  return await apiGet('promociones');
}

/**
 * Procesa un pago para una reservación
 * @param {object} datosPago - { id_reservacion, monto, metodo }
 * @returns {Promise<object>}
 */
async function procesarPagoApi(datosPago) {
  return await apiPost('pagos', datosPago);
}