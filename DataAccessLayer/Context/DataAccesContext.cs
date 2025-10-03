using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer.Context
{
    public class DataAccesContext : DbContext
    {
        // Constructor con opciones (para inyección de dependencias)
        public DataAccesContext(DbContextOptions<DataAccesContext> options) : base(options)
        {
        }

        // Constructor sin parámetros para compatibilidad con migraciones
        public DataAccesContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Solo configurar si no se pasaron opciones (para migraciones y desarrollo local)
            if (!optionsBuilder.IsConfigured)
            {
                // Primero intentar variable de entorno (Docker)
                var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

                // Si no existe variable de entorno, usar el connection string de desarrollo local
                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = "Server=solucionesmkt.com.mx;initial catalog=AdminProyectosNaturaDB;user id=mkt;password=123456789;";
                }

                optionsBuilder.UseSqlServer(connectionString);
            }
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<Brief> Briefs { get; set; }
        public DbSet<EstatusBrief> EstatusBriefs { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<TipoBrief> TiposBrief { get; set; }
        public DbSet<Material> Materiales { get; set; }
        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<Participante> Participantes { get; set; }
        public DbSet<EstatusMaterial> EstatusMateriales { get; set; }
        public DbSet<TipoAlerta> TipoAlerta { get; set; }
        public DbSet<RetrasoMaterial> RetrasoMateriales { get; set; }
        public DbSet<HistorialMaterial> HistorialMateriales { get; set; }
        public DbSet<Prioridad> Prioridad { get; set; }
        public DbSet<PCN> PCN { get; set; }
        public DbSet<Audiencia> Audiencia { get; set; }
        public DbSet<Formato> Formato { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de la relación muchos a uno entre Usuario y Rol
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.UserRol)
                .WithMany(r => r.Usuarios) // Rol tiene muchos Usuarios
                .HasForeignKey(u => u.RolId);  // Especificar que Usuario tiene la clave foránea

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Correo)
                .IsUnique();
            
            modelBuilder.Entity<Menu>()
                .HasOne(m => m.Rol)        // Un Menu tiene un Rol
                .WithMany(r => r.Menus)    // Un Rol tiene muchos Menus
                .HasForeignKey(m => m.RolId); // Llave foránea en Menu
                                              // Configurar la relación de uno a uno entre Usuario y TipoUsuario
           
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Briefs)         // Un Usuario tiene muchos Briefs
                .WithOne(b => b.Usuario)       // Cada Brief tiene un Usuario
                .HasForeignKey(b => b.UsuarioId); // Llave foránea en Brief

            modelBuilder.Entity<Brief>()
                .HasOne(b => b.EstatusBrief)      // Un Brief tiene un EstatusBrief
                .WithMany(t => t.Briefs)       // Un EstatusBrief tiene muchos Briefs
                .HasForeignKey(b => b.EstatusBriefId)  // Llave foránea en Brief
                .OnDelete(DeleteBehavior.Cascade); // Opcional: configuración para la eliminación en cascada

            modelBuilder.Entity<Brief>()
                .HasOne(b => b.TipoBrief)
                .WithMany(tb => tb.Briefs)  // Navegación inversa
                .HasForeignKey(b => b.TipoBriefId)
                .OnDelete(DeleteBehavior.Restrict); // Configura el comportamiento de eliminación

            // Configuración de la relación uno a muchos
            modelBuilder.Entity<Brief>()
                .HasMany(b => b.Materiales)  // Un Brief tiene muchos Materiales
                .WithOne(m => m.Brief)        // Un Material pertenece a un Brief
                .HasForeignKey(m => m.BriefId) // Llave foránea en Material
                .OnDelete(DeleteBehavior.Cascade); // Eliminar materiales cuando se elimine el Brief
           
            // Configuración de la relación uno a uno
            modelBuilder.Entity<Proyecto>()
                .HasOne(p => p.Brief)
                .WithOne(b => b.Proyecto)
                .HasForeignKey<Proyecto>(p => p.BriefId);

            // Configuración de la relación
            modelBuilder.Entity<Alerta>()
                .HasOne(a => a.TipoAlerta)
                .WithMany(t => t.Alertas)
                .HasForeignKey(a => a.IdTipoAlerta);

            modelBuilder.Entity<Participante>()
          .HasKey(p => p.Id); // Definir la clave primaria

            modelBuilder.Entity<Participante>()
                .HasOne(p => p.Brief)
                .WithMany() // Si quieres tener una colección en Brief, puedes usar .WithMany(b => b.Participantes)
                .HasForeignKey(p => p.BriefId)
                .OnDelete(DeleteBehavior.Cascade); // Configurar el comportamiento de eliminación

            modelBuilder.Entity<Participante>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Participantes) // Asegúrate de que la clase Usuario tenga una propiedad ICollection<Participante>
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction); // Cambiado a NoAction para evitar múltiples cascade paths

            modelBuilder.Entity<HistorialMaterial>()
                .HasOne(h => h.Material) // HistorialMaterial tiene un Material
                .WithMany(m => m.Historiales) // Material tiene muchos HistorialMaterial
                .HasForeignKey(h => h.MaterialId) // Llave foránea en HistorialMaterial
                .OnDelete(DeleteBehavior.Cascade); // Configuración de eliminación en cascada

            // Relación de Material con RetrasoMaterial
            modelBuilder.Entity<RetrasoMaterial>()
                .HasOne(r => r.Material)
                .WithMany(m => m.Retrasos)
                .HasForeignKey(r => r.MaterialId);
            // Relación de Material con Prioridad
            modelBuilder.Entity<Material>()
                .HasOne(m => m.Prioridad)
                .WithMany(p => p.Materiales)
                .HasForeignKey(m => m.PrioridadId);

            // Relación de Material con PCN
            modelBuilder.Entity<Material>()
                .HasOne(m => m.PCN)
                .WithMany(p => p.Materiales)
                .HasForeignKey(m => m.PCNId);

            // Relación de Material con Audiencia
            modelBuilder.Entity<Material>()
                .HasOne(m => m.Audiencia)
                .WithMany(a => a.Materiales)
                .HasForeignKey(m => m.AudienciaId);

            // Relación de Material con Formato
            modelBuilder.Entity<Material>()
                .HasOne(m => m.Formato)
                .WithMany(f => f.Materiales)
                .HasForeignKey(m => m.FormatoId);

            // Relación entre HistorialMaterial y Usuario
            modelBuilder.Entity<HistorialMaterial>()
                .HasOne(h => h.Usuario) // HistorialMaterial tiene un Usuario
                .WithMany()             // Un Usuario puede tener muchos HistorialMaterial
                .HasForeignKey(h => h.UsuarioId) // Clave foránea en HistorialMaterial
                .OnDelete(DeleteBehavior.Restrict); // Evitar eliminación en cascada

        }

    }

}
