using System.ComponentModel;
using IdempotentAPI.MinimalAPI;
using ManagementApp.Infrastructure;
using ManagementApp.Infrastructure.Pagination;
using ManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementApp.Endpoints ;

    public static class FuncionarioEndpoints
    {
        public static RouteGroupBuilder MapFuncionarioEndpoints(this RouteGroupBuilder builder)
        {
            var group = builder.MapGroup("/funcionarios")
                .WithTags("Funcionario Endpoints");
            
            //GET ALL
            group.MapGet("", async ([AsParameters] PageParameters pageParam, ManagementDb db) =>
            {
                var page = pageParam.PageNumber < 1 ? 1 : pageParam.PageNumber;
                var size = pageParam.PageSize is < 1 or > 100 ? 20 : pageParam.PageSize;

                var query = db.Funcionarios
                    .AsNoTracking()
                    .OrderBy(f => f.NomeCompleto)
                    .Include(f => f.Filial)
                    .Select(f => new FuncionarioResponseGA(
                        f.FuncionarioId,
                        f.NomeCompleto,
                        f.Cpf,
                        f.Cargo,
                        f.Ativo,
                        f.Filial.Nome
                        ));
                
                var paged = await PagedList<FuncionarioResponseGA>.CreateAsync(query, page, size);

                return Results.Ok(paged);
            }).WithSummary("Retorna lista paginada de Funcionarios")
                .WithDescription("Retorna a lista paginada de funcionarios ordenada por Nome, " + 
                                 "podendo ser definido a quantidade a ser mostrada por página. " +
                                 "Dados informados: informaçao dos funcionarios , numero da pagina, " +
                                 "quantidade de funcionarios por pagina, quantidade total de funcionarios cadastrados, " +
                                 "se possui proxima pagina e se possui pagina anterior.")
                .Produces<List<FuncionarioResponse>>(StatusCodes.Status200OK);
            
            // GET BY ID
            group.MapGet("/{id:guid}",
                async (ManagementDb db, [Description("Identificador unico do funcionario")] Guid id, LinkGenerator lg, HttpContext http) =>
                {
                    var funcionario = await db.Funcionarios
                        .AsNoTracking()
                        .Include(f => f.Filial)
                        .FirstOrDefaultAsync(f => f.FuncionarioId == id);

                    if (funcionario is null)
                        return Results.NotFound(new { message = "Funcionario não encontrado", id });
                    
                    var links = new List<LinkModel>
                    {
                        new(lg.GetPathByName(http, "GetFuncionarioById", new {id})!, "self", "GET"),
                        new(lg.GetPathByName(http, "GetFuncionarioByCpf", new {cpf = funcionario.Cpf})!, "getByCpf", "GET"),
                        new(lg.GetPathByName(http, "UpdateFuncionario", new {id})!, "update", "PUT"),
                        new(lg.GetPathByName(http, "DeleteFuncionario", new {id})!, "delete", "DELETE")
                    };

                    var response = new FuncionarioResponse(
                        funcionario.FuncionarioId,
                        funcionario.NomeCompleto,
                        funcionario.Cpf,
                        funcionario.Cargo,
                        funcionario.Ativo,
                        funcionario.Filial.Nome,
                        links
                        );

                    return Results.Ok(response);
                }).WithName("GetFuncionarioById")
                .WithSummary("Retorna um funcionario pelo ID")
                .WithDescription("Retorna um funcionario buscando pelo ID. " +
                                 "Caso não exista, retorna um erro 404.")
                .Produces<FuncionarioResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            
            // GET BY CPF
            group.MapGet("/{cpf}",
                async (ManagementDb db, [Description("CPF do funcionario")] string cpf, LinkGenerator lg, HttpContext http) =>
                {
                    var funcionario = await db.Funcionarios
                        .AsNoTracking()
                        .Include(f => f.Filial)
                        .FirstOrDefaultAsync(f => f.Cpf == cpf);

                    if (funcionario is null)
                        return Results.NotFound(new { message = "Funcionario não encontrado", cpf });
                    
                    var links = new List<LinkModel>
                    {
                        new(lg.GetPathByName(http, "GetFuncionarioById", new {id = funcionario.FuncionarioId})!, "getById", "GET"),
                        new(lg.GetPathByName(http, "GetFuncionarioByCpf", new {cpf})!, "self", "GET"),
                        new(lg.GetPathByName(http, "UpdateFuncionario", new {id = funcionario.FuncionarioId})!, "update", "PUT"),
                        new(lg.GetPathByName(http, "DeleteFuncionario", new {id = funcionario.FuncionarioId})!, "delete", "DELETE")
                    };

                    var response = new FuncionarioResponse(
                        funcionario.FuncionarioId,
                        funcionario.NomeCompleto,
                        funcionario.Cpf,
                        funcionario.Cargo,
                        funcionario.Ativo,
                        funcionario.Filial.Nome,
                        links
                        );

                    return Results.Ok(response);
                }).WithName("GetFuncionarioByCpf")
                .WithSummary("Retorna um funcionario pelo CPF")
                .WithDescription("Retorna um funcionario buscando pelo seu CPF. " +
                                 "Caso não exista, retorna um erro 404 (Não Encontrado)")
                .Produces<FuncionarioResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            
            // POST
            group.MapPost("", async (HttpContext http, FuncionarioRequest request, LinkGenerator lg) =>
            {
                var db = http.RequestServices.GetRequiredService<ManagementDb>();

                var filial = await db.Filiais
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.FilialId == request.FilialId);

                if (filial is null)
                    return Results.BadRequest(new { message = "Filial de destino inválida ou inexistente.", request.FilialId });

                var func = new Funcionario
                {
                    NomeCompleto = request.NomeCompleto,
                    Cpf = request.Cpf,
                    Cargo = request.Cargo,
                    Ativo = request.Ativo,
                    FilialId = request.FilialId
                };

                db.Funcionarios.Add(func);
                await db.SaveChangesAsync();

                var filialNome = await db.Filiais
                    .Where(f => f.FilialId == func.FilialId)
                    .Select(f => f.Nome)
                    .FirstAsync();

                var links = new List<LinkModel>
                {
                    new(lg.GetPathByName(http, "GetFuncionarioById", new {id = func.FuncionarioId})!, "getById", "GET"),
                    new(lg.GetPathByName(http, "GetFuncionarioByCpf", new {cpf = func.Cpf})!, "getByCpf", "GET"),
                    new(lg.GetPathByName(http, "UpdateFuncionario", new {id = func.FuncionarioId})!, "update", "PUT"),
                    new(lg.GetPathByName(http, "DeleteFuncionario", new {id = func.FuncionarioId})!, "delete", "DELETE")
                };
                
                var response = new FuncionarioResponse(
                    func.FuncionarioId,
                    func.NomeCompleto,
                    func.Cpf,
                    func.Cargo,
                    func.Ativo,
                    filialNome,
                    links
                    );
                
                return Results.Created($"/funcionarios/{func.FuncionarioId}", response);
            }).AddEndpointFilter<IdempotentAPIEndpointFilter>()
                .WithSummary("Cadastra um novo funcionario")
                .WithDescription("Cadastra um novo funcionario no sistema. " +
                                 "Caso a filial não exista, retorna um erro 400.")
                .Produces<FuncionarioResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            
            // PUT
            group.MapPut("/{id:guid}",
                async ([Description("Identificador unico do funcionario")] Guid id, FuncionarioRequest request, ManagementDb db, LinkGenerator lg, HttpContext http) =>
            {
                var func = await db.Funcionarios
                    .FirstOrDefaultAsync(f => f.FuncionarioId == id);

                if (func is null)
                    return Results.NotFound(new { message = "Funcionario não encontrado", id });

                var filial = await db.Filiais
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.FilialId == request.FilialId);

                if (filial is null)
                    return Results.BadRequest(new { message = "Filial de destino inválida ou inexistente.", request.FilialId });
                
                var links = new List<LinkModel>
                {
                    new(lg.GetPathByName(http, "GetFuncionarioById", new {id})!, "getById", "GET"),
                    new(lg.GetPathByName(http, "GetFuncionarioByCpf", new {cpf = func.Cpf})!, "getByCpf", "GET"),
                    new(lg.GetPathByName(http, "UpdateFuncionario", new {id})!, "self", "PUT"),
                    new(lg.GetPathByName(http, "DeleteFuncionario", new {id})!, "delete", "DELETE")
                };
                
                func.NomeCompleto = request.NomeCompleto;
                func.Cpf = request.Cpf;
                func.Cargo = request.Cargo;
                func.Ativo = request.Ativo;
                func.FilialId = request.FilialId;

                await db.SaveChangesAsync();
                
                var filialNome = await db.Filiais
                    .Where(f => f.FilialId == func.FilialId)
                    .Select(f => f.Nome)
                    .FirstAsync();

                var response = new
                {
                    message = "Funcionario atualizado com sucesso!",
                    data = new FuncionarioResponse(
                        func.FuncionarioId,
                        func.NomeCompleto,
                        func.Cpf,
                        func.Cargo,
                        func.Ativo,
                        filialNome,
                        links
                        )
                };
                
                return Results.Ok(response);
            }).WithName("UpdateFuncionario")
                .WithSummary("Atualiza os dados de um funcionario existente")
                .WithDescription("Atualiza os dados de um funcionario buscando pelo seu ID. " +
                                 "Caso o ID passado esteja incorreto ou não exista, retorna um erro 404. " +
                                 "Caso a filial não exista, retorna um erro 400.")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status400BadRequest);
            
            // DELETE
            group.MapDelete("/{id:guid}",
                async ([Description("Identificador unico do funcionario")] Guid id, ManagementDb db) =>
                {
                    var func = await db.Funcionarios.FindAsync(id);
                    if (func is null)
                        return Results.NotFound(new { message = "Funcionario não encontrado", id });

                    db.Funcionarios.Remove(func);
                    await db.SaveChangesAsync();
                    return Results.Ok(new {message = "Funcionario deletado com sucesso!"});
                }).WithName("DeleteFuncionario")
                .WithSummary("Deleta um funcionario existente")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            
            return builder;
        }
    }