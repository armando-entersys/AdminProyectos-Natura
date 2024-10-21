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
    public class BriefService : IBriefService
    {
        private readonly IBriefDal _briefDal;
        public BriefService(IBriefDal briefDal)
        {
            _briefDal = briefDal;
        }

        public void Delete(int id)
        {
            _briefDal.Delete(id);
        }
        public List<Column<Brief>> GetColumnsByUserId(int id)
        {
            return _briefDal.GetColumnsByUserId(id);
        }
        public List<Brief> GetAll()
        {
            return _briefDal.GetAll();
        }
        public IEnumerable<Brief> GetAllbyUserId(int usuarioId)
        {
            return _briefDal.GetAllbyUserId(usuarioId);
        }
        public Brief GetById(int id)
        {
            return _briefDal.GetById(id);
        }


        public void Insert(Brief entity)
        {
            _briefDal.Insert(entity);
        }

        public void Update(Brief entity)
        {
            _briefDal.Update(entity);
        }
        public IEnumerable<EstatusBrief> GetAllEstatusBrief()
        {
            return _briefDal.GetAllEstatusBrief();
        }
        public IEnumerable<TipoBrief> GetAllTipoBrief()
        {
            return _briefDal.GetAllTipoBrief();
        }
        public void InsertProyecto(Proyecto entity)
        {
            _briefDal.InsertProyecto(entity);
        }
        public void InsertMaterial(Material entity)
        {
            _briefDal.InsertMaterial(entity);
            
        }

        public Proyecto GetProyectoByBriefId(int id)
        {
            return _briefDal.GetProyectoByBriefId(id);
        }

        public List<Material> GetMaterialesByBriefId(int id)
        {
            return _briefDal.GetMaterialesByBriefId(id);
        }

        public IEnumerable<ClasificacionProyecto> GetAllClasificacionProyecto()
        {
            return _briefDal.GetAllClasificacionProyecto();
        }
    }
}
