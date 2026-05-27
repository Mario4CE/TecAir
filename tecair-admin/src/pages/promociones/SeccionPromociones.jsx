// SeccionPromociones.jsx — Gestión de promociones conectada al API
//
// Funcionalidad:
//   - Ver promociones como tarjetas
//   - Crear una promoción nueva
//   - Eliminar una promoción
//   - Subir imagen opcional
//   - Muestra si la promoción está vigente o vencida

import { useState, useEffect } from "react";
import { ENDPOINTS, apiFetch } from "../../config/api";

const COLOR_PRINCIPAL = "#6d4fc2";

export default function SeccionPromociones() {
  // Lista de promociones del API
  const [promociones, setPromociones] = useState([]);

  // Lista de rutas para el selector del formulario
  const [rutas, setRutas] = useState([]);

  // Controla si el modal está abierto
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

  // Carga promociones y rutas en paralelo
  const cargarDatos = async () => {
    setCargando(true);
    try {
      const [dataPromociones, dataRutas] = await Promise.all([
        apiFetch(ENDPOINTS.promociones.list),
        apiFetch(ENDPOINTS.rutas.list),
      ]);
      setPromociones(dataPromociones.promociones ?? []);
      setRutas(dataRutas.rutas ?? []);
    } catch (err) {
      console.log("Error:", err);
      mostrarMensaje("Error al cargar los datos.", "danger");
    } finally {
      setCargando(false);
    }
  };

  // Elimina una promoción — pide confirmación primero
  const handleEliminar = async (id_promocion) => {
    if (!window.confirm("¿Está seguro que desea eliminar esta promoción?")) return;
    try {
      await apiFetch(ENDPOINTS.promociones.delete(id_promocion), "DELETE");
      mostrarMensaje("Promoción eliminada correctamente.");
      cargarDatos();
    } catch (err) {
      mostrarMensaje(err.message || "Error al eliminar promoción.", "danger");
    }
  };

  // Crea una nueva promoción y recarga la lista
  const handleGuardar = async (datos) => {
    try {
      await apiFetch(ENDPOINTS.promociones.create, "POST", datos);
      mostrarMensaje("Promoción creada correctamente.");
      setModalAbierto(false);
      cargarDatos();
    } catch (err) {
      mostrarMensaje(err.message || "Error al crear promoción.", "danger");
    }
  };

  if (cargando) {
    return (
      <div className="d-flex align-items-center justify-content-center py-5">
        <div className="spinner-border text-primary me-2"></div>
        <span className="text-muted">Cargando promociones...</span>
      </div>
    );
  }

  return (
    <div>
      <div className="d-flex align-items-center justify-content-between mb-4">
        <h2 className="fw-bold mb-0" style={{ color: "#3c3489" }}>
          Promociones
        </h2>
        <button
          className="btn fw-semibold text-white rounded-3"
          style={{ background: COLOR_PRINCIPAL }}
          onClick={() => setModalAbierto(true)}
        >
          + Nueva promoción
        </button>
      </div>

      {mensaje && (
        <div className={`alert alert-${mensaje.tipo} py-2 small rounded-3 mb-3`}>
          {mensaje.texto}
        </div>
      )}

      {/* Grid de tarjetas */}
      {promociones.length === 0 ? (
        <div className="text-center py-5 text-muted">
          <p className="mb-0">No hay promociones registradas.</p>
        </div>
      ) : (
        <div className="row g-3">
          {promociones.map((p) => (
            <div key={p.id_promocion} className="col-12 col-md-6 col-lg-4">
              <TarjetaPromocion
                promo={p}
                onEliminar={() => handleEliminar(p.id_promocion)}
              />
            </div>
          ))}
        </div>
      )}

      {modalAbierto && (
        <ModalPromocion
          rutas={rutas}
          onGuardar={handleGuardar}
          onCancelar={() => setModalAbierto(false)}
        />
      )}
    </div>
  );
}

// TarjetaPromocion — muestra una promoción como tarjeta
// Determina si está vigente o vencida comparando con la fecha actual
function TarjetaPromocion({ promo, onEliminar }) {
  const hoy = new Date();
  const fechaFin = new Date(promo.fecha_fin);
  const vigente = fechaFin >= hoy;

  const formatFecha = (fecha) =>
    new Date(fecha).toLocaleDateString("es-CR", {
      day: "2-digit", month: "2-digit", year: "numeric",
    });

  return (
    <div className="card border-0 shadow-sm rounded-4 overflow-hidden h-100">
      {/* Imagen o placeholder con gradiente */}
      {promo.imagen ? (
        <img src={promo.imagen} alt={promo.id_ruta}
          style={{ height: 120, objectFit: "cover", width: "100%" }} />
      ) : (
        <div className="d-flex align-items-center justify-content-center"
          style={{ height: 120, background: "linear-gradient(135deg, #6d4fc2, #9b7fe8)", color: "rgba(255,255,255,0.5)", fontSize: 12 }}>
          Sin imagen
        </div>
      )}

      <div className="card-body">
        {/* Ruta y badge de vigencia */}
        <div className="d-flex align-items-start justify-content-between mb-1">
          <h5 className="fw-bold mb-0" style={{ color: "#3c3489" }}>
            Ruta #{promo.id_ruta}
          </h5>
          <span className="badge rounded-pill px-2 py-1"
            style={{
              background: vigente ? "#d1e7dd" : "#e2e3e5",
              color:      vigente ? "#198754" : "#495057",
              fontSize: 11,
            }}>
            {vigente ? "Vigente" : "Vencida"}
          </span>
        </div>

        {/* Precio */}
        <p className="fw-bold mb-1" style={{ fontSize: 22, color: COLOR_PRINCIPAL }}>
          ${parseFloat(promo.precio).toFixed(2)}
        </p>

        {/* Período de la promoción */}
        <p className="text-muted small mb-3">
          {formatFecha(promo.fecha_inicio)} — {formatFecha(promo.fecha_fin)}
        </p>

        <button
          className="btn btn-sm w-100 rounded-3 fw-semibold"
          style={{ background: "#f8d7da", color: "#842029", fontSize: 12 }}
          onClick={onEliminar}
        >
          Eliminar
        </button>
      </div>
    </div>
  );
}

// ModalPromocion — formulario para crear una nueva promoción
function ModalPromocion({ rutas, onGuardar, onCancelar }) {
  const [form, setForm] = useState({
    id_ruta:      "",
    precio:       "",
    fecha_inicio: "",
    fecha_fin:    "",
    imagen:       null,
  });
  const [error, setError] = useState("");

  const handleChange = (e) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  // Convierte la imagen a base64 para enviarla al API
  const handleImagen = (e) => {
    const archivo = e.target.files[0];
    if (!archivo) return;
    const reader = new FileReader();
    reader.onload = () => setForm((prev) => ({ ...prev, imagen: reader.result }));
    reader.readAsDataURL(archivo);
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    setError("");
    if (!form.id_ruta || !form.precio || !form.fecha_inicio || !form.fecha_fin) {
      setError("Todos los campos obligatorios deben completarse.");
      return;
    }
    if (new Date(form.fecha_fin) < new Date(form.fecha_inicio)) {
      setError("La fecha de fin no puede ser anterior a la fecha de inicio.");
      return;
    }
    onGuardar({
      id_ruta:      parseInt(form.id_ruta),
      precio:       parseFloat(form.precio),
      fecha_inicio: form.fecha_inicio,
      fecha_fin:    form.fecha_fin,
      imagen:       form.imagen,
    });
  };

  return (
    <div
      className="position-fixed top-0 start-0 w-100 h-100 d-flex align-items-center justify-content-center"
      style={{ background: "rgba(0,0,0,0.4)", zIndex: 1000 }}
    >
      <div className="card border-0 shadow rounded-4" style={{ width: "100%", maxWidth: 440 }}>
        <div className="card-body p-4">
          <h5 className="fw-bold mb-1" style={{ color: "#3c3489" }}>
            Nueva promoción
          </h5>
          <p className="text-muted small mb-4">Complete los datos de la promoción</p>

          {error && (
            <div className="alert alert-danger py-2 small rounded-3">{error}</div>
          )}

          <form onSubmit={handleSubmit}>
            <div className="mb-3">
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

            <div className="mb-3">
              <label className="form-label fw-semibold small text-secondary">
                Precio en promoción ($) <span className="text-danger">*</span>
              </label>
              <input type="number" name="precio" className="form-control rounded-3"
                placeholder="ej. 199.99" value={form.precio}
                onChange={handleChange} step="0.01" min="0.01" required />
            </div>

            <div className="row g-2 mb-3">
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Fecha inicio <span className="text-danger">*</span>
                </label>
                <input type="date" name="fecha_inicio" className="form-control rounded-3"
                  value={form.fecha_inicio} onChange={handleChange} required />
              </div>
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Fecha fin <span className="text-danger">*</span>
                </label>
                <input type="date" name="fecha_fin" className="form-control rounded-3"
                  value={form.fecha_fin} onChange={handleChange} required />
              </div>
            </div>

            <div className="mb-4">
              <label className="form-label fw-semibold small text-secondary">
                Imagen (opcional)
              </label>
              {/* Preview de imagen si ya hay una seleccionada */}
              {form.imagen && (
                <img src={form.imagen} alt="Preview" className="w-100 rounded-3 mb-2"
                  style={{ height: 100, objectFit: "cover" }} />
              )}
              <div
                className="rounded-3 p-3 text-center small text-muted"
                style={{ border: "1.5px dashed #c4b5f5", background: "#faf8ff", cursor: "pointer" }}
                onClick={() => document.getElementById("inputImagen").click()}
              >
                {form.imagen ? "Cambiar imagen" : "Clic para subir imagen"}
              </div>
              <input id="inputImagen" type="file" accept="image/*"
                className="d-none" onChange={handleImagen} />
            </div>

            <div className="d-flex justify-content-end gap-2 pt-3"
              style={{ borderTop: "0.5px solid #e8e4f8" }}>
              <button type="button" className="btn rounded-3 fw-semibold"
                style={{ background: "#f5f3ff", color: COLOR_PRINCIPAL }}
                onClick={onCancelar}>
                Cancelar
              </button>
              <button type="submit" className="btn rounded-3 fw-semibold text-white"
                style={{ background: COLOR_PRINCIPAL }}>
                Crear promoción
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}