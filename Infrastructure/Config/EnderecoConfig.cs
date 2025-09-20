using ManagementApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManagementApp.Infrastructure.Config ;

    public class EnderecoConfig : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> endereco)
        {
            endereco.HasKey(e => e.FilialId);
            endereco.Property(e => e.FilialId).ValueGeneratedNever();
            
            endereco.Property(e => e.CEP).IsRequired().HasMaxLength(9);
            endereco.Property(e => e.Logradouro).IsRequired().HasMaxLength(150);
            endereco.Property(e => e.Numero).IsRequired().HasMaxLength(10);
            endereco.Property(e => e.Complemento).HasMaxLength(60);
            endereco.Property(e => e.Bairro).IsRequired().HasMaxLength(80);
            endereco.Property(e => e.Cidade).IsRequired().HasMaxLength(80);
            endereco.Property(e => e.UF).IsRequired().HasColumnType("CHAR(2)");
            endereco.Property(e => e.Pais).IsRequired().HasMaxLength(60);
            
            endereco.HasOne(e => e.Filial)
                .WithOne(f => f.Endereco)
                .HasForeignKey<Endereco>(e => e.FilialId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            
            // SEEDS
            endereco.HasData(
                new Endereco {
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
                new Endereco {
                    FilialId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    CEP = "69000-000",
                    Logradouro = "Av. Constantino Nery",
                    Numero = "200",
                    Complemento = null,
                    Bairro = "Centro",
                    Cidade = "Manaus",
                    UF = "AM",
                    Pais = "Brasil"
                },
                new Endereco {
                    FilialId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    CEP = "90010-000",
                    Logradouro = "Av. Borges de Medeiros",
                    Numero = "300",
                    Complemento = null,
                    Bairro = "Centro Histórico",
                    Cidade = "Porto Alegre",
                    UF = "RS",
                    Pais = "Brasil"
                },
                new Endereco {
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
                new Endereco {
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
                new Endereco {
                    FilialId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    CEP = "01415-002",
                    Logradouro = "Rua Augusta",
                    Numero = "600",
                    Complemento = null,
                    Bairro = "Consolação",
                    Cidade = "São Paulo",
                    UF = "SP",
                    Pais = "Brasil"
                },
                new Endereco {
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
                new Endereco {
                    FilialId = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    CEP = "05422-000",
                    Logradouro = "Rua dos Pinheiros",
                    Numero = "800",
                    Complemento = null,
                    Bairro = "Pinheiros",
                    Cidade = "São Paulo",
                    UF = "SP",
                    Pais = "Brasil"
                },
                new Endereco {
                    FilialId = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                    CEP = "01503-000",
                    Logradouro = "Rua Galvão Bueno",
                    Numero = "900",
                    Complemento = null,
                    Bairro = "Liberdade",
                    Cidade = "São Paulo",
                    UF = "SP",
                    Pais = "Brasil"
                },
                new Endereco {
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


        }
    }