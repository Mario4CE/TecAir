// SeccionMaletas.jsx — Gestión de maletas por pasajero
//
// Funcionalidad:
//   - Ver pasajeros chequeados con su cantidad de maletas
//   - Gestionar maletas de un pasajero específico
//   - Agregar maletas a un check-in
//   - Calcular costo extra automáticamente:
//       Maleta 1 → Gratis
//       Maleta 2 → $50.00
//       Maleta 3 en adelante → $75.00 c/u

import { useState } from "react";
import { MOCK_CHECKINS } from "../../config/mockData";

const COLOR_PRINCIPAL = "#6d4fc2";

// Datos de prueba de maletas
// Cuando el API esté lista, estos vendrán del backend
const MOCK_MALETAS_INICIALES = {
  1: [{ num_maleta: "MAL-001", peso: 23.5, color: "Negro", id_checkin: 1 }],
  2: [
    { num_maleta: "MAL-002", peso: 18.0, color: "Azul",  id_checkin: 2 },
    { num_maleta: "MAL-003", peso: 15.0, color: "Rojo",  id_checkin: 2 },
  ],
};

// Calcula el costo extra según la cantidad de maletas 
// Maleta 1 → $0, Maleta 2 → $50, Maleta 3+ → $75 c/u
export function calcularCostoExtra(cantidadMaletas) {
  if (cantidadMaletas <= 1) return 0;
  if (cantidadMaletas === 2) return 50;
  // 3 maletas = $125 (0 + 50 + 75)
  // 4 maletas = $200 (0 + 50 + 75 + 75), etc.
  return 50 + (cantidadMaletas - 2) * 75;
}

// Calcula el costo individual de cada maleta para mostrarlo en la tabla
export function costoPorMaleta(numero) {
  if (numero === 1) return { label: "Gratis",  color: "#198754", bg: "#d1e7dd" };
  if (numero === 2) return { label: "$50.00",  color: "#856404", bg: "#fff3cd" };
  return               { label: "$75.00",  color: "#842029", bg: "#f8d7da" };
}

export default function SeccionMaletas() {
  // Lista de check-ins
  const [checkins] = useState(MOCK_CHECKINS);

  // Maletas agrupadas por id_checkin
  // { 1: [...maletas], 2: [...maletas] }
  const [maletasPorCheckin, setMaletasPorCheckin] = useState(
    MOCK_MALETAS_INICIALES
  );

  // Check-in seleccionado para gestionar sus maletas
  // null = no hay ninguno seleccionado
  const [checkinSeleccionado, setCheckinSeleccionado] = useState(null);

  // Mensaje de éxito o error temporal
  const [mensaje, setMensaje] = useState(null);

  const mostrarMensaje = (texto, tipo = "success") => {
    setMensaje({ texto, tipo });
    setTimeout(() => setMensaje(null), 3000);
  };

  // Devuelve las maletas de un check-in específico
  const getMaletas = (id_checkin) =>
    maletasPorCheckin[id_checkin] ?? [];

  // ── Agregar maleta a un check-in ──
  const handleAgregarMaleta = (nuevaMaleta) => {
    const id = checkinSeleccionado.id_checkin;
    const maletasActuales = getMaletas(id);

    // Verifica que el número de maleta no esté repetido
    const yaExiste = Object.values(maletasPorCheckin)
      .flat()
      .some((m) => m.num_maleta === nuevaMaleta.num_maleta);

    if (yaExiste) {
      mostrarMensaje("Ya existe una maleta con ese número.", "danger");
      return;
    }

    setMaletasPorCheckin((prev) => ({
      ...prev,
      [id]: [...maletasActuales, { ...nuevaMaleta, id_checkin: id }],
    }));
    mostrarMensaje("Maleta agregada correctamente.");
  };

  return (
    <div>
      {/* Encabezado */}
      <div className="mb-4">
        <h2 className="fw-bold mb-0" style={{ color: "#3c3489" }}>
          Maletas
        </h2>
      </div>

      {/* Mensaje de éxito o error */}
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
                {checkins.map((c) => {
                  const maletas = getMaletas(c.id_checkin);
                  const costo = calcularCostoExtra(maletas.length);
                  const esSeleccionado =
                    checkinSeleccionado?.id_checkin === c.id_checkin;

                  return (
                    <tr
                      key={c.id_checkin}
                      style={esSeleccionado ? { background: "#faf8ff" } : {}}
                    >
                      <td className="fw-semibold">#{c.id_checkin}</td>
                      <td>{c.nombre_pasajero}</td>
                      <td>#{c.id_vuelo}</td>
                      <td><b>{c.asiento}</b></td>
                      <td>{maletas.length}</td>
                      <td>
                        <span
                          className="badge rounded-pill px-2 py-1"
                          style={{
                            background: costo === 0 ? "#d1e7dd" : "#fff3cd",
                            color:      costo === 0 ? "#198754" : "#856404",
                            fontSize: 12,
                          }}
                        >
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
                          onClick={() =>
                            setCheckinSeleccionado(
                              esSeleccionado ? null : c
                            )
                          }
                        >
                          {esSeleccionado ? "Cerrar" : "Gestionar maletas"}
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      {/* Panel de gestión — solo aparece si hay un check-in seleccionado */}
      {checkinSeleccionado && (
        <div className="row g-3">

          {/* Lista de maletas del pasajero */}
          <div className="col-12 col-md-8">
            <div className="card border-0 shadow-sm rounded-4">
              <div className="card-body">
                <h5 className="fw-bold mb-3" style={{ color: "#3c3489" }}>
                  Maletas de {checkinSeleccionado.nombre_pasajero} — Check-in #{checkinSeleccionado.id_checkin}
                </h5>

                {/* Tabla de maletas */}
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
                            Este pasajero no tiene maletas registradas.
                          </td>
                        </tr>
                      ) : (
                        getMaletas(checkinSeleccionado.id_checkin).map((m, index) => {
                          const costo = costoPorMaleta(index + 1);
                          return (
                            <tr key={m.num_maleta}>
                              <td>
                                {/* Número de maleta con círculo de color */}
                                <span
                                  className="d-inline-flex align-items-center justify-content-center rounded-circle"
                                  style={{
                                    width: 24, height: 24,
                                    background: "#ede9fa",
                                    color: COLOR_PRINCIPAL,
                                    fontSize: 11, fontWeight: 700,
                                  }}
                                >
                                  {index + 1}
                                </span>
                              </td>
                              <td><code className="small">{m.num_maleta}</code></td>
                              <td>{m.peso} kg</td>
                              <td>{m.color}</td>
                              <td>
                                <span
                                  className="badge rounded-pill px-2 py-1"
                                  style={{
                                    background: costo.bg,
                                    color: costo.color,
                                    fontSize: 12,
                                  }}
                                >
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

                {/* Formulario para agregar maleta */}
                <FormAgregarMaleta onAgregar={handleAgregarMaleta} />
              </div>
            </div>
          </div>

          {/* Resumen de cobro */}
          <div className="col-12 col-md-4">
            <ResumenCobro
              maletas={getMaletas(checkinSeleccionado.id_checkin)}
            />
          </div>
        </div>
      )}
    </div>
  );
}

// FormAgregarMaleta — formulario para agregar una maleta
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
    <div
      className="pt-3"
      style={{ borderTop: "0.5px solid #e8e4f8" }}
    >
      <h6 className="fw-bold mb-3" style={{ color: "#3c3489" }}>
        Agregar maleta
      </h6>

      {error && (
        <div className="alert alert-danger py-2 small rounded-3 mb-3">
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit}>
        <div className="row g-2 mb-2">
          <div className="col-6">
            <label className="form-label fw-semibold small text-secondary">
              # Maleta <span className="text-danger">*</span>
            </label>
            <input
              type="text"
              name="num_maleta"
              className="form-control rounded-3"
              placeholder="ej. MAL-003"
              value={form.num_maleta}
              onChange={handleChange}
              required
            />
          </div>
          <div className="col-6">
            <label className="form-label fw-semibold small text-secondary">
              Peso (kg) <span className="text-danger">*</span>
            </label>
            <input
              type="number"
              name="peso"
              className="form-control rounded-3"
              placeholder="ej. 20.5"
              value={form.peso}
              onChange={handleChange}
              step="0.1"
              min="0.1"
              required
            />
          </div>
        </div>
        <div className="mb-3">
          <label className="form-label fw-semibold small text-secondary">
            Color <span className="text-danger">*</span>
          </label>
          <input
            type="text"
            name="color"
            className="form-control rounded-3"
            placeholder="ej. Negro"
            value={form.color}
            onChange={handleChange}
            required
          />
        </div>
        <button
          type="submit"
          className="btn w-100 rounded-3 fw-semibold text-white"
          style={{ background: COLOR_PRINCIPAL }}
        >
          + Agregar maleta
        </button>
      </form>
    </div>
  );
}

// ResumenCobro — muestra el desglose de costos por maleta
function ResumenCobro({ maletas }) {
  const total = calcularCostoExtra(maletas.length);

  return (
    <div className="card border-0 shadow-sm rounded-4 h-100">
      <div className="card-body">
        <h5 className="fw-bold mb-3" style={{ color: "#3c3489" }}>
          Resumen de cobro
        </h5>

        {/* Desglose por maleta */}
        {maletas.length === 0 ? (
          <p className="text-muted small">Sin maletas registradas.</p>
        ) : (
          maletas.map((m, index) => {
            const costo = costoPorMaleta(index + 1);
            return (
              <div
                key={m.num_maleta}
                className="d-flex justify-content-between py-2"
                style={{ borderBottom: "0.5px solid #f0edf8", fontSize: 13 }}
              >
                <span className="text-muted">Maleta {index + 1}</span>
                <span style={{ color: costo.color, fontWeight: 600 }}>
                  {costo.label}
                </span>
              </div>
            );
          })
        )}

        {/* Total */}
        <div
          className="d-flex justify-content-between align-items-center mt-3 p-2 rounded-3"
          style={{ background: "#f5f3ff" }}
        >
          <span className="fw-bold small">Total extra</span>
          <span className="fw-bold" style={{ color: COLOR_PRINCIPAL, fontSize: 16 }}>
            ${total.toFixed(2)}
          </span>
        </div>

        {/* Referencia de tarifas */}
        <div
          className="mt-3 p-2 rounded-3 small"
          style={{ background: "#fff3cd", color: "#856404" }}
        >
          <b>Tarifas:</b><br />
          1ra maleta — Gratis<br />
          2da maleta — $50.00<br />
          3ra en adelante — $75.00 c/u
        </div>
      </div>
    </div>
  );
}