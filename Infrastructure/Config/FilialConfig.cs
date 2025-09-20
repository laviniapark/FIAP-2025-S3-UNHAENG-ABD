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
            filial.Property(f => f.Cnpj).IsRequired().HasMaxLength(18);
            filial.HasIndex(f => f.Cnpj).IsUnique();
            filial.Property(f => f.Telefone).HasMaxLength(25);
            filial.Property(f => f.DataAbertura).HasColumnType("DATE");
            filial.Property(f => f.DataEncerramento).HasColumnType("DATE");
            
            // SEEDS
            filial.HasData(
                new Filial {
                    FilialId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Nome = "Filial Centro",
                    Cnpj = "12.345.678/0001-01",
                    Telefone = "(11) 3333-1001",
                    DataAbertura = new DateTime(2010, 5, 12),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Nome = "Filial Norte",
                    Cnpj = "23.456.789/0001-02",
                    Telefone = "(92) 4002-8922",
                    DataAbertura = new DateTime(2012, 8, 20),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Nome = "Filial Sul",
                    Cnpj = "34.567.890/0001-03",
                    Telefone = "(51) 3500-3003",
                    DataAbertura = new DateTime(2015, 1, 15),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Nome = "Filial Leste",
                    Cnpj = "45.678.901/0001-04",
                    Telefone = "(31) 3222-4004",
                    DataAbertura = new DateTime(2016, 3, 10),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Nome = "Filial Oeste",
                    Cnpj = "56.789.012/0001-05",
                    Telefone = "(61) 3555-5005",
                    DataAbertura = new DateTime(2018, 7, 1),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    Nome = "Filial Paulista",
                    Cnpj = "67.890.123/0001-06",
                    Telefone = "(11) 3666-6006",
                    DataAbertura = new DateTime(2019, 9, 25),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    Nome = "Filial Moema",
                    Cnpj = "78.901.234/0001-07",
                    Telefone = "(11) 3777-7007",
                    DataAbertura = new DateTime(2020, 2, 18),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    Nome = "Filial Pinheiros",
                    Cnpj = "89.012.345/0001-08",
                    Telefone = "(11) 3888-8008",
                    DataAbertura = new DateTime(2021, 4, 5),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                    Nome = "Filial Liberdade",
                    Cnpj = "90.123.456/0001-09",
                    Telefone = "(11) 3999-9009",
                    DataAbertura = new DateTime(2022, 6, 30),
                    DataEncerramento = null
                },
                new Filial {
                    FilialId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Nome = "Filial Barra Funda",
                    Cnpj = "01.234.567/0001-10",
                    Telefone = "(11) 4000-1010",
                    DataAbertura = new DateTime(2023, 1, 10),
                    DataEncerramento = null
                }
            );

        }
    }