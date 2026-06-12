🚨 ANÁLISIS COMPLETO: PROBLEMAS CON ROUTE.ID
════════════════════════════════════════════════════════════════════════════

## ⚠️ PROBLEMA RAÍZ IDENTIFICADO

**Flight.RouteId almacena el IdRuta del API, no el Route.Id local**

```
SyncService.DescargarVuelosAsync() línea 141:
	RouteId = vueloApi.IdRuta  ← Esto es el ID del API (ej: 15)

Pero Route.Id es autogenerado localmente en SQLite:
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }  ← Esto genera 1, 2, 3... localmente

Resultado: Flight.RouteId (15) nunca coincide con Route.Id (1, 2, 3...)
```

## 📍 LUGARES DONDE ESTO CAUSA PROBLEMAS

### 1. FlightViewModel.SearchFlightsAsync() ✅ YA CORREGIDO
```csharp
// ANTES (FALLABA):
var route = await _databaseService.GetRouteByAirportsAsync(SelectedOrigin.Id, SelectedDestination.Id);
if (route == null) return; // Encontraba ruta (ej: Route.Id = 3)
var flights = await _databaseService.GetFlightsByRouteAsync(route.Id);
// Buscaba: WHERE RouteId = 3, pero Flight.RouteId = 15 → 0 resultados

// AHORA (FUNCIONA):
var flights = await _databaseService.GetFlightsByOriginDestinationAsync(origin, destination);
// Busca directamente por Origin/Destination sincronizados
```

### 2. ReservationDetailViewModel.LoadFlightDetailsAsync() ❌ AÚN FALLARÁ
```csharp
// FALLARÁ:
_route = await _databaseService.GetRouteByIdAsync(SelectedFlight.RouteId);
// Intenta buscar Route.Id = 15 (del API)
// Pero Routes locales solo tienen id 1, 2, 3...
// Resultado: _route = null → OriginAirport y DestinationAirport serán null

// CONSECUENCIA:
OriginAirport = await _databaseService.GetAirportByIdAsync(_route.OriginAirportId);
// NullReferenceException o OriginAirport = null
```

### 3. Cualquier otro código que use GetRouteByIdAsync(flight.RouteId)
```csharp
// Mismo problema en cualquier lugar que intente:
var route = await _databaseService.GetRouteByIdAsync(flight.RouteId);
// Fallará porque RouteId es del API, no del local
```

## 🔧 SOLUCIONES POSIBLES

### Opción A: Agregar ApiId a Route (Recomendado a largo plazo)
```csharp
[Table("Routes")]
public class Route
{
	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }  // ID local autogenerado

	// AGREGAR:
	public int ApiId { get; set; } = 0;  // ID del API

	[NotNull]
	public int OriginAirportId { get; set; }
	[NotNull]
	public int DestinationAirportId { get; set; }
	[NotNull]
	public decimal BasePrice { get; set; }
	public int Duration { get; set; }
	public string IntermediateAirports { get; set; }
}
```

Luego:
1. Implementar DescargarRutasAsync() en SyncService
2. Descargar rutas del API con RouteId = vueloApi.IdRuta
3. Mapear Route.ApiId = vueloApi.IdRuta
4. Usar GetRouteByApiIdAsync(flight.RouteId) en lugar de GetRouteByIdAsync()

### Opción B: Usar Origin/Destination directamente (Solución inmediata)
```csharp
// En ReservationDetailViewModel.LoadFlightDetailsAsync():
// CAMBIAR ESTO:
_route = await _databaseService.GetRouteByIdAsync(SelectedFlight.RouteId);
OriginAirport = await _databaseService.GetAirportByIdAsync(_route.OriginAirportId);
DestinationAirport = await _databaseService.GetAirportByIdAsync(_route.DestinationAirportId);
BaseCost = _route.BasePrice;

// POR ESTO:
// Usar Origin/Destination sincronizados en Flight
var originAirport = await _databaseService.GetAirportByCodeAsync(
	SelectedFlight.Origin.Split(' ')[0]); // "SJO" de "SJO - Juan Santamaría"

var destinationAirport = await _databaseService.GetAirportByCodeAsync(
	SelectedFlight.Destination.Split(' ')[0]); // "LIR" de "LIR - Guanacaste"

OriginAirport = originAirport;
DestinationAirport = destinationAirport;
BaseCost = SelectedFlight.Price;  // Ya sincronizado
```

## 🎯 RECOMENDACIÓN

**Implementar Opción B AHORA (solución rápida)**
**Planificar Opción A para DESPUÉS (solución completa)**

## ✅ CAMBIOS REALIZADOS HASTA AHORA

1. ✅ FlightViewModel.SearchFlightsAsync()
   - Usa GetFlightsByOriginDestinationAsync() en lugar de RouteId
   - Corrige duplicación de código de aeropuerto

## 🔄 CAMBIOS PENDIENTES

1. ❌ ReservationDetailViewModel.LoadFlightDetailsAsync()
   - Necesita usar Origin/Destination en lugar de RouteId
   - O esperar a DescargarRutasAsync()

2. ❌ Implementar DescargarRutasAsync() en SyncService
   - Descargar rutas del API: GET /api/rutas
   - Mapear ApiId correctamente

3. ❌ Agregar Route.ApiId
   - Persistir el ID del API en la tabla Routes

4. ❌ Crear GetRouteByApiIdAsync()
   - Buscar rutas por ApiId

## 📋 CÓDIGO PARA FIXEAR ReservationDetailViewModel

```csharp
public async Task LoadFlightDetailsAsync(int flightId)
{
	try
	{
		IsBusy = true;

		// Obtener vuelo
		SelectedFlight = await _databaseService.GetFlightByIdAsync(flightId);
		if (SelectedFlight == null)
		{
			await Application.Current.MainPage.DisplayAlert("Error", "Vuelo no encontrado", "OK");
			return;
		}

		// OPCIÓN B: Usar Origin/Destination sincronizados
		// Extraer códigos de aeropuerto (primer token)
		var originCode = SelectedFlight.Origin.Split(' ')[0];  // "SJO"
		var destinationCode = SelectedFlight.Destination.Split(' ')[0];  // "LIR"

		// Buscar aeropuertos por código
		OriginAirport = await _databaseService.GetAirportByCodeAsync(originCode);
		DestinationAirport = await _databaseService.GetAirportByCodeAsync(destinationCode);

		// Usar precio sincronizado del vuelo
		BaseCost = SelectedFlight.Price;

		UpdateTotalCost();
	}
	catch (Exception ex)
	{
		await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar detalles: {ex.Message}", "OK");
	}
	finally
	{
		IsBusy = false;
	}
}
```

## 📊 RESUMEN

| Ubicación | Problema | Estado | Solución |
|-----------|----------|--------|----------|
| FlightViewModel.SearchFlightsAsync() | Usa RouteId inconsistente | ✅ FIJO | Usa Origin/Destination |
| ReservationDetailViewModel | Usa RouteId inconsistente | ❌ PENDIENTE | Usa Origin/Destination |
| SyncService | No descarga rutas | ❌ PENDIENTE | Implementar DescargarRutasAsync() |
| Route modelo | No tiene ApiId | ❌ PENDIENTE | Agregar ApiId field |

════════════════════════════════════════════════════════════════════════════
ACCIÓN INMEDIATA: Fixear ReservationDetailViewModel también
ACCIÓN FUTURA: Implementar descarga de rutas del API
════════════════════════════════════════════════════════════════════════════
