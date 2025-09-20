using ManagementApp.Infrastructure;
using ManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementApp.Endpoints ;

    public static class MotoEndpoints
    {
        public static RouteGroupBuilder MapMotoEndpoints(this RouteGroupBuilder builder)
        {
            builder.WithTags("Moto Endpoints");
            
            //GET ALL
            builder.MapGet("/motos", async (ManagementDb db) =>
                await db.Motos
                    .AsNoTracking()
                    .Include(m => m.Filial)
                    .Select(m => new MotoResponse(
                        m.MotoId,
                        m.Placa,
                        m.Marca,
                        m.Modelo,
                        m.Ano,
                        m.Status,
                        m.Filial.Nome))
                    .ToListAsync());
            
            return builder;
        }
    }