/**
 * Auth.js - Funciones de autenticación para TECAir Cliente
 * NOTA: Sistema en fase de prueba - Sin autenticación real
 * Cuando la BD y API estén listas, descomentar las funciones y agregar validaciones
 */

/**
 * Guardar usuario actual en localStorage
 * @param {object} usuario - Datos del usuario
 */
function guardarUsuarioActual(usuario) {
  localStorage.setItem('usuarioActual', JSON.stringify(usuario));
}

/**
 * Obtener usuario actual desde localStorage
 * @returns {object} Datos del usuario actual o null
 */
function obtenerUsuarioActual() {
  const usuarioJson = localStorage.getItem('usuarioActual');
  return usuarioJson ? JSON.parse(usuarioJson) : null;
}

/**
 * Verificar si el usuario está autenticado
 * @returns {boolean} true si está autenticado
 */
function estaAutenticado() {
  const usuario = obtenerUsuarioActual();
  return !!usuario;
}

/**
 * Cerrar sesión del usuario
 */
function salir() {
  // Limpiar localStorage
  localStorage.removeItem('usuarioActual');
  sessionStorage.clear();

  // Redirigir a página de login
  window.location.href = 'index.html';
}

/**
 * Cambiar contraseña del usuario
 * NOTA: Cuando la API esté lista, descomentar y hacer la petición
 */
async function cambiarPassword(passwordActual, passwordNueva) {
  try {
    if (!passwordActual || !passwordNueva) {
      throw new Error('Por favor ingresa ambas contraseñas');
    }

    // TODO: Conectar con API cuando esté lista
    // const resultado = await apiPost('usuarios/cambiar-password', {
    //   passwordActual,
    //   passwordNueva
    // });

    alert('Contraseña cambiarú cuando la BD esté conectada');
    return true;
  } catch (error) {
    console.error('Error al cambiar contraseña:', error);
    throw error;
  }
}

/**
 * Iniciar sesión del usuario
 * NOTA: Función simplificada para fase de prueba
 * Cuando la API esté lista, agregar validación real
 */
async function iniciarSesion(nombre, email = null) {
  try {
    if (!nombre || nombre.trim() === '') {
      throw new Error('Por favor ingresa tu nombre');
    }

    // Por ahora, solo guardamos en localStorage
    // Cuando BD esté lista, aquí hacemos la petición a la API
    const usuario = {
      nombreCompleto: nombre,
      email: email || `${nombre.toLowerCase().replace(' ', '.')}@tecair.cr`,
      telefono: '',
      esEstudiante: false,
      millas: 0
    };

    guardarUsuarioActual(usuario);
    return usuario;
  } catch (error) {
    console.error('Error al iniciar sesión:', error);
    throw error;
  }
}

/**
 * Registrar un nuevo usuario
 * NOTA: Función simplificada para fase de prueba
 * Cuando la API esté lista, agregar validaciones y guardado en BD
 */
async function registrarUsuario(datosUsuario) {
  try {
    // Validar datos requeridos
    if (!datosUsuario.nombreCompleto || !datosUsuario.email) {
      throw new Error('Por favor completa los campos requeridos');
    }

    // Por ahora, solo guardamos en localStorage
    // Cuando BD esté lista, aquí hacemos la petición a la API
    const usuario = {
      nombreCompleto: datosUsuario.nombreCompleto,
      email: datosUsuario.email,
      telefono: datosUsuario.telefono || '',
      esEstudiante: datosUsuario.esEstudiante === 'on' || datosUsuario.esEstudiante === true,
      universidad: datosUsuario.universidad || '',
      carnet: datosUsuario.carnet || '',
      millas: 0
    };

    guardarUsuarioActual(usuario);
    return usuario;
  } catch (error) {
    console.error('Error al registrar usuario:', error);
    throw error;
  }
}

/**
 * Actualizar información del usuario
 * NOTA: Cuando la API esté lista, guardar en BD
 */
async function actualizarDatosUsuario(datosActualizados) {
  try {
    // Obtener usuario actual
    const usuarioActual = obtenerUsuarioActual();
    
    // Actualizar en memoria
    const usuarioActualizado = { ...usuarioActual, ...datosActualizados };
    guardarUsuarioActual(usuarioActualizado);

    // TODO: Cuando BD esté lista
    // const resultado = await apiPut('usuarios/perfil', datosActualizados);

    return usuarioActualizado;
  } catch (error) {
    console.error('Error al actualizar usuario:', error);
    throw error;
  }
}

/**
 * Obtener detalles completos del usuario
 * NOTA: Por ahora retorna datos locales
 * Cuando la API esté lista, hacer petición al servidor
 */
async function obtenerDetallesUsuario() {
  try {
    const usuario = obtenerUsuarioActual();
    
    // TODO: Cuando BD esté lista
    // const resultado = await apiGet('usuarios/detalles');
    // return resultado.usuario;

    return usuario;
  } catch (error) {
    console.error('Error al obtener detalles:', error);
    throw error;
  }
}
