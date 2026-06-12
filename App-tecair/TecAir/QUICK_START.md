🚀 QUICK START: Sincronización de Vuelos en TECAir
==================================================

## CAMBIOS REALIZADOS

✅ Flight.cs (Modelo SQLite)
   - Agregadas: Origin, Destination, Price

✅ VueloApiDto (DTO del API)
   - Agregadas: Origen, Destino
   - Ya existía: Precio

✅ SyncService.DescargarVuelosAsync()
   - Mapea Origen → Origin
   - Mapea Destino → Destination
   - Mapea Precio → Price

✅ Sincronización automática cada 1 minuto
   - Configurada en AutoSyncService
   - Inicia automáticamente en MauiProgram.cs
   - Se pausa en background, reanuda en foreground


## CÓMO USAR EN TU CÓDIGO

### En un ViewModel o Service:

```csharp
public class ReservationViewModel
{
	private readonly AutoSyncService _autoSyncService;

	public ReservationViewModel(AutoSyncService autoSyncService)
	{
		_autoSyncService = autoSyncService;
	}

	public async Task CreateReservation(Reservation reservation)
	{
		// 1. Crear reservación
		await _reservationService.CreateAsync(reservation);

		// 2. Sincronizar inmediatamente
		await _autoSyncService.SincronizarAhoraAsync();

		// 3. Mostrar resultado
		await Application.Current?.MainPage?.DisplayAlert(
			"Éxito", 
			"Reservación creada y sincronizada", 
			"OK");
	}
}
```

### En una Page (XAML.cs):

```csharp
public partial class ReservationDetailPage : ContentPage
{
	private readonly AutoSyncService _autoSyncService;

	public ReservationDetailPage(AutoSyncService autoSyncService)
	{
		InitializeComponent();
		_autoSyncService = autoSyncService;
	}

	private async void OnConfirmClicked(object sender, EventArgs e)
	{
		// Confirmar y sincronizar
		await _autoSyncService.SincronizarAhoraAsync();
		await DisplayAlert("Éxito", "Sincronizado", "OK");
	}
}
```


## DATOS QUE AHORA SE SINCRONIZAN

Cada vuelo descargado incluye:
- 🛫 Origin: "SJO - Juan Santamaría"
- 🛬 Destination: "LIR - Guanacaste"  
- 💰 Price: 105.00


## CUANDO SE SINCRONIZA

✅ **Automáticamente (cada 1 minuto)**
   - Al iniciar app
   - En background cada minuto
   - Se pausa cuando app está minimizada

✅ **Manualmente (inmediato)**
   - Después de registrar usuario: await _autoSyncService.SincronizarAhoraAsync();
   - Después de crear reservación: await _autoSyncService.SincronizarAhoraAsync();
   - Después de cancelar reservación: await _autoSyncService.SincronizarAhoraAsync();


## VERIFICAR QUE FUNCIONA

1. En Output Window (Debug):
   Busca mensajes como:
   - "[AutoSyncService] Ciclo de sincronización iniciado..."
   - "Vuelos sincronizados: X"

2. En SQLite:
   - Abre tecair.db
   - Tabla Flights tendrá columnas: Origin, Destination, Price
   - Los vuelos tendrán datos completos

3. En la app:
   - Los vuelos mostrarán origen, destino y precio
   - Los datos se actualizan cada minuto


## ARCHIVOS MODIFICADOS

1. TecAir/Models/Flight.cs
   - +3 propiedades

2. TecAir/Services/SyncService.cs
   - VueloApiDto: +2 propiedades
   - DescargarVuelosAsync(): +3 líneas de mapeo en creación
   - DescargarVuelosAsync(): +3 líneas de mapeo en actualización

3. TecAir/Services/AutoSyncService.cs
   - ✅ Ya implementado

4. TecAir/MauiProgram.cs
   - ✅ Ya configurado

5. TecAir/App.xaml.cs
   - ✅ Ya configurado con ciclo de vida


## ARCHIVOS DE REFERENCIA

- TecAir/Examples/SyncExamples.cs
  Ejemplos completos de sincronización manual

- TecAir/SYNC_CHANGES_SUMMARY.txt
  Resumen detallado de todos los cambios


## PRÓXIMOS PASOS

1. ✅ Compilar (ya está listo)
2. Probar en emulador/dispositivo
3. Verificar que SQLite tiene las columnas
4. Agregar mostrar origen/destino en vistas
5. Agregar SincronizarAhoraAsync() en operaciones críticas


¡Todo está listo para usar! 🎉
