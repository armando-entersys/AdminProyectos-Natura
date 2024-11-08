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
        public IEnumerable<Usuario> BuscarUsuario(string nombre)
        {
            return _toolsDal.BuscarUsuario(nombre);
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
    }
}
