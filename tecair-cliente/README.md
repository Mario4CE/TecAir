# TECAir - Cliente Web

Sistema de reservación de vuelos para la aerolínea TECAir. Esta es la interfaz cliente web desarrollada en HTML5, CSS3, Bootstrap 5 y JavaScript.

**ESTADO ACTUAL:** 🔄 En desarrollo - Integración con API en progreso
- ✅ Sistema funcional con diseño mejorado
- ✅ Bootstrap 5 integrado en todas las páginas
- ✅ Flujo de usuario completo
- 🔄 Integración con API REST en C#: En progreso

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
- ✅ **Promociones**: Visualización de ofertas especiales con modal de detalle
- ✅ **Programa de Lealtad**: Acumulación de millas
- ✅ **Perfil de Usuario**: Gestión de datos personales

## 🚀 Comenzando

### Requisitos Previos
- Navegador moderno (Chrome, Firefox, Safari, Edge)
- Extensión **Live Server** en VSCode
- API REST ejecutándose en `http://localhost:5000`
- Conexión a internet (para cargar Bootstrap desde CDN)

### Instalación

1. **Clonar el repositorio**
```bash
git clone <URL-REPOSITORIO>
cd TecAir
```

2. **Cambiar a la rama del cliente web**
```bash
git checkout Web-Cliente
```

3. **Abrir con Live Server**
- Clic derecho en `tecair-cliente/index.html`
- Seleccionar **"Open with Live Server"**

4. **Correr la API** (en otra terminal)
```bash
git checkout API
cd tecair-api
dotnet run
```

## 📄 Páginas Disponibles

### `index.html` - Autenticación
- Login de usuarios existentes
- Registro de nuevos usuarios
- Formulario adicional para datos estudiantiles
- Validación de campos en tiempo real

### `menu.html` - Menú Principal
- Bienvenida personalizada con nombre del usuario
- Acceso rápido a las 4 funcionalidades principales
- Promociones destacadas
- Información del programa de lealtad con millas disponibles

### `vuelos.html` - Búsqueda de Vuelos
- Selección de aeropuertos origen/destino
- Filtrado por fechas y número de pasajeros
- Visualización de vuelos disponibles en cards
- Información de escalas y duración

### `reservaciones.html` - Gestión de Reservaciones
- Visualización de reservaciones activas
- Historial de reservaciones anteriores
- Creación de nueva reservación
- Selección de asiento e información de pasaporte opcional

### `promociones.html` - Promociones
- Filtrado de promociones (todas, estudiantes, vigentes)
- Modal con detalles completos de cada promoción
- Cálculo de ahorros y descuentos
- Navegación rápida a reservación

### `perfil.html` - Perfil del Usuario
- Actualización de datos personales
- Cambio de contraseña
- Gestión de sesiones activas
- Preferencias de notificaciones
- Información del programa de lealtad

### `pago.html` - Proceso de Pago
- Resumen de reservación
- Desglose de costos (tarifa base, impuestos, cargo de servicio)
- Métodos de pago: tarjeta de crédito, transferencia bancaria, SINPE Móvil
- Formateo automático de número de tarjeta y fecha de vencimiento

## 🛠️ Tecnologías Utilizadas

- **HTML5**: Estructura semántica con comentarios documentados
- **CSS3**: Diseño responsive y animaciones
- **Bootstrap 5.3.3**: Framework CSS para diseño y componentes
- **Bootstrap Icons 1.11.3**: Íconos consistentes en toda la aplicación
- **JavaScript ES6+**: Lógica de la aplicación
- **Fetch API**: Comunicación con el servidor REST
- **LocalStorage**: Almacenamiento local de sesión del usuario
- **Google Fonts (Nunito)**: Tipografía principal

## 🔧 Configuración de API

La URL base de la API está definida en `js/api.js`:
```javascript
const API_BASE_URL = 'http://localhost:5000/api';
```

## 🔐 Autenticación

**ESTADO ACTUAL:**
- ✅ Sistema funciona con datos en `localStorage`
- ✅ Estructura lista para conectar con API real
- 🔄 Integración con endpoints de usuarios en progreso

El sistema guarda los datos del usuario en `localStorage`:
- `usuarioActual`: Datos del usuario en JSON

**Cuando la integración esté completa:**
- Se usará `authToken` para JWT
- Se validará contra el endpoint `POST /api/usuarios`
- Se identificará el perfil con el header `X-User-Id`

## 📊 Estructura de Datos

### Usuario (localStorage actual)
```javascript
{
  nombreCompleto: "Juan Pérez",
  email: "juan@example.com",
  telefono: "+506 8888 8888",
  esEstudiante: true,
  universidad: "TEC",
  carnet: "2024001",
  millas: 0
}
```

### Usuario (estructura API)
```javascript
{
  idUsuario: 1,
  nombre1: "Juan",
  nombre2: "",
  apellido1: "Pérez",
  apellido2: "",
  correo: "juan@example.com",
  telefono: "+506 8888 8888",
  esEstudiante: true,
  universidad: "TEC",
  carnet: "2024001",
  millas: 0
}
```

### Vuelo (estructura API)
```javascript
{
  idVuelo: 1,
  fechaSalida: "2026-05-20",
  horaSalida: "08:00",
  puerta: "A1",
  estado: "programado",
  matricula: "TEC-001",
  precio: 95.00,
  capacidad: 150,
  origen: "San José",
  destino: "Liberia",
  asientosDisponibles: 45
}
```

## 📝 Convención de Comentarios

### HTML
```html
<!--
=====================================
NOMBRE DE LA SECCIÓN
=====================================
Descripción de qué hace esta parte
-->
```

### JavaScript
```javascript
/**
 * Descripción de la función
 * @param {tipo} nombre - Descripción del parámetro
 * @returns {tipo} Descripción de lo que retorna
 */
function nombreFuncion(nombre) { ... }
```

## 🐛 Debugging

Habilitar logs en la consola:
```javascript
// En js/utils.js
log('Mensaje de prueba', 'log');
log('Advertencia', 'warn');
log('Error', 'error');
```

## 📱 Responsive Design

La página es completamente responsive gracias a Bootstrap:
- 📱 Dispositivos móviles (320px+) — navbar colapsable con menú hamburguesa
- 💻 Tablets (768px+)
- 🖥️ Escritorio (1200px+)

## 🚦 Estado de Integración con API

| Página | Endpoint | Estado |
|--------|----------|--------|
| index.html | `POST /api/usuarios` | ⏳ Pendiente |
| vuelos.html | `GET /api/vuelos` | ⏳ Pendiente |
| vuelos.html | `GET /api/aeropuertos` | ⏳ Pendiente |
| reservaciones.html | `GET /api/reservaciones` | ⏳ Pendiente |
| reservaciones.html | `POST /api/reservaciones` | ⏳ Pendiente |
| pago.html | `POST /api/pagos` | ⏳ Pendiente |
| promociones.html | `GET /api/promociones` | ⏳ Pendiente |
| perfil.html | `GET /api/usuarios/perfil` | ⏳ Pendiente |
| perfil.html | `PUT /api/usuarios/perfil` | ⏳ Pendiente |

## 🚦 Próximos Pasos

### Fase 1: Backend ✅ Completado
- ✅ API REST base creada en C#
- ✅ Endpoints para usuarios, vuelos, reservaciones, pagos, promociones, check-ins y maletas

### Fase 2: Integración Frontend 🔄 En progreso
1. Conectar `index.html` con `POST /api/usuarios`
2. Conectar `vuelos.html` con `GET /api/vuelos` y `GET /api/aeropuertos`
3. Conectar `reservaciones.html` con endpoints de reservaciones
4. Conectar `pago.html` con `POST /api/pagos`
5. Conectar `promociones.html` con `GET /api/promociones`
6. Conectar `perfil.html` con endpoints de usuario

### Fase 3: Aplicación Móvil ⏳ Pendiente
1. Crear app con SQLite
2. Implementar sincronización con API
3. Funcionalidad offline

### Fase 4: Reportes y Documentación ⏳ Pendiente
1. Generador de reportes PDF
2. Documentación técnica completa
3. Manual de usuario

## 👥 Equipo de Desarrollo

- Instituto Tecnológico de Costa Rica
- Bases de Datos (CE3101)
- I Semestre 2026

---

**Última actualización:** Mayo 2026
**Versión:** 1.1.0
