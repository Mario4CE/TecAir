// SeccionCheckin.jsx — Check-in de pasajeros
//
// Funcionalidad:
//   - Ver lista de check-ins filtrable por vuelo
//   - Registrar un nuevo check-in (vuelo, pasajero, asiento)
//   - Ver e imprimir el pase de abordar
//   - Enviar pase de abordar por correo

import { useState } from "react";
import { MOCK_CHECKINS, MOCK_VUELOS } from "../../config/mockData";

const COLOR_PRINCIPAL = "#6d4fc2";

// Pasajeros de prueba con reservación confirmada
// Cuando el API esté lista, estos vendrán filtrados por vuelo
const MOCK_PASAJEROS = [
  { id_usuario: 2, nombre: "María González" },
  { id_usuario: 3, nombre: "Luis Pérez"     },
];

export default function SeccionCheckin() {
  // Lista de check-ins registrados
  const [checkins, setCheckins] = useState(MOCK_CHECKINS);

  // Vuelo seleccionado en el filtro — "todos" muestra todos
  const [filtroVuelo, setFiltroVuelo] = useState("todos");

  // Controla si el modal de nuevo check-in está abierto
  const [modalAbierto, setModalAbierto] = useState(false);

  // Check-in seleccionado para mostrar el pase de abordar
  // null = no se muestra el pase
  const [checkinSeleccionado, setCheckinSeleccionado] = useState(null);

  // Mensaje de éxito o error temporal
  const [mensaje, setMensaje] = useState(null);

  const mostrarMensaje = (texto, tipo = "success") => {
    setMensaje({ texto, tipo });
    setTimeout(() => setMensaje(null), 3000);
  };

  // Filtra los check-ins según el vuelo seleccionado
  const checkinsFiltrados =
    filtroVuelo === "todos"
      ? checkins
      : checkins.filter((c) => c.id_vuelo === parseInt(filtroVuelo));

  // Busca los datos del vuelo de un check-in para mostrarlos en la tabla
  const getVuelo = (id_vuelo) =>
    MOCK_VUELOS.find((v) => v.id_vuelo === id_vuelo);

  // Registrar nuevo check-in 
  const handleNuevoCheckin = (datos) => {
    const nuevoCheckin = {
      id_checkin: checkins.length + 1,
      ...datos,
      maletas: 0, // inicia sin maletas
    };
    setCheckins((prev) => [...prev, nuevoCheckin]);
    setModalAbierto(false);
    mostrarMensaje("Check-in registrado correctamente.");
  };

  // Simular envío de correo 
  const handleEnviarCorreo = () => {
    mostrarMensaje("Pase de abordar enviado por correo.");
    setCheckinSeleccionado(null);
  };

  return (
    <div>
      {/*Encabezado*/}
      <div className="d-flex align-items-center justify-content-between mb-4">
        <h2 className="fw-bold mb-0" style={{ color: "#3c3489" }}>
          Check-in de pasajeros
        </h2>
        <button
          className="btn fw-semibold text-white rounded-3"
          style={{ background: COLOR_PRINCIPAL }}
          onClick={() => setModalAbierto(true)}
        >
          + Nuevo check-in
        </button>
      </div>

      {/* Mensaje de éxito o error */}
      {mensaje && (
        <div className={`alert alert-${mensaje.tipo} py-2 small rounded-3 mb-3`}>
          {mensaje.texto}
        </div>
      )}

      {/* Tabla de check-ins*/}
      <div className="card border-0 shadow-sm rounded-4 mb-4">
        <div className="card-body">

          {/* Filtro por vuelo */}
          <div className="d-flex align-items-center gap-3 mb-3">
            <h5 className="fw-bold mb-0" style={{ color: "#3c3489" }}>
              Pasajeros chequeados
            </h5>
            <div className="d-flex align-items-center gap-2 ms-auto">
              <label className="fw-semibold small text-secondary mb-0">
                Filtrar por vuelo:
              </label>
              <select
                className="form-select form-select-sm rounded-3"
                style={{ width: "auto" }}
                value={filtroVuelo}
                onChange={(e) => setFiltroVuelo(e.target.value)}
              >
                <option value="todos">Todos los vuelos</option>
                {MOCK_VUELOS.map((v) => (
                  <option key={v.id_vuelo} value={v.id_vuelo}>
                    #{v.id_vuelo} — {v.ruta} — {new Date(v.fecha_salida).toLocaleTimeString("es-CR", { hour: "2-digit", minute: "2-digit" })}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead>
                <tr className="table-light">
                  <th className="fw-semibold small text-muted"># Check-in</th>
                  <th className="fw-semibold small text-muted">Pasajero</th>
                  <th className="fw-semibold small text-muted">Vuelo</th>
                  <th className="fw-semibold small text-muted">Asiento</th>
                  <th className="fw-semibold small text-muted">Maletas</th>
                  <th className="fw-semibold small text-muted">Acciones</th>
                </tr>
              </thead>
              <tbody>
                {checkinsFiltrados.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="text-center text-muted py-4 small">
                      No hay check-ins registrados para este vuelo.
                    </td>
                  </tr>
                ) : (
                  checkinsFiltrados.map((c) => {
                    const vuelo = getVuelo(c.id_vuelo);
                    return (
                      <tr key={c.id_checkin}>
                        <td className="fw-semibold">#{c.id_checkin}</td>
                        <td>{c.nombre_pasajero}</td>
                        <td>#{c.id_vuelo} — {vuelo?.ruta ?? "Sin ruta"}</td>
                        <td><b>{c.asiento}</b></td>
                        <td>{c.maletas}</td>
                        <td>
                          <div className="d-flex gap-1">
                            {/* Ver pase de abordar */}
                            <button
                              className="btn btn-sm rounded-2"
                              style={{ background: "#ede9fa", color: COLOR_PRINCIPAL, fontSize: 12 }}
                              onClick={() => setCheckinSeleccionado(c)}
                            >
                              Pase de abordar
                            </button>
                            {/* Botón de maletas — navega a la sección */}
                            <button
                              className="btn btn-sm rounded-2"
                              style={{ background: "#fff3cd", color: "#856404", fontSize: 12 }}
                            >
                              Maletas
                            </button>
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

      {/* ── Modal nuevo check-in ── */}
      {modalAbierto && (
        <ModalNuevoCheckin
          onGuardar={handleNuevoCheckin}
          onCancelar={() => setModalAbierto(false)}
        />
      )}

      {/* ── Modal pase de abordar ── */}
      {checkinSeleccionado && (
        <ModalPaseAbordar
          checkin={checkinSeleccionado}
          vuelo={getVuelo(checkinSeleccionado.id_vuelo)}
          onImprimir={() => window.print()}
          onEnviarCorreo={handleEnviarCorreo}
          onCerrar={() => setCheckinSeleccionado(null)}
        />
      )}
    </div>
  );
}

// ModalNuevoCheckin — formulario para registrar un check-in
function ModalNuevoCheckin({ onGuardar, onCancelar }) {
  const [form, setForm] = useState({
    id_vuelo:   "",
    id_usuario: "",
    asiento:    "",
  });
  const [error, setError] = useState("");

  const handleChange = (e) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!form.id_vuelo || !form.id_usuario || !form.asiento) {
      setError("Todos los campos son obligatorios.");
      return;
    }

    // Busca el nombre del pasajero seleccionado
    const pasajero = MOCK_PASAJEROS.find(
      (p) => p.id_usuario === parseInt(form.id_usuario)
    );

    onGuardar({
      id_vuelo:        parseInt(form.id_vuelo),
      id_usuario:      parseInt(form.id_usuario),
      nombre_pasajero: pasajero?.nombre ?? "Sin nombre",
      asiento:         form.asiento.toUpperCase(),
    });
  };

  return (
    <div
      className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center"
      style={{ background: "rgba(0,0,0,0.4)", zIndex: 1000 }}
    >
      <div className="card border-0 shadow rounded-4" style={{ width: "100%", maxWidth: 420 }}>
        <div className="card-body p-4">
          <h5 className="fw-bold mb-1" style={{ color: "#3c3489" }}>
            Nuevo check-in
          </h5>
          <p className="text-muted small mb-4">
            Registre el check-in del pasajero
          </p>

          {error && (
            <div className="alert alert-danger py-2 small rounded-3">{error}</div>
          )}

          <form onSubmit={handleSubmit}>
            {/* Selector de vuelo */}
            <div className="mb-3">
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
                {/* Solo muestra vuelos abiertos para hacer check-in */}
                {MOCK_VUELOS.filter((v) => v.estado === "abierto").map((v) => (
                  <option key={v.id_vuelo} value={v.id_vuelo}>
                    #{v.id_vuelo} — {v.ruta} — {new Date(v.fecha_salida).toLocaleTimeString("es-CR", { hour: "2-digit", minute: "2-digit" })}
                  </option>
                ))}
              </select>
            </div>

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
                {MOCK_PASAJEROS.map((p) => (
                  <option key={p.id_usuario} value={p.id_usuario}>
                    {p.nombre}
                  </option>
                ))}
              </select>
            </div>

            {/* Asiento */}
            <div className="mb-4">
              <label className="form-label fw-semibold small text-secondary">
                Asiento <span className="text-danger">*</span>
              </label>
              <input
                type="text"
                name="asiento"
                className="form-control rounded-3"
                placeholder="ej. 12A"
                value={form.asiento}
                onChange={handleChange}
                required
              />
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
                Registrar
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}


// ModalPaseAbordar — muestra el pase de abordar
//
// Recibe:
//   checkin        → datos del check-in
//   vuelo          → datos del vuelo
//   onImprimir     → función para imprimir
//   onEnviarCorreo → función para enviar por correo
//   onCerrar       → función para cerrar el modal

function ModalPaseAbordar({ checkin, vuelo, onImprimir, onEnviarCorreo, onCerrar }) {
  return (
    <div
      className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center"
      style={{ background: "rgba(0,0,0,0.4)", zIndex: 1000 }}
    >
      <div className="card border-0 shadow rounded-4" style={{ width: "100%", maxWidth: 360 }}>
        <div className="card-body p-4">

          {/* Encabezado del pase */}
          <div
            className="rounded-3 p-3 mb-4"
            style={{ background: COLOR_PRINCIPAL }}
          >
            <p style={{ color: "rgba(255,255,255,0.7)", fontSize: 11, marginBottom: 4 }}>
              TECAir — Pase de abordar
            </p>
            <h3 className="fw-bold text-white mb-0" style={{ fontSize: 18 }}>
              {vuelo?.ruta ?? "Sin ruta"}
            </h3>
          </div>

          {/* Datos del pasajero y vuelo */}
          <div className="d-flex justify-content-between mb-3">
            <div>
              <p className="text-muted mb-1" style={{ fontSize: 11 }}>Pasajero</p>
              <p className="fw-bold mb-0" style={{ fontSize: 14, color: "#3c3489" }}>
                {checkin.nombre_pasajero}
              </p>
            </div>
            <div className="text-end">
              <p className="text-muted mb-1" style={{ fontSize: 11 }}>Vuelo</p>
              <p className="fw-bold mb-0" style={{ fontSize: 14, color: "#3c3489" }}>
                #{checkin.id_vuelo}
              </p>
            </div>
          </div>

          <div className="d-flex justify-content-between mb-3">
            <div>
              <p className="text-muted mb-1" style={{ fontSize: 11 }}>Hora salida</p>
              <p className="fw-bold mb-0" style={{ fontSize: 14, color: "#3c3489" }}>
                {vuelo ? new Date(vuelo.fecha_salida).toLocaleTimeString("es-CR", { hour: "2-digit", minute: "2-digit" }) : "--"}
              </p>
            </div>
            <div className="text-end">
              <p className="text-muted mb-1" style={{ fontSize: 11 }}>Puerta</p>
              <p className="fw-bold mb-0" style={{ fontSize: 14, color: "#3c3489" }}>
                {vuelo?.puerta ?? "--"}
              </p>
            </div>
          </div>

          {/* Asiento destacado */}
          <div
            className="text-center rounded-3 py-3 mb-4"
            style={{ background: "#f5f3ff" }}
          >
            <p className="text-muted mb-1" style={{ fontSize: 11 }}>Asiento</p>
            <p className="fw-bold mb-0" style={{ fontSize: 36, color: COLOR_PRINCIPAL }}>
              {checkin.asiento}
            </p>
          </div>

          {/* Botones de acción */}
          <div className="d-flex gap-2 mb-2">
            <button
              className="btn flex-fill rounded-3 fw-semibold"
              style={{ background: "#f5f3ff", color: COLOR_PRINCIPAL, fontSize: 13 }}
              onClick={onImprimir}
            >
              Imprimir
            </button>
            <button
              className="btn flex-fill rounded-3 fw-semibold text-white"
              style={{ background: COLOR_PRINCIPAL, fontSize: 13 }}
              onClick={onEnviarCorreo}
            >
              Enviar correo
            </button>
          </div>
          <button
            className="btn w-100 rounded-3 fw-semibold text-muted"
            style={{ background: "#f5f3ff", fontSize: 13 }}
            onClick={onCerrar}
          >
            Cerrar
          </button>
        </div>
      </div>
    </div>
  );
}