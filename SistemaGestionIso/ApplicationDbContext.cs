using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaGestionIso.Entidades;

namespace SistemaGestionIso
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<NormaIso> NormaIsos { get; set; }
        public DbSet<Clausula> Clausulas { get; set; }
        public DbSet<Requisito> Requisitos { get; set; }
        public DbSet<Cumplimiento> Cumplimientos { get; set; }
        public DbSet<Evidencia> Evidencias { get; set; }
        public DbSet<DocumentoSGI> DocumentoSGIs { get; set; }
        public DbSet<DocumentoVersion> DocumentoVersions { get; set; }
        public DbSet<NoConformidad> NoConformidads { get; set; }
        public DbSet<AccionCorrectiva> AccionCorrectivas { get; set; }
    }
}
