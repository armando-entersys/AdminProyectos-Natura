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
            // Configuración de la relación uno-a-uno
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.UserRol)
                .WithOne(r => r.Usuario)
                .HasForeignKey<Usuario>(u => u.RolId);  // Especificar que Usuario tiene la clave foránea

            base.OnModelCreating(modelBuilder);
        }

    }
}
