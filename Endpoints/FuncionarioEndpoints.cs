using ManagementApp.Infrastructure;
using ManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementApp.Endpoints ;

    public static class FuncionarioEndpoints
    {
        public static RouteGroupBuilder MapFuncionarioEndpoints(this RouteGroupBuilder builder)
        {
            builder.WithTags("Funcionario Endpoints");
            
            //GET ALL
            builder.MapGet("/funcionarios", async (ManagementDb db) =>
                await db.Funcionarios
                    .AsNoTracking()
                    .Include(f => f.Filial)
                    .Select(f => new FuncionarioResponse(
                        f.FuncionarioId,
                        f.NomeCompleto,
                        f.Cpf,
                        f.Cargo,
                        f.Ativo,
                        f.Filial.Nome
                        ))
                    .ToListAsync());
            
            return builder;
        }
    }