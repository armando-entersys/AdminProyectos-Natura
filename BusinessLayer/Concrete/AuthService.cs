using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Context;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class AuthService : IAuthService
    {

        private readonly IAuthDal _authDal;
        private readonly IEmailSender _emailSender;
        private readonly IToolsService _toolsService;

        public AuthService(IAuthDal authDal, IEmailSender emailSender, IToolsService toolsService)
        {
            _authDal = authDal;
            _emailSender = emailSender;
            _toolsService = toolsService;
        }

        public async Task<Usuario> Autenticar(string correo, string contrasena)
        {
            Usuario usuario = await _authDal.Autenticar(correo, contrasena);
            return usuario;
        }
        public async Task<Usuario> Registro(string correo, string Nombre)
        {
            Usuario usuario = await _authDal.Registro(correo, Nombre);
            return usuario;
        }
        public IEnumerable<Menu> GetMenusByRole(int rolId)
        {
            IEnumerable<Menu> menu = _authDal.GetMenusByRole(rolId);
            return menu;
        }
        public respuestaServicio SolicitudUsuario(Usuario usuario)
        {
            return _authDal.SolicitudUsuario(usuario);
        }

        public respuestaServicio CambioPasswordEmail(string correo)
        {
            var resp = _authDal.CambioPasswordEmail(correo);

            return resp;
        }

        public respuestaServicio CambiarPasswordUsuario(Usuario usuario)
        {
            return _authDal.CambiarPasswordUsuario(usuario); 
        }

        public Usuario ObtenerUsuarioByRol(int RolId)
        {
            return _authDal.ObtenerUsuarioByRol(RolId);
        }
    }
}
