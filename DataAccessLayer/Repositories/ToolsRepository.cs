using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Context;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ToolsRepository : IToolsDal
    {
        private readonly DataAccesContext _context;

        public ToolsRepository(DataAccesContext context)
        {
            _context = context;
        }
        public IEnumerable<Usuario> GetUsuarioByRol(int rolId)
        {
            return _context.Usuarios
                                 .Where(m => m.RolId == rolId)
                                 .ToList();
        }
        public IEnumerable<Usuario> GetUsuarioBySolicitud()
        {
            return _context.Usuarios
                                 .Where(m => m.SolicitudRegistro == true)
                                 .ToList();
        }
        public Usuario CambioSolicitudUsuario(int id,bool estatus)
        {

            Usuario usuario = _context.Usuarios.Where(q => q.Id == id).FirstOrDefault();
            usuario.Estatus = estatus;
            usuario.FechaModificacion = DateTime.Now;
            _context.SaveChanges();
            return usuario;
        }
    }
}
