using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class RolService : IRolService
    {
        private readonly IRolDal _rolDal;
        public RolService(IRolDal rolDal)
        {
            _rolDal = rolDal;
        }

        public void TDelete(int id)
        {
            _rolDal.Delete(id);
        }

        public List<Rol> TGetAll()
        {
            return _rolDal.GetAll();
        }

        public Rol TGetById(int id)
        {
            return _rolDal.GetById(id);
        }


        public void TInsert(Rol entity)
        {
            _rolDal.Insert(entity);
        }

        public void TUpdate(Rol entity)
        {
            _rolDal.Update(entity);
        }

    }
}

