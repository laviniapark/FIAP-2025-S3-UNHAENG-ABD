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
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ManagementDb).Assembly);
            
        }
    }