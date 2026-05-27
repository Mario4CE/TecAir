// SeccionUsuarios.jsx — Gestión de usuarios conectada al API
//
// Funcionalidad:
//   - Ver lista de usuarios con búsqueda y filtro
//   - Crear nuevo usuario
//   - Editar usuario existente
//   - Eliminar usuario
//   - El admin principal (id 1) no se puede editar ni eliminar

import { useState, useEffect } from "react";
import { ENDPOINTS, apiFetch } from "../../config/api";

const COLOR_PRINCIPAL = "#6d4fc2";

// Devuelve la configuración visual según el tipo de usuario
function getTipoConfig(usuario) {
  if (usuario.es_admin)      return { label: "Admin",      bg: "#ede9fa", color: "#6d4fc2" };
  if (usuario.es_estudiante) return { label: "Estudiante", bg: "#cfe2ff", color: "#0d6efd" };
  return                            { label: "Cliente",    bg: "#e2e3e5", color: "#495057" };
}

export default function SeccionUsuarios() {
  // Lista de usuarios del API
  const [usuarios, setUsuarios] = useState([]);

  // Texto de búsqueda por nombre o correo
  const [busqueda, setBusqueda] = useState("");

  // Filtro por tipo: "todos" | "admin" | "cliente" | "estudiante"
  const [filtroTipo, setFiltroTipo] = useState("todos");

  // Controla si el modal está abierto
  const [modalAbierto, setModalAbierto] = useState(false);

  // Usuario seleccionado para editar — null = modo creación
  const [usuarioEditando, setUsuarioEditando] = useState(null);

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
    cargarUsuarios();
  }, []);

  const cargarUsuarios = async () => {
    setCargando(true);
    try {
      const data = await apiFetch(ENDPOINTS.usuarios.list);
      setUsuarios(data.usuarios ?? []);
    } catch (err) {
      console.log("Error:", err);
      mostrarMensaje("Error al cargar usuarios.", "danger");
    } finally {
      setCargando(false);
    }
  };

  // Filtra usuarios según búsqueda y tipo seleccionado
  const usuariosFiltrados = usuarios.filter((u) => {
    const nombreCompleto = `${u.nombre1} ${u.nombre2} ${u.apellido1} ${u.apellido2}`.toLowerCase();
    const coincideBusqueda =
      nombreCompleto.includes(busqueda.toLowerCase()) ||
      u.correo.toLowerCase().includes(busqueda.toLowerCase());

    const coincideTipo =
      filtroTipo === "todos"      ? true :
      filtroTipo === "admin"      ? u.es_admin :
      filtroTipo === "estudiante" ? u.es_estudiante :
      filtroTipo === "cliente"    ? (!u.es_admin && !u.es_estudiante) :
      true;

    return coincideBusqueda && coincideTipo;
  });

  // Abre el modal en modo creación
  const handleNuevo = () => {
    setUsuarioEditando(null);
    setModalAbierto(true);
  };

  // Abre el modal en modo edición con los datos del usuario
  const handleEditar = (usuario) => {
    setUsuarioEditando(usuario);
    setModalAbierto(true);
  };

  // Elimina un usuario — pide confirmación primero
  const handleEliminar = async (id_usuario) => {
    if (!window.confirm("¿Está seguro que desea eliminar este usuario?")) return;
    try {
      await apiFetch(ENDPOINTS.usuarios.delete(id_usuario), "DELETE");
      mostrarMensaje("Usuario eliminado correctamente.");
      cargarUsuarios();
    } catch (err) {
      mostrarMensaje(err.message || "Error al eliminar usuario.", "danger");
    }
  };

  // Guarda un usuario nuevo o editado
  const handleGuardar = async (datos) => {
    try {
      if (usuarioEditando) {
        // Modo edición: llama al endpoint PUT /usuarios/{id}
        await apiFetch(ENDPOINTS.usuarios.update(usuarioEditando.id_usuario), "PUT", datos);
        mostrarMensaje("Usuario actualizado correctamente.");
      } else {
        // Modo creación: llama al endpoint POST /usuarios
        await apiFetch(ENDPOINTS.usuarios.create, "POST", datos);
        mostrarMensaje("Usuario creado correctamente.");
      }
      setModalAbierto(false);
      setUsuarioEditando(null);
      cargarUsuarios();
    } catch (err) {
      mostrarMensaje(err.message || "Error al guardar usuario.", "danger");
    }
  };

  if (cargando) {
    return (
      <div className="d-flex align-items-center justify-content-center py-5">
        <div className="spinner-border text-primary me-2"></div>
        <span className="text-muted">Cargando usuarios...</span>
      </div>
    );
  }

  return (
    <div>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <h2 className="fw-bold mb-0" style={{ color: "#3c3489" }}>
          Usuarios
        </h2>
        <button
          className="btn fw-semibold text-white rounded-3"
          style={{ background: COLOR_PRINCIPAL }}
          onClick={handleNuevo}
        >
          + Nuevo usuario
        </button>
      </div>

      {mensaje && (
        <div className={`alert alert-${mensaje.tipo} py-2 small rounded-3 mb-3`}>
          {mensaje.texto}
        </div>
      )}

      <div className="card border-0 shadow-sm rounded-4">
        <div className="card-body">
          {/* Búsqueda y filtro por tipo */}
          <div className="d-flex align-items-center gap-2 mb-3">
            <input
              type="text"
              className="form-control rounded-3"
              style={{ maxWidth: 260 }}
              placeholder="Buscar por nombre o correo..."
              value={busqueda}
              onChange={(e) => setBusqueda(e.target.value)}
            />
            <select
              className="form-select rounded-3"
              style={{ width: "auto" }}
              value={filtroTipo}
              onChange={(e) => setFiltroTipo(e.target.value)}
            >
              <option value="todos">Todos</option>
              <option value="admin">Admins</option>
              <option value="cliente">Clientes</option>
              <option value="estudiante">Estudiantes</option>
            </select>
            {/* Contador de resultados */}
            <span className="text-muted small ms-auto">
              {usuariosFiltrados.length} usuario{usuariosFiltrados.length !== 1 ? "s" : ""}
            </span>
          </div>

          <div className="table-responsive">
            <table className="table table-hover align-middle mb-0">
              <thead>
                <tr className="table-light">
                  <th className="fw-semibold small text-muted">Usuario</th>
                  <th className="fw-semibold small text-muted">Correo</th>
                  <th className="fw-semibold small text-muted">Teléfono</th>
                  <th className="fw-semibold small text-muted">Tipo</th>
                  <th className="fw-semibold small text-muted">Acciones</th>
                </tr>
              </thead>
              <tbody>
                {usuariosFiltrados.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="text-center text-muted py-4 small">
                      No se encontraron usuarios.
                    </td>
                  </tr>
                ) : (
                  usuariosFiltrados.map((u) => {
                    const tipo = getTipoConfig(u);
                    // El primer usuario no se puede eliminar para evitar accidentes
                    const esAdminPrincipal = u.id_usuario === 1;

                    return (
                      <tr key={u.id_usuario}>
                        <td>
                          {/* Avatar con la inicial del nombre */}
                          <span
                            className="d-inline-flex align-items-center justify-content-center rounded-circle me-2"
                            style={{ width: 28, height: 28, background: tipo.bg, color: tipo.color, fontSize: 11, fontWeight: 700 }}>
                            {u.nombre1?.[0] ?? "?"}
                          </span>
                          {u.nombre1} {u.apellido1}
                        </td>
                        <td>{u.correo}</td>
                        <td>{u.telefono}</td>
                        <td>
                          <span className="badge rounded-pill px-2 py-1"
                            style={{ background: tipo.bg, color: tipo.color, fontSize: 11 }}>
                            {tipo.label}
                          </span>
                        </td>
                        <td>
                          {esAdminPrincipal ? (
                            <span className="text-muted small">Sin acciones</span>
                          ) : (
                            <div className="d-flex gap-1">
                              <button
                                className="btn btn-sm rounded-2"
                                style={{ background: "#ede9fa", color: COLOR_PRINCIPAL, fontSize: 12 }}
                                onClick={() => handleEditar(u)}
                              >
                                Editar
                              </button>
                              <button
                                className="btn btn-sm rounded-2"
                                style={{ background: "#f8d7da", color: "#842029", fontSize: 12 }}
                                onClick={() => handleEliminar(u.id_usuario)}
                              >
                                Eliminar
                              </button>
                            </div>
                          )}
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
        <ModalUsuario
          usuario={usuarioEditando}
          onGuardar={handleGuardar}
          onCancelar={() => {
            setModalAbierto(false);
            setUsuarioEditando(null);
          }}
        />
      )}
    </div>
  );
}

// ModalUsuario — formulario para crear o editar un usuario
// Si recibe usuario → modo edición (precarga los datos)
// Si usuario es null → modo creación (formulario vacío)
function ModalUsuario({ usuario, onGuardar, onCancelar }) {
  const [form, setForm] = useState({
    nombre1:       usuario?.nombre1       ?? "",
    nombre2:       usuario?.nombre2       ?? "",
    apellido1:     usuario?.apellido1     ?? "",
    apellido2:     usuario?.apellido2     ?? "",
    telefono:      usuario?.telefono      ?? "",
    correo:        usuario?.correo        ?? "",
    es_estudiante: usuario?.es_estudiante ?? false,
    universidad:   usuario?.universidad   ?? "",
    carnet:        usuario?.carnet        ?? "",
  });
  const [error, setError] = useState("");

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setForm({ ...form, [name]: type === "checkbox" ? checked : value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    setError("");
    if (!form.nombre1 || !form.apellido1 || !form.correo || !form.telefono) {
      setError("Los campos marcados con * son obligatorios.");
      return;
    }
    onGuardar(form);
  };

  return (
    <div
      className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center"
      style={{ background: "rgba(0,0,0,0.4)", zIndex: 1000 }}
    >
      <div className="card border-0 shadow rounded-4" style={{ width: "100%", maxWidth: 460 }}>
        <div className="card-body p-4">
          <h5 className="fw-bold mb-1" style={{ color: "#3c3489" }}>
            {usuario ? "Editar usuario" : "Nuevo usuario"}
          </h5>
          <p className="text-muted small mb-4"
            style={{ borderBottom: "0.5px solid #e8e4f8", paddingBottom: 12 }}>
            {usuario ? "Modifique los datos del usuario" : "Complete los datos del nuevo usuario"}
          </p>

          {error && (
            <div className="alert alert-danger py-2 small rounded-3">{error}</div>
          )}

          <form onSubmit={handleSubmit}>
            <div className="row g-2 mb-3">
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Primer nombre <span className="text-danger">*</span>
                </label>
                <input type="text" name="nombre1" className="form-control rounded-3"
                  placeholder="ej. Juan" value={form.nombre1} onChange={handleChange} required />
              </div>
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Segundo nombre
                </label>
                <input type="text" name="nombre2" className="form-control rounded-3"
                  placeholder="ej. Carlos" value={form.nombre2} onChange={handleChange} />
              </div>
            </div>

            <div className="row g-2 mb-3">
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Primer apellido <span className="text-danger">*</span>
                </label>
                <input type="text" name="apellido1" className="form-control rounded-3"
                  placeholder="ej. Pérez" value={form.apellido1} onChange={handleChange} required />
              </div>
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Segundo apellido
                </label>
                <input type="text" name="apellido2" className="form-control rounded-3"
                  placeholder="ej. Mora" value={form.apellido2} onChange={handleChange} />
              </div>
            </div>

            <div className="row g-2 mb-3">
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Correo <span className="text-danger">*</span>
                </label>
                <input type="email" name="correo" className="form-control rounded-3"
                  placeholder="usuario@correo.com" value={form.correo}
                  onChange={handleChange} required />
              </div>
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Teléfono <span className="text-danger">*</span>
                </label>
                <input type="tel" name="telefono" className="form-control rounded-3"
                  placeholder="8888-8888" value={form.telefono}
                  onChange={handleChange} required />
              </div>
            </div>

            {/* Checkbox de estudiante */}
            <div className="mb-3">
              <div className="form-check">
                <input className="form-check-input" type="checkbox"
                  name="es_estudiante" id="esEstudiante"
                  checked={form.es_estudiante} onChange={handleChange} />
                <label className="form-check-label small fw-semibold" htmlFor="esEstudiante">
                  Es estudiante universitario
                </label>
              </div>
            </div>

            {/* Campos de estudiante — solo aparecen si es_estudiante es true */}
            {form.es_estudiante && (
              <div className="row g-2 mb-3 p-3 bg-light rounded-3">
                <div className="col-12">
                  <label className="form-label fw-semibold small text-secondary">
                    Universidad <span className="text-danger">*</span>
                  </label>
                  <input type="text" name="universidad" className="form-control rounded-3"
                    placeholder="ej. Instituto Tecnológico de Costa Rica"
                    value={form.universidad} onChange={handleChange}
                    required={form.es_estudiante} />
                </div>
                <div className="col-12">
                  <label className="form-label fw-semibold small text-secondary">
                    Carnet <span className="text-danger">*</span>
                  </label>
                  <input type="text" name="carnet" className="form-control rounded-3"
                    placeholder="ej. 2024123456" value={form.carnet}
                    onChange={handleChange} required={form.es_estudiante} />
                </div>
              </div>
            )}

            <div className="d-flex justify-content-end gap-2 pt-3"
              style={{ borderTop: "0.5px solid #e8e4f8" }}>
              <button type="button" className="btn rounded-3 fw-semibold"
                style={{ background: "#f5f3ff", color: COLOR_PRINCIPAL }}
                onClick={onCancelar}>
                Cancelar
              </button>
              <button type="submit" className="btn rounded-3 fw-semibold text-white"
                style={{ background: COLOR_PRINCIPAL }}>
                {usuario ? "Guardar cambios" : "Crear usuario"}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}