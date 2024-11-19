using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IToolsService
    {
        IEnumerable<Usuario> GetUsuarioByRol(int rolId);
        IEnumerable<Usuario> GetUsuarioBySolicitud();
        respuestaServicio CambioSolicitud(int id, bool estatus);
        IEnumerable<Alerta> ObtenerAlertaUsuario(int id);
        IEnumerable<Usuario> BuscarUsuario(string nombre, int rolId);
        Participante AgregarParticipante(Participante _participante);
        List<Participante> ObtenerParticipantes(int BriefId);
        List<Alerta> ObtenerAlertas(int IdUsuario);
        Alerta CrearAlerta(Alerta alerta);
        int ConteoAlertas(int IdUsuario);
        List<TipoAlerta> ObtenerTiposAlerta();
        void UpdateAlerta(int Id);
    }
}
