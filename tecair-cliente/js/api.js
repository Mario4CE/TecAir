/**
 * API.js - Funciones para comunicación con la API
 */

// URL base de la API (cambiar según necesidad)
const API_BASE_URL = 'http://localhost:3000/api';

/**
 * Realiza una petición GET a la API
 * @param {string} endpoint - Endpoint de la API
 * @returns {Promise} Respuesta de la API
 */
async function apiGet(endpoint) {
  try {
    const token = localStorage.getItem('authToken');
    const headers = {
      'Content-Type': 'application/json',
    };

    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    const respuesta = await fetch(`${API_BASE_URL}/${endpoint}`, {
      method: 'GET',
      headers: headers
    });

    if (!respuesta.ok) {
      throw new Error(`Error ${respuesta.status}: ${respuesta.statusText}`);
    }

    return await respuesta.json();
  } catch (error) {
    console.error('Error en apiGet:', error);
    throw error;
  }
}

/**
 * Realiza una petición POST a la API
 * @param {string} endpoint - Endpoint de la API
 * @param {object} datos - Datos a enviar
 * @returns {Promise} Respuesta de la API
 */
async function apiPost(endpoint, datos) {
  try {
    const token = localStorage.getItem('authToken');
    const headers = {
      'Content-Type': 'application/json',
    };

    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    const respuesta = await fetch(`${API_BASE_URL}/${endpoint}`, {
      method: 'POST',
      headers: headers,
      body: JSON.stringify(datos)
    });

    if (!respuesta.ok) {
      const error = await respuesta.json();
      throw new Error(error.mensaje || `Error ${respuesta.status}`);
    }

    return await respuesta.json();
  } catch (error) {
    console.error('Error en apiPost:', error);
    throw error;
  }
}

/**
 * Realiza una petición PUT a la API
 * @param {string} endpoint - Endpoint de la API
 * @param {object} datos - Datos a actualizar
 * @returns {Promise} Respuesta de la API
 */
async function apiPut(endpoint, datos) {
  try {
    const token = localStorage.getItem('authToken');
    const headers = {
      'Content-Type': 'application/json',
    };

    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    const respuesta = await fetch(`${API_BASE_URL}/${endpoint}`, {
      method: 'PUT',
      headers: headers,
      body: JSON.stringify(datos)
    });

    if (!respuesta.ok) {
      const error = await respuesta.json();
      throw new Error(error.mensaje || `Error ${respuesta.status}`);
    }

    return await respuesta.json();
  } catch (error) {
    console.error('Error en apiPut:', error);
    throw error;
  }
}

/**
 * Realiza una petición DELETE a la API
 * @param {string} endpoint - Endpoint de la API
 * @returns {Promise} Respuesta de la API
 */
async function apiDelete(endpoint) {
  try {
    const token = localStorage.getItem('authToken');
    const headers = {
      'Content-Type': 'application/json',
    };

    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    const respuesta = await fetch(`${API_BASE_URL}/${endpoint}`, {
      method: 'DELETE',
      headers: headers
    });

    if (!respuesta.ok) {
      throw new Error(`Error ${respuesta.status}: ${respuesta.statusText}`);
    }

    return await respuesta.json();
  } catch (error) {
    console.error('Error en apiDelete:', error);
    throw error;
  }
}

/**
 * Obtiene la lista de vuelos disponibles
 * @param {object} parametros - Parámetros de búsqueda
 * @returns {Promise} Lista de vuelos
 */
async function obtenerVuelos(parametros) {
  const query = new URLSearchParams(parametros).toString();
  return await apiGet(`vuelos?${query}`);
}

/**
 * Obtiene las reservaciones del usuario
 * @returns {Promise} Lista de reservaciones
 */
async function obtenerReservaciones() {
  return await apiGet('reservaciones');
}

/**
 * Crea una nueva reservación
 * @param {object} datosReservacion - Datos de la reservación
 * @returns {Promise} Confirmación de reservación
 */
async function crearReservacion(datosReservacion) {
  return await apiPost('reservaciones', datosReservacion);
}

/**
 * Obtiene las promociones disponibles
 * @returns {Promise} Lista de promociones
 */
async function obtenerPromociones() {
  return await apiGet('promociones');
}

/**
 * Obtiene el perfil del usuario actual
 * @returns {Promise} Datos del usuario
 */
async function obtenerPerfil() {
  return await apiGet('usuarios/perfil');
}

/**
 * Actualiza el perfil del usuario
 * @param {object} datos - Datos a actualizar
 * @returns {Promise} Confirmación de actualización
 */
async function actualizarPerfil(datos) {
  return await apiPut('usuarios/perfil', datos);
}

/**
 * Procesa un pago
 * @param {object} datosPago - Datos de pago
 * @returns {Promise} Confirmación de pago
 */
async function procesarPago(datosPago) {
  return await apiPost('pagos', datosPago);
}
