// ─────────────────────────────────────────────────────────
// LoginPage.jsx — Pantalla de inicio de sesión
// Solo para funcionarios del aeropuerto (admins)
//
// Para crear nuevos admins, se hace desde el dashboard
// en la sección de Gestión de Usuarios
// ─────────────────────────────────────────────────────────

import { useState } from "react";
import { ENDPOINTS, apiFetch } from "../config/api";
import { MOCK_USUARIO_ADMIN } from "../config/mockData";

// Modo mock: true mientras el API no esté lista
// Cuando el backend esté listo, cambiar a false
const USE_MOCK = false;

// Color principal — igual que en Dashboard.jsx
const COLOR_PRINCIPAL = "#6d4fc2";

export default function LoginPage({ onLoginSuccess }) {
  return (
    <div
      className="min-vh-100 d-flex align-items-center justify-content-center"
      style={{ background: "#f5f3ff" }} // fondo morado muy clarito
    >
      <div style={{ width: "100%", maxWidth: 420 }}>

        {/* Encabezado */}
        <div className="text-center mb-4">
          <h1 className="fw-bold mb-0" style={{ color: "#3c3489", fontSize: 26 }}>
            TECAir
          </h1>
          <p className="text-muted small mb-0">Portal de Administración</p>
        </div>

        {/* Tarjeta del formulario */}
        <div
          className="card border-0 shadow-sm rounded-4"
          style={{ borderTop: `4px solid ${COLOR_PRINCIPAL}` }}
        >
          <div className="card-body p-4">
            <h5 className="fw-bold mb-1" style={{ color: "#3c3489" }}>
              Inicio de sesión
            </h5>
            <p className="text-muted small mb-4">
              Acceso exclusivo para funcionarios
            </p>

            {/* Formulario de login */}
            <FormLogin onLoginSuccess={onLoginSuccess} />
          </div>
        </div>

        <p className="text-center text-muted small mt-3">
          © 2026 TECAir — Instituto Tecnológico de Costa Rica
        </p>
      </div>
    </div>
  );
}

// FormLogin — formulario con correo y contraseña
//
// useState guarda lo que el usuario escribe en cada campo
// handleChange actualiza el estado cada vez que se escribe algo
// handleSubmit se ejecuta cuando el usuario presiona "Ingresar"
function FormLogin({ onLoginSuccess }) {
  // Estado del formulario — guarda los valores de los inputs
  const [form, setForm] = useState({ correo: "", contrasena: "" });

  // Estado para mostrar mensajes de error
  const [error, setError] = useState("");

  // Estado para mostrar el spinner mientras carga
  const [cargando, setCargando] = useState(false);

  // Se ejecuta cada vez que el usuario escribe en un input
  // Actualiza solo el campo que cambió, deja los demás igual
  const handleChange = (e) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  // Se ejecuta cuando el usuario presiona el botón Ingresar
  const handleSubmit = async (e) => {
    // Evita que el formulario recargue la página (comportamiento default del HTML)
    e.preventDefault();
    setError("");
    setCargando(true);

    try {
      if (USE_MOCK) {
        // Modo desarrollo: simula la llamada al API 
        // setTimeout simula el tiempo de respuesta del servidor
        await new Promise((r) => setTimeout(r, 600));

        if (
          form.correo === "admin@tecair.com" &&
          form.contrasena === "admin123"
        ) {
          // Login exitoso: le pasamos el usuario y token a App.jsx
          onLoginSuccess(MOCK_USUARIO_ADMIN, "mock-token-123");
        } else {
          setError("Correo o contraseña incorrectos.");
        }
      } else {
        //  Producción: llamada real al API 
        const data = await apiFetch(ENDPOINTS.usuarios.list);
        const usuarios = data.usuarios ?? [];
        const usuario = usuarios.find(u => u.correo === form.correo);

        if (!usuario) {
          setError("Usuario no encontrado.");
          return;
        }
        if (!usuario.es_admin) {
          setError("No tienes permisos para acceder al portal de administración.");
          return;
        }
        onLoginSuccess(usuario, "token-" + usuario.id_usuario);
      }
    } catch (err) {
      // Si el API devuelve un error, lo mostramos al usuario
      setError(err.message || "Error al iniciar sesión.");
    } finally {
      // Siempre quitamos el spinner al terminar, haya error o no
      setCargando(false);
    }
  };

  return (
    <form onSubmit={handleSubmit}>

      {/* Mensaje de error — solo aparece si hay un error */}
      {error && (
        <div className="alert alert-danger py-2 small rounded-3">
          {error}
        </div>
      )}

      {/* Campo de correo */}
      <div className="mb-3">
        <label className="form-label fw-semibold small text-secondary">
          Correo electrónico
        </label>
        <input
          type="email"
          name="correo"
          className="form-control rounded-3"
          placeholder="funcionario@tecair.com"
          value={form.correo}
          onChange={handleChange}
          required
          autoComplete="email"
        />
      </div>

      {/* Campo de contraseña */}
      <div className="mb-4">
        <label className="form-label fw-semibold small text-secondary">
          Contraseña
        </label>
        <input
          type="password"
          name="contrasena"
          className="form-control rounded-3"
          placeholder="••••••••"
          value={form.contrasena}
          onChange={handleChange}
          required
          autoComplete="current-password"
        />
      </div>

      {/* Botón de envío */}
      <button
        type="submit"
        className="btn w-100 rounded-3 fw-semibold text-white"
        style={{ background: COLOR_PRINCIPAL }}
        disabled={cargando}
      >
        {/* Muestra spinner mientras carga, texto normal si no */}
        {cargando ? (
          <>
            <span className="spinner-border spinner-border-sm me-2"></span>
            Verificando…
          </>
        ) : (
          "Ingresar"
        )}
      </button>

      {/* Aviso de modo desarrollo — desaparece cuando USE_MOCK = false */}
      {USE_MOCK && (
        <p className="text-center text-muted small mt-3 mb-0">
          Modo desarrollo · <code>admin@tecair.com</code> / <code>admin123</code>
        </p>
      )}
    </form>
  );
}