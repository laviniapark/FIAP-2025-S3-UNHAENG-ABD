using System.ComponentModel;
using IdempotentAPI.MinimalAPI;
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
                    .ToListAsync())
                .WithSummary("Retorna a lista de todas as motos")
                .WithDescription("Retorna a lista de todas as motos, juntamente com o nome da Filial que ela pertence")
                .Produces<List<MotoResponse>>(StatusCodes.Status200OK);
            
            // GET BY ID
            builder.MapGet("/motos/{id:guid}",
                async (ManagementDb db, [Description("Identificador unico da Moto")] Guid id) =>
                {
                    var moto = await db.Motos
                        .AsNoTracking()
                        .Include(m => m.Filial)
                        .FirstOrDefaultAsync(m => m.MotoId == id);

                    if (moto is null)
                        return Results.NotFound(new { message = "Moto não encontrada", id });

                    var response = new MotoResponse(
                        moto.MotoId,
                        moto.Placa,
                        moto.Marca,
                        moto.Modelo,
                        moto.Ano,
                        moto.Status,
                        moto.Filial.Nome
                        );
                    
                    return Results.Ok(response);
                }).WithSummary("Retorna uma moto pelo ID")
                .WithDescription("Retorna uma moto buscando pelo ID, caso não exista, retorna um erro 404 (Nao Encontrado)")
                .Produces<MotoResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            
            // GET BY PLACA
            builder.MapGet("/motos/{placa}", async (ManagementDb db, [Description("Placa da Moto")] string placa) =>
            {
                
                var p = placa.Trim().ToUpperInvariant();
                
                var moto = await db.Motos
                    .AsNoTracking()
                    .Include(m => m.Filial)
                    .FirstOrDefaultAsync(m => m.Placa == p);

                if (moto is null)
                    return Results.NotFound(new { message = "Moto não encontrada", placa });

                var response = new MotoResponse(
                    moto.MotoId,
                    moto.Placa,
                    moto.Marca,
                    moto.Modelo,
                    moto.Ano,
                    moto.Status,
                    moto.Filial.Nome
                    );

                return Results.Ok(response);
            })
                .WithSummary("Retorna uma moto pela placa")
                .WithDescription("Retorna uma moto pela placa. " +
                                 "Caso não exista, retorna um erro 404 (Não Encontrado)")
                .Produces<MotoResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            
            // POST
            builder.MapPost("/motos", async (HttpContext http, MotoRequest request) =>
            {
                var db = http.RequestServices.GetRequiredService<ManagementDb>();
                
                if (!await db.Filiais.AnyAsync(f => f.FilialId == request.FilialId))
                    return Results.BadRequest(new { message = "Filial de destino inválida ou inexistente.", request.FilialId });


                var moto = new Moto
                {
                    Placa = request.Placa,
                    Marca = request.Marca,
                    Modelo = request.Modelo,
                    Ano = request.Ano,
                    Status = request.Status,
                    FilialId = request.FilialId
                };

                db.Motos.Add(moto);
                await db.SaveChangesAsync();
                
                var filialNome = await db.Filiais
                    .Where(f => f.FilialId == moto.FilialId)
                    .Select(f => f.Nome)
                    .FirstAsync();

                var response = new MotoResponse(
                    moto.MotoId,
                    moto.Placa,
                    moto.Marca,
                    moto.Modelo,
                    moto.Ano,
                    moto.Status,
                    filialNome
                    );

                return Results.Created($"/motos/{response.Motoid}", response);
            })
                .AddEndpointFilter<IdempotentAPIEndpointFilter>()
                .WithSummary("Cadastra uma nova moto")
                .WithDescription("Cadastra uma nova moto no sistema. " +
                                 "Se o cadastro for concluído com sucesso, será possível ver " +
                                 "o caminho com ID para pesquisas. " +
                                 "Caso a filial não exista, retorna um erro 400.")
                .Produces<MotoResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            
            // PUT
            builder.MapPut("/motos/{id:guid}",
                async ([Description("Identificador unico da moto")] Guid id, MotoRequest request, ManagementDb db) =>
                {

                    var moto = await db.Motos
                        .FirstOrDefaultAsync(m => m.MotoId == id);

                    if (moto is null)
                        return Results.NotFound(new { message = "Moto não encontrada", id });

                    if (!await db.Filiais.AnyAsync(f => f.FilialId == request.FilialId))
                        return
                            Results.BadRequest(
                                new { message = "Filial de destino inválida ou inexistente.", request.FilialId });

                    moto.Placa = request.Placa;
                    moto.Marca = request.Marca;
                    moto.Modelo = request.Modelo;
                    moto.Ano = request.Ano;
                    moto.Status = request.Status;
                    moto.FilialId = request.FilialId;

                    await db.SaveChangesAsync();
                    return Results.NoContent();


                })
                .WithSummary("Atualiza os dados de uma moto existente")
                .WithDescription("Atualiza os dados de uma moto existente buscando pelo seu ID. " +
                                 "Caso o ID passado esteja incorreto ou não exista, retorna um erro 404. " +
                                 "Caso a filial não exista, retorna um erro 400.")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status400BadRequest);
            
            // DELETE
            builder.MapDelete("/motos/{id:guid}", async
                ([Description("Identificador unico da moto")] Guid id, ManagementDb db) =>
                {
                    var moto = await db.Motos.FindAsync(id);
                    if (moto is null)
                        return Results.NotFound(new { message = "Moto não encontrada", id });

                    db.Motos.Remove(moto);
                    await db.SaveChangesAsync();
                    return Results.NoContent();
                })
                .WithSummary("Deleta uma moto existente")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
            
            
            
            return builder;
        }
    }