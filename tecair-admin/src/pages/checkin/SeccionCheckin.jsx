// SeccionCheckin.jsx — Check-in de pasajeros
//
// Funcionalidad:
//   - Ver lista de check-ins filtrable por vuelo
//   - Registrar un nuevo check-in (vuelo, pasajero, asiento)
//   - Ver e imprimir el pase de abordar
//   - Enviar pase de abordar por correo

import { useState, useEffect } from "react";
import { ENDPOINTS, apiFetch } from "../../config/api";

const COLOR_PRINCIPAL = "#6d4fc2";

export default function SeccionCheckin() {
  const [checkins, setCheckins]     = useState([]);
  const [vuelos, setVuelos]         = useState([]);
  const [usuarios, setUsuarios]     = useState([]);
  const [filtroVuelo, setFiltroVuelo] = useState("todos");
  const [modalAbierto, setModalAbierto] = useState(false);
  const [checkinSeleccionado, setCheckinSeleccionado] = useState(null);
  const [cargando, setCargando]     = useState(true);
  const [mensaje, setMensaje]       = useState(null);

  const mostrarMensaje = (texto, tipo = "success") => {
    setMensaje({ texto, tipo });
    setTimeout(() => setMensaje(null), 3000);
  };

  // Carga inicial de datos
  useEffect(() => {
    cargarDatos();
  }, []);

  const cargarDatos = async () => {
    setCargando(true);
    try {
      const [dataCheckins, dataVuelos, dataUsuarios] = await Promise.all([
        apiFetch(ENDPOINTS.checkins.list),
        apiFetch(ENDPOINTS.vuelos.list),
        apiFetch(ENDPOINTS.usuarios.list),
      ]);
      setCheckins(dataCheckins.checkins ?? []);
      setVuelos(dataVuelos.vuelos ?? []);
      setUsuarios(dataUsuarios.usuarios ?? []);
    } catch (err) {
      console.log("Error:", err);
      mostrarMensaje("Error al cargar los datos.", "danger");
    } finally {
      setCargando(false);
    }
  };

  // Filtra checkins por vuelo seleccionado
  const checkinsFiltrados = filtroVuelo === "todos"
    ? checkins
    : checkins.filter((c) => c.id_vuelo === parseInt(filtroVuelo));

  // Busca los datos del vuelo de un checkin
  const getVuelo = (id_vuelo) =>
    vuelos.find((v) => v.id_vuelo === id_vuelo);

  // Busca el nombre del usuario de un checkin
  const getNombreUsuario = (id_usuario) => {
    const u = usuarios.find((u) => u.id_usuario === id_usuario);
    return u ? `${u.nombre1} ${u.apellido1}` : `Usuario #${id_usuario}`;
  };

  // ── Registrar nuevo check-in ──
  const handleNuevoCheckin = async (datos) => {
    try {
      await apiFetch(ENDPOINTS.checkins.create, "POST", datos);
      mostrarMensaje("Check-in registrado correctamente.");
      setModalAbierto(false);
      cargarDatos();
    } catch (err) {
      mostrarMensaje(err.message || "Error al registrar check-in.", "danger");
    }
  };

  // ── Enviar pase de abordar por correo ──
  const handleEnviarCorreo = () => {
    mostrarMensaje("Pase de abordar enviado por correo.");
    setCheckinSeleccionado(null);
  };

  if (cargando) {
    return (
      <div className="d-flex align-items-center justify-content-center py-5">
        <div className="spinner-border text-primary me-2"></div>
        <span className="text-muted">Cargando check-ins...</span>
      </div>
    );
  }

  return (
    <div>
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

      {mensaje && (
        <div className={`alert alert-${mensaje.tipo} py-2 small rounded-3 mb-3`}>
          {mensaje.texto}
        </div>
      )}

      <div className="card border-0 shadow-sm rounded-4 mb-4">
        <div className="card-body">
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
                {vuelos.map((v) => (
                  <option key={v.id_vuelo} value={v.id_vuelo}>
                    #{v.id_vuelo} — {v.origen} → {v.destino}
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
                  <th className="fw-semibold small text-muted">Acciones</th>
                </tr>
              </thead>
              <tbody>
                {checkinsFiltrados.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="text-center text-muted py-4 small">
                      No hay check-ins registrados.
                    </td>
                  </tr>
                ) : (
                  checkinsFiltrados.map((c) => {
                    const vuelo = getVuelo(c.id_vuelo);
                    return (
                      <tr key={c.id_checkin}>
                        <td className="fw-semibold">#{c.id_checkin}</td>
                        <td>{getNombreUsuario(c.id_usuario)}</td>
                        <td>#{c.id_vuelo} — {vuelo?.origen} → {vuelo?.destino}</td>
                        <td><b>{c.asiento}</b></td>
                        <td>
                          <div className="d-flex gap-1">
                            <button
                              className="btn btn-sm rounded-2"
                              style={{ background: "#ede9fa", color: COLOR_PRINCIPAL, fontSize: 12 }}
                              onClick={() => setCheckinSeleccionado({ ...c, nombrePasajero: getNombreUsuario(c.id_usuario) })}
                            >
                              Pase de abordar
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

      {modalAbierto && (
        <ModalNuevoCheckin
          vuelos={vuelos}
          usuarios={usuarios}
          onGuardar={handleNuevoCheckin}
          onCancelar={() => setModalAbierto(false)}
        />
      )}

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

// ModalNuevoCheckin
// ModalPaseAbordar — muestra el pase de abordar y permite generarlo como PDF
// Recibe los datos del checkin y vuelo para mostrarlos
function ModalPaseAbordar({ checkin, vuelo, onEnviarCorreo, onCerrar }) {

  // Genera el PDF del pase de abordar usando jsPDF
  const handleGenerarPDF = () => {
    // jsPDF se carga desde el CDN en index.html
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF({
      orientation: "portrait",
      unit: "mm",
      format: [100, 150], // Tamaño tipo tarjeta de embarque
    });

    // Color morado de TECAir
    const morado = [109, 79, 194];

    // Encabezado con fondo morado
    doc.setFillColor(...morado);
    doc.rect(0, 0, 100, 35, "F");

    // Título
    doc.setTextColor(255, 255, 255);
    doc.setFontSize(14);
    doc.setFont("helvetica", "bold");
    doc.text("TECAir", 10, 12);

    doc.setFontSize(9);
    doc.setFont("helvetica", "normal");
    doc.text("Pase de Abordar", 10, 19);

    // Ruta
    doc.setFontSize(13);
    doc.setFont("helvetica", "bold");
    // Divide la ruta en dos líneas si es muy larga
    const origen  = vuelo?.origen  ?? "";
    const destino = vuelo?.destino ?? "";
    doc.setFontSize(11);
    doc.text(origen,  10, 26);
    doc.text(`-> ${destino}`, 10, 33);

    // Línea divisora
    doc.setDrawColor(...morado);
    doc.setLineWidth(0.5);
    doc.line(10, 40, 90, 40);

    // Datos del pasajero y vuelo
    doc.setTextColor(80, 80, 80);
    doc.setFontSize(8);
    doc.setFont("helvetica", "normal");

    const datos = [
      ["Pasajero",    checkin.nombrePasajero ?? `Usuario #${checkin.id_usuario}`],
      ["Vuelo",       `#${checkin.id_vuelo}`],
      ["Fecha",       vuelo?.fecha_salida ?? "--"],
      ["Hora salida", vuelo?.hora_salida  ?? "--"],
      ["Puerta",      vuelo?.puerta       ?? "--"],
    ];

    let y = 50;
    datos.forEach(([etiqueta, valor]) => {
      doc.setFont("helvetica", "normal");
      doc.setTextColor(130, 130, 130);
      doc.text(etiqueta, 10, y);

      doc.setFont("helvetica", "bold");
      doc.setTextColor(50, 50, 50);
      doc.text(valor, 50, y);
      y += 9;
    });

    // Asiento destacado
    doc.setFillColor(245, 243, 255);
    doc.roundedRect(10, y + 2, 80, 25, 3, 3, "F");

    doc.setTextColor(130, 130, 130);
    doc.setFontSize(8);
    doc.setFont("helvetica", "normal");
    doc.text("Asiento", 50, y + 10, { align: "center" });

    doc.setTextColor(...morado);
    doc.setFontSize(22);
    doc.setFont("helvetica", "bold");
    doc.text(checkin.asiento ?? "--", 50, y + 22, { align: "center" });

    // Footer
    doc.setTextColor(180, 180, 180);
    doc.setFontSize(7);
    doc.setFont("helvetica", "normal");
    doc.text("TECAir — Instituto Tecnologico de Costa Rica", 50, 145, { align: "center" });

    // Descarga el PDF
    doc.save(`pase-abordar-${checkin.id_checkin}.pdf`);
  };

  return (
    <div
      className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center"
      style={{ background: "rgba(0,0,0,0.4)", zIndex: 1000 }}
    >
      <div className="card border-0 shadow rounded-4" style={{ width: "100%", maxWidth: 360 }}>
        <div className="card-body p-4">
          {/* Encabezado del pase */}
          <div className="rounded-3 p-3 mb-4" style={{ background: "#6d4fc2" }}>
            <p style={{ color: "rgba(255,255,255,0.7)", fontSize: 11, marginBottom: 4 }}>
              TECAir — Pase de abordar
            </p>
            <h3 className="fw-bold text-white mb-0" style={{ fontSize: 18 }}>
              {vuelo?.origen ?? ""} → {vuelo?.destino ?? ""}
            </h3>
          </div>

          {/* Datos del pasajero */}
          <div className="d-flex justify-content-between mb-3">
            <div>
              <p className="text-muted mb-1" style={{ fontSize: 11 }}>Pasajero</p>
              <p className="fw-bold mb-0" style={{ fontSize: 14, color: "#3c3489" }}>
                {checkin.nombrePasajero ?? `Usuario #${checkin.id_usuario}`}
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
                {vuelo?.hora_salida ?? "--"}
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
            <p className="fw-bold mb-0" style={{ fontSize: 36, color: "#6d4fc2" }}>
              {checkin.asiento}
            </p>
          </div>

          {/* Botones de acción */}
          <div className="d-flex gap-2 mb-2">
            {/* Genera y descarga el PDF */}
            <button
              className="btn flex-fill rounded-3 fw-semibold"
              style={{ background: "#f5f3ff", color: "#6d4fc2", fontSize: 13 }}
              onClick={handleGenerarPDF}
            >
              Descargar PDF
            </button>
            {/* Simula envío por correo */}
            <button
              className="btn flex-fill rounded-3 fw-semibold text-white"
              style={{ background: "#6d4fc2", fontSize: 13 }}
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