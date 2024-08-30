using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IAuthService
    {
        Task<Usuario> Autenticar(string correo, string contrasena);
        Task<Usuario> Registro(string correo, string Nombre);
        IEnumerable<Menu> GetMenusByRole(int rolId);
    }
    
}
