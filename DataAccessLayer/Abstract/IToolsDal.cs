using DataAccessLayer.Concrete;
using DataAccessLayer.Migrations;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IToolsDal
    {
        IEnumerable<Usuario> GetUsuarioByRol(int rolId);
        IEnumerable<Usuario> GetUsuarioBySolicitud();
        Usuario CambioSolicitudUsuario(int id, bool estatus);
        IEnumerable<Alerta> ObtenerAlertaUsuario(int id);
        IEnumerable<Usuario> BuscarUsuario(string nombre, int rolId);
        Participante AgregarParticipante(Participante _participante);
        List<Participante> ObtenerParticipantes(int BriefId);
        List<Alerta> ObtenerAlertas(int IdUsuario);
        Alerta CrearAlerta(Alerta alerta);
        List<TipoAlerta> ObtenerTiposAlerta();
        void UpdateAlerta(int Id);
        int GetUnreadAlertsCount(int usuarioId);
        #region Catalogo Audiencia
        void DeleteAudiencia(int id);
        List<Audiencia> GetAllAudiencia();
        Audiencia GetByAudienciaId(int id);
        void InsertAudiencia(Audiencia entity);
        void UpdateAudiencia(Audiencia entity);
        #endregion
        #region Catalogo TipoBrief
        void DeleteTipoBrief(int id);
        List<TipoBrief> GetAllTipoBrief();
        TipoBrief GetByTipoBriefId(int id);
        void InsertTipoBrief(TipoBrief entity);
        void UpdateTipoBrief(TipoBrief entity);
        #endregion
        #region Catalogo TipoAlerta
        void DeleteTipoAlerta(int id);
        List<TipoAlerta> GetAllTipoAlerta();
        TipoAlerta GetByTipoAlertaId(int id);
        void InsertTipoAlerta(TipoAlerta entity);
        void UpdateTipoAlerta(TipoAlerta entity);
        #endregion
        #region Catalogo Prioridad
        void DeletePrioridad(int id);
        List<Prioridad> GetAllPrioridad();
        Prioridad GetByPrioridadId(int id);
        void InsertPrioridad(Prioridad entity);
        void UpdatePrioridad(Prioridad entity);
        #endregion
        #region Catalogo PCN
        void DeletePCN(int id);
        List<PCN> GetAllPCN();
        PCN GetByPCNId(int id);
        void InsertPCN(PCN entity);
        void UpdatePCN(PCN entity);
        #endregion
        #region Catalogo EstatusMaterial
        void DeleteEstatusMaterial(int id);
        List<EstatusMaterial> GetAllEstatusMaterial();
        EstatusMaterial GetByEstatusMaterialId(int id);
        void InsertEstatusMaterial(EstatusMaterial entity);
        void UpdateEstatusMaterial(EstatusMaterial entity);
        #endregion
        #region Catalogo EstatusBrief
        void DeleteEstatusBrief(int id);
        List<EstatusBrief> GetAllEstatusBrief();
        EstatusBrief GetByEstatusBriefId(int id);
        void InsertEstatusBrief(EstatusBrief entity);
        void UpdateEstatusBrief(EstatusBrief entity);
        #endregion
        #region Catalogo Formato
        void DeleteFormato(int id);
        List<Formato> GetAllFormato();
        Formato GetByFormatoId(int id);
        void InsertFormato(Formato entity);
        void UpdateFormato(Formato entity);
        #endregion


    }
}
