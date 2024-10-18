using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IAuthDal
    {
        Task<Usuario> Autenticar(string correo, string contrasena);
        Task<Usuario> Registro(string correo, string Nombre);
        IEnumerable<Menu> GetMenusByRole(int rolId);
        respuestaServicio SolicitudUsuario(Usuario usuario);
    }
}
