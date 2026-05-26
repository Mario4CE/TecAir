// SeccionMaletas.jsx — Gestión de maletas conectada al API
//
// Funcionalidad:
//   - Ver pasajeros chequeados con su cantidad de maletas
//   - Gestionar maletas de un pasajero específico
//   - Agregar maletas a un check-in
//   - Calcular costo extra automáticamente:
//       Maleta 1 → Gratis
//       Maleta 2 → $50.00
//       Maleta 3 en adelante → $75.00 c/u

import { useState, useEffect } from "react";
import { ENDPOINTS, apiFetch } from "../../config/api";

const COLOR_PRINCIPAL = "#6d4fc2";

// Calcula el costo extra total según la cantidad de maletas
// Ejemplos: 1 maleta = $0, 2 maletas = $50, 3 maletas = $125
export function calcularCostoExtra(cantidad) {
  if (cantidad <= 1) return 0;
  if (cantidad === 2) return 50;
  return 50 + (cantidad - 2) * 75;
}

// Devuelve la etiqueta y color del costo según el número de maleta
export function costoPorMaleta(numero) {
  if (numero === 1) return { label: "Gratis", color: "#198754", bg: "#d1e7dd" };
  if (numero === 2) return { label: "$50.00", color: "#856404", bg: "#fff3cd" };
  return               { label: "$75.00", color: "#842029", bg: "#f8d7da" };
}

export default function SeccionMaletas() {
  // Lista de check-ins del API
  const [checkins, setCheckins]   = useState([]);

  // Lista de maletas del API
  const [maletas, setMaletas]     = useState([]);

  // Lista de vuelos para mostrar la ruta en la tabla
  const [vuelos, setVuelos]       = useState([]);

  // Lista de usuarios para mostrar el nombre del pasajero
  const [usuarios, setUsuarios]   = useState([]);

  // Check-in seleccionado para gestionar sus maletas
  // null = no hay ninguno seleccionado
  const [checkinSeleccionado, setCheckinSeleccionado] = useState(null);

  // Controla el spinner de carga
  const [cargando, setCargando]   = useState(true);

  // Mensaje temporal de éxito o error
  const [mensaje, setMensaje]     = useState(null);

  const mostrarMensaje = (texto, tipo = "success") => {
    setMensaje({ texto, tipo });
    setTimeout(() => setMensaje(null), 3000);
  };

  // Se ejecuta una vez al cargar el componente
  useEffect(() => {
    cargarDatos();
  }, []);

  // Carga todos los datos necesarios en paralelo
  const cargarDatos = async () => {
    setCargando(true);
    try {
      const [dataCheckins, dataMaletas, dataVuelos, dataUsuarios] = await Promise.all([
        apiFetch(ENDPOINTS.checkins.list),
        apiFetch(ENDPOINTS.maletas.list),
        apiFetch(ENDPOINTS.vuelos.list),
        apiFetch(ENDPOINTS.usuarios.list),
      ]);
      setCheckins(dataCheckins.checkins ?? []);
      setMaletas(dataMaletas.maletas ?? []);
      setVuelos(dataVuelos.vuelos ?? []);
      setUsuarios(dataUsuarios.usuarios ?? []);
    } catch (err) {
      console.log("Error:", err);
      mostrarMensaje("Error al cargar los datos.", "danger");
    } finally {
      setCargando(false);
    }
  };

  // Filtra las maletas que pertenecen a un check-in específico
  const getMaletas = (id_checkin) =>
    maletas.filter((m) => m.id_checkin === id_checkin);

  // Busca el nombre completo del usuario por su id
  const getNombreUsuario = (id_usuario) => {
    const u = usuarios.find((u) => u.id_usuario === id_usuario);
    return u ? `${u.nombre1} ${u.apellido1}` : `Usuario #${id_usuario}`;
  };

  // Busca los datos del vuelo por su id
  const getVuelo = (id_vuelo) =>
    vuelos.find((v) => v.id_vuelo === id_vuelo);

  // Agrega una maleta al check-in seleccionado
  // Llama al endpoint POST /maletas y recarga la lista
  const handleAgregarMaleta = async (nuevaMaleta) => {
    try {
      await apiFetch(ENDPOINTS.maletas.create, "POST", {
        ...nuevaMaleta,
        id_checkin: checkinSeleccionado.id_checkin,
      });
      mostrarMensaje("Maleta agregada correctamente.");
      const dataMaletas = await apiFetch(ENDPOINTS.maletas.list);
      setMaletas(dataMaletas.maletas ?? []);
    } catch (err) {
      mostrarMensaje(err.message || "Error al agregar maleta.", "danger");
    }
  };

  if (cargando) {
    return (
      <div className="d-flex align-items-center justify-content-center py-5">
        <div className="spinner-border text-primary me-2"></div>
        <span className="text-muted">Cargando maletas...</span>
      </div>
    );
  }

  return (
    <div>
      <div className="mb-4">
        <h2 className="fw-bold mb-0" style={{ color: "#3c3489" }}>Maletas</h2>
      </div>

      {mensaje && (
        <div className={`alert alert-${mensaje.tipo} py-2 small rounded-3 mb-3`}>
          {mensaje.texto}
        </div>
      )}

      {/* Tabla de pasajeros chequeados */}
      <div className="card border-0 shadow-sm rounded-4 mb-4">
        <div className="card-body">
          <h5 className="fw-bold mb-3" style={{ color: "#3c3489" }}>
            Pasajeros chequeados
          </h5>
          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead>
                <tr className="table-light">
                  <th className="fw-semibold small text-muted"># Check-in</th>
                  <th className="fw-semibold small text-muted">Pasajero</th>
                  <th className="fw-semibold small text-muted">Vuelo</th>
                  <th className="fw-semibold small text-muted">Asiento</th>
                  <th className="fw-semibold small text-muted">Maletas</th>
                  <th className="fw-semibold small text-muted">Costo extra</th>
                  <th className="fw-semibold small text-muted">Acciones</th>
                </tr>
              </thead>
              <tbody>
                {checkins.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="text-center text-muted py-4 small">
                      No hay pasajeros chequeados.
                    </td>
                  </tr>
                ) : (
                  checkins.map((c) => {
                    const maletasCheckin = getMaletas(c.id_checkin);
                    const costo = calcularCostoExtra(maletasCheckin.length);
                    const esSeleccionado = checkinSeleccionado?.id_checkin === c.id_checkin;
                    const vuelo = getVuelo(c.id_vuelo);

                    return (
                      <tr key={c.id_checkin}
                        style={esSeleccionado ? { background: "#faf8ff" } : {}}>
                        <td className="fw-semibold">#{c.id_checkin}</td>
                        <td>{getNombreUsuario(c.id_usuario)}</td>
                        <td>#{c.id_vuelo} — {vuelo?.origen} → {vuelo?.destino}</td>
                        <td><b>{c.asiento}</b></td>
                        <td>{maletasCheckin.length}</td>
                        <td>
                          <span className="badge rounded-pill px-2 py-1"
                            style={{
                              background: costo === 0 ? "#d1e7dd" : "#fff3cd",
                              color:      costo === 0 ? "#198754" : "#856404",
                              fontSize: 12,
                            }}>
                            ${costo.toFixed(2)}
                          </span>
                        </td>
                        <td>
                          <button
                            className="btn btn-sm rounded-2 fw-semibold"
                            style={{
                              background: esSeleccionado ? COLOR_PRINCIPAL : "#ede9fa",
                              color:      esSeleccionado ? "#fff" : COLOR_PRINCIPAL,
                              fontSize: 12,
                            }}
                            onClick={() => setCheckinSeleccionado(esSeleccionado ? null : c)}
                          >
                            {esSeleccionado ? "Cerrar" : "Gestionar maletas"}
                          </button>
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

      {/* Panel de gestión — solo aparece si hay un check-in seleccionado */}
      {checkinSeleccionado && (
        <div className="row g-3">
          <div className="col-12 col-md-8">
            <div className="card border-0 shadow-sm rounded-4">
              <div className="card-body">
                <h5 className="fw-bold mb-3" style={{ color: "#3c3489" }}>
                  Maletas de {getNombreUsuario(checkinSeleccionado.id_usuario)} — Check-in #{checkinSeleccionado.id_checkin}
                </h5>
                <div className="table-responsive mb-4">
                  <table className="table align-middle mb-0">
                    <thead>
                      <tr className="table-light">
                        <th className="fw-semibold small text-muted">#</th>
                        <th className="fw-semibold small text-muted">Número</th>
                        <th className="fw-semibold small text-muted">Peso</th>
                        <th className="fw-semibold small text-muted">Color</th>
                        <th className="fw-semibold small text-muted">Costo</th>
                      </tr>
                    </thead>
                    <tbody>
                      {getMaletas(checkinSeleccionado.id_checkin).length === 0 ? (
                        <tr>
                          <td colSpan={5} className="text-center text-muted py-3 small">
                            Sin maletas registradas.
                          </td>
                        </tr>
                      ) : (
                        getMaletas(checkinSeleccionado.id_checkin).map((m, index) => {
                          const costo = costoPorMaleta(index + 1);
                          return (
                            <tr key={m.num_maleta}>
                              <td>
                                {/* Círculo con el número de maleta */}
                                <span
                                  className="d-inline-flex align-items-center justify-content-center rounded-circle"
                                  style={{ width: 24, height: 24, background: "#ede9fa", color: COLOR_PRINCIPAL, fontSize: 11, fontWeight: 700 }}>
                                  {index + 1}
                                </span>
                              </td>
                              <td><code className="small">{m.num_maleta}</code></td>
                              <td>{m.peso} kg</td>
                              <td>{m.color}</td>
                              <td>
                                <span className="badge rounded-pill px-2 py-1"
                                  style={{ background: costo.bg, color: costo.color, fontSize: 12 }}>
                                  {costo.label}
                                </span>
                              </td>
                            </tr>
                          );
                        })
                      )}
                    </tbody>
                  </table>
                </div>
                <FormAgregarMaleta onAgregar={handleAgregarMaleta} />
              </div>
            </div>
          </div>
          <div className="col-12 col-md-4">
            <ResumenCobro maletas={getMaletas(checkinSeleccionado.id_checkin)} />
          </div>
        </div>
      )}
    </div>
  );
}

// FormAgregarMaleta — formulario para agregar una maleta al check-in seleccionado
function FormAgregarMaleta({ onAgregar }) {
  const [form, setForm] = useState({ num_maleta: "", peso: "", color: "" });
  const [error, setError] = useState("");

  const handleChange = (e) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = (e) => {
    e.preventDefault();
    setError("");
    if (!form.num_maleta || !form.peso || !form.color) {
      setError("Todos los campos son obligatorios.");
      return;
    }
    if (isNaN(form.peso) || parseFloat(form.peso) <= 0) {
      setError("El peso debe ser un número mayor a 0.");
      return;
    }
    onAgregar({
      num_maleta: form.num_maleta.toUpperCase(),
      peso:       parseFloat(form.peso),
      color:      form.color,
    });
    // Limpia el formulario después de agregar
    setForm({ num_maleta: "", peso: "", color: "" });
  };

  return (
    <div className="pt-3" style={{ borderTop: "0.5px solid #e8e4f8" }}>
      <h6 className="fw-bold mb-3" style={{ color: "#3c3489" }}>Agregar maleta</h6>
      {error && (
        <div className="alert alert-danger py-2 small rounded-3 mb-3">{error}</div>
      )}
      <form onSubmit={handleSubmit}>
        <div className="row g-2 mb-2">
          <div className="col-6">
            <label className="form-label fw-semibold small text-secondary">
              # Maleta <span className="text-danger">*</span>
            </label>
            <input type="text" name="num_maleta" className="form-control rounded-3"
              placeholder="ej. MAL-003" value={form.num_maleta}
              onChange={handleChange} required />
          </div>
          <div className="col-6">
            <label className="form-label fw-semibold small text-secondary">
              Peso (kg) <span className="text-danger">*</span>
            </label>
            <input type="number" name="peso" className="form-control rounded-3"
              placeholder="ej. 20.5" value={form.peso}
              onChange={handleChange} step="0.1" min="0.1" required />
          </div>
        </div>
        <div className="mb-3">
          <label className="form-label fw-semibold small text-secondary">
            Color <span className="text-danger">*</span>
          </label>
          <input type="text" name="color" className="form-control rounded-3"
            placeholder="ej. Negro" value={form.color}
            onChange={handleChange} required />
        </div>
        <button type="submit" className="btn w-100 rounded-3 fw-semibold text-white"
          style={{ background: COLOR_PRINCIPAL }}>
          + Agregar maleta
        </button>
      </form>
    </div>
  );
}

// ResumenCobro — muestra el desglose de costos por maleta y el total
function ResumenCobro({ maletas }) {
  const total = calcularCostoExtra(maletas.length);

  return (
    <div className="card border-0 shadow-sm rounded-4 h-100">
      <div className="card-body">
        <h5 className="fw-bold mb-3" style={{ color: "#3c3489" }}>Resumen de cobro</h5>
        {maletas.length === 0 ? (
          <p className="text-muted small">Sin maletas registradas.</p>
        ) : (
          maletas.map((m, index) => {
            const costo = costoPorMaleta(index + 1);
            return (
              <div key={m.num_maleta} className="d-flex justify-content-between py-2"
                style={{ borderBottom: "0.5px solid #f0edf8", fontSize: 13 }}>
                <span className="text-muted">Maleta {index + 1}</span>
                <span style={{ color: costo.color, fontWeight: 600 }}>{costo.label}</span>
              </div>
            );
          })
        )}
        <div className="d-flex justify-content-between align-items-center mt-3 p-2 rounded-3"
          style={{ background: "#f5f3ff" }}>
          <span className="fw-bold small">Total extra</span>
          <span className="fw-bold" style={{ color: COLOR_PRINCIPAL, fontSize: 16 }}>
            ${total.toFixed(2)}
          </span>
        </div>
        <div className="mt-3 p-2 rounded-3 small" style={{ background: "#fff3cd", color: "#856404" }}>
          <b>Tarifas:</b><br />
          1ra maleta — Gratis<br />
          2da maleta — $50.00<br />
          3ra en adelante — $75.00 c/u
        </div>
      </div>
    </div>
  );
}