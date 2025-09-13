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
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + "ApiPG_Salt"));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
