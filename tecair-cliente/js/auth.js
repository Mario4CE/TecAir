/**
 * auth.js — Funciones de autenticación para TECAir Cliente
 *
 * Maneja login, registro y sesión del usuario.
 * Conectado al API real en http://localhost:5000/api
 */

/**
 * Guarda el usuario actual en localStorage
 * @param {object} usuario - Datos del usuario
 */
function guardarUsuarioActual(usuario) {
  localStorage.setItem('usuarioActual', JSON.stringify(usuario));
}

/**
 * Obtiene el usuario actual desde localStorage
 * @returns {object|null} Datos del usuario o null
 */
function obtenerUsuarioActual() {
  const usuarioJson = localStorage.getItem('usuarioActual');
  return usuarioJson ? JSON.parse(usuarioJson) : null;
}

/**
 * Verifica si el usuario está autenticado
 * @returns {boolean}
 */
function estaAutenticado() {
  return !!obtenerUsuarioActual();
}

/**
 * Cierra la sesión del usuario
 */
function salir() {
  localStorage.removeItem('usuarioActual');
  sessionStorage.clear();
  window.location.href = 'index.html';
}

/**
 * Inicia sesión buscando el usuario por correo en el API
 * @param {string} correo
 * @returns {Promise<object>} Usuario autenticado
 */
async function iniciarSesion(correo) {
  try {
    // Trae todos los usuarios y busca por correo
    const data = await apiGet('usuarios');
    const usuarios = data.usuarios ?? [];
    const usuario = usuarios.find(u => u.correo === correo);

    if (!usuario) {
      throw new Error('Usuario no encontrado. Verifica tu correo.');
    }

    if (usuario.es_admin) {
      throw new Error('Este portal es solo para clientes. Use el portal de administración.');
    }

    // Guarda el usuario en localStorage
    guardarUsuarioActual(usuario);
    return usuario;
  } catch (error) {
    console.error('Error al iniciar sesión:', error);
    throw error;
  }
}

/**
 * Registra un nuevo usuario en el API
 * @param {object} datosUsuario
 * @returns {Promise<object>} Usuario creado
 */
async function registrarUsuario(datosUsuario) {
  try {
    if (!datosUsuario.correo || !datosUsuario.nombre1) {
      throw new Error('Por favor completa los campos requeridos.');
    }

    // Llama al endpoint POST /usuarios
    const resultado = await apiPost('usuarios', {
      nombre1:       datosUsuario.nombre1,
      nombre2:       datosUsuario.nombre2 || '',
      apellido1:     datosUsuario.apellido1,
      apellido2:     datosUsuario.apellido2 || '',
      telefono:      datosUsuario.telefono,
      correo:        datosUsuario.correo,
      es_estudiante: datosUsuario.esEstudiante === 'on' || datosUsuario.esEstudiante === true,
      universidad:   datosUsuario.universidad || '',
      carnet:        datosUsuario.carnet || '',
      millas:        0,
    });

    const usuario = resultado.usuario ?? resultado;
    guardarUsuarioActual(usuario);
    return usuario;
  } catch (error) {
    console.error('Error al registrar usuario:', error);
    throw error;
  }
}