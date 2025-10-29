namespace SistemaGestionIso.Servicios
{
    public class Constantes
    {
        public const string RolAdministrador = "Administrador";
        public const string RolEncargadoSig = "EncargadoSig";
        public const string RolUsuarioEstandar = "UsuarioEstandar";
        public const string RolAuditor = "Auditor";

        // Tipos de Auditoría
        public const string AuditoriaInterna = "Interna";
        public const string AuditoriaExterna = "Externa";
        public const string AuditoriaProveedores = "De Proveedores";
        public const string AuditoriaCertificacion = "De Certificación";
        public const string AuditoriaSeguimiento = "De Seguimiento";

        // Tipos de Hallazgo
        public const string HallazgoConformidad = "Conformidad";
        public const string HallazgoNoConformidad = "No Conformidad";
        public const string HallazgoObservacion = "Observación";
    }
}
