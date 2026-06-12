📝 REFERENCIA DE CAMBIOS DE CÓDIGO
==================================

## 1. Flight.cs (MODIFICADO)

Ubicación: TecAir/Models/Flight.cs

AGREGADAS estas propiedades al final de la clase:

	// Información del origen del vuelo
	public string Origin { get; set; } = "";

	// Información del destino del vuelo
	public string Destination { get; set; } = "";

	// Precio del vuelo
	public decimal Price { get; set; }


## 2. VueloApiDto (MODIFICADO)

Ubicación: TecAir/Services/SyncService.cs
(Línea aproximada: 495-510)

AGREGADAS estas propiedades al final de la clase:

	public string Origen { get; set; } = "";
	public string Destino { get; set; } = "";


## 3. DescargarVuelosAsync() - Creación de Vuelo (MODIFICADO)

Ubicación: TecAir/Services/SyncService.cs
(Línea aproximada: 130-147)

CAMBIO 1 - Cuando se crea un vuelo nuevo:

await _databaseService.CreateFlightAsync(new Flight
{
	ApiId = vueloApi.IdVuelo,
	FlightNumber = $"TEC-{vueloApi.IdVuelo:D3}",
	RouteId = vueloApi.IdRuta,
	DepartureTime = DateTime.Parse($"{vueloApi.FechaSalida} {vueloApi.HoraSalida}"),
	ArrivalTime = DateTime.Parse($"{vueloApi.FechaSalida} {vueloApi.HoraSalida}"),
	Status = MapearEstado(vueloApi.Estado),
	AvailableSeats = vueloApi.AsientosDisponibles,
	Origin = vueloApi.Origen,              // ← LÍNEA NUEVA
	Destination = vueloApi.Destino,        // ← LÍNEA NUEVA
	Price = vueloApi.Precio,               // ← LÍNEA NUEVA
});


## 4. DescargarVuelosAsync() - Actualización de Vuelo (MODIFICADO)

Ubicación: TecAir/Services/SyncService.cs
(Línea aproximada: 149-156)

CAMBIO 2 - Cuando se actualiza un vuelo existente:

vueloLocal.Status = MapearEstado(vueloApi.Estado);
vueloLocal.AvailableSeats = vueloApi.AsientosDisponibles;
vueloLocal.Origin = vueloApi.Origen;              // ← LÍNEA NUEVA
vueloLocal.Destination = vueloApi.Destino;        // ← LÍNEA NUEVA
vueloLocal.Price = vueloApi.Precio;               // ← LÍNEA NUEVA
await _databaseService.UpdateFlightAsync(vueloLocal);


## 5. AutoSyncService.cs (CREADO - YA ESTÁ HECHO)

Ubicación: TecAir/Services/AutoSyncService.cs

Este archivo ya está creado con:
- ✅ Sincronización automática cada 1 minuto
- ✅ Verificación de conectividad
- ✅ Pausa en background, reanuda en foreground
- ✅ Método SincronizarAhoraAsync() para sincronización manual


## 6. MauiProgram.cs (MODIFICADO - YA ESTÁ HECHO)

Ubicación: TecAir/MauiProgram.cs

Cambios realizados:
- ✅ Registrado AutoSyncService como Singleton
- ✅ AutoSyncService inicializado en MainThread
- ✅ Se inicia automáticamente


## 7. App.xaml.cs (MODIFICADO - YA ESTÁ HECHO)

Ubicación: TecAir/App.xaml.cs

Cambios realizados:
- ✅ Agregados métodos OnResume() y OnSleep()
- ✅ Resume/Stop de AutoSyncService según ciclo de vida


## INYECCIÓN DE DEPENDENCIAS EN VISTAS

Para usar AutoSyncService en tus ViewModels o Pages:

### En ViewModel:

public class MiViewModel
{
	private readonly AutoSyncService _autoSyncService;

	public MiViewModel(AutoSyncService autoSyncService)
	{
		_autoSyncService = autoSyncService;
	}

	public async Task MiOperacion()
	{
		// ... tu código ...
		await _autoSyncService.SincronizarAhoraAsync();
	}
}

### En Page (XAML.cs):

public partial class MiPage : ContentPage
{
	private readonly AutoSyncService _autoSyncService;

	public MiPage(AutoSyncService autoSyncService)
	{
		InitializeComponent();
		_autoSyncService = autoSyncService;
	}

	private async void OnButtonClicked(object sender, EventArgs e)
	{
		await _autoSyncService.SincronizarAhoraAsync();
	}
}

### Registrar en MauiProgram.cs:

builder.Services.AddSingleton<MiPage>(sp =>
	new MiPage(sp.GetRequiredService<AutoSyncService>()));


## EJEMPLO COMPLETO: CREAR RESERVACIÓN

```csharp
public async Task CreateReservationAsync(int flightId, int passengerId)
{
	try
	{
		// 1. Crear la reservación
		var reservation = new Reservation
		{
			FlightId = flightId,
			PassengerId = passengerId,
			ReservationDate = DateTime.Now,
			Status = 0
		};

		var createdReservation = await _databaseService.CreateReservationAsync(reservation);

		// 2. Sincronizar inmediatamente
		await _autoSyncService.SincronizarAhoraAsync();

		// 3. Mostrar confirmación
		await MainThread.InvokeOnMainThreadAsync(async () =>
		{
			await Application.Current?.MainPage?.DisplayAlert(
				"Éxito",
				$"Reservación #{createdReservation.Id} creada y sincronizada",
				"OK");
		});
	}
	catch (Exception ex)
	{
		Debug.WriteLine($"Error creando reservación: {ex.Message}");
		await Application.Current?.MainPage?.DisplayAlert(
			"Error",
			"No se pudo crear la reservación",
			"OK");
	}
}
```


## VERIFICACIÓN

✅ Compilación: Debe compilar sin errores
✅ Flight.cs: Debe tener Origin, Destination, Price
✅ VueloApiDto: Debe tener Origen, Destino
✅ SyncService: Debe mapear los 3 campos tanto en creación como en actualización
✅ AutoSyncService: Debe ejecutarse cada minuto y manejar conectividad


## LOGS ESPERADOS (Output Window)

[AutoSyncService] Iniciando servicio de sincronización automática...
[AutoSyncService] Iniciando ciclo de sincronización...
[AutoSyncService] Sincronización exitosa: Vuelos: 25, Promociones: 5, Usuarios: 1
Vuelos sincronizados: 25


¡Listo para usar! 🎉
