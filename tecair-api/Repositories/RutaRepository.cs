using TecAir.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using TecAir.Api.Data;
using TecAir.Api.Interfaces;
using TecAir.Api.Models;

namespace TecAir.Api.Repositories;

/*
Descripción:
Implementa la persistencia de rutas y escalas en EF Core.
Entradas:
Recibe TecAirDb y entidades del dominio de rutas.
Salidas:
Retorna estructuras detalladas de rutas para respuesta.
Restricciones:
No contiene validaciones de negocio complejas.
*/
public sealed class RutaRepository(TecAirDb db) : IRutaRepository
{
    public async Task<List<RutaResponse>> GetRutasDetalladasAsync()
    {
        var rutas = await db.Rutas.AsNoTracking()
            .Select(x => x.IdRuta)
            .OrderBy(x => x)
            .ToListAsync();
        var resultado = new List<RutaResponse>();

        foreach (var idRuta in rutas)
        {
            resultado.Add(await GetRutaDetalladaAsync(idRuta));
        }

        return resultado;
    }

    public async Task<RutaResponse> GetRutaDetalladaAsync(int idRuta)
    {
        var escalas = await db.Escalas
            .AsNoTracking()
            .Where(x => x.IdRuta == idRuta)
            .OrderBy(x => x.Orden)
            .Join(db.Aeropuertos.AsNoTracking(),
                e => e.IdAeropuerto,
                a => a.IdAeropuerto,
                (e, a) => new
                {
                    id_aeropuerto = a.IdAeropuerto,
                    nombre = a.Nombre ?? string.Empty,
                    ubicacion = a.Ubicacion ?? string.Empty,
                    orden = e.Orden,
                    tipo = e.Tipo
                })
            .ToListAsync();

        return new RutaResponse(idRuta, escalas.Cast<object>().ToList());
    }

    public async Task AddRutaAsync(Ruta ruta) => await db.Rutas.AddAsync(ruta);

    public async Task AddEscalaAsync(Escala escala) => await db.Escalas.AddAsync(escala);

    public async Task SaveChangesAsync() => await db.SaveChangesAsync();
}
