# TECAir - Aplicación Móvil MAUI

## 📱 Descripción
Aplicación móvil multiplataforma para gestionar reservaciones de vuelos de la aerolínea TECAir. Desarrollo con .NET MAUI y SQLite para base de datos local sin conexión.

## 🏗️ Estructura del Proyecto

```
TecAir/
├── Models/                 # Entidades para SQLite
│   ├── User.cs            # Usuario del sistema
│   ├── Airport.cs         # Aeropuerto
│   ├── Aircraft.cs        # Aeronave
│   ├── Route.cs           # Ruta de vuelo
│   ├── Flight.cs          # Vuelo
│   ├── Reservation.cs     # Reservación
│   ├── Luggage.cs         # Equipaje/Maleta
│   ├── Promotion.cs       # Promoción
│   └── BoardingPass.cs    # Pase de abordar
│
├── Services/              # Servicios de la aplicación
│   └── DatabaseService.cs # Gestión de BD SQLite local
│
├── ViewModels/            # Lógica de presentación MVVM
│   ├── BaseViewModel.cs   # Clase base para todos los ViewModels
│   ├── UserViewModel.cs   # Gestión de usuarios
│   ├── FlightViewModel.cs # Búsqueda de vuelos
│   ├── ReservationViewModel.cs # Gestión de reservaciones
│   └── PromotionViewModel.cs   # Gestión de promociones
│
├── Views/                 # Interfaces de usuario XAML
│   ├── LoginPage.xaml     # Página de inicio de sesión
│   ├── HomePage.xaml      # Página principal (menú)
│   ├── FlightsSearchPage.xaml   # Búsqueda de vuelos
│   ├── ReservationsPage.xaml    # Mis reservaciones
│   └── PromotionsPage.xaml      # Promociones disponibles
│
├── Utilities/             # Clases utilitarias
│   └── (Converters, Helpers, etc.)
│
├── Resources/             # Recursos (imágenes, fuentes, etc.)
├── Platforms/             # Configuración específica por plataforma
├── MauiProgram.cs        # Configuración de dependencias
├── AppShell.xaml         # Estructura de navegación
└── App.xaml              # Configuración general de la app
```

## 🎯 Funcionalidades Implementadas

### ✅ Completo
- [x] Estructura base de carpetas
- [x] Modelos de datos para SQLite
- [x] Servicio de base de datos con datos iniciales
- [x] ViewModels con patrón MVVM
- [x] Navegación con AppShell
- [x] Páginas principales (UI básica)

### 🔄 En Desarrollo (TODO)
- [ ] Lógica de autenticación/login
- [ ] Integración de ViewModels con las páginas
- [ ] Búsqueda de vuelos funcional
- [ ] Sistema de reservaciones
- [ ] Cálculo de tarifa de maletas
- [ ] Pre-check-in
- [ ] Sincronización con API (cuando esté lista)

## 📦 Dependencias Instaladas
- `sqlite-net-pcl` - ORM para SQLite

## 🚀 Cómo Ejecutar

1. **Restaurar paquetes NuGet:**
   ```bash
   dotnet restore
   ```

2. **Compilar el proyecto:**
   ```bash
   dotnet build
   ```

3. **Ejecutar en Windows:**
   ```bash
   dotnet run -f net10.0-windows10.0.19041.0
   ```

4. **Ejecutar en Android (requiere emulador o dispositivo):**
   ```bash
   dotnet build -f net10.0-android -c Release
   ```

## 📊 Base de Datos Local

La aplicación crea automáticamente una base de datos SQLite en el directorio `FileSystem.AppDataDirectory` con:

- **Usuarios**: Para gestión de cuentas
- **Aeropuertos**: Registra todos los aeropuertos disponibles
- **Aeronaves**: Información de aviones
- **Rutas**: Conexiones entre aeropuertos
- **Vuelos**: Instancias de vuelos programados
- **Reservaciones**: Reservas de usuarios
- **Equipaje**: Maletas asociadas a reservaciones
- **Promociones**: Ofertas especiales activas
- **Pases de Abordar**: Documentos de abordaje

La base de datos se inicializa automáticamente con datos de prueba al primer lanzamiento.

## 🎨 Paleta de Colores
- Primario: #007AFF (Azul)
- Éxito: #34C759 (Verde)
- Alerta: #FF9500 (Naranja)
- Peligro: #FF3B30 (Rojo)
- Neutrales: #F5F5F5, #CCCCCC, #666666

## 📝 Próximos Pasos

1. **Conectar ViewModels a las Views** - Usar binding para sincronizar datos
2. **Implementar Autenticación** - Sistema de login/registro funcional
3. **Completar Funcionalidades** - Reservación, check-in, pago
4. **Integrar API** - Conectar con tecair-api cuando esté lista
5. **Testing** - Pruebas unitarias y de UI
6. **Estilos Avanzados** - Mejorar apariencia visual

## 📄 Notas
- La app funciona completamente sin conexión usando SQLite
- Los datos son persistentes entre sesiones
- Estructura lista para sincronización futura con API REST
