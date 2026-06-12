🏗️ ARQUITECTURA DE SINCRONIZACIÓN
==================================

## FLUJO DE DATOS

┌─────────────────────────────────────────────────────────────────┐
│                     API REMOTO (PostgreSQL)                      │
│  GET /api/vuelos → [{id_vuelo, origen, destino, precio, ...}]   │
└──────────────────────────┬──────────────────────────────────────┘
						   │
						   ↓
┌─────────────────────────────────────────────────────────────────┐
│              SyncService.DescargarVuelosAsync()                   │
│                                                                   │
│  1. Descarga JSON del API                                        │
│  2. Deserializa a VueloApiDto                                    │
│  3. Mapea a modelo Flight                                        │
│     - Origen → Origin                                            │
│     - Destino → Destination                                      │
│     - Precio → Price                                             │
│  4. Crea o actualiza en SQLite                                   │
└──────────────────────────┬──────────────────────────────────────┘
						   │
						   ↓
┌─────────────────────────────────────────────────────────────────┐
│                    SQLite LOCAL (tecair.db)                      │
│  Table: Flights                                                  │
│  - Id, FlightNumber, RouteId, AircraftId                         │
│  - DepartureTime, ArrivalTime, Status, AvailableSeats            │
│  - ApiId                                                         │
│  - Origin (NUEVO) ← "SJO - Juan Santamaría"                     │
│  - Destination (NUEVO) ← "LIR - Guanacaste"                     │
│  - Price (NUEVO) ← 105.00                                        │
└──────────────────────────┬──────────────────────────────────────┘
						   │
						   ↓
┌─────────────────────────────────────────────────────────────────┐
│               VISTAS (Pages, ViewModels)                          │
│  Mostrar vuelos con origen, destino y precio                     │
└─────────────────────────────────────────────────────────────────┘


## FLUJO DE SINCRONIZACIÓN EN TIEMPO REAL

	┌─ AutoSyncService.Start() (MauiProgram.cs)
	│
	├─→ Timer cada 1 minuto (60000 ms)
	│   │
	│   └─→ ExecuteSyncCycleAsync()
	│       │
	│       ├─ Verificar: Connectivity.Current.NetworkAccess == Internet
	│       │
	│       ├─ Si SÍ hay internet:
	│       │   └─→ SyncService.SincronizarAsync()
	│       │       └─→ DescargarVuelosAsync() ← Mapea Origin, Destination, Price
	│       │
	│       └─ Si NO hay internet:
	│           └─→ Omitir sincronización
	│
	└─ OnSleep(): AutoSyncService.Stop()    [Pausa en background]
	└─ OnResume(): AutoSyncService.Resume()  [Reanuda en foreground]


## CICLO DE VIDA DE LA APLICACIÓN

	App Inicia
		 ↓
	MauiProgram.CreateMauiApp()
		 ├─ Registra servicios
		 ├─ Crea tablas SQLite
		 └─ Inicia AutoSyncService.Start()
					  ↓
	Usuario usa app
		 ├─ Sincronización automática cada 1 minuto (background)
		 └─ Operaciones manuales: await _autoSyncService.SincronizarAhoraAsync()
					  ↓
	App.OnSleep() (usuario minimiza app)
		 └─ AutoSyncService.Stop()  [Pausa]
					  ↓
	App.OnResume() (usuario abre app)
		 └─ AutoSyncService.Resume()  [Reanuda]
					  ↓
	App Cierra
		 └─ AutoSyncService.Dispose()


## MAPEO DE DATOS

VueloApiDto (del API)      →      Flight (SQLite)
────────────────────────────────────────────────
IdVuelo                    →      ApiId
FechaSalida + HoraSalida   →      DepartureTime
Estado                     →      Status
Matricula                  →      AircraftId (relacionado)
IdRuta                     →      RouteId
AsientosDisponibles        →      AvailableSeats
Origen              (NUEVO)→      Origin
Destino             (NUEVO)→      Destination
Precio              (NUEVO)→      Price


## COMPONENTES

┌───────────────────────────────────────────────────────────────┐
│                    SERVICIOS (Dependency Injection)           │
├───────────────────────────────────────────────────────────────┤
│                                                               │
│ 1. DatabaseService                                           │
│    ├─ Gestiona SQLite local                                 │
│    ├─ Crear/Leer/Actualizar registros                       │
│    └─ CreateTableAsync<Flight>() → Crea columnas            │
│                                                               │
│ 2. SyncService                                               │
│    ├─ Descarga datos del API                                │
│    ├─ Mapea a modelos locales                               │
│    ├─ Sincroniza SQLite con PostgreSQL                      │
│    └─ SincronizarAsync() → (bool exito, string mensaje)     │
│                                                               │
│ 3. AutoSyncService (NUEVO)                                  │
│    ├─ Ejecuta sincronización cada 1 minuto                  │
│    ├─ Verifica conectividad                                 │
│    ├─ Pausa en background / Reanuda en foreground           │
│    ├─ Start()                                               │
│    ├─ Stop()                                                │
│    ├─ Resume()                                              │
│    └─ SincronizarAhoraAsync() → Sincronización inmediata   │
│                                                               │
└───────────────────────────────────────────────────────────────┘

┌───────────────────────────────────────────────────────────────┐
│                    MODELOS                                    │
├───────────────────────────────────────────────────────────────┤
│                                                               │
│ Flight.cs (SQLite)                                          │
│ ├─ Propiedades existentes                                   │
│ ├─ + Origin (string) = ""                                   │
│ ├─ + Destination (string) = ""                              │
│ └─ + Price (decimal)                                        │
│                                                               │
│ VueloApiDto (DTO del API)                                   │
│ ├─ Propiedades existentes                                   │
│ ├─ + Origen (string) = ""                                   │
│ └─ + Destino (string) = ""                                  │
│                                                               │
└───────────────────────────────────────────────────────────────┘

┌───────────────────────────────────────────────────────────────┐
│                 PUNTOS DE SINCRONIZACIÓN                      │
├───────────────────────────────────────────────────────────────┤
│                                                               │
│ 1. Automática (cada 1 minuto)                               │
│    - AutoSyncService ejecuta SincronizarAsync()             │
│                                                               │
│ 2. Manual Inmediata                                          │
│    - await _autoSyncService.SincronizarAhoraAsync()         │
│    - Después de registrar usuario                           │
│    - Después de crear reservación                           │
│    - Después de cancelar reservación                        │
│    - Operaciones críticas/financieras                       │
│                                                               │
│ 3. Al iniciar app                                            │
│    - DatabaseService.InitializeAsync()                      │
│    - AutoSyncService.Start()                                │
│                                                               │
│ 4. Ciclo de vida                                             │
│    - OnSleep() → AutoSyncService.Stop()                     │
│    - OnResume() → AutoSyncService.Resume()                  │
│                                                               │
└───────────────────────────────────────────────────────────────┘


## MANEJO DE ERRORES

SyncService.SincronizarAsync()
	│
	├─ SI: Éxito
	│  └─→ (true, "Vuelos: 25, Promociones: 5, Usuarios: 1")
	│
	├─ SI: Sin internet
	│  └─→ AutoSyncService omite ciclo
	│
	├─ SI: Error en request
	│  └─→ (false, "Error descargando vuelos")
	│
	└─ SI: Exception
	   └─→ Debug.WriteLine + (false, mensaje)
		   [Nunca interrumpe la app]


## LOGS EN OUTPUT WINDOW

[AutoSyncService] Iniciando servicio de sincronización automática...
[MauiProgram] Inicialización completada - Sincronización automática iniciada
[AutoSyncService] Iniciando ciclo de sincronización...
[AutoSyncService] Sincronización exitosa: Vuelos: 25, Promociones: 5, Usuarios: 1
Vuelos sincronizados: 25
[AutoSyncService] Servicio de sincronización automática detenido.
[AutoSyncService] Reanudando sincronización automática...


¡Sistema de sincronización completamente implementado! 🚀
