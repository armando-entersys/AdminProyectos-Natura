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
    public class ToolsService : IToolsService
    {
        private readonly IToolsDal _toolsDal;

        public ToolsService(IToolsDal toolsDal)
        {
            _toolsDal = toolsDal;
        }
        public IEnumerable<Usuario> GetUsuarioByRol(int rolId)
        {
            IEnumerable<Usuario> usuarios = _toolsDal.GetUsuarioByRol(rolId);
            return usuarios;
        }
        public IEnumerable<Usuario> GetUsuarioBySolicitud()
        {
            IEnumerable<Usuario> usuarios = _toolsDal.GetUsuarioBySolicitud();
            return usuarios;
        }
        public respuestaServicio CambioSolicitud(int id, bool estatus)
        {
            respuestaServicio resp = new respuestaServicio();
            try
            {
                Usuario usuario = _toolsDal.CambioSolicitudUsuario(id, estatus);
                resp.Exito = true;
                resp.Mensaje = "Solicitud Exitosa";
            }
            catch   (Exception ex) {
                resp.Exito = false;
                resp.Mensaje = "Solicitud Fallida";
            }
            

            return resp;
        }

        public IEnumerable<Alerta> ObtenerAlertaUsuario(int id)
        {
            return _toolsDal.ObtenerAlertaUsuario(id);
        }
        public IEnumerable<Usuario> BuscarUsuario(string nombre, int rolId)
        {
            return _toolsDal.BuscarUsuario(nombre,rolId);
        }

        public Participante AgregarParticipante(Participante _participante)
        {
            return _toolsDal.AgregarParticipante(_participante);
        }

        public List<Participante> ObtenerParticipantes(int BriefId)
        {
            return _toolsDal.ObtenerParticipantes(BriefId);
        }

        public List<Alerta> ObtenerAlertas(int IdUsuario)
        {
            return _toolsDal.ObtenerAlertas(IdUsuario);
        }

        public Alerta CrearAlerta(Alerta alerta)
        {
            return _toolsDal.CrearAlerta(alerta);
        }

        public int ConteoAlertas(int IdUsuario)
        {
           var contAlertas = ObtenerAlertas(IdUsuario).Count();
            return contAlertas;
        }

        public List<TipoAlerta> ObtenerTiposAlerta()
        {
            return _toolsDal.ObtenerTiposAlerta();
        }


        public void UpdateAlerta(int Id)
        {
             _toolsDal.UpdateAlerta(Id);
        }

        public int GetUnreadAlertsCount(int usuarioId)
        {
            return _toolsDal.GetUnreadAlertsCount(usuarioId);
        }

        #region Catalogo Audiencia
        public void DeleteAudiencia(int id)
        {
            _toolsDal.DeleteAudiencia(id);
        }

        public List<Audiencia> GetAllAudiencia()
        {
            return _toolsDal.GetAllAudiencia();
        }

        public Audiencia GetByAudienciaId(int id)
        {
            return _toolsDal.GetByAudienciaId((int)id);
        }

        public void InsertAudiencia(Audiencia entity)
        {
            _toolsDal.InsertAudiencia(entity);
        }

        public void UpdateAudiencia(Audiencia entity)
        {
            _toolsDal.UpdateAudiencia(entity);
        }
        #endregion
        #region Catalogo TipoBrief
        public void DeleteTipoBrief(int id)
        {
            _toolsDal.DeleteTipoBrief(id);
        }

        public List<TipoBrief> GetAllTipoBrief()
        {
            return _toolsDal.GetAllTipoBrief();
        }

        public TipoBrief GetByTipoBriefId(int id)
        {
            return _toolsDal.GetByTipoBriefId((int)id);
        }

        public void InsertTipoBrief(TipoBrief entity)
        {
            _toolsDal.InsertTipoBrief(entity);
        }

        public void UpdateTipoBrief(TipoBrief entity)
        {
            _toolsDal.UpdateTipoBrief(entity);
        }
        #endregion
        #region Catalogo TipoAlerta
        public void DeleteTipoAlerta(int id)
        {
            _toolsDal.DeleteTipoAlerta(id);
        }

        public List<TipoAlerta> GetAllTipoAlerta()
        {
            return _toolsDal.GetAllTipoAlerta();
        }

        public TipoAlerta GetByTipoAlertaId(int id)
        {
            return _toolsDal.GetByTipoAlertaId((int)id);
        }

        public void InsertTipoAlerta(TipoAlerta entity)
        {
            _toolsDal.InsertTipoAlerta(entity);
        }

        public void UpdateTipoAlerta(TipoAlerta entity)
        {
            _toolsDal.UpdateTipoAlerta(entity);
        }
        #endregion
        #region Catalogo Prioridad
        public void DeletePrioridad(int id)
        {
            _toolsDal.DeletePrioridad(id);
        }

        public List<Prioridad> GetAllPrioridad()
        {
            return _toolsDal.GetAllPrioridad();
        }

        public Prioridad GetByPrioridadId(int id)
        {
            return _toolsDal.GetByPrioridadId((int)id);
        }

        public void InsertPrioridad(Prioridad entity)
        {
            _toolsDal.InsertPrioridad(entity);
        }

        public void UpdatePrioridad(Prioridad entity)
        {
            _toolsDal.UpdatePrioridad(entity);
        }
        #endregion
        #region Catalogo PCN
        public void DeletePCN(int id)
        {
            _toolsDal.DeletePCN(id);
        }

        public List<PCN> GetAllPCN()
        {
            return _toolsDal.GetAllPCN();
        }

        public PCN GetByPCNId(int id)
        {
            return _toolsDal.GetByPCNId((int)id);
        }

        public void InsertPCN(PCN entity)
        {
            _toolsDal.InsertPCN(entity);
        }

        public void UpdatePCN(PCN entity)
        {
            _toolsDal.UpdatePCN(entity);
        }
        #endregion
        #region Catalogo EstatusMaterial
        public void DeleteEstatusMaterial(int id)
        {
            _toolsDal.DeleteEstatusMaterial(id);
        }

        public List<EstatusMaterial> GetAllEstatusMaterial()
        {
            return _toolsDal.GetAllEstatusMaterial();
        }

        public EstatusMaterial GetByEstatusMaterialId(int id)
        {
            return _toolsDal.GetByEstatusMaterialId((int)id);
        }

        public void InsertEstatusMaterial(EstatusMaterial entity)
        {
            _toolsDal.InsertEstatusMaterial(entity);
        }

        public void UpdateEstatusMaterial(EstatusMaterial entity)
        {
            _toolsDal.UpdateEstatusMaterial(entity);
        }
        #endregion
        #region Catalogo EstatusBrief
        public void DeleteEstatusBrief(int id)
        {
            _toolsDal.DeleteEstatusBrief(id);
        }

        public List<EstatusBrief> GetAllEstatusBrief()
        {
            return _toolsDal.GetAllEstatusBrief();
        }

        public EstatusBrief GetByEstatusBriefId(int id)
        {
            return _toolsDal.GetByEstatusBriefId((int)id);
        }

        public void InsertEstatusBrief(EstatusBrief entity)
        {
            _toolsDal.InsertEstatusBrief(entity);
        }

        public void UpdateEstatusBrief(EstatusBrief entity)
        {
            _toolsDal.UpdateEstatusBrief(entity);
        }
        #endregion
        #region Catalogo Formato
        public void DeleteFormato(int id)
        {
            _toolsDal.DeleteFormato(id);
        }

        public List<Formato> GetAllFormato()
        {
            return _toolsDal.GetAllFormato();
        }

        public Formato GetByFormatoId(int id)
        {
            return _toolsDal.GetByFormatoId((int)id);
        }

        public void InsertFormato(Formato entity)
        {
            _toolsDal.InsertFormato(entity);
        }

        public void UpdateFormato(Formato entity)
        {
            _toolsDal.UpdateFormato(entity);
        }
        #endregion

    }
}
