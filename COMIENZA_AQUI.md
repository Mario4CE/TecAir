# 🚀 COMIENZA AQUÍ - Guía Rápida

Bienvenido a la integración PostgreSQL + App Móvil de TecAir.

## 📖 ¿Por dónde empezar?

### 1️⃣ **Leo primero (10 min)**
👉 [`INTEGRACION_POSTGRESQL_RESUMEN.md`](./INTEGRACION_POSTGRESQL_RESUMEN.md)
- Qué se implementó
- Cómo funciona
- Próximos pasos

### 2️⃣ **Entiendo la arquitectura (15 min)**
👉 [`App-tecair/TecAir/ARQUITECTURA_DETALLADA.md`](./App-tecair/TecAir/ARQUITECTURA_DETALLADA.md)
- Diagramas completos
- Flujos de sincronización
- Mapeo de datos

### 3️⃣ **Completo la implementación (45 min)**
👉 [`App-tecair/TecAir/SINCRONIZACION_GUIA.md`](./App-tecair/TecAir/SINCRONIZACION_GUIA.md)
- Pasos 1-5 para completar
- DTOs que faltan
- Endpoints requeridos

### 4️⃣ **Pruebo todo (30 min)**
👉 [`App-tecair/TecAir/GUIA_TESTING.md`](./App-tecair/TecAir/GUIA_TESTING.md)
- 10 pruebas detalladas
- Scripts PowerShell listos
- Troubleshooting

---

## ✅ Lo Que Ya Está Hecho

### 5 Nuevos Servicios Creados:
```
App-tecair/TecAir/Services/
├─ ApiService.cs ✅
├─ AppConfigService.cs ✅
├─ SyncQueueService.cs ✅
├─ AuthenticationService.cs ✅ (MEJORADO)
└─ ImprovedSyncService.cs ✅ (REFERENCIA)
```

### 3 Documentos de Referencia:
```
├─ SINCRONIZACION_GUIA.md
├─ ARQUITECTURA_DETALLADA.md
└─ GUIA_TESTING.md
```

### MauiProgram Actualizado:
```csharp
✅ Todos los servicios registrados
✅ Inicialización en el orden correcto
✅ Configuration + API + Database listos
```

---

## ⚡ Acciones Inmediatas

### HOY (Máximo 2 horas):

1. **Compilar** el proyecto
   ```bash
   cd App-tecair/TecAir
   dotnet build
   ```

2. **Verificar** que API tiene estos endpoints
   - ✅ POST /api/auth/login
   - ? GET /api/vuelos
   - ? POST /api/usuarios
   - ? POST /api/reservaciones

3. **Leer** INTEGRACION_POSTGRESQL_RESUMEN.md (10 min)

### ESTA SEMANA:

4. Crear DTOs en `Models/ApiDtos.cs`
5. Actualizar `SyncService.cs` con lógica mejorada
6. Testing exhaustivo (con/sin conexión)
7. Agregar UI indicadores de sincronización

---

## 🎯 Resultados Esperados

### Después de compilar + completar:

✅ **Login automático**
   - Con conexión: Login en API + JWT token
   - Sin conexión: Login local en SQLite

✅ **Crear reservación offline**
   - Se guarda en SQLite
   - Se agrega a SyncQueue
   - UI muestra: ⏳ "Pendiente de sincronizar"

✅ **Recuperar conexión**
   - SyncService detecta conexión automáticamente
   - Envía datos pendientes al API
   - UI muestra: ✓ "Sincronizado"

✅ **Reintentos automáticos**
   - Si falla: Espera 1 min
   - Si falla: Espera 2 min
   - Si falla: Espera 4 min
   - Si falla 3 veces: Usuario lo ve

---

## 📊 Stack Completo

```
Frontend Mobile (MAUI)
  ├─ XAML Pages (UI)
  ├─ ViewModels (Lógica)
  └─ Services (Negocio)
     ├─ AuthenticationService (JWT + Local)
     ├─ ApiService (HTTP centralizado)
     ├─ SyncService (Bidireccional)
     ├─ SyncQueueService (Reintentos)
     ├─ DatabaseService (SQLite)
     └─ AppConfigService (Config + Tokens)
        ↓
Backend API (.NET)
  ├─ ApiEndpoints (REST)
  ├─ Services (Negocio)
  └─ Infrastructure (EF Core)
     ↓
PostgreSQL Database
  ├─ Usuarios
  ├─ Vuelos
  ├─ Reservaciones
  └─ ...
```

---

## 🔧 Compilar Ahora

```powershell
# PowerShell - Navegar a la carpeta
cd C:\Users\morer\OneDrive\Documentos\GitHub\TecAir\App-tecair\TecAir

# Compilar
dotnet build

# Si hay errores, ver detalles
dotnet build --verbosity detailed
```

**Debe compilar sin errores en menos de 1 minuto.**

---

## 🆘 Si Algo No Funciona

| Problema | Solución |
|----------|----------|
| "ApiService not found" | Revisar MauiProgram.cs - debe tener `AddSingleton<ApiService>()` |
| "SyncQueue table error" | Ejecutar desde cero (eliminar tecair.db) |
| "Token not persisting" | Verificar que `AppConfigService.SetAuthTokenAsync()` se llama |
| "API retorna 404" | Verificar URL en AppConfigService (actualmente 172.18.48.117:5000) |
| "Compilación lenta" | `dotnet clean` y reintentar |

---

## 📞 Próximas Preguntas

Una vez hayas leído y compilado, preguntas frecuentes:

1. **¿Dónde están los DTOs?** 
   → Ver SINCRONIZACION_GUIA.md Paso 2

2. **¿Cómo cambiar URL del API?**
   → `AppConfigService.SetApiUrlAsync("http://nueva-url")`

3. **¿Cómo hacer login sin conexión?**
   → `AuthenticationService.LoginAsync(..., forceLocal: true)`

4. **¿Cómo ver la base de datos SQLite?**
   → Ver GUIA_TESTING.md - Sección "Debugging"

5. **¿Qué pasa si falla la sincronización?**
   → Se reintentan con backoff exponencial, usuario ve estado

---

## 🎓 Conceptos Clave

1. **JWT Token**: Se guarda en AppConfigService, se envía automáticamente
2. **SyncQueue**: Cola de operaciones, reintentos automáticos
3. **Backoff Exponencial**: 1, 2, 4, 8 minutos entre reintentos
4. **SQLite Local**: Fuente de verdad local, sincroniza con PostgreSQL
5. **Fallback**: Sin conexión usa local, con conexión usa API

---

## 📋 Checklist de Lectura

- [ ] Leí INTEGRACION_POSTGRESQL_RESUMEN.md
- [ ] Comprendí ARQUITECTURA_DETALLADA.md
- [ ] Compilé el proyecto sin errores
- [ ] Verifiqué endpoints del API
- [ ] Revisé GUIA_TESTING.md
- [ ] Listo para completar la implementación

---

**¡Bienvenido! Adelante con la integración.** 🚀
