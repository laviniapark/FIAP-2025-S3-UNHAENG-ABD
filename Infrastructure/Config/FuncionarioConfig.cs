using ManagementApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManagementApp.Infrastructure.Config ;

    public class FuncionarioConfig : IEntityTypeConfiguration<Funcionario>
    {
        public void Configure(EntityTypeBuilder<Funcionario> funcionario)
        {
            funcionario.HasKey(f => f.FuncionarioId);
            
            funcionario.Property(f => f.NomeCompleto).IsRequired().HasMaxLength(80);
            funcionario.Property(f => f.Cpf).IsRequired().HasMaxLength(14);
            funcionario.HasIndex(f => f.Cpf).IsUnique();
            funcionario.Property(f => f.Cargo).HasConversion<string>().HasMaxLength(20);
            funcionario.Property(f => f.Ativo).HasConversion(
                v => v ? "Y" : "N",
                v => v == "Y"
                )
                .HasColumnType("CHAR(1)")
                .HasDefaultValue("Y");
            
            funcionario.HasOne(f => f.Filial)
                .WithMany(f => f.Funcionarios)
                .HasForeignKey(f => f.FilialId);
            
            // SEEDS
            funcionario.HasData(
                new Funcionario {
                    FuncionarioId = Guid.Parse("11111111-bbbb-bbbb-bbbb-111111111111"),
                    NomeCompleto = "Maria Souza",
                    Cpf = "123.456.789-01",
                    Cargo = Funcionario.CargoEnum.DonoFilial,
                    Ativo = true,
                    FilialId = Guid.Parse("11111111-1111-1111-1111-111111111111")
                },
                new Funcionario {
                    FuncionarioId = Guid.Parse("22222222-bbbb-bbbb-bbbb-222222222222"),
                    NomeCompleto = "João Lima",
                    Cpf = "987.654.321-00",
                    Cargo = Funcionario.CargoEnum.Mecanico,
                    Ativo = true,
                    FilialId = Guid.Parse("11111111-1111-1111-1111-111111111111")
                },
                new Funcionario {
                    FuncionarioId = Guid.Parse("33333333-bbbb-bbbb-bbbb-333333333333"),
                    NomeCompleto = "Ana Oliveira",
                    Cpf = "321.654.987-22",
                    Cargo = Funcionario.CargoEnum.Gestor,
                    Ativo = true,
                    FilialId = Guid.Parse("22222222-2222-2222-2222-222222222222")
                },
                new Funcionario {
                    FuncionarioId = Guid.Parse("44444444-bbbb-bbbb-bbbb-444444444444"),
                    NomeCompleto = "Carlos Mendes",
                    Cpf = "456.789.123-33",
                    Cargo = Funcionario.CargoEnum.Funcionario,
                    Ativo = true,
                    FilialId = Guid.Parse("22222222-2222-2222-2222-222222222222")
                },
                new Funcionario {
                    FuncionarioId = Guid.Parse("55555555-bbbb-bbbb-bbbb-555555555555"),
                    NomeCompleto = "Juliana Ferreira",
                    Cpf = "147.258.369-44",
                    Cargo = Funcionario.CargoEnum.DonoFilial,
                    Ativo = true,
                    FilialId = Guid.Parse("33333333-3333-3333-3333-333333333333")
                },
                new Funcionario {
                    FuncionarioId = Guid.Parse("66666666-bbbb-bbbb-bbbb-666666666666"),
                    NomeCompleto = "Paulo Henrique",
                    Cpf = "258.369.147-55",
                    Cargo = Funcionario.CargoEnum.Mecanico,
                    Ativo = true,
                    FilialId = Guid.Parse("44444444-4444-4444-4444-444444444444")
                },
                new Funcionario {
                    FuncionarioId = Guid.Parse("77777777-bbbb-bbbb-bbbb-777777777777"),
                    NomeCompleto = "Fernanda Dias",
                    Cpf = "369.147.258-66",
                    Cargo = Funcionario.CargoEnum.Gestor,
                    Ativo = true,
                    FilialId = Guid.Parse("55555555-5555-5555-5555-555555555555")
                },
                new Funcionario {
                    FuncionarioId = Guid.Parse("88888888-bbbb-bbbb-bbbb-888888888888"),
                    NomeCompleto = "Ricardo Gomes",
                    Cpf = "741.852.963-77",
                    Cargo = Funcionario.CargoEnum.Funcionario,
                    Ativo = true,
                    FilialId = Guid.Parse("66666666-6666-6666-6666-666666666666")
                },
                new Funcionario {
                    FuncionarioId = Guid.Parse("99999999-bbbb-bbbb-bbbb-999999999999"),
                    NomeCompleto = "Camila Rocha",
                    Cpf = "852.963.741-88",
                    Cargo = Funcionario.CargoEnum.Mecanico,
                    Ativo = true,
                    FilialId = Guid.Parse("77777777-7777-7777-7777-777777777777")
                },
                new Funcionario {
                    FuncionarioId = Guid.Parse("aaaaaaaa-bbbb-bbbb-bbbb-aaaaaaaaaaaa"),
                    NomeCompleto = "Lucas Almeida",
                    Cpf = "963.741.852-99",
                    Cargo = Funcionario.CargoEnum.Gestor,
                    Ativo = true,
                    FilialId = Guid.Parse("88888888-8888-8888-8888-888888888888")
                }
            );

        }
    }