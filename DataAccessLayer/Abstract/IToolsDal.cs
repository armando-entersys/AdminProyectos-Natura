using DataAccessLayer.Concrete;
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
    }
}
