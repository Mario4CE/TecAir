/**
 * api.js - Funciones para comunicación con la API de TECAir
 *
 * Cambia API_BASE_URL para apuntar al servidor real.
 * Todos los demás archivos usan las funciones de este archivo
 * para comunicarse con la API. No escribir la URL directamente
 * en las páginas.
 */

// =====================================
// CONFIGURACIÓN CENTRAL DE LA API
// =====================================
// Cambiar este valor cuando el backend esté en producción
const API_BASE_URL = 'http://localhost:5000/api';

// =====================================
// FUNCIONES BASE DE COMUNICACIÓN
// =====================================

/**
 * Realiza una petición GET a la API
 * @param {string} endpoint - Endpoint de la API
 * @returns {Promise} Respuesta de la API
 */
async function apiGet(endpoint) {
    try {
        const usuario = obtenerUsuarioActual ? obtenerUsuarioActual() : null;
        const headers = {
            'Content-Type': 'application/json',
        };

        // Identificar al usuario con el header X-User-Id
        if (usuario && usuario.idUsuario) {
            headers['X-User-Id'] = usuario.idUsuario;
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
        const usuario = obtenerUsuarioActual ? obtenerUsuarioActual() : null;
        const headers = {
            'Content-Type': 'application/json',
        };

        if (usuario && usuario.idUsuario) {
            headers['X-User-Id'] = usuario.idUsuario;
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
        const usuario = obtenerUsuarioActual ? obtenerUsuarioActual() : null;
        const headers = {
            'Content-Type': 'application/json',
        };

        if (usuario && usuario.idUsuario) {
            headers['X-User-Id'] = usuario.idUsuario;
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
        const usuario = obtenerUsuarioActual ? obtenerUsuarioActual() : null;
        const headers = {
            'Content-Type': 'application/json',
        };

        if (usuario && usuario.idUsuario) {
            headers['X-User-Id'] = usuario.idUsuario;
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

// =====================================
// FUNCIONES DE USUARIOS
// =====================================

/**
 * Registra un nuevo usuario en la base de datos
 * @param {object} datosUsuario - Datos del nuevo usuario
 * @returns {Promise} Usuario creado
 */
async function registrarUsuario(datosUsuario) {
    return await apiPost('usuarios', datosUsuario);
}

/**
 * Obtiene el perfil del usuario actual
 * @returns {Promise} Datos del usuario
 */
async function obtenerPerfil() {
    return await apiGet('usuarios/perfil');
}

/**
 * Actualiza el perfil del usuario actual
 * @param {object} datos - Datos a actualizar
 * @returns {Promise} Confirmación de actualización
 */
async function actualizarPerfil(datos) {
    return await apiPut('usuarios/perfil', datos);
}

// =====================================
// FUNCIONES DE AEROPUERTOS
// =====================================

/**
 * Obtiene la lista de aeropuertos disponibles
 * @returns {Promise} Lista de aeropuertos
 */
async function obtenerAeropuertos() {
    return await apiGet('aeropuertos');
}

// =====================================
// FUNCIONES DE VUELOS
// =====================================

/**
 * Obtiene la lista de vuelos disponibles con filtros opcionales
 * @param {object} parametros - Parámetros de búsqueda (origen, destino)
 * @returns {Promise} Lista de vuelos
 */
async function obtenerVuelos(parametros) {
    const query = new URLSearchParams(parametros).toString();
    return await apiGet(`vuelos?${query}`);
}

// =====================================
// FUNCIONES DE RESERVACIONES
// =====================================

/**
 * Obtiene las reservaciones del usuario actual
 * @param {number} idUsuario - ID del usuario
 * @returns {Promise} Lista de reservaciones
 */
async function obtenerReservaciones(idUsuario) {
    return await apiGet(`reservaciones?idUsuario=${idUsuario}`);
}

/**
 * Crea una nueva reservación
 * @param {object} datosReservacion - Datos de la reservación (idUsuario, idVuelo)
 * @returns {Promise} Confirmación de reservación
 */
async function crearReservacion(datosReservacion) {
    return await apiPost('reservaciones', datosReservacion);
}

/**
 * Cancela una reservación existente
 * @param {number} idReservacion - ID de la reservación a cancelar
 * @returns {Promise} Confirmación de cancelación
 */
async function cancelarReservacion(idReservacion) {
    return await apiPost(`reservaciones/${idReservacion}/cancelar`, {});
}

// =====================================
// FUNCIONES DE PAGOS
// =====================================

/**
 * Procesa el pago de una reservación
 * @param {object} datosPago - Datos del pago (idReservacion, monto, metodo)
 * @returns {Promise} Confirmación de pago
 */
async function procesarPago(datosPago) {
    return await apiPost('pagos', datosPago);
}

// =====================================
// FUNCIONES DE PROMOCIONES
// =====================================

/**
 * Obtiene las promociones disponibles
 * @returns {Promise} Lista de promociones
 */
async function obtenerPromociones() {
    return await apiGet('promociones');
}