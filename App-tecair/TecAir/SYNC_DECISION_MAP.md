🗺️ MAPA DE DECISIONES: CUÁNDO SINCRONIZAR
═════════════════════════════════════════════════════════════════════════════

```
						¿NECESITAS SINCRONIZAR?
							   │
				┌──────────────┴──────────────┐
				│                             │
				▼                             ▼
		¿OPERACIÓN CRÍTICA?           ¿SOLO LECTURA?
		(Usuario registra,             (Ver vuelos,
		 crea reservación,              promociones,
		 compra millas, etc.)           usuarios)
				│                             │
				▼                             ▼
			  ✅ SÍ                         ✅ NO
				│                             │
				│                             │
		await _autoSyncService             Espera
		.SincronizarAhoraAsync()           1 minuto
				│                             │
				│                             │
		Se sincroniza               AutoSyncService
		INMEDIATAMENTE              se ejecuta
				│                   automáticamente
				│                             │
				└─────────────┬──────────────┘
							  │
							  ▼
					  DATOS SINCRONIZADOS
					(SQLite actualizado)
```


🎯 EJEMPLOS POR CASO DE USO
═════════════════════════════════════════════════════════════════════════════

┌─────────────────────────────────────────────────────────────────────────┐
│ CASO 1: USUARIO SE REGISTRA                                             │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│ 1. Usuario ingresa email, password, nombre                            │
│ 2. Click en "Registrar"                                               │
│ 3. ViewModel llama: await authService.RegisterAsync(...)             │
│ 4. ✅ LLAMAR SINCRONIZACIÓN MANUAL:                                    │
│    await _autoSyncService.SincronizarAhoraAsync()                    │
│ 5. Usuario ve confirmación "Registro completado y sincronizado"      │
│                                                                         │
│ ¿Por qué? El registro es crítico y necesita reflejarse en el servidor  │
│          inmediatamente, sin esperar 1 minuto.                        │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│ CASO 2: USUARIO VE LISTA DE VUELOS                                      │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│ 1. Usuario abre "Buscar Vuelos"                                       │
│ 2. App carga vuelos de SQLite local                                   │
│ 3. ❌ NO LLAMAR sincronización manual                                   │
│ 4. AutoSyncService sincroniza automáticamente cada 1 minuto          │
│ 5. La lista se actualiza automáticamente con nuevos datos            │
│                                                                         │
│ ¿Por qué? Es solo lectura, no requiere sincronización inmediata.     │
│          Los datos se actualizan automáticamente en background.       │
│                                                                         │
│ RESULTADO: Origin, Destination, Price están disponibles para mostrar  │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│ CASO 3: USUARIO CREA RESERVACIÓN                                        │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│ 1. Usuario selecciona vuelo                                           │
│ 2. Click en "Reservar"                                                │
│ 3. ViewModel llama: await reservationService.CreateAsync(...)       │
│ 4. ✅ LLAMAR SINCRONIZACIÓN MANUAL:                                    │
│    await _autoSyncService.SincronizarAhoraAsync()                   │
│ 5. Usuario ve confirmación "Reservación creada y sincronizada"      │
│                                                                         │
│ ¿Por qué? La reservación es crítica y afecta disponibilidad de       │
│          asientos. Necesita sincronización inmediata para evitar     │
│          sobreventa.                                                  │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│ CASO 4: USUARIO CANCELA RESERVACIÓN                                     │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│ 1. Usuario selecciona reservación                                     │
│ 2. Click en "Cancelar"                                                │
│ 3. ViewModel llama: await reservationService.CancelAsync(...)       │
│ 4. ✅ LLAMAR SINCRONIZACIÓN MANUAL:                                    │
│    await _autoSyncService.SincronizarAhoraAsync()                   │
│ 5. Usuario ve confirmación "Reservación cancelada y sincronizada"   │
│                                                                         │
│ ¿Por qué? La cancelación libera un asiento, otros usuarios deben    │
│          verlo disponible inmediatamente.                             │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│ CASO 5: USUARIO VE PROMOCIONES                                          │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│ 1. Usuario abre sección "Promociones"                                 │
│ 2. App carga promociones de SQLite local                             │
│ 3. ❌ NO LLAMAR sincronización manual                                   │
│ 4. AutoSyncService sincroniza automáticamente cada 1 minuto          │
│ 5. Nuevas promociones aparecen automáticamente                       │
│                                                                         │
│ ¿Por qué? Es solo lectura, puede esperar 1 minuto.                  │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│ CASO 6: USUARIO ACTUALIZA PERFIL                                        │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│ 1. Usuario va a "Mi Perfil"                                           │
│ 2. Cambia teléfono, universidad, etc.                                │
│ 3. Click en "Guardar"                                                 │
│ 4. ViewModel llama: await userService.UpdateProfileAsync(...)      │
│ 5. ✅ LLAMAR SINCRONIZACIÓN MANUAL:                                    │
│    await _autoSyncService.SincronizarAhoraAsync()                   │
│ 6. Usuario ve confirmación "Perfil actualizado y sincronizado"      │
│                                                                         │
│ ¿Por qué? Los cambios de perfil son críticos y necesitan             │
│          reflejarse en el servidor inmediatamente.                    │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│ CASO 7: USUARIO COMPRA MILLAS/PUNTOS                                    │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│ 1. Usuario va a "Millas"                                              │
│ 2. Click en "Comprar Millas"                                         │
│ 3. Procesa pago (transacción financiera)                            │
│ 4. ViewModel llama: await milesService.PurchaseAsync(...)           │
│ 5. ✅ LLAMAR SINCRONIZACIÓN MANUAL:                                    │
│    await _autoSyncService.SincronizarAhoraAsync()                   │
│ 6. Usuario ve confirmación "Compra completada y sincronizada"       │
│                                                                         │
│ ¿Por qué? Las transacciones financieras son CRÍTICAS.                │
│          Deben sincronizarse inmediatamente para auditoría y         │
│          seguridad.                                                   │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│ CASO 8: APP EN BACKGROUND                                               │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│ 1. Usuario tiene app abierta                                         │
│ 2. Usuario presiona home (app va a background)                      │
│ 3. App.OnSleep() se ejecuta                                          │
│ 4. AutoSyncService.Stop() se llama                                   │
│ 5. ⏸️ Sincronización automática SE PAUSA                              │
│                                                                         │
│ Usuario abre app nuevamente                                          │
│ 1. App.OnResume() se ejecuta                                         │
│ 2. AutoSyncService.Resume() se llama                                 │
│ 3. ▶️ Sincronización automática SE REANUDA                            │
│ 4. Datos se sincronizar nuevamente                                   │
│                                                                         │
│ ¿Por qué? Ahorrar batería y recursos cuando app no está activa.    │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────┐
│ CASO 9: SIN CONECTIVIDAD DE INTERNET                                    │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│ 1. Usuario abre app sin WiFi/datos                                   │
│ 2. AutoSyncService intenta sincronizar                               │
│ 3. Verifica: Connectivity.Current.NetworkAccess == Internet          │
│ 4. ❌ NO hay internet                                                  │
│ 5. Log: "[AutoSyncService] Sin conexión a Internet. Omitida."       │
│ 6. Espera al siguiente ciclo (1 minuto después)                     │
│ 7. Usuario activa WiFi                                               │
│ 8. En el siguiente ciclo: Sincronización EXITOSA                   │
│                                                                         │
│ ¿Por qué? La app es offline-first. Funciona sin internet.           │
│          Sincroniza cuando vuelve la conectividad.                   │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘


📊 TABLA DE DECISIÓN RÁPIDA
═════════════════════════════════════════════════════════════════════════════

Operación                          │ ¿Sincronizar Ahora? │ ¿Por Qué?
───────────────────────────────────┼────────────────────┼──────────────────
Registrar usuario                  │ ✅ SÍ              │ Crítico
Login                              │ ✅ SÍ              │ Crítico
Crear reservación                  │ ✅ SÍ              │ Afecta disponibilidad
Cancelar reservación               │ ✅ SÍ              │ Libera asientos
Comprar millas                     │ ✅ SÍ              │ Transacción financiera
Actualizar perfil                  │ ✅ SÍ              │ Crítico
Ver lista de vuelos                │ ❌ NO              │ Solo lectura
Ver promociones                    │ ❌ NO              │ Solo lectura
Ver mis reservaciones              │ ❌ NO              │ Solo lectura
Ver mis millas                     │ ❌ NO              │ Solo lectura
Buscar vuelos por ruta             │ ❌ NO              │ Solo lectura
Ver detalles de vuelo              │ ❌ NO              │ Solo lectura
Cambiar idioma                     │ ❌ NO              │ Local solo
Cambiar tema oscuro                │ ❌ NO              │ Local solo


🔄 SINCRONIZACIÓN AUTOMÁTICA (Siempre activa en background)
═════════════════════════════════════════════════════════════════════════════

Cada 1 minuto:
├─ Verifica conectividad
├─ Si hay internet:
│  ├─ Descarga vuelos → Origin, Destination, Price
│  ├─ Descarga promociones
│  ├─ Descarga usuarios
│  └─ Sube cambios locales pendientes
└─ Si NO hay internet:
   └─ Espera al siguiente ciclo


═════════════════════════════════════════════════════════════════════════════
REGLA DE ORO: Si tu operación modificó datos críticos,
			  SIEMPRE llama await _autoSyncService.SincronizarAhoraAsync()
═════════════════════════════════════════════════════════════════════════════
