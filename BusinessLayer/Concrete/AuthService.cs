using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Context;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class AuthService : IAuthService
    {
       
        private readonly IAuthDal _authDal;

        public AuthService(IAuthDal authDal)
        {
            _authDal = authDal;
        }

        public async Task<Usuario> Autenticar(string correo, string contrasena)
        {
            Usuario usuario = await _authDal.Autenticar(correo, contrasena);
            return usuario;
        }
        public async Task<Usuario> Registro(string correo, string Nombre)
        {
            Usuario usuario = await _authDal.Registro(correo,Nombre);
            return usuario;
        }
        public IEnumerable<Menu> GetMenusByRole(int rolId)
        {
            IEnumerable<Menu> menu =  _authDal.GetMenusByRole(rolId);
            return menu;
        }
        public respuestaServicio SolicitudUsuario(Usuario usuario)
        {
           return _authDal.SolicitudUsuario(usuario);
        }
    }
}
