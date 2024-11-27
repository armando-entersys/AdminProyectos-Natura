using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
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
        public IEnumerable<Brief> GetAllbyUserId(int usuarioId, bool onlybrief)
        {
            return _briefDal.GetAllbyUserId(usuarioId,onlybrief);
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

        public void EliminarMaterial(int id)
        {
            _briefDal.EliminarMaterial(id);
        }

        public ConteoProyectos ObtenerConteoProyectos(int UsuarioId)
        {
            return _briefDal.ObtenerConteoProyectos(UsuarioId);
        }

        public ConteoProyectos ObtenerConteoMateriales(int UsuarioId)
        {
            return _briefDal.ObtenerConteoMateriales(UsuarioId);
        }

        public int ObtenerConteoProyectoFecha(int UsuarioId)
        {
            return _briefDal.ObtenerConteoProyectoFecha(UsuarioId);
        }

        public List<Material> GetMaterialesByUser(int id)
        {
            return _briefDal.GetMaterialesByUser(id);
        }

        public List<Material> GetMaterialesFilter(Material material)
        {
            return _briefDal.GetMaterialesFilter(material);
        }

        public IEnumerable<Audiencia> GetAllAudiencias()
        {
            return _briefDal.GetAllAudiencias();
        }

        public IEnumerable<Formato> GetAllFormatos()
        {
            return _briefDal.GetAllFormatos();
        }

        public IEnumerable<PCN> GetAllPCN()
        {
            return _briefDal.GetAllPCN();
        }

        public IEnumerable<Prioridad> GetAllPrioridades()
        {
            return _briefDal.GetAllPrioridades();
        }

        public void EliminarParticipante(int id)
        {
            _briefDal.EliminarParticipante(id);
        }

        public ConteoMateriales ObtenerConteoEstatusMateriales(int UsuarioId)
        {
            return _briefDal.ObtenerConteoEstatusMateriales(UsuarioId);
        }

        public IEnumerable<EstatusMaterial> GetAllEstatusMateriales()
        {
            return _briefDal.GetAllEstatusMateriales();
        }

        public void ActualizaHistorialMaterial(HistorialMaterial historialMaterial)
        {
            _briefDal.ActualizaHistorialMaterial(historialMaterial);
        }

        public void ActualizaRetrasoMaterial(RetrasoMaterial retrasoMaterial)
        {
            _briefDal.ActualizaRetrasoMaterial(retrasoMaterial);
        }

        public IEnumerable<HistorialMaterial> GetAllHistorialMateriales(int MaterialId)
        {
            return _briefDal.GetAllHistorialMateriales(MaterialId);
        }

        public Material GetMaterial(int id)
        {
            return _briefDal.GetMaterial(id);
        }
    }
}
