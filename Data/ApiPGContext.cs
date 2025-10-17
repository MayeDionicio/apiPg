using Microsoft.EntityFrameworkCore;
using ApiPG.Models;

namespace ApiPG.Data
{
    public class ApiPGContext : DbContext
    {
        public ApiPGContext(DbContextOptions<ApiPGContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Nivel> Niveles { get; set; }
        public DbSet<NivelDeParticipante> NivelesDeParticipantes { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<Recurso> Recursos { get; set; }
        public DbSet<AsignacionDeRecurso> AsignacionesDeRecurso { get; set; }
        public DbSet<RegistroDeUsoDeRecurso> RegistrosDeUsoDeRecurso { get; set; }
        public DbSet<Devocional> Devocionales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PrimerNombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CorreoElectronico).IsRequired().HasMaxLength(255);
                entity.Property(e => e.NombreDeUsuario).IsRequired().HasMaxLength(50);
                entity.Property(e => e.HashDeContrasena).IsRequired();
                entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.EstaActivo).HasDefaultValue(true);

                // Relación con Rol
                entity.HasOne(e => e.Rol)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(e => e.IdRol)
                      .OnDelete(DeleteBehavior.Restrict);

                // Índices únicos
                entity.HasIndex(e => e.CorreoElectronico).IsUnique();
                entity.HasIndex(e => e.NombreDeUsuario).IsUnique();
            });

            // Configuración para Rol
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.EstaActivo).HasDefaultValue(true);

                // Índice único para el nombre del rol
                entity.HasIndex(e => e.Nombre).IsUnique();

                // Datos semilla para roles
                entity.HasData(
                    new Rol { Id = 1, Nombre = "Administrator", Descripcion = "Full system access", CreadoEn = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    // 'User' renombrado a 'Participante'
                    new Rol { Id = 2, Nombre = "Participante", Descripcion = "Acceso de participante / beneficiario", CreadoEn = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new Rol { Id = 3, Nombre = "Viewer", Descripcion = "Read-only access", CreadoEn = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new Rol { Id = 4, Nombre = "Manager", Descripcion = "Management access", CreadoEn = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
                    // Nuevo rol: Coordinador
                    new Rol { Id = 5, Nombre = "Coordinador", Descripcion = "Coordinación y supervisión", CreadoEn = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc) }
                );
            });

            // Datos semilla para usuarios
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasData(
                    new Usuario
                    {
                        Id = 1,
                        PrimerNombre = "Admin",
                        Apellido = "System",
                        CorreoElectronico = "admin@apipg.com",
                        NombreDeUsuario = "admin",
                        HashDeContrasena = HashPassword("admin123"),
                        IdRol = 1,
                        CreadoEn = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new Usuario
                    {
                        Id = 2,
                        PrimerNombre = "Juan",
                        Apellido = "Pérez",
                        CorreoElectronico = "juan.perez@email.com",
                        NombreDeUsuario = "jperez",
                        HashDeContrasena = HashPassword("user123"),
                        IdRol = 2,
                        CreadoEn = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new Usuario
                    {
                        Id = 3,
                        PrimerNombre = "María",
                        Apellido = "García",
                        CorreoElectronico = "maria.garcia@email.com",
                        NombreDeUsuario = "mgarcia",
                        HashDeContrasena = HashPassword("user123"),
                        IdRol = 3,
                        CreadoEn = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new Usuario
                    {
                        Id = 4,
                        PrimerNombre = "Carlos",
                        Apellido = "Coordinador",
                        CorreoElectronico = "coordinador@apipg.com",
                        NombreDeUsuario = "coordinador",
                        HashDeContrasena = HashPassword("coord123"),
                        IdRol = 5,
                        CreadoEn = new DateTime(2024, 1, 25, 0, 0, 0, DateTimeKind.Utc)
                    }
                );
            });

            // Configuración para Nivel
            modelBuilder.Entity<Nivel>(entity =>
            {
                entity.ToTable("Niveles");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.EstaActivo).HasDefaultValue(true);

                // Voluntario (Usuario) relación opcional
                entity.HasOne(e => e.Voluntario)
                    .WithMany()
                    .HasForeignKey(e => e.IdVoluntario)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración para NivelDeParticipante (many-to-many)
            modelBuilder.Entity<NivelDeParticipante>(entity =>
            {
                entity.ToTable("NivelesDeParticipantes");
                entity.HasKey(e => new { e.IdNivel, e.IdUsuario });

                entity.Property(e => e.AsignadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.EstaActivo).HasDefaultValue(true);

                entity.HasOne(e => e.Nivel)
                      .WithMany(l => l.ParticipantesDelNivel)
                      .HasForeignKey(e => e.IdNivel)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.IdUsuario)
                      .OnDelete(DeleteBehavior.Cascade);
            });

        // Configuración para Asistencia
        modelBuilder.Entity<Asistencia>(entity =>
        {
            entity.ToTable("Asistencias");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Fecha).IsRequired();
            entity.Property(e => e.Presente).IsRequired();
            entity.Property(e => e.Observaciones).HasMaxLength(500);
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Nivel)
                .WithMany()
                .HasForeignKey(e => e.IdNivel)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración para Recurso
        modelBuilder.Entity<Recurso>(entity =>
        {
            entity.ToTable("Recursos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Categoria).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Cantidad).IsRequired();
            entity.Property(e => e.CantidadDisponible).IsRequired();
            entity.Property(e => e.Unidad).HasMaxLength(50);
            entity.Property(e => e.Ubicacion).HasMaxLength(200);
            entity.Property(e => e.ValorEstimado).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Notas).HasMaxLength(1000);
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.EstaActivo).HasDefaultValue(true);

            // Índices
            entity.HasIndex(e => e.Nombre);
            entity.HasIndex(e => e.Categoria);
            entity.HasIndex(e => e.EstaActivo);
        });

        // Configuración para AsignacionDeRecurso
        modelBuilder.Entity<AsignacionDeRecurso>(entity =>
        {
            entity.ToTable("AsignacionesDeRecurso");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdRecurso).IsRequired();
            entity.Property(e => e.IdVoluntario).IsRequired();
            entity.Property(e => e.CantidadAsignada).IsRequired();
            entity.Property(e => e.Estado).IsRequired();
            entity.Property(e => e.AsignadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.NotasIniciales).HasMaxLength(1000);
            entity.Property(e => e.NotasDelVoluntario).HasMaxLength(1000);
            entity.Property(e => e.EstaActivo).HasDefaultValue(true);

            // Relaciones
            entity.HasOne(e => e.Recurso)
                .WithMany(r => r.AsignacionesDeRecurso)
                .HasForeignKey(e => e.IdRecurso)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Voluntario)
                .WithMany()
                .HasForeignKey(e => e.IdVoluntario)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AsignadoPor)
                .WithMany()
                .HasForeignKey(e => e.AsignadoPorIdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            entity.HasIndex(e => e.IdRecurso);
            entity.HasIndex(e => e.IdVoluntario);
            entity.HasIndex(e => e.Estado);
            entity.HasIndex(e => e.AsignadoEn);
        });

        // Configuración para RegistroDeUsoDeRecurso
        modelBuilder.Entity<RegistroDeUsoDeRecurso>(entity =>
        {
            entity.ToTable("RegistrosDeUsoDeRecurso");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdAsignacionDeRecurso).IsRequired();
            entity.Property(e => e.TipoDeEvento).IsRequired();
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.AccionesTomadas).HasMaxLength(1000);
            entity.Property(e => e.Recomendaciones).HasMaxLength(1000);
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ReportadoPorIdUsuario).IsRequired();
            entity.Property(e => e.UrlsDeFotos).HasMaxLength(500);
            entity.Property(e => e.NotasDeResolucion).HasMaxLength(1000);

            // Relaciones
            entity.HasOne(e => e.AsignacionDeRecurso)
                .WithMany(ra => ra.RegistrosDeUso)
                .HasForeignKey(e => e.IdAsignacionDeRecurso)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ReportadoPor)
                .WithMany()
                .HasForeignKey(e => e.ReportadoPorIdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ResueltoPor)
                .WithMany()
                .HasForeignKey(e => e.ResueltoPorIdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            entity.HasIndex(e => e.IdAsignacionDeRecurso);
            entity.HasIndex(e => e.TipoDeEvento);
            entity.HasIndex(e => e.CreadoEn);
            entity.HasIndex(e => e.EstaResuelto);
        });

        // Configuración para Devocional
        modelBuilder.Entity<Devocional>(entity =>
        {
            entity.ToTable("Devocionales");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Pasaje).HasMaxLength(200);
            entity.Property(e => e.VoluntarioAsignado).HasMaxLength(100);
            entity.Property(e => e.TextoClave).HasMaxLength(1000);
            entity.Property(e => e.Objetivo).HasMaxLength(1000);
            entity.Property(e => e.Idea).HasMaxLength(1000);
            entity.Property(e => e.Aplicacion).HasMaxLength(2000);
            entity.Property(e => e.Reto).HasMaxLength(1000);
            entity.Property(e => e.Oracion).HasMaxLength(1000);
            entity.Property(e => e.Recursos).HasMaxLength(2000);
            entity.Property(e => e.CreadoEn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.EstaActivo).HasDefaultValue(true);
            entity.Property(e => e.Estado).HasDefaultValue(DevocionalEstado.Borrador);

            // Relaciones
            entity.HasOne(e => e.CreadoPorUsuario)
                .WithMany()
                .HasForeignKey(e => e.CreadoPorIdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ActualizadoPorUsuario)
                .WithMany()
                .HasForeignKey(e => e.ActualizadoPorIdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.VoluntarioAsignadoUser)
                .WithMany()
                .HasForeignKey(e => e.VoluntarioAsignadoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            entity.HasIndex(e => e.Titulo);
            entity.HasIndex(e => e.FechaProgramada);
            entity.HasIndex(e => e.Estado);
            entity.HasIndex(e => e.VoluntarioAsignadoId);
            entity.HasIndex(e => e.CreadoEn);
            entity.HasIndex(e => e.EstaActivo);
        });
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + "ApiPG_Salt"));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
