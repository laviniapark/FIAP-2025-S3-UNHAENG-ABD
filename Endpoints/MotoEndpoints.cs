using System.ComponentModel;
using IdempotentAPI.MinimalAPI;
using ManagementApp.Infrastructure;
using ManagementApp.Infrastructure.Pagination;
using ManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementApp.Endpoints ;

    public static class MotoEndpoints
    {
        public static RouteGroupBuilder MapMotoEndpoints(this RouteGroupBuilder builder)
        {
            var group = builder.MapGroup("/motos")
                .WithTags("Moto Endpoints");
            
            //GET ALL
            group.MapGet("", async ([AsParameters] PageParameters pageParam, ManagementDb db) =>
            {
                var page = pageParam.PageNumber < 1 ? 1 : pageParam.PageNumber;
                var size = pageParam.PageSize is < 1 or > 100 ? 20 : pageParam.PageSize;
                
                var query = db.Motos
                    .AsNoTracking()
                    .OrderBy(m => m.Placa)
                    .Include(m => m.Filial)
                    .Select(m => new MotoResponseGA(
                        m.MotoId,
                        m.Placa,
                        m.Marca,
                        m.Modelo,
                        m.Ano,
                        m.Status,
                        m.Filial.Nome));

                var paged = await PagedList<MotoResponseGA>.CreateAsync(query, page, size);

                return Results.Ok(paged);
            }).WithSummary("Retorna a lista paginada de Motos")
                .WithDescription("Retorna a lista paginada de motos ordenada por Placa, " + 
                                 "podendo ser definido a quantidade a ser mostrada por página. " +
                                 "Dados informados: informaçao das motos , numero da pagina, " +
                                 "quantidade de motos por pagina, quantidade total de motos cadastradas, " +
                                 "se possui proxima pagina e se possui pagina anterior.")
                .Produces<List<MotoResponse>>(StatusCodes.Status200OK);
            
            // GET BY ID
            group.MapGet("/{id:guid}",
                async (ManagementDb db, [Description("Identificador unico da Moto")] Guid id, LinkGenerator lg, HttpContext http) =>
                {
                    var moto = await db.Motos
                        .AsNoTracking()
                        .Include(m => m.Filial)
                        .FirstOrDefaultAsync(m => m.MotoId == id);

                    if (moto is null)
                        return Results.NotFound(new { message = "Moto não encontrada", id });
                    
                    var links = new List<LinkModel>
                    {
                        new(lg.GetPathByName(http, "GetMotoById", new {id})!, "self", "GET"),
                        new(lg.GetPathByName(http, "GetMotoByPlaca", new {placa = moto.Placa})!, "getByPlaca", "GET"),
                        new(lg.GetPathByName(http, "UpdateMoto", new {id})!, "update", "PUT"),
                        new(lg.GetPathByName(http, "DeleteMoto", new {id})!, "delete", "DELETE")
                    };

                    var response = new MotoResponse(
                        moto.MotoId,
                        moto.Placa,
                        moto.Marca,
                        moto.Modelo,
                        moto.Ano,
                        moto.Status,
                        moto.Filial.Nome,
                        links
                        );
                    
                    return Results.Ok(response);
                }).WithName("GetMotoById")
                .WithSummary("Retorna uma moto pelo ID")
                .WithDescription("Retorna uma moto buscando pelo ID, caso não exista, retorna um erro 404 (Nao Encontrado)")
                .Produces<MotoResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            
            // GET BY PLACA
            group.MapGet("/{placa}", async (ManagementDb db, [Description("Placa da Moto")] string placa, LinkGenerator lg, HttpContext http) =>
            {
                
                var p = placa.Trim().ToUpperInvariant();
                
                var moto = await db.Motos
                    .AsNoTracking()
                    .Include(m => m.Filial)
                    .FirstOrDefaultAsync(m => m.Placa == p);

                if (moto is null)
                    return Results.NotFound(new { message = "Moto não encontrada", placa });
                
                var links = new List<LinkModel>
                {
                    new(lg.GetPathByName(http, "GetMotoById", new {id = moto.MotoId})!, "getById", "GET"),
                    new(lg.GetPathByName(http, "GetMotoByPlaca", new {placa})!, "self", "GET"),
                    new(lg.GetPathByName(http, "UpdateMoto", new {id = moto.MotoId})!, "update", "PUT"),
                    new(lg.GetPathByName(http, "DeleteMoto", new {id = moto.MotoId})!, "delete", "DELETE")
                };

                var response = new MotoResponse(
                    moto.MotoId,
                    moto.Placa,
                    moto.Marca,
                    moto.Modelo,
                    moto.Ano,
                    moto.Status,
                    moto.Filial.Nome,
                    links
                    );

                return Results.Ok(response);
            }).WithName("GetMotoByPlaca")
                .WithSummary("Retorna uma moto pela placa")
                .WithDescription("Retorna uma moto pela placa. " +
                                 "Caso não exista, retorna um erro 404 (Não Encontrado)")
                .Produces<MotoResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            
            // POST
            group.MapPost("", async (HttpContext http, MotoRequest request, LinkGenerator lg) =>
            {
                var db = http.RequestServices.GetRequiredService<ManagementDb>();
                
                var filial = await db.Filiais
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.FilialId == request.FilialId);

                if (filial is null)
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

                var links = new List<LinkModel>
                {
                    new(lg.GetPathByName(http, "GetMotoById", new {id = moto.MotoId})!, "getById", "GET"),
                    new(lg.GetPathByName(http, "GetMotoByPlaca", new {placa = moto.Placa})!, "getByPlaca", "GET"),
                    new(lg.GetPathByName(http, "UpdateMoto", new {id = moto.MotoId})!, "update", "PUT"),
                    new(lg.GetPathByName(http, "DeleteMoto", new {id = moto.MotoId})!, "delete", "DELETE")
                };
                
                var response = new MotoResponse(
                    moto.MotoId,
                    moto.Placa,
                    moto.Marca,
                    moto.Modelo,
                    moto.Ano,
                    moto.Status,
                    filialNome,
                    links
                    );

                return Results.Created($"/motos/{response.Motoid}", response);
            }).AddEndpointFilter<IdempotentAPIEndpointFilter>()
                .WithSummary("Cadastra uma nova moto")
                .WithDescription("Cadastra uma nova moto no sistema. " +
                                 "Caso a filial não exista, retorna um erro 400.")
                .Produces<MotoResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            
            // PUT
            group.MapPut("/{id:guid}",
                async ([Description("Identificador unico da moto")] Guid id, MotoRequest request, ManagementDb db, LinkGenerator lg, HttpContext http) =>
                {

                    var moto = await db.Motos
                        .FirstOrDefaultAsync(m => m.MotoId == id);

                    if (moto is null)
                        return Results.NotFound(new { message = "Moto não encontrada", id });

                    var filial = await db.Filiais
                        .AsNoTracking()
                        .FirstOrDefaultAsync(f => f.FilialId == request.FilialId);

                    if (filial is null)
                        return Results.BadRequest(new { message = "Filial de destino inválida ou inexistente.", request.FilialId });
                    
                    var links = new List<LinkModel>
                    {
                        new(lg.GetPathByName(http, "GetMotoById", new {id})!, "getById", "GET"),
                        new(lg.GetPathByName(http, "GetMotoByPlaca", new {placa = moto.Placa})!, "getByPlaca", "GET"),
                        new(lg.GetPathByName(http, "UpdateMoto", new {id})!, "self", "PUT"),
                        new(lg.GetPathByName(http, "DeleteMoto", new {id})!, "delete", "DELETE")
                    };

                    moto.Placa = request.Placa;
                    moto.Marca = request.Marca;
                    moto.Modelo = request.Modelo;
                    moto.Ano = request.Ano;
                    moto.Status = request.Status;
                    moto.FilialId = request.FilialId;

                    await db.SaveChangesAsync();
                    
                    var filialNome = await db.Filiais
                        .Where(f => f.FilialId == moto.FilialId)
                        .Select(f => f.Nome)
                        .FirstAsync();

                    var response = new
                    {
                        message = "Moto atualizada com sucesso!",
                        data = new MotoResponse(
                            moto.MotoId,
                            moto.Placa,
                            moto.Marca,
                            moto.Modelo,
                            moto.Ano,
                            moto.Status,
                            filialNome,
                            links
                            )
                    };
                    
                    return Results.Ok(response);
                    
                }).WithName("UpdateMoto")
                .WithSummary("Atualiza os dados de uma moto existente")
                .WithDescription("Atualiza os dados de uma moto existente buscando pelo seu ID. " +
                                 "Caso o ID passado esteja incorreto ou não exista, retorna um erro 404. " +
                                 "Caso a filial não exista, retorna um erro 400.")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status400BadRequest);
            
            // DELETE
            group.MapDelete("/{id:guid}", async
                ([Description("Identificador unico da moto")] Guid id, ManagementDb db) =>
                {
                    var moto = await db.Motos.FindAsync(id);
                    if (moto is null)
                        return Results.NotFound(new { message = "Moto não encontrada", id });

                    db.Motos.Remove(moto);
                    await db.SaveChangesAsync();
                    
                    return Results.Ok(new {message = "Moto deletada com sucesso!"});
                }).WithName("DeleteMoto")
                .WithSummary("Deleta uma moto existente")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            
            
            
            return builder;
        }
    }