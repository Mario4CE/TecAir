// SeccionUsuarios.jsx — Gestión de usuarios
//
// Funcionalidad:
//   - Ver lista de usuarios con búsqueda y filtro
//   - Crear nuevo administrador
//   - Editar usuario existente
//   - Eliminar usuario
//   - El admin principal no se puede editar ni eliminar

import { useState } from "react";

const COLOR_PRINCIPAL = "#6d4fc2";

// Usuarios de prueba
// Cuando el API esté lista, estos vendrán del backend
const MOCK_USUARIOS = [
  {
    id_usuario: 1, nombre1: "Admin", nombre2: "", apellido1: "TECAir",
    apellido2: "", telefono: "2222-0000", correo: "admin@tecair.com",
    es_estudiante: false, es_admin: true, universidad: "", carnet: "",
  },
  {
    id_usuario: 2, nombre1: "María", nombre2: "José", apellido1: "González",
    apellido2: "Pérez", telefono: "8888-1111", correo: "maria@correo.com",
    es_estudiante: true, es_admin: false, universidad: "Instituto Tecnológico de Costa Rica", carnet: "2024001",
  },
  {
    id_usuario: 3, nombre1: "Luis", nombre2: "", apellido1: "Pérez",
    apellido2: "Mora", telefono: "8888-2222", correo: "luis@correo.com",
    es_estudiante: false, es_admin: false, universidad: "", carnet: "",
  },
];

// Devuelve la configuración visual según el tipo de usuario
function getTipoConfig(usuario) {
  if (usuario.es_admin)       return { label: "Admin",      bg: "#ede9fa", color: "#6d4fc2" };
  if (usuario.es_estudiante)  return { label: "Estudiante", bg: "#cfe2ff", color: "#0d6efd" };
  return                             { label: "Cliente",    bg: "#e2e3e5", color: "#495057" };
}

export default function SeccionUsuarios() {
  // Lista de usuarios
  const [usuarios, setUsuarios] = useState(MOCK_USUARIOS);

  // Texto de búsqueda por nombre o correo
  const [busqueda, setBusqueda] = useState("");

  // Filtro por tipo: "todos" | "admin" | "cliente" | "estudiante"
  const [filtroTipo, setFiltroTipo] = useState("todos");

  // Controla si el modal está abierto
  const [modalAbierto, setModalAbierto] = useState(false);

  // Usuario seleccionado para editar — null = modo creación
  const [usuarioEditando, setUsuarioEditando] = useState(null);

  // Mensaje temporal de éxito o error
  const [mensaje, setMensaje] = useState(null);

  const mostrarMensaje = (texto, tipo = "success") => {
    setMensaje({ texto, tipo });
    setTimeout(() => setMensaje(null), 3000);
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

  // Abrir modal en modo creación
  const handleNuevoAdmin = () => {
    setUsuarioEditando(null);
    setModalAbierto(true);
  };

  // Abrir modal en modo edición
  const handleEditar = (usuario) => {
    setUsuarioEditando(usuario);
    setModalAbierto(true);
  };

  // Eliminar usuario
  const handleEliminar = (id_usuario) => {
    if (!window.confirm("¿Está seguro que desea eliminar este usuario?")) return;
    setUsuarios((prev) => prev.filter((u) => u.id_usuario !== id_usuario));
    mostrarMensaje("Usuario eliminado correctamente.");
  };

  // Guardar usuario (nuevo o editado)
  const handleGuardar = (datos) => {
    if (usuarioEditando) {
      // Modo edición: actualiza el usuario existente
      setUsuarios((prev) =>
        prev.map((u) =>
          u.id_usuario === usuarioEditando.id_usuario ? { ...u, ...datos } : u
        )
      );
      mostrarMensaje("Usuario actualizado correctamente.");
    } else {
      // Modo creación: agrega nuevo admin
      const nuevo = {
        id_usuario: usuarios.length + 1,
        ...datos,
        es_admin: true, // siempre es admin al crearse desde aquí
      };
      setUsuarios((prev) => [...prev, nuevo]);
      mostrarMensaje("Administrador creado correctamente.");
    }
    setModalAbierto(false);
    setUsuarioEditando(null);
  };

  return (
    <div>
      {/* Encabezado */}
      <div className="d-flex align-items-center justify-content-between mb-4">
        <h2 className="fw-bold mb-0" style={{ color: "#3c3489" }}>
          Usuarios
        </h2>
        <button
          className="btn fw-semibold text-white rounded-3"
          style={{ background: COLOR_PRINCIPAL }}
          onClick={handleNuevoAdmin}
        >
          + Nuevo admin
        </button>
      </div>

      {/* Mensaje de éxito o error */}
      {mensaje && (
        <div className={`alert alert-${mensaje.tipo} py-2 small rounded-3 mb-3`}>
          {mensaje.texto}
        </div>
      )}

      {/* Tabla de usuarios */}
      <div className="card border-0 shadow-sm rounded-4">
        <div className="card-body">

          {/* Búsqueda y filtro */}
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
                    // El admin principal (id 1) no se puede tocar
                    const esAdminPrincipal = u.id_usuario === 1;

                    return (
                      <tr key={u.id_usuario}>
                        <td>
                          {/* Avatar con inicial del nombre */}
                          <span
                            className="d-inline-flex align-items-center justify-content-center rounded-circle me-2"
                            style={{
                              width: 28, height: 28,
                              background: tipo.bg,
                              color: tipo.color,
                              fontSize: 11, fontWeight: 700,
                            }}
                          >
                            {u.nombre1[0]}
                          </span>
                          {u.nombre1} {u.apellido1}
                        </td>
                        <td>{u.correo}</td>
                        <td>{u.telefono}</td>
                        <td>
                          <span
                            className="badge rounded-pill px-2 py-1"
                            style={{ background: tipo.bg, color: tipo.color, fontSize: 11 }}
                          >
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

      {/* Modal crear/editar */}
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

// ModalUsuario — formulario para crear admin o editar usuario
//
// Si recibe usuario → modo edición (precarga los datos)
// Si usuario es null → modo creación de admin (formulario vacío)
function ModalUsuario({ usuario, onGuardar, onCancelar }) {
  const [form, setForm] = useState({
    nombre1:    usuario?.nombre1    ?? "",
    nombre2:    usuario?.nombre2    ?? "",
    apellido1:  usuario?.apellido1  ?? "",
    apellido2:  usuario?.apellido2  ?? "",
    telefono:   usuario?.telefono   ?? "",
    correo:     usuario?.correo     ?? "",
    contrasena: "",
  });
  const [error, setError] = useState("");

  const handleChange = (e) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = (e) => {
    e.preventDefault();
    setError("");

    if (!form.nombre1 || !form.apellido1 || !form.correo || !form.telefono) {
      setError("Los campos marcados con * son obligatorios.");
      return;
    }
    // Solo exige contraseña al crear, no al editar
    if (!usuario && !form.contrasena) {
      setError("La contraseña es obligatoria al crear un admin.");
      return;
    }
    if (!usuario && form.contrasena.length < 8) {
      setError("La contraseña debe tener al menos 8 caracteres.");
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
            {usuario ? "Editar usuario" : "Nuevo administrador"}
          </h5>
          <p className="text-muted small mb-4"
            style={{ borderBottom: "0.5px solid #e8e4f8", paddingBottom: 12 }}
          >
            {usuario
              ? "Modifique los datos del usuario"
              : "Este usuario tendrá acceso al portal de administración"}
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

            <div className="mb-3">
              <label className="form-label fw-semibold small text-secondary">
                Correo electrónico <span className="text-danger">*</span>
              </label>
              <input type="email" name="correo" className="form-control rounded-3"
                placeholder="funcionario@tecair.com" value={form.correo}
                onChange={handleChange} required />
            </div>

            <div className="row g-2 mb-3">
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Teléfono <span className="text-danger">*</span>
                </label>
                <input type="tel" name="telefono" className="form-control rounded-3"
                  placeholder="8888-8888" value={form.telefono} onChange={handleChange} required />
              </div>
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Contraseña {!usuario && <span className="text-danger">*</span>}
                </label>
                <input type="password" name="contrasena" className="form-control rounded-3"
                  placeholder={usuario ? "Dejar vacío para no cambiar" : "Mínimo 8 caracteres"}
                  value={form.contrasena} onChange={handleChange}
                  minLength={!usuario ? 8 : undefined} />
              </div>
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
                {usuario ? "Guardar cambios" : "Crear admin"}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}