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
    public class AudiciaService : IAudienciaService
    {
        private readonly IAudienciaDal _dataDal;
        public AudiciaService(IUsuarioDal usuarioDal)
        {
            _dataDal = _dataDal;
        }

        public void TDelete(int id)
        {
            _dataDal.Delete(id);
        }

        public List<Audiencia> TGetAll()
        {
           return _dataDal.GetAll();
        }

        public Audiencia TGetById(int id)
        {
           return _dataDal.GetById(id);
        }
       

        public void TInsert(Audiencia entity)
        {
            _dataDal.Insert(entity);
            
        }

        public void TUpdate(Audiencia entity)
        {
            _dataDal.Update(entity);
        }

    }
}
