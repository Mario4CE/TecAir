// ─────────────────────────────────────────────────────────
// SeccionVuelos.jsx — Gestión de vuelos del aeropuerto
//
// Funcionalidad:
//   - Ver lista de vuelos
//   - Crear un vuelo nuevo
//   - Abrir un vuelo (pendiente → abierto)
//   - Cerrar un vuelo (abierto → cerrado)
//     solo si falta menos de 1 hora para la salida
// ─────────────────────────────────────────────────────────

import { useState } from "react";
import { MOCK_VUELOS } from "../../config/mockData";

const COLOR_PRINCIPAL = "#6d4fc2";

// Configuración visual de cada estado posible
const ESTADO_CONFIG = {
  abierto:   { color: "#198754", bg: "#d1e7dd", label: "Abierto"   },
  pendiente: { color: "#856404", bg: "#fff3cd", label: "Pendiente" },
  cerrado:   { color: "#495057", bg: "#e2e3e5", label: "Cerrado"   },
};

// Datos de prueba para rutas y aviones
// Reemplazar por llamadas al API cuando esté lista
const MOCK_RUTAS = [
  { id_ruta: 1, descripcion: "SJO → MIA" },
  { id_ruta: 2, descripcion: "SJO → BOG" },
];
const MOCK_AVIONES = [
  { matricula: "TEC-001", capacidad: 150 },
  { matricula: "TEC-002", capacidad: 180 },
];

export default function SeccionVuelos() {
  // Lista de vuelos — inicia con los datos mockeados
  const [vuelos, setVuelos] = useState(MOCK_VUELOS);

  // Controla si el modal de nuevo vuelo está abierto
  const [modalAbierto, setModalAbierto] = useState(false);

  // Mensaje de éxito o error para mostrar al usuario
  const [mensaje, setMensaje] = useState(null);

  // Muestra un mensaje temporal por 3 segundos
  const mostrarMensaje = (texto, tipo = "success") => {
    setMensaje({ texto, tipo });
    setTimeout(() => setMensaje(null), 3000);
  };

  // ── Abrir vuelo ──
  // Cambia el estado de pendiente a abierto
  const handleAbrir = (id_vuelo) => {
    setVuelos((prev) =>
      prev.map((v) =>
        v.id_vuelo === id_vuelo ? { ...v, estado: "abierto" } : v
      )
    );
    mostrarMensaje("Vuelo abierto correctamente.");

    // Cuando el API esté lista, reemplazar el setVuelos por:
    // await apiFetch(ENDPOINTS.vuelos.abrir(id_vuelo), "PUT");
    // luego recargar la lista
  };

  // ── Cerrar vuelo ──
  // Solo permite cerrar si falta menos de 1 hora para la salida
  const handleCerrar = (vuelo) => {
    const ahora = new Date();
    const salida = new Date(vuelo.fecha_salida);
    const diferenciaMs = salida - ahora;
    const diferenciaHoras = diferenciaMs / (1000 * 60 * 60);

    if (diferenciaHoras > 1) {
      mostrarMensaje(
        "Solo se puede cerrar el vuelo una hora antes de la salida.",
        "danger"
      );
      return;
    }

    setVuelos((prev) =>
      prev.map((v) =>
        v.id_vuelo === vuelo.id_vuelo ? { ...v, estado: "cerrado" } : v
      )
    );
    mostrarMensaje("Vuelo cerrado correctamente.");
  };

  // ── Agregar vuelo nuevo ──
  // Recibe los datos del formulario y los agrega a la lista
  const handleNuevoVuelo = (datosVuelo) => {
    const nuevoVuelo = {
      // Genera un ID temporal — el API lo reemplazará con el real
      id_vuelo: vuelos.length + 1,
      ...datosVuelo,
      estado: "pendiente",
    };
    setVuelos((prev) => [...prev, nuevoVuelo]);
    setModalAbierto(false);
    mostrarMensaje("Vuelo registrado correctamente.");
  };

  return (
    <div>
      {/* ── Encabezado ── */}
      <div className="d-flex align-items-center justify-content-between mb-4">
        <h2 className="fw-bold mb-0" style={{ color: "#3c3489" }}>
          Gestión de vuelos
        </h2>
        <button
          className="btn fw-semibold text-white rounded-3"
          style={{ background: COLOR_PRINCIPAL }}
          onClick={() => setModalAbierto(true)}
        >
          + Nuevo vuelo
        </button>
      </div>

      {/* ── Mensaje de éxito o error ── */}
      {/* Solo aparece cuando hay un mensaje activo */}
      {mensaje && (
        <div
          className={`alert alert-${mensaje.tipo} py-2 small rounded-3 mb-3`}
        >
          {mensaje.texto}
        </div>
      )}

      {/* ── Tabla de vuelos ── */}
      <div className="card border-0 shadow-sm rounded-4 mb-4">
        <div className="card-body">
          <h5 className="fw-bold mb-3" style={{ color: "#3c3489" }}>
            Vuelos registrados
          </h5>
          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead>
                <tr className="table-light">
                  <th className="fw-semibold small text-muted"># Vuelo</th>
                  <th className="fw-semibold small text-muted">Ruta</th>
                  <th className="fw-semibold small text-muted">Fecha salida</th>
                  <th className="fw-semibold small text-muted">Puerta</th>
                  <th className="fw-semibold small text-muted">Avión</th>
                  <th className="fw-semibold small text-muted">Estado</th>
                  <th className="fw-semibold small text-muted">Acciones</th>
                </tr>
              </thead>
              <tbody>
                {vuelos.map((v) => {
                  const cfg = ESTADO_CONFIG[v.estado] ?? ESTADO_CONFIG.pendiente;
                  return (
                    <tr key={v.id_vuelo}>
                      <td className="fw-semibold">#{v.id_vuelo}</td>
                      <td>{v.ruta}</td>
                      <td>
                        {/* Formatea la fecha completa con hora */}
                        {new Date(v.fecha_salida).toLocaleString("es-CR", {
                          day: "2-digit", month: "2-digit", year: "numeric",
                          hour: "2-digit", minute: "2-digit",
                        })}
                      </td>
                      <td>{v.puerta}</td>
                      <td><code className="small">{v.matricula}</code></td>
                      <td>
                        <span
                          className="badge rounded-pill px-2 py-1"
                          style={{ background: cfg.bg, color: cfg.color, fontSize: 12 }}
                        >
                          {cfg.label}
                        </span>
                      </td>
                      <td>
                        <div className="d-flex gap-1">
                          {/* Botón Abrir: solo si está pendiente */}
                          {v.estado === "pendiente" && (
                            <button
                              className="btn btn-sm rounded-2"
                              style={{ background: "#d1e7dd", color: "#198754", fontSize: 12 }}
                              onClick={() => handleAbrir(v.id_vuelo)}
                            >
                              Abrir
                            </button>
                          )}

                          {/* Botón Cerrar: solo si está abierto */}
                          {v.estado === "abierto" && (
                            <button
                              className="btn btn-sm rounded-2"
                              style={{ background: "#f8d7da", color: "#842029", fontSize: 12 }}
                              onClick={() => handleCerrar(v)}
                            >
                              Cerrar
                            </button>
                          )}

                          {/* Si está cerrado no hay acciones */}
                          {v.estado === "cerrado" && (
                            <span className="text-muted small">Sin acciones</span>
                          )}
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      {/* ── Modal de nuevo vuelo ── */}
      {/* Solo se renderiza si modalAbierto = true */}
      {modalAbierto && (
        <ModalNuevoVuelo
          onGuardar={handleNuevoVuelo}
          onCancelar={() => setModalAbierto(false)}
        />
      )}
    </div>
  );
}

// ─────────────────────────────────────────────────────────
// ModalNuevoVuelo — formulario para registrar un vuelo
//
// Recibe:
//   onGuardar  → función que recibe los datos del formulario
//   onCancelar → función para cerrar el modal sin guardar
// ─────────────────────────────────────────────────────────
function ModalNuevoVuelo({ onGuardar, onCancelar }) {
  // Estado del formulario con valores iniciales vacíos
  const [form, setForm] = useState({
    id_ruta:      "",
    matricula:    "",
    fecha_salida: "",
    puerta:       "",
  });

  const [error, setError] = useState("");

  const handleChange = (e) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = (e) => {
    e.preventDefault();
    setError("");

    // Validación básica
    if (!form.id_ruta || !form.matricula || !form.fecha_salida || !form.puerta) {
      setError("Todos los campos son obligatorios.");
      return;
    }

    // Busca la descripción de la ruta seleccionada para mostrarla en la tabla
    const rutaSeleccionada = MOCK_RUTAS.find(
      (r) => r.id_ruta === parseInt(form.id_ruta)
    );

    onGuardar({
      ...form,
      ruta: rutaSeleccionada?.descripcion ?? "Sin ruta",
    });
  };

  return (
    // Fondo oscuro detrás del modal
    <div
      className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center"
      style={{ background: "rgba(0,0,0,0.4)", zIndex: 1000 }}
    >
      <div
        className="card border-0 shadow rounded-4"
        style={{ width: "100%", maxWidth: 480 }}
      >
        <div className="card-body p-4">
          <h5 className="fw-bold mb-1" style={{ color: "#3c3489" }}>
            Registrar nuevo vuelo
          </h5>
          <p className="text-muted small mb-4">
            Complete los datos del vuelo
          </p>

          {error && (
            <div className="alert alert-danger py-2 small rounded-3">
              {error}
            </div>
          )}

          <form onSubmit={handleSubmit}>
            <div className="row g-3">

              {/* Selector de ruta */}
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Ruta <span className="text-danger">*</span>
                </label>
                <select
                  name="id_ruta"
                  className="form-select rounded-3"
                  value={form.id_ruta}
                  onChange={handleChange}
                  required
                >
                  <option value="">Seleccionar ruta</option>
                  {MOCK_RUTAS.map((r) => (
                    <option key={r.id_ruta} value={r.id_ruta}>
                      {r.descripcion}
                    </option>
                  ))}
                </select>
              </div>

              {/* Selector de avión */}
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Avión <span className="text-danger">*</span>
                </label>
                <select
                  name="matricula"
                  className="form-select rounded-3"
                  value={form.matricula}
                  onChange={handleChange}
                  required
                >
                  <option value="">Seleccionar avión</option>
                  {MOCK_AVIONES.map((a) => (
                    <option key={a.matricula} value={a.matricula}>
                      {a.matricula} ({a.capacidad} pax)
                    </option>
                  ))}
                </select>
              </div>

              {/* Fecha y hora de salida */}
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Fecha y hora de salida <span className="text-danger">*</span>
                </label>
                <input
                  type="datetime-local"
                  name="fecha_salida"
                  className="form-control rounded-3"
                  value={form.fecha_salida}
                  onChange={handleChange}
                  required
                />
              </div>

              {/* Puerta de abordaje */}
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Puerta de abordaje <span className="text-danger">*</span>
                </label>
                <input
                  type="text"
                  name="puerta"
                  className="form-control rounded-3"
                  placeholder="ej. A3"
                  value={form.puerta}
                  onChange={handleChange}
                  required
                />
              </div>
            </div>

            {/* Botones de acción */}
            <div className="d-flex justify-content-end gap-2 mt-4 pt-3"
              style={{ borderTop: "0.5px solid #e8e4f8" }}
            >
              <button
                type="button"
                className="btn rounded-3 fw-semibold"
                style={{ background: "#f5f3ff", color: COLOR_PRINCIPAL }}
                onClick={onCancelar}
              >
                Cancelar
              </button>
              <button
                type="submit"
                className="btn rounded-3 fw-semibold text-white"
                style={{ background: COLOR_PRINCIPAL }}
              >
                Guardar vuelo
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}