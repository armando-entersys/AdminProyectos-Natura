using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IBriefService
    {
        void Delete(int id);
        List<Brief> GetAll();
        List<Column<Brief>> GetColumnsByUserId(int id);
        IEnumerable<Brief> GetAllbyUserId(int usuarioId);
        Brief GetById(int id);
        void Insert(Brief entity);
        void Update(Brief entity);
        IEnumerable<EstatusBrief> GetAllEstatusBrief();
        IEnumerable<TipoBrief> GetAllTipoBrief();
        IEnumerable<ClasificacionProyecto> GetAllClasificacionProyecto();
        void InsertProyecto(Proyecto entity);
        void InsertMaterial(Material entity);
        Proyecto GetProyectoByBriefId(int id);
        List<Material> GetMaterialesByBriefId(int id);
        void EliminarMaterial(int id);
        void EliminarParticipante(int id);
        ConteoProyectos ObtenerConteoProyectos(int UsuarioId);
        ConteoProyectos ObtenerConteoMateriales(int UsuarioId);
        int ObtenerConteoProyectoFecha(int UsuarioId);
        List<Material> GetMaterialesByUser(int id);
        List<Material> GetMaterialesFilter(Material material);
        public IEnumerable<Audiencia> GetAllAudiencias();
        public IEnumerable<Formato> GetAllFormatos();
        public IEnumerable<PCN> GetAllPCN();
        public IEnumerable<Prioridad> GetAllPrioridades();
        ConteoMateriales ObtenerConteoEstatusMateriales(int UsuarioId);
        IEnumerable<EstatusMaterial> GetAllEstatusMateriales();
        void ActualizaHistorialMaterial(HistorialMaterial historialMaterial);
        void ActualizaRetrasoMaterial(RetrasoMaterial retrasoMaterial);
    }
}
