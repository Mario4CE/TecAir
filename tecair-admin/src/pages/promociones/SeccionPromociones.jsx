// SeccionPromociones.jsx — Gestión de promociones
//
// Funcionalidad:
//   - Ver promociones como tarjetas
//   - Crear una promoción nueva
//   - Editar una promoción existente
//   - Eliminar una promoción
//   - Subir imagen opcional
//   - Muestra si la promoción está vigente o vencida

import { useState } from "react";
import { MOCK_PROMOCIONES, MOCK_RUTAS } from "../../config/mockData";

const COLOR_PRINCIPAL = "#6d4fc2";

export default function SeccionPromociones() {
  // Lista de promociones — inicia con los datos mockeados
  const [promociones, setPromociones] = useState(MOCK_PROMOCIONES);

  // Controla si el modal está abierto
  const [modalAbierto, setModalAbierto] = useState(false);

  // Promoción seleccionada para editar — null = modo creación
  const [promoEditando, setPromoEditando] = useState(null);

  // Mensaje de éxito o error temporal
  const [mensaje, setMensaje] = useState(null);

  const mostrarMensaje = (texto, tipo = "success") => {
    setMensaje({ texto, tipo });
    setTimeout(() => setMensaje(null), 3000);
  };

  // Abrir modal en modo creación
  const handleNueva = () => {
    setPromoEditando(null);
    setModalAbierto(true);
  };

  // Abrir modal en modo edición
  const handleEditar = (promo) => {
    setPromoEditando(promo);
    setModalAbierto(true);
  };

  // Eliminar promoción
  const handleEliminar = (id_promocion) => {
    if (!window.confirm("¿Está seguro que desea eliminar esta promoción?")) return;
    setPromociones((prev) =>
      prev.filter((p) => p.id_promocion !== id_promocion)
    );
    mostrarMensaje("Promoción eliminada correctamente.");
  };

  // Guardar promoción (nueva o editada)
  const handleGuardar = (datos) => {
    if (promoEditando) {
      // Modo edición: reemplaza la promoción existente
      setPromociones((prev) =>
        prev.map((p) =>
          p.id_promocion === promoEditando.id_promocion
            ? { ...p, ...datos }
            : p
        )
      );
      mostrarMensaje("Promoción actualizada correctamente.");
    } else {
      // Modo creación: agrega una nueva promoción
      const nueva = {
        id_promocion: promociones.length + 1,
        ...datos,
      };
      setPromociones((prev) => [...prev, nueva]);
      mostrarMensaje("Promoción creada correctamente.");
    }
    setModalAbierto(false);
    setPromoEditando(null);
  };

  return (
    <div>
      {/* Encabezado */}
      <div className="d-flex align-items-center justify-content-between mb-4">
        <h2 className="fw-bold mb-0" style={{ color: "#3c3489" }}>
          Promociones
        </h2>
        <button
          className="btn fw-semibold text-white rounded-3"
          style={{ background: COLOR_PRINCIPAL }}
          onClick={handleNueva}
        >
          + Nueva promoción
        </button>
      </div>

      {/* Mensaje de éxito o error */}
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
                onEditar={() => handleEditar(p)}
                onEliminar={() => handleEliminar(p.id_promocion)}
              />
            </div>
          ))}
        </div>
      )}

      {/* Modal de crear/editar */}
      {modalAbierto && (
        <ModalPromocion
          promo={promoEditando}
          onGuardar={handleGuardar}
          onCancelar={() => {
            setModalAbierto(false);
            setPromoEditando(null);
          }}
        />
      )}
    </div>
  );
}

// TarjetaPromocion — muestra una promoción como tarjeta
//
// Determina automáticamente si está vigente o vencida
// comparando las fechas con la fecha actual
function TarjetaPromocion({ promo, onEditar, onEliminar }) {
  const hoy = new Date();
  const fechaFin = new Date(promo.fecha_fin);

  // La promoción está vigente si la fecha de fin es futura
  const vigente = fechaFin >= hoy;

  // Formatea una fecha para mostrarla como DD/MM/YYYY
  const formatFecha = (fecha) =>
    new Date(fecha).toLocaleDateString("es-CR", {
      day: "2-digit", month: "2-digit", year: "numeric",
    });

  return (
    <div className="card border-0 shadow-sm rounded-4 overflow-hidden h-100">
      {/* Imagen o placeholder */}
      {promo.imagen ? (
        <img
          src={promo.imagen}
          alt={promo.ruta}
          style={{ height: 120, objectFit: "cover", width: "100%" }}
        />
      ) : (
        <div
          className="d-flex align-items-center justify-content-center"
          style={{
            height: 120,
            background: "linear-gradient(135deg, #6d4fc2, #9b7fe8)",
            color: "rgba(255,255,255,0.5)",
            fontSize: 12,
          }}
        >
          Sin imagen
        </div>
      )}

      <div className="card-body">
        {/* Ruta y badge de vigencia */}
        <div className="d-flex align-items-start justify-content-between mb-1">
          <h5 className="fw-bold mb-0" style={{ color: "#3c3489" }}>
            {promo.ruta}
          </h5>
          <span
            className="badge rounded-pill px-2 py-1"
            style={{
              background: vigente ? "#d1e7dd" : "#e2e3e5",
              color:      vigente ? "#198754" : "#495057",
              fontSize: 11,
            }}
          >
            {vigente ? "Vigente" : "Vencida"}
          </span>
        </div>

        {/* Precio */}
        <p className="fw-bold mb-1" style={{ fontSize: 22, color: COLOR_PRINCIPAL }}>
          ${parseFloat(promo.precio).toFixed(2)}
        </p>

        {/* Período */}
        <p className="text-muted small mb-3">
          {formatFecha(promo.fecha_inicio)} — {formatFecha(promo.fecha_fin)}
        </p>

        {/* Botones de acción */}
        <div className="d-flex gap-2">
          <button
            className="btn btn-sm flex-fill rounded-3 fw-semibold"
            style={{ background: "#f5f3ff", color: COLOR_PRINCIPAL, fontSize: 12 }}
            onClick={onEditar}
          >
            Editar
          </button>
          <button
            className="btn btn-sm flex-fill rounded-3 fw-semibold"
            style={{ background: "#f8d7da", color: "#842029", fontSize: 12 }}
            onClick={onEliminar}
          >
            Eliminar
          </button>
        </div>
      </div>
    </div>
  );
}

// ModalPromocion — formulario para crear o editar
//
// Si recibe promo → modo edición (precarga los datos)
// Si promo es null → modo creación (formulario vacío)
function ModalPromocion({ promo, onGuardar, onCancelar }) {
  // Si estamos editando, precarga los datos de la promoción
  const [form, setForm] = useState({
    id_ruta:      promo?.id_ruta      ?? "",
    ruta:         promo?.ruta         ?? "",
    precio:       promo?.precio       ?? "",
    fecha_inicio: promo?.fecha_inicio ?? "",
    fecha_fin:    promo?.fecha_fin    ?? "",
    imagen:       promo?.imagen       ?? null,
  });

  const [error, setError] = useState("");

  const handleChange = (e) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  // Maneja la selección de imagen
  // Convierte el archivo a base64 para mostrarlo como preview
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

    // Busca la descripción de la ruta seleccionada
    const rutaSeleccionada = MOCK_RUTAS.find(
      (r) => r.id_ruta === parseInt(form.id_ruta)
    );

    onGuardar({
      ...form,
      id_ruta: parseInt(form.id_ruta),
      precio:  parseFloat(form.precio),
      ruta:    rutaSeleccionada?.descripcion ?? form.ruta,
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
            {promo ? "Editar promoción" : "Nueva promoción"}
          </h5>
          <p className="text-muted small mb-4">
            {promo ? "Modifique los datos de la promoción" : "Complete los datos de la nueva promoción"}
          </p>

          {error && (
            <div className="alert alert-danger py-2 small rounded-3">{error}</div>
          )}

          <form onSubmit={handleSubmit}>
            {/* Selector de ruta */}
            <div className="mb-3">
              <label className="form-label fw-semibold small text-secondary">
                Ruta <span className="text-danger">*</span>
              </label>
              <select
                name="id_ruta"
                className="form-select rounded-3"
                value={form.id_ruta}
                onChange={handleChange}
                required
              >
                <option value="">Seleccionar ruta</option>
                {MOCK_RUTAS.map((r) => (
                  <option key={r.id_ruta} value={r.id_ruta}>
                    {r.descripcion}
                  </option>
                ))}
              </select>
            </div>

            {/* Precio */}
            <div className="mb-3">
              <label className="form-label fw-semibold small text-secondary">
                Precio en promoción ($) <span className="text-danger">*</span>
              </label>
              <input
                type="number"
                name="precio"
                className="form-control rounded-3"
                placeholder="ej. 199.99"
                value={form.precio}
                onChange={handleChange}
                step="0.01"
                min="0.01"
                required
              />
            </div>

            {/* Fechas */}
            <div className="row g-2 mb-3">
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Fecha inicio <span className="text-danger">*</span>
                </label>
                <input
                  type="date"
                  name="fecha_inicio"
                  className="form-control rounded-3"
                  value={form.fecha_inicio}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="col-6">
                <label className="form-label fw-semibold small text-secondary">
                  Fecha fin <span className="text-danger">*</span>
                </label>
                <input
                  type="date"
                  name="fecha_fin"
                  className="form-control rounded-3"
                  value={form.fecha_fin}
                  onChange={handleChange}
                  required
                />
              </div>
            </div>

            {/* Imagen opcional */}
            <div className="mb-4">
              <label className="form-label fw-semibold small text-secondary">
                Imagen (opcional)
              </label>
              {/* Preview de imagen si ya hay una seleccionada */}
              {form.imagen && (
                <img
                  src={form.imagen}
                  alt="Preview"
                  className="w-100 rounded-3 mb-2"
                  style={{ height: 100, objectFit: "cover" }}
                />
              )}
              <div
                className="rounded-3 p-3 text-center small text-muted"
                style={{
                  border: "1.5px dashed #c4b5f5",
                  background: "#faf8ff",
                  cursor: "pointer",
                }}
                onClick={() => document.getElementById("inputImagen").click()}
              >
                {form.imagen ? "Cambiar imagen" : "Clic para subir imagen"}
              </div>
              <input
                id="inputImagen"
                type="file"
                accept="image/*"
                className="d-none"
                onChange={handleImagen}
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
                {promo ? "Guardar cambios" : "Crear promoción"}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}