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
        public DbSet<TipoUsuario> TiposUsuario { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<Brief> Briefs { get; set; }
        public DbSet<TipoBrief> TiposBrief { get; set; }
        public DbSet<EstatusBrief> EstatusBriefs { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Rol> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la relación uno-a-uno
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.UserRol)
                .WithOne(r => r.Usuario)
                .HasForeignKey<Usuario>(u => u.RolId);  // Especificar que Usuario tiene la clave foránea
                                                        // Configurar la relación de muchos a uno entre Menu y Rol
            modelBuilder.Entity<Menu>()
                .HasOne(m => m.Rol)        // Un Menu tiene un Rol
                .WithMany(r => r.Menus)    // Un Rol tiene muchos Menus
                .HasForeignKey(m => m.RolId); // Llave foránea en Menu
                                              // Configurar la relación de uno a uno entre Usuario y TipoUsuario
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.TipoUsuario)  // Un Usuario tiene un TipoUsuario
                .WithOne(t => t.Usuario)     // Un TipoUsuario tiene un Usuario
                .HasForeignKey<Usuario>(u => u.TipoUsuarioId); // Llave foránea en Usuario
                                                               // Configurar la relación de uno a muchos entre Usuario y Brief
            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Briefs)         // Un Usuario tiene muchos Briefs
                .WithOne(b => b.Usuario)       // Cada Brief tiene un Usuario
                .HasForeignKey(b => b.UsuarioId); // Llave foránea en Brief

            modelBuilder.Entity<Brief>()
              .HasOne(b => b.TipoBrief)      // Un Brief tiene un TipoBrief
              .WithMany(t => t.Briefs)       // Un TipoBrief tiene muchos Briefs
              .HasForeignKey(b => b.TipoBriefId)  // Llave foránea en Brief
              .OnDelete(DeleteBehavior.Cascade); // Opcional: configuración para la eliminación en cascada
        }

    }

}
