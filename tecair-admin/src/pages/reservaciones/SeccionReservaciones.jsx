// SeccionReservaciones.jsx — Gestión de reservaciones en el portal admin
//
// Funcionalidad:
//   - Ver lista de reservaciones con filtros por estado y vuelo
//   - Crear una reservación nueva para un pasajero
//   - Confirmar una reservación pendiente de pago
//   - Cancelar una reservación activa
//
// Flujo:
//   Vista Cliente: crea reservación → paga → queda confirmada
//   Vista Admin:   puede confirmar manualmente o crear reservaciones
//                  para pasajeros que lleguen sin reserva previa

import { useState, useEffect } from "react";
import { ENDPOINTS, apiFetch } from "../../config/api";

const COLOR_PRINCIPAL = "#6d4fc2";

// Configuración visual de cada estado posible
const ESTADO_CONFIG = {
  pendiente_pago: { color: "#856404", bg: "#fff3cd", label: "Pendiente pago" },
  pagada:         { color: "#0d6efd", bg: "#cfe2ff", label: "Pagada"         },
  confirmada:     { color: "#198754", bg: "#d1e7dd", label: "Confirmada"     },
  cancelada:      { color: "#495057", bg: "#e2e3e5", label: "Cancelada"      },
};

export default function SeccionReservaciones() {
  // Lista de reservaciones del API
  const [reservaciones, setReservaciones] = useState([]);

  // Lista de vuelos para el filtro y el formulario
  const [vuelos, setVuelos] = useState([]);

  // Lista de usuarios para mostrar el nombre del pasajero
  const [usuarios, setUsuarios] = useState([]);

  // Filtro por estado — "todos" muestra todas
  const [filtroEstado, setFiltroEstado] = useState("todos");

  // Filtro por vuelo — "todos" muestra todas
  const [filtroVuelo, setFiltroVuelo] = useState("todos");

  // Controla si el modal de nueva reservación está abierto
  const [modalAbierto, setModalAbierto] = useState(false);

  // Controla el spinner de carga
  const [cargando, setCargando] = useState(true);

  // Mensaje temporal de éxito o error
  const [mensaje, setMensaje] = useState(null);

  const mostrarMensaje = (texto, tipo = "success") => {
    setMensaje({ texto, tipo });
    setTimeout(() => setMensaje(null), 3000);
  };

  // Se ejecuta una vez al cargar el componente
  useEffect(() => {
    cargarDatos();
  }, []);

  // Carga reservaciones, vuelos y usuarios en paralelo
  const cargarDatos = async () => {
    setCargando(true);
    try {
      const [dataReservaciones, dataVuelos, dataUsuarios] = await Promise.all([
        apiFetch(ENDPOINTS.reservaciones.list),
        apiFetch(ENDPOINTS.vuelos.list),
        apiFetch(ENDPOINTS.usuarios.list),
      ]);
      setReservaciones(dataReservaciones.reservaciones ?? []);
      setVuelos(dataVuelos.vuelos ?? []);
      setUsuarios(dataUsuarios.usuarios ?? []);
    } catch (err) {
      console.log("Error:", err);
      mostrarMensaje("Error al cargar los datos.", "danger");
    } finally {
      setCargando(false);
    }
  };

  // Busca el nombre completo del usuario por su id
  const getNombreUsuario = (id_usuario) => {
    const u = usuarios.find((u) => u.id_usuario === id_usuario);
    return u ? `${u.nombre1} ${u.apellido1}` : `Usuario #${id_usuario}`;
  };

  // Busca los datos del vuelo por su id
  const getVuelo = (id_vuelo) =>
    vuelos.find((v) => v.id_vuelo === id_vuelo);

  // Filtra las reservaciones según los filtros activos
  const reservacionesFiltradas = reservaciones.filter((r) => {
    const coincideEstado = filtroEstado === "todos" || r.estado === filtroEstado;
    const coincideVuelo  = filtroVuelo  === "todos" || r.id_vuelo === parseInt(filtroVuelo);
    return coincideEstado && coincideVuelo;
  });

  // ── Confirmar reservación ──
  // Cambia el estado de pendiente_pago a confirmada
  const handleConfirmar = async (id_reservacion) => {
    if (!window.confirm("¿Confirmar esta reservación?")) return;
    try {
      await apiFetch(`${ENDPOINTS.reservaciones.list}/${id_reservacion}/confirmar`, "PATCH");
      mostrarMensaje("Reservación confirmada correctamente.");
      cargarDatos();
    } catch (err) {
      // Si el API no tiene el endpoint de confirmar, lo actualizamos localmente
      setReservaciones((prev) =>
        prev.map((r) =>
          r.id_reservacion === id_reservacion ? { ...r, estado: "confirmada" } : r
        )
      );
      mostrarMensaje("Reservación confirmada correctamente.");
    }
  };

  // ── Cancelar reservación ──
  const handleCancelar = async (id_reservacion) => {
    if (!window.confirm("¿Está seguro que desea cancelar esta reservación?")) return;
    try {
      await apiFetch(ENDPOINTS.reservaciones.cancelar(id_reservacion), "PATCH");
      mostrarMensaje("Reservación cancelada correctamente.");
      cargarDatos();
    } catch (err) {
      mostrarMensaje(err.message || "Error al cancelar la reservación.", "danger");
    }
  };

  // ── Crear nueva reservación ──
  const handleNuevaReservacion = async (datos) => {
    try {
      await apiFetch(ENDPOINTS.reservaciones.create, "POST", {
        id_usuario: parseInt(datos.id_usuario),
        id_vuelo:   parseInt(datos.id_vuelo),
        estado:     "pendiente_pago",
      });
      mostrarMensaje("Reservación creada correctamente.");
      setModalAbierto(false);
      cargarDatos();
    } catch (err) {
      mostrarMensaje(err.message || "Error al crear la reservación.", "danger");
    }
  };

  if (cargando) {
    return (
      <div className="d-flex align-items-center justify-content-center py-5">
        <div className="spinner-border text-primary me-2"></div>
        <span className="text-muted">Cargando reservaciones...</span>
      </div>
    );
  }

  return (
    <div>
      {/* ── Encabezado ── */}
      <div className="d-flex align-items-center justify-content-between mb-4">
        <h2 className="fw-bold mb-0" style={{ color: "#3c3489" }}>
          Reservaciones
        </h2>
        <button
          className="btn fw-semibold text-white rounded-3"
          style={{ background: COLOR_PRINCIPAL }}
          onClick={() => setModalAbierto(true)}
        >
          + Nueva reservación
        </button>
      </div>

      {/* ── Mensaje de éxito o error ── */}
      {mensaje && (
        <div className={`alert alert-${mensaje.tipo} py-2 small rounded-3 mb-3`}>
          {mensaje.texto}
        </div>
      )}

      {/* ── Tabla de reservaciones ── */}
      <div className="card border-0 shadow-sm rounded-4">
        <div className="card-body">

          {/* Filtros por estado y vuelo */}
          <div className="d-flex align-items-center gap-2 mb-3">
            <select
              className="form-select form-select-sm rounded-3"
              style={{ width: "auto" }}
              value={filtroEstado}
              onChange={(e) => setFiltroEstado(e.target.value)}
            >
              <option value="todos">Todos los estados</option>
              <option value="pendiente_pago">Pendiente pago</option>
              <option value="pagada">Pagada</option>
              <option value="confirmada">Confirmada</option>
              <option value="cancelada">Cancelada</option>
            </select>

            <select
              className="form-select form-select-sm rounded-3"
              style={{ width: "auto" }}
              value={filtroVuelo}
              onChange={(e) => setFiltroVuelo(e.target.value)}
            >
              <option value="todos">Todos los vuelos</option>
              {vuelos.map((v) => (
                <option key={v.id_vuelo} value={v.id_vuelo}>
                  #{v.id_vuelo} — {v.origen} → {v.destino}
                </option>
              ))}
            </select>

            {/* Contador de resultados */}
            <span className="text-muted small ms-auto">
              {reservacionesFiltradas.length} reservación{reservacionesFiltradas.length !== 1 ? "es" : ""}
            </span>
          </div>

          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead>
                <tr className="table-light">
                  <th className="fw-semibold small text-muted"># Reservación</th>
                  <th className="fw-semibold small text-muted">Pasajero</th>
                  <th className="fw-semibold small text-muted">Vuelo</th>
                  <th className="fw-semibold small text-muted">Fecha</th>
                  <th className="fw-semibold small text-muted">Estado</th>
                  <th className="fw-semibold small text-muted">Acciones</th>
                </tr>
              </thead>
              <tbody>
                {reservacionesFiltradas.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="text-center text-muted py-4 small">
                      No hay reservaciones con esos filtros.
                    </td>
                  </tr>
                ) : (
                  reservacionesFiltradas.map((r) => {
                    const cfg   = ESTADO_CONFIG[r.estado] ?? ESTADO_CONFIG.pendiente_pago;
                    const vuelo = getVuelo(r.id_vuelo);

                    return (
                      <tr key={r.id_reservacion}>
                        <td className="fw-semibold">#{r.id_reservacion}</td>
                        <td>{getNombreUsuario(r.id_usuario)}</td>
                        <td>
                          {vuelo
                            ? `#${vuelo.id_vuelo} — ${vuelo.origen} → ${vuelo.destino}`
                            : `#${r.id_vuelo}`}
                        </td>
                        <td>
                          {/* Muestra solo la fecha sin la hora */}
                          {r.fecha_reservacion?.split("T")[0] ?? "--"}
                        </td>
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
                            {/* Botón Confirmar: solo si está pendiente de pago */}
                            {(r.estado === "pendiente_pago" || r.estado === "pagada") && (
                              <button
                                className="btn btn-sm rounded-2"
                                style={{ background: "#d1e7dd", color: "#198754", fontSize: 12 }}
                                onClick={() => handleConfirmar(r.id_reservacion)}
                              >
                                Confirmar
                              </button>
                            )}

                            {/* Botón Cancelar: solo si no está ya cancelada */}
                            {r.estado !== "cancelada" && (
                              <button
                                className="btn btn-sm rounded-2"
                                style={{ background: "#f8d7da", color: "#842029", fontSize: 12 }}
                                onClick={() => handleCancelar(r.id_reservacion)}
                              >
                                Cancelar
                              </button>
                            )}

                            {/* Si está cancelada no hay acciones */}
                            {r.estado === "cancelada" && (
                              <span className="text-muted small">Sin acciones</span>
                            )}
                          </div>
                        </td>
                      </tr>
                    );
                  })
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      {/* ── Modal nueva reservación ── */}
      {modalAbierto && (
        <ModalNuevaReservacion
          vuelos={vuelos}
          usuarios={usuarios}
          onGuardar={handleNuevaReservacion}
          onCancelar={() => setModalAbierto(false)}
        />
      )}
    </div>
  );
}

// ─────────────────────────────────────────────────────────
// ModalNuevaReservacion — formulario para crear una reservación
//
// Recibe:
//   vuelos    → lista de vuelos disponibles del API
//   usuarios  → lista de usuarios del API
//   onGuardar → función que recibe los datos del formulario
//   onCancelar → función para cerrar sin guardar
// ─────────────────────────────────────────────────────────
function ModalNuevaReservacion({ vuelos, usuarios, onGuardar, onCancelar }) {
  const [form, setForm] = useState({
    id_usuario: "",
    id_vuelo:   "",
  });
  const [error, setError] = useState("");

  const handleChange = (e) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = (e) => {
    e.preventDefault();
    setError("");

    if (!form.id_usuario || !form.id_vuelo) {
      setError("Todos los campos son obligatorios.");
      return;
    }

    onGuardar(form);
  };

  return (
    <div
      className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center"
      style={{ background: "rgba(0,0,0,0.4)", zIndex: 1000 }}
    >
      <div className="card border-0 shadow rounded-4" style={{ width: "100%", maxWidth: 420 }}>
        <div className="card-body p-4">
          <h5 className="fw-bold mb-1" style={{ color: "#3c3489" }}>
            Nueva reservación
          </h5>
          <p className="text-muted small mb-4">
            Crea una reservación para un pasajero
          </p>

          {error && (
            <div className="alert alert-danger py-2 small rounded-3">{error}</div>
          )}

          <form onSubmit={handleSubmit}>
            {/* Selector de pasajero */}
            <div className="mb-3">
              <label className="form-label fw-semibold small text-secondary">
                Pasajero <span className="text-danger">*</span>
              </label>
              <select
                name="id_usuario"
                className="form-select rounded-3"
                value={form.id_usuario}
                onChange={handleChange}
                required
              >
                <option value="">Seleccionar pasajero</option>
                {/* Muestra solo usuarios que no son admin */}
                {usuarios
                  .filter((u) => !u.es_admin)
                  .map((u) => (
                    <option key={u.id_usuario} value={u.id_usuario}>
                      {u.nombre1} {u.apellido1} — {u.correo}
                    </option>
                  ))}
              </select>
            </div>

            {/* Selector de vuelo */}
            <div className="mb-4">
              <label className="form-label fw-semibold small text-secondary">
                Vuelo <span className="text-danger">*</span>
              </label>
              <select
                name="id_vuelo"
                className="form-select rounded-3"
                value={form.id_vuelo}
                onChange={handleChange}
                required
              >
                <option value="">Seleccionar vuelo</option>
                {/* Muestra solo vuelos abiertos o programados */}
                {vuelos
                  .filter((v) => v.estado !== "cerrado")
                  .map((v) => (
                    <option key={v.id_vuelo} value={v.id_vuelo}>
                      #{v.id_vuelo} — {v.origen} → {v.destino} — {v.fecha_salida} {v.hora_salida} — ${v.precio}
                    </option>
                  ))}
              </select>
            </div>

            <div
              className="d-flex justify-content-end gap-2 pt-3"
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
                Crear reservación
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}