using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaGestionIso.Entidades;

namespace SistemaGestionIso
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected ApplicationDbContext() { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<NormaIso> NormaIsos { get; set; }
        public DbSet<Clausula> Clausulas { get; set; }
        public DbSet<Requisito> Requisitos { get; set; }
        public DbSet<Cumplimiento> Cumplimientos { get; set; }
        public DbSet<Evidencia> Evidencias { get; set; }
        public DbSet<DocumentoSGI> DocumentoSGIs { get; set; }
        public DbSet<DocumentoVersion> DocumentoVersiones { get; set; }
        public DbSet<NoConformidad> NoConformidades { get; set; }
        public DbSet<AccionCorrectiva> AccionCorrectivas { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<Hallazgo> Hallazgos { get; set; }
    }
}
