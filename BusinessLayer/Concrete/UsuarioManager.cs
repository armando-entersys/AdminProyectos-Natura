using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class UsuarioManager : IUsuarioService
    {
        private readonly IUsuarioDal _usuarioDal;
        public UsuarioManager(IUsuarioDal usuarioDal)
        {
            _usuarioDal = usuarioDal;
        }

        public void TDelete(int id)
        {
            _usuarioDal.Delete(id);
        }

        public List<Usuario> TGetAll()
        {
           return  _usuarioDal.GetAll();
        }

        public Usuario TGetById(int id)
        {
           return _usuarioDal.GetById(id);
        }
       

        public void TInsert(Usuario entity)
        {
            entity.FechaRegistro = DateTime.Now;
            entity.FechaModificacion = DateTime.Now;
            entity.CambioContrasena = false;
            _usuarioDal.Insert(entity);
            
        }

        public void TUpdate(Usuario entity)
        {
            entity.FechaModificacion = DateTime.Now;
            _usuarioDal.Update(entity);
        }

    }
}
