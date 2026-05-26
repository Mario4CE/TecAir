// Dashboard.jsx — Panel principal de la Vista Aeropuerto
//
// Este archivo contiene toda la pantalla del admin:
//   - Navbar arriba con el nombre y navegación
//   - Tarjetas de resumen del día con datos reales del API
//   - Tabla de vuelos con acciones

import { useState, useEffect } from "react";
import { ENDPOINTS, apiFetch } from "../config/api";
import SeccionVuelos from "./vuelos/SeccionVuelos";
import SeccionCheckin from "./checkin/SeccionCheckin";
import SeccionMaletas from "./maletas/SeccionMaletas";
import SeccionPromociones from "./promociones/SeccionPromociones";
import SeccionUsuarios from "./usuarios/SeccionUsuarios";

const COLOR_PRINCIPAL = "#6d4fc2";
const COLOR_FONDO = "#f5f3ff";

// Lista de secciones del menú de navegación
// Para agregar una nueva sección, solo se agrega un objeto aquí
const NAV_ITEMS = [
  { id: "inicio",      etiqueta: "Inicio"       },
  { id: "vuelos",      etiqueta: "Vuelos"        },
  { id: "checkin",     etiqueta: "Check-in"      },
  { id: "maletas",     etiqueta: "Maletas"       },
  { id: "promociones", etiqueta: "Promociones"   },
  { id: "usuarios",    etiqueta: "Usuarios"      },
];

// Componente principal — lo que exportamos y usa App.jsx
// Recibe: usuario (datos del admin) y onLogout (función para cerrar sesión)
export default function Dashboard({ usuario, onLogout }) {
  // useState guarda qué sección está activa en el menú
  const [seccionActiva, setSeccionActiva] = useState("inicio");

  return (
    <div style={{ minHeight: "100vh", background: COLOR_FONDO }}>

      {/* Navbar superior */}
      <nav style={{ background: COLOR_PRINCIPAL }}>

        {/* Fila 1: nombre de la app y datos del usuario */}
        <div
          className="d-flex align-items-center justify-content-between px-4 py-2"
          style={{ borderBottom: "1px solid rgba(255,255,255,0.12)" }}
        >
          <span className="fw-bold text-white" style={{ fontSize: 16 }}>
            TECAir — Portal de Administración
          </span>

          <div className="d-flex align-items-center gap-2">
            {/* Círculo con la inicial del nombre */}
            <div
              className="d-flex align-items-center justify-content-center rounded-circle"
              style={{
                width: 28, height: 28,
                background: "rgba(255,255,255,0.2)",
                color: "#fff", fontSize: 11, fontWeight: 600,
              }}
            >
              {usuario?.nombre1?.[0] ?? "A"}
            </div>
            <span style={{ color: "rgba(255,255,255,0.85)", fontSize: 12 }}>
              {usuario?.nombre1} {usuario?.apellido1}
            </span>
            <button
              className="btn btn-sm ms-2 text-white"
              style={{
                background: "rgba(255,255,255,0.15)",
                border: "none", fontSize: 11, borderRadius: 6,
              }}
              onClick={onLogout}
            >
              Salir
            </button>
          </div>
        </div>

        {/* Fila 2: links de navegación */}
        <div className="d-flex align-items-center gap-1 px-4 py-1">
          {NAV_ITEMS.map((item) => (
            <button
              key={item.id}
              onClick={() => setSeccionActiva(item.id)}
              style={{
                background: seccionActiva === item.id
                  ? "rgba(255,255,255,0.15)"
                  : "transparent",
                color: seccionActiva === item.id
                  ? "#fff"
                  : "rgba(255,255,255,0.65)",
                border: "none",
                borderRadius: 6,
                padding: "5px 12px",
                fontSize: 12,
                fontWeight: 500,
                cursor: "pointer",
              }}
            >
              {item.etiqueta}
            </button>
          ))}
        </div>
      </nav>

      {/* Contenido principal */}
      <main className="p-4">
        <ContenidoSeccion
          seccion={seccionActiva}
          usuario={usuario}
          onNavegar={setSeccionActiva}
        />
      </main>
    </div>
  );
}

// ContenidoSeccion — decide qué pantalla mostrar según la sección activa
function ContenidoSeccion({ seccion, usuario, onNavegar }) {
  switch (seccion) {
    case "inicio":
      return <SeccionInicio usuario={usuario} onNavegar={onNavegar} />;
    case "vuelos":
      return <SeccionVuelos usuario={usuario} onNavegar={onNavegar} />;
    case "checkin":
      return <SeccionCheckin usuario={usuario} onNavegar={onNavegar} />;
    case "maletas":
      return <SeccionMaletas usuario={usuario} onNavegar={onNavegar} />;
    case "promociones":
      return <SeccionPromociones usuario={usuario} onNavegar={onNavegar} />;
    case "usuarios":
      return <SeccionUsuarios />;
    default:
      return null;
  }
}

// SeccionInicio — pantalla de inicio con tarjetas y tabla de vuelos
// Carga los datos reales del API al montar el componente
function SeccionInicio({ usuario, onNavegar }) {
  // Estado para las estadísticas del dashboard
  const [stats, setStats] = useState({
    vuelos_hoy:          0,
    pasajeros_hoy:       0,
    checkins_pendientes: 0,
    vuelos_cerrados:     0,
  });

  // Lista de vuelos para la tabla
  const [vuelos, setVuelos] = useState([]);

  // Controla el spinner de carga
  const [cargando, setCargando] = useState(true);

  // Se ejecuta una vez al cargar el componente
  useEffect(() => {
    const cargarDatos = async () => {
      try {
        // Carga vuelos y checkins en paralelo
        const [dataVuelos, dataCheckins] = await Promise.all([
          apiFetch(ENDPOINTS.vuelos.list),
          apiFetch(ENDPOINTS.checkins.list),
        ]);

        const listaVuelos   = dataVuelos.vuelos     ?? [];
        const listaCheckins = dataCheckins.checkins ?? [];

        setVuelos(listaVuelos);

        // Calcula las estadísticas a partir de los datos reales
        setStats({
          vuelos_hoy:          listaVuelos.length,
          pasajeros_hoy:       listaCheckins.length,
          checkins_pendientes: listaCheckins.length,
          vuelos_cerrados:     listaVuelos.filter((v) => v.estado === "cerrado").length,
        });
      } catch (err) {
        console.log("Error cargando datos del inicio:", err);
      } finally {
        setCargando(false);
      }
    };
    cargarDatos();
  }, []);

  // Configuración visual de cada estado posible de un vuelo
  const estadoConfig = {
    abierto:    { color: "#198754", bg: "#d1e7dd", label: "Abierto"    },
    programado: { color: "#856404", bg: "#fff3cd", label: "Programado" },
    pendiente:  { color: "#856404", bg: "#fff3cd", label: "Pendiente"  },
    cerrado:    { color: "#495057", bg: "#e2e3e5", label: "Cerrado"    },
  };

  // Definición de las tarjetas de resumen
  const tarjetas = [
    {
      label: "Vuelos hoy",
      valor: stats.vuelos_hoy,
      color: COLOR_PRINCIPAL,
      bg: "#ede9fa",
      seccion: "vuelos",
    },
    {
      label: "Pasajeros hoy",
      valor: stats.pasajeros_hoy,
      color: "#0d6efd",
      bg: "#cfe2ff",
      seccion: "checkin",
    },
    {
      label: "Check-ins pendientes",
      valor: stats.checkins_pendientes,
      color: "#856404",
      bg: "#fff3cd",
      seccion: "checkin",
    },
    {
      label: "Vuelos cerrados",
      valor: stats.vuelos_cerrados,
      color: "#495057",
      bg: "#e2e3e5",
      seccion: "vuelos",
    },
  ];

  if (cargando) {
    return (
      <div className="d-flex align-items-center justify-content-center py-5">
        <div className="spinner-border text-primary me-2"></div>
        <span className="text-muted">Cargando...</span>
      </div>
    );
  }

  return (
    <>
      {/* Saludo con nombre del usuario y fecha actual */}
      <div className="mb-4">
        <h2 className="fw-bold mb-0" style={{ color: "#3c3489" }}>
          Bienvenido, {usuario?.nombre1}
        </h2>
        <p className="text-muted small mb-0">
          {new Date().toLocaleDateString("es-CR", {
            weekday: "long", year: "numeric",
            month: "long",   day: "numeric",
          })}
        </p>
      </div>

      {/* Tarjetas de resumen con datos reales */}
      <div className="row g-3 mb-4">
        {tarjetas.map((card) => (
          <div key={card.label} className="col-6 col-md-3">
            <button
              className="card border-0 shadow-sm rounded-4 w-100 text-start h-100"
              style={{ cursor: "pointer", background: "#fff" }}
              onClick={() => onNavegar(card.seccion)}
            >
              <div className="card-body">
                <p className="text-muted small mb-1">{card.label}</p>
                <p className="fw-bold mb-0 fs-4" style={{ color: card.color }}>
                  {card.valor}
                </p>
              </div>
            </button>
          </div>
        ))}
      </div>

      {/* Tabla de vuelos con datos reales */}
      <div className="card border-0 shadow-sm rounded-4">
        <div className="card-body">
          <div className="d-flex align-items-center justify-content-between mb-3">
            <h5 className="fw-bold mb-0" style={{ color: "#3c3489" }}>
              Vuelos del día
            </h5>
            <button
              className="btn btn-sm rounded-3 fw-semibold text-white"
              style={{ background: COLOR_PRINCIPAL, fontSize: 13 }}
              onClick={() => onNavegar("vuelos")}
            >
              Ver todos
            </button>
          </div>

          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead>
                <tr className="table-light">
                  <th className="fw-semibold small text-muted"># Vuelo</th>
                  <th className="fw-semibold small text-muted">Ruta</th>
                  <th className="fw-semibold small text-muted">Salida</th>
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
                    const cfg = estadoConfig[v.estado] ?? estadoConfig.programado;
                    return (
                      <tr key={v.id_vuelo}>
                        <td className="fw-semibold">#{v.id_vuelo}</td>
                        <td>{v.origen} → {v.destino}</td>
                        <td>{v.hora_salida}</td>
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
                            <button
                              className="btn btn-sm rounded-2"
                              style={{ background: "#ede9fa", color: COLOR_PRINCIPAL, fontSize: 12 }}
                              onClick={() => onNavegar("checkin")}
                            >
                              Check-in
                            </button>
                            {(v.estado === "pendiente" || v.estado === "programado") && (
                              <button
                                className="btn btn-sm rounded-2"
                                style={{ background: "#d1e7dd", color: "#198754", fontSize: 12 }}
                              >
                                Abrir
                              </button>
                            )}
                            {v.estado === "abierto" && (
                              <button
                                className="btn btn-sm rounded-2"
                                style={{ background: "#f8d7da", color: "#842029", fontSize: 12 }}
                              >
                                Cerrar
                              </button>
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
    </>
  );
}