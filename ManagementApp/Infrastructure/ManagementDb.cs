using ManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementApp.Infrastructure ;

    public class ManagementDb : DbContext
    {
        public DbSet<Moto> Motos { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Filial> Filiais { get; set; }
        
        public ManagementDb(DbContextOptions<ManagementDb> options) : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ManagementDb).Assembly);
            
        }
    }