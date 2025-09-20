using ManagementApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManagementApp.Infrastructure.Config ;

    public class FilialConfig : IEntityTypeConfiguration<Filial>
    {
        public void Configure(EntityTypeBuilder<Filial> filial)
        {
            filial.HasKey(f => f.FilialId);
            
            filial.Property(f => f.Nome).IsRequired().HasMaxLength(100);
            filial.Property(f => f.Cnpj).IsRequired().HasMaxLength(14).HasColumnType("VARCHAR2(14)");
            filial.HasIndex(f => f.Cnpj).IsUnique();
            filial.Property(f => f.Telefone).HasMaxLength(25);
            filial.Property(f => f.DataAbertura).HasColumnType("DATE");
            filial.Property(f => f.DataEncerramento).HasColumnType("DATE");
            
            // SEEDS
            filial.HasData(
                new Filial {
                    FilialId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Nome = "Filial Centro",
                    Cnpj = "12345678000101",
                    Telefone = "(11) 3333-1001",
                    DataAbertura = new DateTime(2010, 5, 12),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Nome = "Filial Norte",
                    Cnpj = "23456789000102",
                    Telefone = "(92) 4002-8922",
                    DataAbertura = new DateTime(2012, 8, 20),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Nome = "Filial Sul",
                    Cnpj = "34567890000103",
                    Telefone = "(51) 3500-3003",
                    DataAbertura = new DateTime(2015, 1, 15),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Nome = "Filial Leste",
                    Cnpj = "45678901000104",
                    Telefone = "(31) 3222-4004",
                    DataAbertura = new DateTime(2016, 3, 10),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Nome = "Filial Oeste",
                    Cnpj = "56789012000105",
                    Telefone = "(61) 3555-5005",
                    DataAbertura = new DateTime(2018, 7, 1),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    Nome = "Filial Paulista",
                    Cnpj = "67890123000106",
                    Telefone = "(11) 3666-6006",
                    DataAbertura = new DateTime(2019, 9, 25),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    Nome = "Filial Moema",
                    Cnpj = "78901234000107",
                    Telefone = "(11) 3777-7007",
                    DataAbertura = new DateTime(2020, 2, 18),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    Nome = "Filial Pinheiros",
                    Cnpj = "89012345000108",
                    Telefone = "(11) 3888-8008",
                    DataAbertura = new DateTime(2021, 4, 5),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                    Nome = "Filial Liberdade",
                    Cnpj = "90123456000109",
                    Telefone = "(11) 3999-9009",
                    DataAbertura = new DateTime(2022, 6, 30),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Nome = "Filial Barra Funda",
                    Cnpj = "01234567000110",
                    Telefone = "(11) 4000-1010",
                    DataAbertura = new DateTime(2023, 1, 10),
                    DataEncerramento = null
                }
            );
            
            filial.OwnsOne(x => x.Endereco, eb =>
            {
                eb.WithOwner();
                eb.Property(p => p.CEP).IsRequired().HasMaxLength(9).HasColumnName("END_CEP");
                eb.Property(p => p.Logradouro).IsRequired().HasMaxLength(120).HasColumnName("END_LOGRADOURO");
                eb.Property(p => p.Numero).IsRequired().HasMaxLength(10).HasColumnName("END_NUMERO");
                eb.Property(p => p.Complemento).HasMaxLength(60).HasColumnName("END_COMPLEMENTO");
                eb.Property(p => p.Bairro).IsRequired().HasMaxLength(80).HasColumnName("END_BAIRRO");
                eb.Property(p => p.Cidade).IsRequired().HasMaxLength(80).HasColumnName("END_CIDADE");
                eb.Property(p => p.UF).IsRequired().HasMaxLength(2).HasColumnName("END_UF");
                eb.Property(p => p.Pais).IsRequired().HasMaxLength(80).HasColumnName("END_PAIS");
                
                eb.HasData(
                new {
                    FilialId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    CEP = "01310-100",
                    Logradouro = "Av. Paulista",
                    Numero = "1000",
                    Complemento = "Conjunto 101",
                    Bairro = "Bela Vista",
                    Cidade = "São Paulo",
                    UF = "SP",
                    Pais = "Brasil"
                },
                new {
                    FilialId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    CEP = "69000-000",
                    Logradouro = "Av. Constantino Nery",
                    Numero = "200",
                    Bairro = "Centro",
                    Cidade = "Manaus",
                    UF = "AM",
                    Pais = "Brasil"
                },
                new {
                    FilialId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    CEP = "90010-000",
                    Logradouro = "Av. Borges de Medeiros",
                    Numero = "300",
                    Bairro = "Centro Histórico",
                    Cidade = "Porto Alegre",
                    UF = "RS",
                    Pais = "Brasil"
                },
                new {
                    FilialId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    CEP = "31000-000",
                    Logradouro = "Av. Cristiano Machado",
                    Numero = "400",
                    Complemento = "Sala 5",
                    Bairro = "Cidade Nova",
                    Cidade = "Belo Horizonte",
                    UF = "MG",
                    Pais = "Brasil"
                },
                new  {
                    FilialId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    CEP = "70040-010",
                    Logradouro = "SBN Quadra 1",
                    Numero = "500",
                    Complemento = "Bloco B",
                    Bairro = "Asa Norte",
                    Cidade = "Brasília",
                    UF = "DF",
                    Pais = "Brasil"
                },
                new  {
                    FilialId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    CEP = "01415-002",
                    Logradouro = "Rua Augusta",
                    Numero = "600",
                    Bairro = "Consolação",
                    Cidade = "São Paulo",
                    UF = "SP",
                    Pais = "Brasil"
                },
                new  {
                    FilialId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    CEP = "04077-000",
                    Logradouro = "Av. Ibirapuera",
                    Numero = "700",
                    Complemento = "Loja 3",
                    Bairro = "Moema",
                    Cidade = "São Paulo",
                    UF = "SP",
                    Pais = "Brasil"
                },
                new  {
                    FilialId = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    CEP = "05422-000",
                    Logradouro = "Rua dos Pinheiros",
                    Numero = "800",
                    Bairro = "Pinheiros",
                    Cidade = "São Paulo",
                    UF = "SP",
                    Pais = "Brasil"
                },
                new  {
                    FilialId = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                    CEP = "01503-000",
                    Logradouro = "Rua Galvão Bueno",
                    Numero = "900",
                    Bairro = "Liberdade",
                    Cidade = "São Paulo",
                    UF = "SP",
                    Pais = "Brasil"
                },
                new  {
                    FilialId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    CEP = "01152-000",
                    Logradouro = "Av. Marquês de São Vicente",
                    Numero = "1000",
                    Complemento = "Galpão 2",
                    Bairro = "Barra Funda",
                    Cidade = "São Paulo",
                    UF = "SP",
                    Pais = "Brasil"
                }
            );
            });
            filial.Navigation(x => x.Endereco).IsRequired();

        }
    }