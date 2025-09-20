using ManagementApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManagementApp.Infrastructure.Config ;

    public class MotoConfig : IEntityTypeConfiguration<Moto>
    {
        public void Configure(EntityTypeBuilder<Moto> moto)
        {
            moto.HasKey(m => m.MotoId);
            
            moto.Property(m => m.Placa).IsRequired().HasMaxLength(7);
            moto.HasIndex(m => m.Placa).IsUnique();
            moto.Property(m => m.Marca).IsRequired().HasMaxLength(30);
            moto.Property(m => m.Modelo).IsRequired().HasMaxLength(30);
            moto.Property(m => m.Ano).IsRequired().HasColumnType("NUMBER(4)");
            moto.Property(m => m.Status).HasConversion<string>().HasMaxLength(20);
            
            moto.HasOne(m => m.Filial)
                .WithMany(f => f.Motos)
                .HasForeignKey(m => m.FilialId);
            
            // SEEDS
            moto.HasData(
                new Moto {
                    MotoId = Guid.Parse("11111111-aaaa-aaaa-aaaa-111111111111"),
                    Placa = "ABC1D23",
                    Marca = "Honda",
                    Modelo = "CG 160",
                    Ano = 2020,
                    Status = Moto.StatusEnum.Disponivel,
                    FilialId = Guid.Parse("11111111-1111-1111-1111-111111111111")
                },
                new Moto {
                    MotoId = Guid.Parse("22222222-aaaa-aaaa-aaaa-222222222222"),
                    Placa = "XYZ9H88",
                    Marca = "Yamaha",
                    Modelo = "Factor 150",
                    Ano = 2021,
                    Status = Moto.StatusEnum.Ocupada,
                    FilialId = Guid.Parse("11111111-1111-1111-1111-111111111111")
                },
                new Moto {
                    MotoId = Guid.Parse("33333333-aaaa-aaaa-aaaa-333333333333"),
                    Placa = "JKL4F55",
                    Marca = "Honda",
                    Modelo = "Biz 125",
                    Ano = 2019,
                    Status = Moto.StatusEnum.Manutencao,
                    FilialId = Guid.Parse("22222222-2222-2222-2222-222222222222")
                },
                new Moto {
                    MotoId = Guid.Parse("44444444-aaaa-aaaa-aaaa-444444444444"),
                    Placa = "MNO7G66",
                    Marca = "Yamaha",
                    Modelo = "YBR 125",
                    Ano = 2018,
                    Status = Moto.StatusEnum.Disponivel,
                    FilialId = Guid.Parse("22222222-2222-2222-2222-222222222222")
                },
                new Moto {
                    MotoId = Guid.Parse("55555555-aaaa-aaaa-aaaa-555555555555"),
                    Placa = "PQR8H77",
                    Marca = "Honda",
                    Modelo = "XRE 300",
                    Ano = 2022,
                    Status = Moto.StatusEnum.Disponivel,
                    FilialId = Guid.Parse("33333333-3333-3333-3333-333333333333")
                },
                new Moto {
                    MotoId = Guid.Parse("66666666-aaaa-aaaa-aaaa-666666666666"),
                    Placa = "STU2J99",
                    Marca = "Honda",
                    Modelo = "PCX 150",
                    Ano = 2023,
                    Status = Moto.StatusEnum.Ocupada,
                    FilialId = Guid.Parse("44444444-4444-4444-4444-444444444444")
                },
                new Moto {
                    MotoId = Guid.Parse("77777777-aaaa-aaaa-aaaa-777777777777"),
                    Placa = "VWX3K11",
                    Marca = "Yamaha",
                    Modelo = "NMAX 160",
                    Ano = 2021,
                    Status = Moto.StatusEnum.Disponivel,
                    FilialId = Guid.Parse("55555555-5555-5555-5555-555555555555")
                },
                new Moto {
                    MotoId = Guid.Parse("88888888-aaaa-aaaa-aaaa-888888888888"),
                    Placa = "YZA5L22",
                    Marca = "Honda",
                    Modelo = "CB 500F",
                    Ano = 2017,
                    Status = Moto.StatusEnum.Manutencao,
                    FilialId = Guid.Parse("66666666-6666-6666-6666-666666666666")
                },
                new Moto {
                    MotoId = Guid.Parse("99999999-aaaa-aaaa-aaaa-999999999999"),
                    Placa = "BCD6M33",
                    Marca = "Suzuki",
                    Modelo = "Burgman 125",
                    Ano = 2020,
                    Status = Moto.StatusEnum.Disponivel,
                    FilialId = Guid.Parse("77777777-7777-7777-7777-777777777777")
                },
                new Moto {
                    MotoId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Placa = "EFG7N44",
                    Marca = "Kawasaki",
                    Modelo = "Z400",
                    Ano = 2022,
                    Status = Moto.StatusEnum.Ocupada,
                    FilialId = Guid.Parse("88888888-8888-8888-8888-888888888888")
                }
            );
        }
    }