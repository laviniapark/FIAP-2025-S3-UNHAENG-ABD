using System.ComponentModel;
using System.Text.RegularExpressions;
using IdempotentAPI.MinimalAPI;
using ManagementApp.Infrastructure;
using ManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementApp.Endpoints ;

    public static class FilialEndpoints
    {
        public static RouteGroupBuilder MapFilialEndpoints(this RouteGroupBuilder builder)
        {
            builder.WithTags("Filial Endpoints");

            // GET ALL
            builder.MapGet("/filiais", async (ManagementDb db) => 
                    await db.Filiais
                        .AsNoTracking()
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
                            ))
                        .ToListAsync())
                .WithSummary("Retorna a lista de todas as filiais")
                .WithDescription("Retorna a lista de todas as filiais, juntamente com o endereço cadastrado")
                .Produces<List<FilialResponse>>(StatusCodes.Status200OK);

            // GET BY ID
            builder.MapGet("/filiais/{id:guid}",
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
            builder.MapGet("/filiais/{cnpj}", async (ManagementDb db, [Description("CNPJ da Filial")] string cnpj) =>
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
                             "Caso não exista, retorna um erro 404 (Não Encontrado). " +
                             "Caso o usuário tenha inserido o formato errado (com símbolos), retorna um erro 400 com explicação do erro")
            .Produces<FilialResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

            // POST
            builder.MapPost("/filiais",
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

                    return Results.Created($"/filiais/{response.Cnpj}", response);
                })
                .AddEndpointFilter<IdempotentAPIEndpointFilter>()
                .WithSummary("Cadastra uma nova filial")
                .WithDescription("Cadastra uma nova filial no sistema. " +
                                 "Se o cadastro for concluído com sucesso, será possível ver " +
                                 "tanto o ID quanto o caminho com o cnpj incluso para pesquisas.")
                .Produces<FilialResponse>(StatusCodes.Status201Created);

            // PUT
            builder.MapPut("/filiais/{id:guid}", async (Guid id, FilialRequest request, ManagementDb db) =>
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
            builder.MapDelete("/filiais/{id:guid}/encerrar", async (Guid id, ManagementDb db) =>
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