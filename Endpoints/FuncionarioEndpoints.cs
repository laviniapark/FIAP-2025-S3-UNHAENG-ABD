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
                    .Select(f => new FuncionarioResponse(
                        f.FuncionarioId,
                        f.NomeCompleto,
                        f.Cpf,
                        f.Cargo,
                        f.Ativo,
                        f.Filial.Nome
                        ));
                
                var paged = await PagedList<FuncionarioResponse>.CreateAsync(query, page, size);

                return Results.Ok(paged);
            })
                .WithSummary("Retorna lista paginada de Funcionarios")
                .WithDescription("Retorna a lista paginada de funcionarios ordenada por Nome, " + 
                                 "podendo ser definido a quantidade a ser mostrada por página. " +
                                 "Dados informados: informaçao dos funcionarios , numero da pagina, " +
                                 "quantidade de funcionarios por pagina, quantidade total de funcionarios cadastrados, " +
                                 "se possui proxima pagina e se possui pagina anterior.")
                .Produces<List<FuncionarioResponse>>(StatusCodes.Status200OK);
            
            // GET BY ID
            group.MapGet("/{id:guid}",
                async (ManagementDb db, [Description("Identificador unico do funcionario")] Guid id) =>
                {
                    var funcionario = await db.Funcionarios
                        .AsNoTracking()
                        .Include(f => f.Filial)
                        .FirstOrDefaultAsync(f => f.FuncionarioId == id);

                    if (funcionario is null)
                        return Results.NotFound(new { message = "Funcionario não encontrado", id });

                    var response = new FuncionarioResponse(
                        funcionario.FuncionarioId,
                        funcionario.NomeCompleto,
                        funcionario.Cpf,
                        funcionario.Cargo,
                        funcionario.Ativo,
                        funcionario.Filial.Nome
                        );

                    return Results.Ok(response);
                })
                .WithSummary("Retorna um funcionario pelo ID")
                .WithDescription("Retorna um funcionario buscando pelo ID. " +
                                 "Caso não exista, retorna um erro 404.")
                .Produces<FuncionarioResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            
            // GET BY CPF
            group.MapGet("/{cpf}",
                async (ManagementDb db, [Description("CPF do funcionario")] string cpf) =>
                {
                    var funcionario = await db.Funcionarios
                        .AsNoTracking()
                        .Include(f => f.Filial)
                        .FirstOrDefaultAsync(f => f.Cpf == cpf);

                    if (funcionario is null)
                        return Results.NotFound(new { message = "Funcionario não encontrado", cpf });

                    var response = new FuncionarioResponse(
                        funcionario.FuncionarioId,
                        funcionario.NomeCompleto,
                        funcionario.Cpf,
                        funcionario.Cargo,
                        funcionario.Ativo,
                        funcionario.Filial.Nome
                        );

                    return Results.Ok(response);
                })
                .WithSummary("Retorna um funcionario pelo CPF")
                .WithDescription("Retorna um funcionario buscando pelo seu CPF. " +
                                 "Caso não exista, retorna um erro 404 (Não Encontrado)")
                .Produces<FuncionarioResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);
            
            // POST
            group.MapPost("", async (HttpContext http, FuncionarioRequest request) =>
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

                var response = new FuncionarioResponse(
                    func.FuncionarioId,
                    func.NomeCompleto,
                    func.Cpf,
                    func.Cargo,
                    func.Ativo,
                    filialNome
                    );
                
                return Results.Created($"/funcionarios/{func.FuncionarioId}", response);
            })
                .AddEndpointFilter<IdempotentAPIEndpointFilter>()
                .WithSummary("Cadastra um novo funcionario")
                .WithDescription("Cadastra um novo funcionario no sistema. " +
                                 "Caso a filial não exista, retorna um erro 400.")
                .Produces<FuncionarioResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
            
            // PUT
            group.MapPut("/{id:guid}",
                async ([Description("Identificador unico do funcionario")] Guid id, FuncionarioRequest request, ManagementDb db) =>
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
                
                func.NomeCompleto = request.NomeCompleto;
                func.Cpf = request.Cpf;
                func.Cargo = request.Cargo;
                func.Ativo = request.Ativo;
                func.FilialId = request.FilialId;

                await db.SaveChangesAsync();
                return Results.NoContent();
            })
                .WithSummary("Atualiza os dados de um funcionario existente")
                .WithDescription("Atualiza os dados de um funcionario buscando pelo seu ID. " +
                                 "Caso o ID passado esteja incorreto ou não exista, retorna um erro 404. " +
                                 "Caso a filial não exista, retorna um erro 400.")
                .Produces(StatusCodes.Status204NoContent)
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
                    return Results.NoContent();
                })
                .WithSummary("Deleta um funcionario existente")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
            
            return builder;
        }
    }