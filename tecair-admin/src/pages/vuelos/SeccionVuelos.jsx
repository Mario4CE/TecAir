// SeccionVuelos.jsx — Gestión de vuelos del aeropuerto
//
// Funcionalidad:
//   - Ver lista de vuelos
//   - Crear un vuelo nuevo
//   - Abrir un vuelo (pendiente → abierto)
//   - Cerrar un vuelo (abierto → cerrado)
//     solo si falta menos de 1 hora para la salida

// useEffect se agrega para cargar datos al abrir la página
import { useState, useEffect } from "react";
import { ENDPOINTS, apiFetch } from "../../config/api";

const COLOR_PRINCIPAL = "#6d4fc2";

const ESTADO_CONFIG = {
  abierto:    { color: "#198754", bg: "#d1e7dd", label: "Abierto"    },
  pendiente:  { color: "#856404", bg: "#fff3cd", label: "Pendiente"  },
  programado: { color: "#856404", bg: "#fff3cd", label: "Programado" },
  cerrado:    { color: "#495057", bg: "#e2e3e5", label: "Cerrado"    },
};

export default function SeccionVuelos() {
  const [vuelos, setVuelos]         = useState([]);
  const [rutas, setRutas]           = useState([]);
  const [aviones, setAviones]       = useState([]);
  const [cargando, setCargando]     = useState(true);
  const [modalAbierto, setModalAbierto] = useState(false);
  const [mensaje, setMensaje]       = useState(null);

  const mostrarMensaje = (texto, tipo = "success") => {
    setMensaje({ texto, tipo });
    setTimeout(() => setMensaje(null), 3000);
  };

  // ── Carga inicial de datos ──
  // useEffect con [] se ejecuta una sola vez al montar el componente
  // Es equivalente a "cuando la página abre, trae los datos del API"
  useEffect(() => {
    cargarDatos();
  }, []);

  const cargarDatos = async () => {
    setCargando(true);
    try {
      const [dataVuelos, dataRutas, dataAviones] = await Promise.all([
        apiFetch(ENDPOINTS.vuelos.list),
        apiFetch(ENDPOINTS.rutas.list),
        apiFetch(ENDPOINTS.aviones.list),
      ]);
      console.log("Vuelos:", dataVuelos);
      console.log("Rutas:", dataRutas);
      console.log("Aviones:", dataAviones);
      setVuelos(dataVuelos.vuelos ?? []);
      setRutas(dataRutas.rutas ?? []);
      setAviones(dataAviones.aviones ?? []);
    } catch (err) {
      console.log("Error:", err); // <- agrega esto
      mostrarMensaje("Error al cargar los datos.", "danger");
    } finally {
      setCargando(false);
    }
  };

  // ── Abrir vuelo ──
  // Llama al endpoint PATCH /vuelos/{id}/abrir
  const handleAbrir = async (id_vuelo) => {
    try {
      await apiFetch(ENDPOINTS.vuelos.abrir(id_vuelo), "PATCH");
      mostrarMensaje("Vuelo abierto correctamente.");
      // Recarga la lista para reflejar el cambio
      cargarDatos();
    } catch (err) {
      mostrarMensaje(err.message || "Error al abrir el vuelo.", "danger");
    }
  };

  // ── Cerrar vuelo ──
  // Solo permite cerrar si falta menos de 1 hora para la salida
  const handleCerrar = async (vuelo) => {
    const ahora = new Date();
    const salida = new Date(`${vuelo.fecha_salida}T${vuelo.hora_salida ?? "00:00"}`);
    const diferenciaHoras = (salida - ahora) / (1000 * 60 * 60);

    if (diferenciaHoras > 1) {
      mostrarMensaje("Solo se puede cerrar el vuelo una hora antes de la salida.", "danger");
      return;
    }

    try {
      await apiFetch(ENDPOINTS.vuelos.cerrar(vuelo.id_vuelo), "PATCH");
      mostrarMensaje("Vuelo cerrado correctamente.");
      cargarDatos();
    } catch (err) {
      mostrarMensaje(err.message || "Error al cerrar el vuelo.", "danger");
    }
  };

  // ── Crear vuelo nuevo ──
  // Llama al endpoint POST /vuelos
  const handleNuevoVuelo = async (datosVuelo) => {
  try {
    console.log("Enviando:", datosVuelo); // agrega esto
    await apiFetch(ENDPOINTS.vuelos.create, "POST", datosVuelo);
    mostrarMensaje("Vuelo registrado correctamente.");
    setModalAbierto(false);
    cargarDatos();
  } catch (err) {
    console.log("Error detallado:", err.message); // agrega esto
    mostrarMensaje(err.message || "Error al crear el vuelo.", "danger");
  }
};

  // Muestra spinner mientras cargan los datos
  if (cargando) {
    return (
      <div className="d-flex align-items-center justify-content-center py-5">
        <div className="spinner-border text-primary me-2"></div>
        <span className="text-muted">Cargando vuelos...</span>
      </div>
    );
  }

  return (
    <div>
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

      {mensaje && (
        <div className={`alert alert-${mensaje.tipo} py-2 small rounded-3 mb-3`}>
          {mensaje.texto}
        </div>
      )}

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
                {vuelos.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="text-center text-muted py-4 small">
                      No hay vuelos registrados.
                    </td>
                  </tr>
                ) : (
                  vuelos.map((v) => {
                    const cfg = ESTADO_CONFIG[v.estado] ?? ESTADO_CONFIG.programado;
                    return (
                      <tr key={v.id_vuelo}>
                        <td className="fw-semibold">#{v.id_vuelo}</td>
                        <td>
                          {/* La ruta viene como objeto con origen y destino desde el API */}
                          {v.origen ?? ""} → {v.destino ?? ""}
                        </td>
                        <td>
                          {v.fecha_salida} {v.hora_salida}
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
                            {(v.estado === "pendiente" || v.estado === "programado") && (
                              <button
                                className="btn btn-sm rounded-2"
                                style={{ background: "#d1e7dd", color: "#198754", fontSize: 12 }}
                                onClick={() => handleAbrir(v.id_vuelo)}
                              >
                                Abrir
                              </button>
                            )}
                            {v.estado === "abierto" && (
                              <button
                                className="btn btn-sm rounded-2"
                                style={{ background: "#f8d7da", color: "#842029", fontSize: 12 }}
                                onClick={() => handleCerrar(v)}
                              >
                                Cerrar
                              </button>
                            )}
                            {v.estado === "cerrado" && (
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

      {modalAbierto && (
        <ModalNuevoVuelo
          rutas={rutas}
          aviones={aviones}
          onGuardar={handleNuevoVuelo}
          onCancelar={() => setModalAbierto(false)}
        />
      )}
    </div>
  );
}

// ─────────────────────────────────────────────────────────
// ModalNuevoVuelo — ahora recibe rutas y aviones del API
// ─────────────────────────────────────────────────────────
function ModalNuevoVuelo({ rutas, aviones, onGuardar, onCancelar }) {
  const [form, setForm] = useState({
    id_ruta:      "",
    matricula:    "",
    fecha_salida: "",
    hora_salida:  "",
    puerta:       "",
    precio:       "",
  });
  const [error, setError] = useState("");

  const handleChange = (e) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!form.id_ruta || !form.matricula || !form.fecha_salida || !form.puerta) {
      setError("Todos los campos obligatorios deben completarse.");
      return;
    }

    const horaSalida = form.hora_salida
      ? form.hora_salida.length === 5
        ? `${form.hora_salida}:00`
        : form.hora_salida
      : "08:00:00";

    // El API usa snake_case para deserializar (SnakeCaseLower)
    onGuardar({
      id_ruta:      parseInt(form.id_ruta),
      matricula:    form.matricula,
      fecha_salida: form.fecha_salida,
      hora_salida:  horaSalida,
      puerta:       form.puerta,
      precio:       form.precio ? parseFloat(form.precio) : 0,
    });
  };

  return (
    <div
      className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center"
      style={{ background: "rgba(0,0,0,0.4)", zIndex: 1000 }}
    >
      <div className="card border-0 shadow rounded-4" style={{ width: "100%", maxWidth: 480 }}>
        <div className="card-body p-4">
          <h5 className="fw-bold mb-1" style={{ color: "#3c3489" }}>
            Registrar nuevo vuelo
          </h5>
          <p className="text-muted small mb-4">Complete los datos del vuelo</p>

          {error && (
            <div className="alert alert-danger py-2 small rounded-3">{error}</div>
          )}

          <form onSubmit={handleSubmit}>
            <div className="row g-3">
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Ruta <span className="text-danger">*</span>
                </label>
                <select name="id_ruta" className="form-select rounded-3"
                  value={form.id_ruta} onChange={handleChange} required>
                  <option value="">Seleccionar ruta</option>
                  {/* Rutas que vienen del API */}
                  {rutas.map((r) => {
                    // Busca la escala de origen y destino dentro del arreglo de escalas
                    const origen  = r.escalas?.find((e) => e.tipo === "origen");
                    const destino = r.escalas?.find((e) => e.tipo === "destino");
                    return (
                      <option key={r.id_ruta} value={r.id_ruta}>
                        {origen?.nombre ?? "?"} → {destino?.nombre ?? "?"}
                      </option>
                    );
                  })}
                </select>
              </div>

              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Avión <span className="text-danger">*</span>
                </label>
                <select name="matricula" className="form-select rounded-3"
                  value={form.matricula} onChange={handleChange} required>
                  <option value="">Seleccionar avión</option>
                  {/* Aviones que vienen del API */}
                  {aviones.map((a) => (
                    <option key={a.matricula} value={a.matricula}>
                      {a.matricula} ({a.capacidad} pax)
                    </option>
                  ))}
                </select>
              </div>

              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Fecha de salida <span className="text-danger">*</span>
                </label>
                <input type="date" name="fecha_salida" className="form-control rounded-3"
                  value={form.fecha_salida} onChange={handleChange} required />
              </div>

              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Hora de salida
                </label>
                <input type="time" name="hora_salida" className="form-control rounded-3"
                  value={form.hora_salida} onChange={handleChange} />
              </div>

              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Puerta <span className="text-danger">*</span>
                </label>
                <input type="text" name="puerta" className="form-control rounded-3"
                  placeholder="ej. A3" value={form.puerta} onChange={handleChange} required />
              </div>

              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Precio ($)
                </label>
                <input type="number" name="precio" className="form-control rounded-3"
                  placeholder="ej. 350.00" value={form.precio}
                  onChange={handleChange} step="0.01" min="0" />
              </div>
            </div>

            <div className="d-flex justify-content-end gap-2 mt-4 pt-3"
              style={{ borderTop: "0.5px solid #e8e4f8" }}>
              <button type="button" className="btn rounded-3 fw-semibold"
                style={{ background: "#f5f3ff", color: COLOR_PRINCIPAL }}
                onClick={onCancelar}>
                Cancelar
              </button>
              <button type="submit" className="btn rounded-3 fw-semibold text-white"
                style={{ background: COLOR_PRINCIPAL }}>
                Guardar vuelo
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}