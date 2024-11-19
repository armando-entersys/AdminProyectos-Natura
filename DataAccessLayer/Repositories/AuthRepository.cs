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

namespace DataAccessLayer.Repositories
{
    public  class AuthRepository<T> : IAuthDal where T : class
    {
        private readonly DataAccesContext _context;

        public AuthRepository(DataAccesContext context)
        {
            _context = context;
        }
        public async Task<Usuario> Autenticar(string correo, string contrasena)
        {
            Usuario usuario = await _context.Usuarios.Where(q => q.Correo == correo && q.Contrasena == contrasena).FirstOrDefaultAsync();
            if (usuario != null)
            {
                usuario.UserRol = new Rol();
                usuario.UserRol = await _context.Roles.Where(q => q.Id == usuario.RolId).FirstOrDefaultAsync();
                usuario.UserRol.Menus = _context.Menus.Where(q => q.RolId == usuario.RolId).ToList();
            }
            return usuario;
        }
        public async Task<Usuario> Registro(string correo,string Nombre)
        {
            Usuario usuario =  _context.Usuarios.Where(q => q.Correo == correo).FirstOrDefault();
           
            _context.Add(usuario = new Usuario());
            await _context.SaveChangesAsync();
            return usuario;
            
        }
        public IEnumerable<Menu> GetMenusByRole(int rolId)
        {
            return  _context.Menus
                                 .Where(m => m.RolId == rolId)
                                 .OrderBy(m => m.Orden)
                                 .ToList();
        }
        public respuestaServicio SolicitudUsuario(Usuario usuario)
        {
            respuestaServicio resp = new respuestaServicio();
            Usuario usuarioBD = _context.Usuarios.Where(q => q.Correo == usuario.Correo).FirstOrDefault();
            if (usuarioBD == null)
            {
                _context.Add(usuario);
                _context.SaveChanges();
                resp.Exito = true;
                resp.Mensaje = "Solicitud registrada con exito";
                
            }
            else
            {
                resp.Exito = false;
                resp.Mensaje = "Correo registrado anteriormente";
            }
            return resp;
        }
        public respuestaServicio CambioPasswordEmail(string correo)
        {
            respuestaServicio resp = new respuestaServicio();
            Usuario usuarioBD = _context.Usuarios.Where(q => q.Correo == correo).FirstOrDefault();
            if (usuarioBD != null)
            {
                usuarioBD.CambioContrasena = true;
                _context.Update(usuarioBD);
                _context.SaveChanges();
                resp.Datos = usuarioBD;
                resp.Exito = true;
                resp.Mensaje = "Solicitud exitosa";

            }
            else
            {
                resp.Exito = false;
                resp.Mensaje = "Correo no registrado";
            }
            return resp;
        }
        public respuestaServicio CambiarPasswordUsuario(Usuario usuario)
        {
            respuestaServicio resp = new respuestaServicio();
            Usuario usuarioBD = _context.Usuarios.Where(q => q.Id == usuario.Id).FirstOrDefault();
            if (usuarioBD != null)
            {
                usuarioBD.CambioContrasena = false;
                usuarioBD.Contrasena = usuario.Contrasena;
                _context.Update(usuarioBD);
                _context.SaveChanges();
                resp.Datos = usuarioBD;
                resp.Exito = true;
                resp.Mensaje = "Solicitud exitosa";

            }
            else
            {
                resp.Exito = false;
                resp.Mensaje = "Correo no registrado";
            }
            return resp;
        }
        public Usuario ObtenerUsuarioByRol(int RolId)
        {
            var Usuario = _context.Usuarios.Where(q => q.RolId == RolId).FirstOrDefault();

            return Usuario;
        }
    }

}
