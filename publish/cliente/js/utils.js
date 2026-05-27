/**
 * Utils.js - Funciones utilitarias para TECAir Cliente
 */

/**
 * Proteger una ruta verificando que el usuario esté autenticado
 * NOTA: En fase de prueba, permite acceso sin autenticación
 * Cuando la BD esté lista, descomentar la verificación
 */
function protegerRuta() {
  // Sistema en fase de prueba - permitir acceso sin autenticación real
  // TODO: Cuando BD esté lista, descomentar esta verificación:
  // if (!estaAutenticado()) {
  //   window.location.href = 'index.html';
  // }
}

/**
 * Formatear una fecha a formato legible
 * @param {string} fecha - Fecha en formato ISO
 * @returns {string} Fecha formateada
 */
function formatearFecha(fecha) {
  if (!fecha) return '';
  
  const opciones = { 
    year: 'numeric', 
    month: 'long', 
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  };
  
  return new Date(fecha).toLocaleDateString('es-CR', opciones);
}

/**
 * Formatear moneda a formato $
 * @param {number} monto - Monto a formatear
 * @returns {string} Monto formateado
 */
function formatearMoneda(monto) {
  return new Intl.NumberFormat('es-CR', {
    style: 'currency',
    currency: 'USD'
  }).format(monto);
}

/**
 * Validar formato de email
 * @param {string} email - Email a validar
 * @returns {boolean} true si es válido
 */
function validarEmail(email) {
  const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return regex.test(email);
}

/**
 * Validar número de teléfono
 * @param {string} telefono - Teléfono a validar
 * @returns {boolean} true si es válido
 */
function validarTelefono(telefono) {
  const regex = /^\+?[\d\s\-()]{7,}$/;
  return regex.test(telefono);
}

/**
 * Validar contraseña (mínimo 6 caracteres)
 * @param {string} password - Contraseña a validar
 * @returns {boolean} true si es válida
 */
function validarPassword(password) {
  return password && password.length >= 6;
}

/**
 * Limpiar espacios en blanco en objeto
 * @param {object} objeto - Objeto a limpiar
 * @returns {object} Objeto limpio
 */
function limpiarEspacios(objeto) {
  const objLimpio = {};
  for (const [clave, valor] of Object.entries(objeto)) {
    objLimpio[clave] = typeof valor === 'string' ? valor.trim() : valor;
  }
  return objLimpio;
}

/**
 * Guardar datos en sessionStorage
 * @param {string} clave - Clave para almacenar
 * @param {any} valor - Valor a almacenar
 */
function guardarSessionStorage(clave, valor) {
  sessionStorage.setItem(clave, JSON.stringify(valor));
}

/**
 * Obtener datos de sessionStorage
 * @param {string} clave - Clave a obtener
 * @returns {any} Valor almacenado o null
 */
function obtenerSessionStorage(clave) {
  const valor = sessionStorage.getItem(clave);
  return valor ? JSON.parse(valor) : null;
}

/**
 * Limpiar sessionStorage
 * @param {string} clave - Clave a limpiar (opcional)
 */
function limpiarSessionStorage(clave = null) {
  if (clave) {
    sessionStorage.removeItem(clave);
  } else {
    sessionStorage.clear();
  }
}

/**
 * Controlar elemento loading
 * @param {string} elementoId - ID del elemento
 * @param {boolean} mostrar - true para mostrar, false para ocultar
 */
function controlarLoading(elementoId, mostrar) {
  const elemento = document.getElementById(elementoId);
  if (elemento) {
    if (mostrar) {
      elemento.style.display = 'flex';
    } else {
      elemento.style.display = 'none';
    }
  }
}

/**
 * Mostrar alerta simple
 * @param {string} mensaje - Mensaje a mostrar
 * @param {string} tipo - Tipo de alerta: 'success', 'error', 'info'
 */
function mostrarAlertaSimple(mensaje, tipo = 'info') {
  alert(mensaje);
}

/**
 * Copiar texto al portapapeles
 * @param {string} texto - Texto a copiar
 * @returns {Promise} Promesa que se resuelve cuando se copia
 */
async function copiarAlPortapapeles(texto) {
  try {
    await navigator.clipboard.writeText(texto);
    return true;
  } catch (err) {
    console.error('Error al copiar:', err);
    return false;
  }
}

/**
 * Descargar archivo
 * @param {string} contenido - Contenido del archivo
 * @param {string} nombreArchivo - Nombre del archivo
 * @param {string} tipo - Tipo MIME del archivo
 */
function descargarArchivo(contenido, nombreArchivo, tipo = 'text/plain') {
  const blob = new Blob([contenido], { type: tipo });
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = url;
  link.download = nombreArchivo;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  window.URL.revokeObjectURL(url);
}

/**
 * Generar ID único
 * @returns {string} ID único
 */
function generarIdUnico() {
  return `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
}

/**
 * Esperar un tiempo determinado (en ms)
 * @param {number} ms - Milisegundos a esperar
 * @returns {Promise} Promesa que se resuelve después del tiempo
 */
function esperar(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

/**
 * Obtener parámetro de URL
 * @param {string} parametro - Nombre del parámetro
 * @returns {string} Valor del parámetro o null
 */
function obtenerParametroURL(parametro) {
  const params = new URLSearchParams(window.location.search);
  return params.get(parametro);
}

/**
 * Redirigir a otra página
 * @param {string} url - URL de destino
 * @param {number} retraso - Retraso en ms antes de redirigir
 */
function redirigir(url, retraso = 0) {
  if (retraso > 0) {
    setTimeout(() => {
      window.location.href = url;
    }, retraso);
  } else {
    window.location.href = url;
  }
}

/**
 * Obfuscar email para mostrar en pantalla
 * @param {string} email - Email a obfuscar
 * @returns {string} Email obfuscado
 */
function obfuscarEmail(email) {
  const [usuario, dominio] = email.split('@');
  const usuarioOfuscado = usuario.substring(0, 2) + '*'.repeat(usuario.length - 2);
  return `${usuarioOfuscado}@${dominio}`;
}

/**
 * Obtener contraste de color (blanco o negro)
 * @param {string} color - Color en formato hex
 * @returns {string} 'white' o 'black'
 */
function obtenerContrasteColor(color) {
  const r = parseInt(color.substr(1, 2), 16);
  const g = parseInt(color.substr(3, 2), 16);
  const b = parseInt(color.substr(5, 2), 16);
  const luminancia = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
  return luminancia > 0.5 ? 'black' : 'white';
}

/**
 * Validar si es dispositivo móvil
 * @returns {boolean} true si es móvil
 */
function esMobile() {
  return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
}

/**
 * Scrollear a un elemento
 * @param {string} elementoId - ID del elemento
 */
function scrollearA(elementoId) {
  const elemento = document.getElementById(elementoId);
  if (elemento) {
    elemento.scrollIntoView({ behavior: 'smooth' });
  }
}

/**
 * Cambiar tema (oscuro/claro)
 * @param {string} tema - 'light' o 'dark'
 */
function cambiarTema(tema) {
  localStorage.setItem('tema', tema);
  
  if (tema === 'dark') {
    document.documentElement.style.setProperty('--bg-color', '#1a1a1a');
    document.documentElement.style.setProperty('--text-color', '#ffffff');
  } else {
    document.documentElement.style.setProperty('--bg-color', '#ffffff');
    document.documentElement.style.setProperty('--text-color', '#000000');
  }
}

/**
 * Obtener tema actual
 * @returns {string} Tema actual
 */
function obtenerTemaActual() {
  return localStorage.getItem('tema') || 'light';
}

/**
 * Validar si es conexión segura (HTTPS)
 * @returns {boolean} true si es HTTPS
 */
function esConexionSegura() {
  return window.location.protocol === 'https:' || window.location.hostname === 'localhost';
}

/**
 * Logger con timestamp
 * @param {string} mensaje - Mensaje a loguear
 * @param {string} tipo - Tipo: 'log', 'warn', 'error'
 */
function log(mensaje, tipo = 'log') {
  const timestamp = new Date().toLocaleTimeString('es-CR');
  const logMessage = `[${timestamp}] ${mensaje}`;
  
  if (tipo === 'error') {
    console.error(logMessage);
  } else if (tipo === 'warn') {
    console.warn(logMessage);
  } else {
    console.log(logMessage);
  }
}
