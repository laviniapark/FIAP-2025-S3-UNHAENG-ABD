using ManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementApp.Infrastructure ;

    public class ManagementDb : DbContext
    {
        public ManagementDb(DbContextOptions options) : base(options)
        {
        }
        
        public DbSet<Moto> Motos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Filial> Filiais { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var moto = modelBuilder.Entity<Moto>();
            moto.HasKey(m => m.MotoId);
            
            moto.Property(m => m.Placa).IsRequired().HasMaxLength(7);
            moto.HasIndex(m => m.Placa).IsUnique();
            moto.Property(m => m.Modelo).IsRequired().HasMaxLength(30);
            moto.Property(m => m.Ano).IsRequired().HasColumnType("NUMBER(4)");
            moto.Property(m => m.Status).HasConversion<string>().HasMaxLength(20);
            
            moto.HasOne(m => m.Filial)
                .WithMany(f => f.Motos)
                .HasForeignKey(m => m.FilialId);
            
            
            var funcionario = modelBuilder.Entity<Funcionario>();
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
            
            var filial = modelBuilder.Entity<Filial>();
            filial.HasKey(f => f.FilialId);
            
            filial.Property(f => f.Nome).IsRequired().HasMaxLength(100);
            filial.Property(f => f.Cnpj).IsRequired().HasMaxLength(18);
            filial.HasIndex(f => f.Cnpj).IsUnique();
            filial.Property(f => f.Telefone).HasMaxLength(25);
            filial.Property(f => f.DataAbertura).HasColumnType("DATE");
            filial.Property(f => f.DataEncerramento).HasColumnType("DATE");
            
            var endereco = modelBuilder.Entity<Endereco>();
            endereco.HasKey(e => e.FilialId);
            
            endereco.Property(e => e.CEP).IsRequired().HasMaxLength(9);
            endereco.Property(e => e.Logradouro).IsRequired().HasMaxLength(150);
            endereco.Property(e => e.Numero).IsRequired().HasMaxLength(10);
            endereco.Property(e => e.Complemento).HasMaxLength(60);
            endereco.Property(e => e.Bairro).IsRequired().HasMaxLength(80);
            endereco.Property(e => e.Cidade).IsRequired().HasMaxLength(80);
            endereco.Property(e => e.UF).IsRequired().HasMaxLength(2);
            endereco.Property(e => e.Pais).IsRequired().HasMaxLength(60);
            
            filial.HasOne(f => f.Endereco)
                .WithOne(e => e.Filial)
                .HasForeignKey<Endereco>(e => e.FilialId)
                .OnDelete(DeleteBehavior.Restrict);
            
        }
    }