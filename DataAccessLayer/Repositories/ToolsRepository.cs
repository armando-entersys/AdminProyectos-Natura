using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Context;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ToolsRepository : IToolsDal
    {
        private readonly DataAccesContext _context;

        public ToolsRepository(DataAccesContext context)
        {
            _context = context;
        }
        public IEnumerable<Usuario> GetUsuarioByRol(int rolId)
        {
            return _context.Usuarios
                                 .Where(m => m.RolId == rolId)
                                 .ToList();
        }
        public IEnumerable<Usuario> GetUsuarioBySolicitud()
        {
            return _context.Usuarios
                                 .Where(m => m.SolicitudRegistro == true && m.Estatus == false)
                                 .ToList();
        }
        public Usuario CambioSolicitudUsuario(int id,bool estatus)
        {
            Usuario usuario = _context.Usuarios.Where(q => q.Id == id).FirstOrDefault();
            usuario.Estatus = estatus;
            usuario.FechaModificacion = DateTime.Now;
            _context.SaveChanges();
            return usuario;
        }
        public IEnumerable<Alerta> ObtenerAlertaUsuario(int id)
        {
           return _context.Alertas.Where(q => q.IdUsuario == id).ToList();
        }
        public IEnumerable<Usuario> BuscarUsuario(string nombre, int rolId) 
        {
            var usuarios = new List<Usuario>();
            if (rolId != 0)
            {
                usuarios = _context.Usuarios.Where(q => q.RolId == rolId && q.Nombre.ToUpper().Contains(nombre)).ToList();

            }
            else
            {
                usuarios = _context.Usuarios.Where(q => q.Nombre.ToUpper().Contains(nombre)).ToList();
            }

            return usuarios;

        }
        public Participante AgregarParticipante(Participante _participante)
        {
            var participante = _context.Participantes.Where(q => q.BriefId == _participante.BriefId && q.UsuarioId == _participante.UsuarioId).FirstOrDefault();

            if (participante == null)
            {
                _context.Set<Participante>().Add(_participante);
                _context.SaveChanges();
            }
           
            return participante;
        }
        public List<Participante> ObtenerParticipantes(int BriefId)
        {
           var participantes =  _context.Participantes.Where(q => q.BriefId == BriefId)
                                .Select(u => new Participante
                                {
                                    Id = u.Id,
                                    UsuarioId = u.UsuarioId,
                                    BriefId = u.BriefId,
                                    Usuario = _context.Usuarios.Where(p => p.Id == u.UsuarioId).FirstOrDefault()
                                }).ToList();

            return participantes;
        }
        public List<Alerta> ObtenerAlertas(int IdUsuario)
        {
            var Alertas = _context.Alertas.Where(q => q.IdUsuario == IdUsuario && q.lectura == false)
                                          .Select(p => new Alerta
                                          {
                                              Id = p.Id,
                                              IdUsuario = p.IdUsuario,
                                              Nombre = p.Nombre,
                                              Descripcion = p.Descripcion,
                                              lectura = p.lectura,
                                              Accion = p.Accion,
                                              FechaCreacion = p.FechaCreacion,
                                              IdTipoAlerta = p.IdTipoAlerta,
                                              TipoAlerta = _context.TipoAlerta.Where(u => u.Id == p.IdTipoAlerta).FirstOrDefault()
                                          }) .ToList();

            return Alertas;
        }
        public int GetUnreadAlertsCount(int usuarioId)
        {
            try
            {
                // Realizar la consulta a la base de datos para contar las alertas no leídas
                var unreadAlertsCount = _context.Alertas
                                                  .Where(a => a.IdUsuario == usuarioId && !a.lectura)
                                                  .Count();
                return unreadAlertsCount;
            }
            catch (Exception ex)
            {
                // Manejar la excepción de la base de datos si ocurre
                throw new Exception("Error al obtener el conteo de alertas no leídas", ex);
            }
        }
        public Alerta CrearAlerta(Alerta alerta)
        {
            if(alerta.IdUsuario == 0)
            {
                alerta.IdUsuario = _context.Usuarios.Where(q => q.RolId == 1).Select(p => p.Id).FirstOrDefault();
            }
            var Alerta = _context.Add(alerta);
            _context.SaveChanges();
            return alerta;
        }
        public List<TipoAlerta> ObtenerTiposAlerta()
        {
            var TiposAlerta = _context.TipoAlerta.ToList();
            
            return TiposAlerta;
        }
        public void UpdateAlerta(int Id)
        {
            var Alerta = _context.Alertas.Where(q => q.Id == Id).FirstOrDefault();
            Alerta.lectura = true;
            _context.SaveChanges();
            
        }
    }
}
