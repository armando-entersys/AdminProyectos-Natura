using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Context
{
    public class DataAccesContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=solucionesmkt.com.mx;initial catalog=AdminProyectosNaturaDB;user id=mkt;password=123456789;");

        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<Brief> Briefs { get; set; }
        public DbSet<EstatusBrief> EstatusBriefs { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<TipoBrief> TiposBrief { get; set; }
        public DbSet<Material> Materiales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
        }

    }

}
