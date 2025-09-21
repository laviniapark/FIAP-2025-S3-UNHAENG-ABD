using System.ComponentModel;
using System.Text.RegularExpressions;
using IdempotentAPI.MinimalAPI;
using ManagementApp.Infrastructure;
using ManagementApp.Infrastructure.Pagination;
using ManagementApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManagementApp.Endpoints ;

    public static class FilialEndpoints
    {
        public static RouteGroupBuilder MapFilialEndpoints(this RouteGroupBuilder builder)
        {
            var group = builder.MapGroup("/filiais")
                .WithTags("Filial Endpoints");

            // GET ALL
            group.MapGet("", async ([AsParameters] PageParameters pageParam, ManagementDb db) =>
            {
                var page = pageParam.PageNumber < 1 ? 1 : pageParam.PageNumber;
                var size = pageParam.PageSize is < 1 or > 100 ? 20 : pageParam.PageSize;
                
                var query = db.Filiais
                    .AsNoTracking()
                    .OrderBy(f => f.Nome)
                    .Select(f => new FilialResponse(
                        f.FilialId,
                        f.Nome,
                        f.Cnpj,
                        f.Telefone,
                        f.DataAbertura,
                        f.DataEncerramento,
                        new EnderecoResponse(
                            f.Endereco.CEP,
                            f.Endereco.Logradouro,
                            f.Endereco.Numero,
                            f.Endereco.Complemento,
                            f.Endereco.Bairro,
                            f.Endereco.Cidade,
                            f.Endereco.UF,
                            f.Endereco.Pais
                            )
                        ));

                var paged = await PagedList<FilialResponse>.CreateAsync(query, page, size);
                
                return Results.Ok(paged);
            })
                .WithSummary("Retorna lista paginada de Filiais")
                .WithDescription("Retorna a lista paginada de filiais ordenada por Nome, " + 
                                 "podendo ser definido a quantidade a ser mostrada por página. " +
                                 "Dados informados: informaçao das filiais , numero da pagina, " +
                                 "quantidade de filiais por pagina, quantidade total de filiais cadastradas, " +
                                 "se possui proxima pagina e se possui pagina anterior.")
                .Produces<List<FilialResponse>>(StatusCodes.Status200OK);

            // GET BY ID
            group.MapGet("/{id:guid}",
                async (ManagementDb db, [Description("Identificador único da Filial")] Guid id) =>
                {
                    var filial = await db.Filiais
                        .AsNoTracking()
                        .FirstOrDefaultAsync(f => f.FilialId == id);

                    if (filial is null)
                        return Results.NotFound(new { message = "Filial não encontrada", id });
                    
                    var response = new FilialResponse(
                        filial.FilialId,
                        filial.Nome,
                        filial.Cnpj,
                        filial.Telefone,
                        filial.DataAbertura,
                        filial.DataEncerramento,
                        new EnderecoResponse(
                            filial.Endereco.CEP,
                            filial.Endereco.Logradouro,
                            filial.Endereco.Numero,
                            filial.Endereco.Complemento,
                            filial.Endereco.Bairro,
                            filial.Endereco.Cidade,
                            filial.Endereco.UF,
                            filial.Endereco.Pais
                            )
                        );
                    
                    return Results.Ok(response);
                }
                ).WithSummary("Retorna uma filial pelo ID")
                .WithDescription("Retorna uma filial buscando pelo ID, caso não exista, retorna um erro 404 (Nao Encontrado)")
                .Produces<FilialResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            // GET BY CNPJ
            group.MapGet("/{cnpj}", async (ManagementDb db, [Description("CNPJ da Filial")] string cnpj) =>
            {
                var filial = await db.Filiais
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Cnpj == cnpj);

                if (filial is null)
                    return Results.NotFound(new { message = "Filial não encontrada", cnpj });
                
                var response = new FilialResponse(
                    filial.FilialId,
                    filial.Nome,
                    filial.Cnpj,
                    filial.Telefone,
                    filial.DataAbertura,
                    filial.DataEncerramento,
                    new EnderecoResponse(
                        filial.Endereco.CEP,
                        filial.Endereco.Logradouro,
                        filial.Endereco.Numero,
                        filial.Endereco.Complemento,
                        filial.Endereco.Bairro,
                        filial.Endereco.Cidade,
                        filial.Endereco.UF,
                        filial.Endereco.Pais
                        )
                    );
                
                return Results.Ok(response);
            }).WithSummary("Retorna uma filial pelo CNPJ")
            .WithDescription("Retorna uma filial pelo CNPJ. " +
                             "Caso não exista, retorna um erro 404 (Não Encontrado). ")
            .Produces<FilialResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            // POST
            group.MapPost("",
                async (HttpContext http, FilialRequest filialRequest) =>
                {
                    var db = http.RequestServices.GetRequiredService<ManagementDb>();
                    
                    var dataAbertura = filialRequest.DataAbertura ?? DateTime.UtcNow.Date;

                    var filial = new Filial
                    {
                        FilialId = Guid.NewGuid(),
                        Nome = filialRequest.Nome,
                        Cnpj = filialRequest.Cnpj,
                        Telefone = filialRequest.Telefone,
                        DataAbertura = dataAbertura,
                        DataEncerramento = filialRequest.DataEncerramento,
                        Endereco = new Endereco
                        {
                            CEP = filialRequest.Endereco.CEP,
                            Logradouro = filialRequest.Endereco.Logradouro,
                            Numero = filialRequest.Endereco.Numero,
                            Complemento =  filialRequest.Endereco.Complemento,
                            Bairro = filialRequest.Endereco.Bairro,
                            Cidade = filialRequest.Endereco.Cidade,
                            UF = filialRequest.Endereco.UF,
                            Pais = filialRequest.Endereco.Pais
                        }
                    };
                    
                    db.Filiais.Add(filial);
                    await db.SaveChangesAsync();
                    
                    var response = new FilialResponse(
                        filial.FilialId,
                        filial.Nome,
                        filial.Cnpj,
                        filial.Telefone,
                        filial.DataAbertura,
                        filial.DataEncerramento,
                        new EnderecoResponse(
                            filial.Endereco.CEP,
                            filial.Endereco.Logradouro,
                            filial.Endereco.Numero,
                            filial.Endereco.Complemento,
                            filial.Endereco.Bairro,
                            filial.Endereco.Cidade,
                            filial.Endereco.UF,
                            filial.Endereco.Pais
                            )
                        );

                    return Results.Created($"/filiais/{response.FilialId}", response);
                })
                .AddEndpointFilter<IdempotentAPIEndpointFilter>()
                .WithSummary("Cadastra uma nova filial")
                .Produces<FilialResponse>(StatusCodes.Status201Created);

            // PUT
            group.MapPut("/{id:guid}", async (Guid id, FilialRequest request, ManagementDb db) =>
            {
                var filial = await db.Filiais.FirstOrDefaultAsync(f => f.FilialId == id);
                
                if (filial is null)
                    return Results.NotFound(new { message = "Filial não encontrada", id });
                
                filial.Nome = request.Nome;
                filial.Cnpj = request.Cnpj;
                filial.Telefone = request.Telefone;
                filial.DataAbertura = request.DataAbertura?.Date ?? filial.DataAbertura;
                filial.DataEncerramento = request.DataEncerramento;
                
                filial.Endereco.CEP = request.Endereco.CEP;
                filial.Endereco.Logradouro = request.Endereco.Logradouro;
                filial.Endereco.Numero = request.Endereco.Numero;
                filial.Endereco.Complemento = request.Endereco.Complemento;
                filial.Endereco.Bairro = request.Endereco.Bairro;
                filial.Endereco.Cidade = request.Endereco.Cidade;
                filial.Endereco.UF = request.Endereco.UF;
                filial.Endereco.Pais = request.Endereco.Pais;
                
                await db.SaveChangesAsync();
                return Results.NoContent();
                
            })
                .WithSummary("Atualiza os dados de uma filial existente")
                .WithDescription("Atualiza os dados de uma filial existente buscando pelo seu ID. " +
                                 "Caso o ID passado esteja incorreto ou não exista, retorna um erro 404.")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);

            // SOFT DELETE
            group.MapDelete("/{id:guid}/encerrar", async (Guid id, ManagementDb db) =>
            {
                var filial = await db.Filiais.FindAsync(id);
                if (filial is null)
                    return Results.NotFound(new { message = "Filial não encontrada", id });

                filial.DataEncerramento = DateTime.UtcNow.Date;
                await db.SaveChangesAsync();

                return Results.NoContent();
            })
                .WithSummary("Encerra uma filial existente")
                .WithDescription("Diferente de um DELETE normal, ele não vai apagar os dados. " +
                                 "Quando o usuário escolher encerrar, a data de encerramento vai ser " +
                                 "definida pela data que foi feita a requisição, e os dados da filial " +
                                 "continuarão existindo no banco, para fins de armazenamento de histórico")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound);
            
            return builder;
        }
    }