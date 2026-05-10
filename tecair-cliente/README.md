# TECAir - Cliente Web

Sistema de reservación de vuelos para la aerolínea TECAir. Esta es la interfaz cliente web desarrollada en HTML5, CSS3 y JavaScript.

**ESTADO ACTUAL:** ⚠️ Fase de Prueba - Sin autenticación real
- ✅ Sistema funcional como cascaron
- ✅ Flujo de usuario completo
- ⏳ Autenticación real: Pendiente conectar a BD PostgreSQL + API C#

## 📁 Estructura del Proyecto

```
tecair-cliente/
├── index.html              # Página de login/registro
├── menu.html               # Menú principal del cliente
├── vuelos.html             # Búsqueda y selección de vuelos
├── reservaciones.html      # Gestión de reservaciones
├── promociones.html        # Visualización de promociones
├── perfil.html             # Perfil del usuario
├── pago.html               # Proceso de pago
├── css/
│   ├── styles.css          # Estilos generales
│   └── login.css           # Estilos específicos del login
└── js/
    ├── api.js              # Funciones de comunicación con API
    ├── auth.js             # Funciones de autenticación
    └── utils.js            # Funciones utilitarias
```

## 🎯 Características Principales

### Vista de Reservaciones (Cliente)
- ✅ **Gestión de Usuario**: Registro e inicio de sesión
- ✅ **Búsqueda de Vuelos**: Filtrado por origen, destino, fecha
- ✅ **Reservación de Vuelos**: Selección de asientos
- ✅ **Sistema de Pago**: Tarjeta de crédito, transferencia, SINPE
- ✅ **Promociones**: Visualización de ofertas especiales
- ✅ **Programa de Lealtad**: Acumulación de millas
- ✅ **Perfil de Usuario**: Gestión de datos personales

## 🚀 Comenzando

### Requisitos Previos
- Navegador moderno (Chrome, Firefox, Safari, Edge)
- Servidor API ejecutándose en `http://localhost:3000`
- Conexión a internet

### Instalación

1. **Clonar el repositorio**
```bash
git clone <URL-REPOSITORIO>
cd Proyecto\ 1/tecair-cliente
```

2. **Servir archivos localmente**
```bash
#usando solo live server de VSCode

# Usando Python
python -m http.server 8000

# O usando Node.js (si tienes http-server instalado)
npx http-server
```

3. **Acceder a la aplicación**
```
http://localhost:8000
```

## 📄 Páginas Disponibles

### `index.html` - Autenticación
- Login de usuarios existentes
- Registro de nuevos usuarios
- Formulario para estudiantes con datos adicionales

### `menu.html` - Menú Principal
- Bienvenida personalizada
- Acceso rápido a funcionalidades principales
- Promociones destacadas
- Información del programa de lealtad

### `vuelos.html` - Búsqueda de Vuelos
- Selección de aeropuertos origen/destino
- Filtrado por fechas y número de pasajeros
- Visualización de vuelos disponibles
- Información de escalas y duración

### `reservaciones.html` - Gestión de Reservaciones
- Visualización de reservaciones activas
- Historial de reservaciones anteriores
- Creación de nueva reservación
- Selección de asientos

### `promociones.html` - Promociones
- Filtrado de promociones (todas, estudiantes, vigentes)
- Detalles de descuentos
- Cálculo de ahorros
- Navegación rápida a reservación

### `perfil.html` - Perfil del Usuario
- Actualización de datos personales
- Cambio de contraseña
- Gestión de sesiones activas
- Preferencias de notificaciones
- Información de programa de lealtad

### `pago.html` - Proceso de Pago
- Resumen de reservación
- Desglose de costos
- Métodos de pago (tarjeta, transferencia, SINPE)
- Procesamiento seguro de pagos

## 🔐 Autenticación

**ESTADO ACTUAL (Fase de Prueba):**
- ✅ Sistema funciona sin autenticación real
- ✅ Datos guardados en `localStorage`
- ⏳ Validaciones con API: Se agregaran cuando BD esté lista

El sistema actualmente guarda los datos del usuario en `localStorage`:
- `usuarioActual`: Datos del usuario en JSON

**Cuando la BD esté lista:**
- Se agregará `authToken` para JWT
- Se implementarán validaciones reales en el servidor
- Se conectará a la API REST en C#

## 🛠️ Tecnologías Utilizadas

- **HTML5**: Estructura semántica
- **CSS3**: Diseño responsive y animaciones
- **JavaScript ES6+**: Lógica de la aplicación
- **Fetch API**: Comunicación con servidor
- **LocalStorage**: Almacenamiento local de datos
- **luego se debe implementar Angular/React, Bootstrap

## 📱 Responsive Design

La página es completamente responsive:
- 📱 Dispositivos móviles (320px+)
- 💻 Tablets (768px+)
- 🖥️ Escritorio (1200px+)

## 🔧 Configuración de API

Cambiar la URL base de la API en `js/api.js`:
```javascript
const API_BASE_URL = 'http://localhost:3000/api';
```

## 📊 Estructura de Datos

### Usuario
```javascript
{
  id: "usuario_001",
  nombreCompleto: "Juan Pérez",
  email: "juan@example.com",
  telefono: "+506 2345 6789",
  esEstudiante: true,
  universidad: "TEC",
  carnet: "2024001",
  millas: 2450,
  fechaRegistro: "2026-05-01"
}
```

### Reservación
```javascript
{
  id: "RES001",
  usuario_id: "usuario_001",
  numero: "TEC4521",
  origen: "SJO",
  destino: "LIR",
  salida: "2026-05-15T08:00:00",
  llegada: "2026-05-15T10:15:00",
  asiento: "12A",
  estado: "Confirmada",
  monto: 95.00
}
```

### Promoción
```javascript
{
  id: 1,
  origen: "San José",
  destino: "Liberia",
  descuento: 30,
  precioOriginal: 150,
  precioPromocional: 105,
  validos: "May 1 - May 31, 2026",
  estudiantes: true
}
```

## 🐛 Debugging

Habilitar logs en la consola:
```javascript
// En js/utils.js
log('Mensaje de prueba', 'log');
log('Advertencia', 'warn');
log('Error', 'error');
```

## 📝 Notas Importantes

**FASE ACTUAL - CASCARON FUNCIONAL:**
- ✅ No se requiere autenticación para acceder
- ✅ Flujo de usuario completo y funcional
- ✅ Datos se almacenan en `localStorage` (localmente)
- ✅ Listo para conectar a API cuando esté disponible

**SIN AUTENTICACIÓN REAL:**
- ⏳ Verificación con BD: Pendiente
- ⏳ API REST: Pendiente
- ⏳ Encriptación de datos: Pendiente

**Sin Base de Datos Local**: Este cliente no incluye SQLite. La app móvil será la responsable de SQLite.

**Datos Simulados**: Los vuelos, reservaciones y promociones son datos de ejemplo para pruebas.

**Próxima Fase:**
1. ✅ Crear API REST en C# con autenticación JWT
2. ✅ Configurar BD PostgreSQL
3. ✅ Conectar cliente web a API
4. ✅ Habilitar validaciones reales (descomentar en auth.js y utils.js)

## 🚦 Próximos Pasos

### Fase 1: Backend (EN PROGRESO)
1. Crear API REST en C#
2. Configurar base de datos PostgreSQL
3. Implementar autenticación JWT
4. Crear endpoints para:
   - Usuarios (login, registro, perfil)
   - Vuelos (búsqueda, creación)
   - Reservaciones (CRUD)
   - Promociones (CRUD)
   - Pagos (procesamiento)

### Fase 2: Integración Frontend (en desarrollo)
1. Conectar cliente web a API
2. Habilitar validaciones reales
3. Implementar manejo de errores
4. Testing de flujo completo

### Fase 3: Aplicación Móvil (en desarrollo)
1. Crear app con SQLite
2. Implementar sincronización con API
3. Funcionalidad offline

### Fase 4: Reportes y Documentación
1. Generador de reportes PDF
2. Documentación técnica completa
3. Manual de usuario

## 👥 Equipo de Desarrollo

- Instituto Tecnológico de Costa Rica
- Bases de Datos (CE3101)
- I Semestre 2026


---

**Última actualización:** Mayo 2026
**Versión:** 1.0.0
