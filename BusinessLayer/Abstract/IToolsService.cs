using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IToolsService
    {
        IEnumerable<Usuario> GetUsuarioByRol(int rolId);
        IEnumerable<Usuario> GetUsuarioBySolicitud();
        respuestaServicio CambioSolicitud(int id, bool estatus);
    }
}
