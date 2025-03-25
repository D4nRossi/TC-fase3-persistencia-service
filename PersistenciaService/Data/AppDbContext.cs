using MassTransit;
using Microsoft.EntityFrameworkCore;
using PersistenciaService.Data.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PersistenciaService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<ContatoModel> CONTATO_CTT { get; set; }
        public DbSet<MunicipioModel> MUNICIPIO_MNC { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContatoModel>().ToTable("CONTATO_CTT").HasKey(x => x.CTT_ID);
            modelBuilder.Entity<MunicipioModel>().ToTable("MUNICIPIO_MNC").HasKey(x => x.MNC_ID);
        }
    }
}
