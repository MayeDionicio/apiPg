using Microsoft.EntityFrameworkCore;
using ApiPG.Models;

namespace ApiPG.Data
{
    public class ApiPGContext : DbContext
    {
        public ApiPGContext(DbContextOptions<ApiPGContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<LevelParticipant> LevelParticipants { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceAssignment> ResourceAssignments { get; set; }
        public DbSet<ResourceUsageLog> ResourceUsageLogs { get; set; }
        public DbSet<Devocional> Devocionales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                // Relación con Role
                entity.HasOne(e => e.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Índices únicos
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Configuración para Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                // Índice único para el nombre del rol
                entity.HasIndex(e => e.Name).IsUnique();

                // Datos semilla para roles
                entity.HasData(
                    new Role { Id = 1, Name = "Administrator", Description = "Full system access", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    // 'User' renombrado a 'Participante'
                    new Role { Id = 2, Name = "Participante", Description = "Acceso de participante / beneficiario", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new Role { Id = 3, Name = "Viewer", Description = "Read-only access", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                    new Role { Id = 4, Name = "Manager", Description = "Management access", CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
                    // Nuevo rol: Coordinador
                    new Role { Id = 5, Name = "Coordinador", Description = "Coordinación y supervisión", CreatedAt = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc) }
                );
            });

            // Datos semilla para usuarios
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasData(
                    new User
                    {
                        Id = 1,
                        FirstName = "Admin",
                        LastName = "System",
                        Email = "admin@apipg.com",
                        Username = "admin",
                        PasswordHash = HashPassword("admin123"),
                        RoleId = 1,
                        CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new User
                    {
                        Id = 2,
                        FirstName = "Juan",
                        LastName = "Pérez",
                        Email = "juan.perez@email.com",
                        Username = "jperez",
                        PasswordHash = HashPassword("user123"),
                        RoleId = 2,
                        CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new User
                    {
                        Id = 3,
                        FirstName = "María",
                        LastName = "García",
                        Email = "maria.garcia@email.com",
                        Username = "mgarcia",
                        PasswordHash = HashPassword("user123"),
                        RoleId = 3,
                        CreatedAt = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc)
                    },
                    new User
                    {
                        Id = 4,
                        FirstName = "Carlos",
                        LastName = "Coordinador",
                        Email = "coordinador@apipg.com",
                        Username = "coordinador",
                        PasswordHash = HashPassword("coord123"),
                        RoleId = 5,
                        CreatedAt = new DateTime(2024, 1, 25, 0, 0, 0, DateTimeKind.Utc)
                    }
                );
            });

            // Configuración para Level
            modelBuilder.Entity<Level>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                // Tutor (User) relación opcional
                entity.HasOne(e => e.Tutor)
                    .WithMany()
                    .HasForeignKey(e => e.TutorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración para LevelParticipant (many-to-many)
            modelBuilder.Entity<LevelParticipant>(entity =>
            {
                entity.HasKey(e => new { e.LevelId, e.UserId });

                entity.Property(e => e.AssignedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.IsActive).HasDefaultValue(true);

                entity.HasOne(e => e.Level)
                      .WithMany(l => l.LevelParticipants)
                      .HasForeignKey(e => e.LevelId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

        // Configuración para Attendances
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.Present).IsRequired();
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Level)
                .WithMany()
                .HasForeignKey(e => e.LevelId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración para Resource
        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.AvailableQuantity).IsRequired();
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.EstimatedValue).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            // Índices
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsActive);
        });

        // Configuración para ResourceAssignment
        modelBuilder.Entity<ResourceAssignment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ResourceId).IsRequired();
            entity.Property(e => e.VolunteerId).IsRequired();
            entity.Property(e => e.QuantityAssigned).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.AssignedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.InitialNotes).HasMaxLength(1000);
            entity.Property(e => e.VolunteerNotes).HasMaxLength(1000);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            // Relaciones
            entity.HasOne(e => e.Resource)
                .WithMany(r => r.ResourceAssignments)
                .HasForeignKey(e => e.ResourceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Volunteer)
                .WithMany()
                .HasForeignKey(e => e.VolunteerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AssignedByUser)
                .WithMany()
                .HasForeignKey(e => e.AssignedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            entity.HasIndex(e => e.ResourceId);
            entity.HasIndex(e => e.VolunteerId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.AssignedAt);
        });

        // Configuración para ResourceUsageLog
        modelBuilder.Entity<ResourceUsageLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ResourceAssignmentId).IsRequired();
            entity.Property(e => e.EventType).IsRequired();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.ActionsTaken).HasMaxLength(1000);
            entity.Property(e => e.Recommendations).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ReportedByUserId).IsRequired();
            entity.Property(e => e.PhotoUrls).HasMaxLength(500);
            entity.Property(e => e.ResolutionNotes).HasMaxLength(1000);

            // Relaciones
            entity.HasOne(e => e.ResourceAssignment)
                .WithMany(ra => ra.UsageLogs)
                .HasForeignKey(e => e.ResourceAssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ReportedByUser)
                .WithMany()
                .HasForeignKey(e => e.ReportedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ResolvedByUser)
                .WithMany()
                .HasForeignKey(e => e.ResolvedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Índices
            entity.HasIndex(e => e.ResourceAssignmentId);
            entity.HasIndex(e => e.EventType);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsResolved);
        });

        // Configuración para Devocional
        modelBuilder.Entity<Devocional>(entity =>
        {
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
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Estado).HasDefaultValue(DevocionalEstado.Borrador);

            // Relaciones
            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.UpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.UpdatedByUserId)
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
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsActive);
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
