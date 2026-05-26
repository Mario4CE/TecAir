using TecAir.Contracts.Dtos;
using TecAir.Application.Interfaces;
using TecAir.Domain.Models;

namespace TecAir.Application.Services;

/*
Descripción:
Implementa las reglas de negocio del módulo de rutas.
Entradas:
Recibe IRutaRepository y DTOs del endpoint.
Salidas:
Retorna rutas detalladas para respuesta de API.
Restricciones:
Debe asegurar al menos origen y destino por ruta creada.
*/
public sealed class RutaService(IRutaRepository rutaRepository) : IRutaService
{
    public async Task<List<RutaResponse>> GetRutasAsync() => await rutaRepository.GetRutasDetalladasAsync();

    public async Task<RutaResponse> CrearRutaAsync(RutaRequest datos)
    {
        if (datos.Escalas.Count < 2)
            throw new InvalidOperationException("Debe indicar al menos origen y destino en escalas.");

        var ruta = new Ruta();
        await rutaRepository.AddRutaAsync(ruta);
        await rutaRepository.SaveChangesAsync();

        for (var index = 0; index < datos.Escalas.Count; index++)
        {
            var escala = datos.Escalas[index];
            await rutaRepository.AddEscalaAsync(new Escala
            {
                IdRuta = ruta.IdRuta,
                Orden = escala.Orden ?? index + 1,
                IdAeropuerto = escala.IdAeropuerto,
                Tipo = escala.Tipo ?? (index == 0 ? "origen" : index == datos.Escalas.Count - 1 ? "destino" : "escala")
            });
        }

        await rutaRepository.SaveChangesAsync();
        return await rutaRepository.GetRutaDetalladaAsync(ruta.IdRuta);
    }
}
