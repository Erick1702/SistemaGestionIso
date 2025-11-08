using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaGestionIso.Entidades;
using SistemaGestionIso.Servicios;

namespace SistemaGestionIso.Utilidades
{
    public static class Seeding
    {
        private static List<string> roles = new List<string>
        {
            Constantes.RolAdministrador,
            Constantes.RolEncargadoSig,
            Constantes.RolUsuarioEstandar,
            Constantes.RolAuditor
        };

        // Datos del usuario administrador por defecto
        private const string AdminEmail = "admin@gmail.com";
        private const string AdminPassword = "Abc123!";
        private const string AdminPrimerNombre = "Administrador";
        private const string AdminPrimerApellido = "Sistema";
        private const string AdminCelular = "+51999999";

        public static void Aplicar(DbContext context, bool _)
        {
            // 1. Crear roles
            foreach (var rol in roles)
            {
                var rolDB = context.Set<IdentityRole>().FirstOrDefault(r => r.Name == rol);
                if (rolDB is null)
                {
                    context.Set<IdentityRole>().Add(new IdentityRole
                    {
                        Name = rol,
                        NormalizedName = rol.ToUpper()
                    });
                    context.SaveChanges(); //Verificar si sale error por estar dentro del foreach
                }
            }
            // 2. Crear usuario administrador
            CrearUsuarioAdministrador(context);
        }

        //Asyncrono
        public static async Task AplicarAsync(DbContext context, bool _, CancellationToken cancellationToken)
        {
            foreach (var rol in roles)
            {
                var rolDB = await context.Set<IdentityRole>().FirstOrDefaultAsync(r => r.Name == rol);

                if (rolDB is null)
                {
                    context.Set<IdentityRole>().Add(new IdentityRole
                    {
                        Name = rol,
                        NormalizedName = rol.ToUpper()
                    });
                    await context.SaveChangesAsync(cancellationToken); //Verificar si sale error por estar dentro del foreach
                }
            }
            // 2. Crear usuario administrador
            await CrearUsuarioAdministradorAsync(context, cancellationToken);
        }

        private static void CrearUsuarioAdministrador(DbContext context)
        {
            // Verificar si ya existe el usuario administrador
            var usuarioExistente = context.Set<Usuario>()
                .FirstOrDefault(u => u.Email == AdminEmail);

            if (usuarioExistente == null)
            {
                var hasher = new PasswordHasher<Usuario>();
                var adminUserId = Guid.NewGuid().ToString();

                var adminUser = new Usuario
                {
                    Id = adminUserId,
                    UserName = AdminEmail,
                    NormalizedUserName = AdminEmail.ToUpper(),
                    Email = AdminEmail,
                    NormalizedEmail = AdminEmail.ToUpper(),
                    EmailConfirmed = true,
                    PrimerNombre = AdminPrimerNombre,
                    SegundoNombre = "",
                    PrimerApellido = AdminPrimerApellido,
                    SegundoApellido = "",
                    Celular = AdminCelular,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = false,
                    TwoFactorEnabled = false,
                    AccessFailedCount = 0
                };

                adminUser.PasswordHash = hasher.HashPassword(adminUser, AdminPassword);

                context.Set<Usuario>().Add(adminUser);
                context.SaveChanges();

                // Asignar rol de Administrador
                var adminRole = context.Set<IdentityRole>()
                    .FirstOrDefault(r => r.Name == Constantes.RolAdministrador);

                if (adminRole != null)
                {
                    context.Set<IdentityUserRole<string>>().Add(new IdentityUserRole<string>
                    {
                        UserId = adminUserId,
                        RoleId = adminRole.Id
                    });
                    context.SaveChanges();
                }
            }
        }

        private static async Task CrearUsuarioAdministradorAsync(DbContext context, CancellationToken cancellationToken)
        {
            // Verificar si ya existe el usuario administrador
            var usuarioExistente = await context.Set<Usuario>()
                .FirstOrDefaultAsync(u => u.Email == AdminEmail, cancellationToken);

            if (usuarioExistente == null)
            {
                var hasher = new PasswordHasher<Usuario>();
                var adminUserId = Guid.NewGuid().ToString();

                var adminUser = new Usuario
                {
                    Id = adminUserId,
                    UserName = AdminEmail,
                    NormalizedUserName = AdminEmail.ToUpper(),
                    Email = AdminEmail,
                    NormalizedEmail = AdminEmail.ToUpper(),
                    EmailConfirmed = true,
                    PrimerNombre = AdminPrimerNombre,
                    SegundoNombre = "",
                    PrimerApellido = AdminPrimerApellido,
                    SegundoApellido = "",
                    Celular = AdminCelular,
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = false,
                    TwoFactorEnabled = false,
                    AccessFailedCount = 0
                };

                adminUser.PasswordHash = hasher.HashPassword(adminUser, AdminPassword);

                context.Set<Usuario>().Add(adminUser);
                await context.SaveChangesAsync(cancellationToken);

                // Asignar rol de Administrador
                var adminRole = await context.Set<IdentityRole>()
                    .FirstOrDefaultAsync(r => r.Name == Constantes.RolAdministrador, cancellationToken);

                if (adminRole != null)
                {
                    context.Set<IdentityUserRole<string>>().Add(new IdentityUserRole<string>
                    {
                        UserId = adminUserId,
                        RoleId = adminRole.Id
                    });
                    await context.SaveChangesAsync(cancellationToken);
                }
            }
        }
    }
}
