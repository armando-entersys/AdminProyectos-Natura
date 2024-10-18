using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IToolsDal
    {
        IEnumerable<Usuario> GetUsuarioByRol(int rolId);
        IEnumerable<Usuario> GetUsuarioBySolicitud();
        Usuario CambioSolicitudUsuario(int id, bool estatus);
    }
}
