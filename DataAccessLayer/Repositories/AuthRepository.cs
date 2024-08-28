using DataAccessLayer.Abstract;
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

            return usuario;
        }

        public async Task<Usuario> Registro(string correo,string Nombre)
        {
            Usuario usuario = _context.Usuarios.Where(q => q.Correo == correo).FirstOrDefault();
           
            _context.Add(usuario = new Usuario());
            await _context.SaveChangesAsync();
            return usuario;
            
        }
    }

}
