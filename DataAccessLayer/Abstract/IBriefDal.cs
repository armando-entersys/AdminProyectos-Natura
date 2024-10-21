using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IBriefDal
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
    }
}
