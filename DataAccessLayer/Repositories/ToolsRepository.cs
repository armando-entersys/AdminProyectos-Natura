using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Context;
using DataAccessLayer.EntityFramework;
using DataAccessLayer.Migrations;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
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
            usuarios = usuarios.Select(q => new Usuario
            {
                Id = q.Id,
                Nombre = q.Nombre,
                ApellidoPaterno = q.ApellidoPaterno,
                ApellidoMaterno = q.ApellidoMaterno,
                Correo = q.Correo,
                Contrasena = q.Contrasena,
                RolId = q.RolId,
                UserRol = _context.Roles.Where(p => p.Id == q.RolId).FirstOrDefault(),
                Estatus = q.Estatus,
                FechaRegistro = q.FechaRegistro,
                FechaModificacion = q.FechaModificacion,
                CambioContrasena = q.CambioContrasena,
                SolicitudRegistro = q.SolicitudRegistro

            }).ToList();
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

        #region Catalogo Audiencia
        public void DeleteAudiencia(int id)
        {
            var value = _context.Set<Audiencia>().Find(id);
            _context.Set<Audiencia>().Remove(value);
            _context.SaveChanges();
        }

        public List<Audiencia> GetAllAudiencia()
        {
            return _context.Set<Audiencia>().ToList();
        }

        public Audiencia GetByAudienciaId(int id)
        {
            return _context.Set<Audiencia>().Find(id);
        }

        public void InsertAudiencia(Audiencia entity)
        {
            _context.Set<Audiencia>().Add(entity);
            _context.SaveChanges();
        }

        public void UpdateAudiencia(Audiencia entity)
        {
            _context.Set<Audiencia>().Update(entity);
            _context.SaveChanges();
        }
        #endregion
        #region Catalogo TipoBrief
        public void DeleteTipoBrief(int id)
        {
            var value = _context.Set<TipoBrief>().Find(id);
            _context.Set<TipoBrief>().Remove(value);
            _context.SaveChanges();
        }

        public List<TipoBrief> GetAllTipoBrief()
        {
            return _context.Set<TipoBrief>().ToList();
        }

        public TipoBrief GetByTipoBriefId(int id)
        {
            return _context.Set<TipoBrief>().Find(id);
        }

        public void InsertTipoBrief(TipoBrief entity)
        {
            _context.Set<TipoBrief>().Add(entity);
            _context.SaveChanges();
        }

        public void UpdateTipoBrief(TipoBrief entity)
        {
            _context.Set<TipoBrief>().Update(entity);
            _context.SaveChanges();
        }
        #endregion
        #region Catalogo TipoAlerta
        public void DeleteTipoAlerta(int id)
        {
            var value = _context.Set<TipoAlerta>().Find(id);
            _context.Set<TipoAlerta>().Remove(value);
            _context.SaveChanges();
        }

        public List<TipoAlerta> GetAllTipoAlerta()
        {
            return _context.Set<TipoAlerta>().ToList();
        }

        public TipoAlerta GetByTipoAlertaId(int id)
        {
            return _context.Set<TipoAlerta>().Find(id);
        }

        public void InsertTipoAlerta(TipoAlerta entity)
        {
            _context.Set<TipoAlerta>().Add(entity);
            _context.SaveChanges();
        }

        public void UpdateTipoAlerta(TipoAlerta entity)
        {
            _context.Set<TipoAlerta>().Update(entity);
            _context.SaveChanges();
        }
        #endregion
        #region Catalogo Prioridad
        public void DeletePrioridad(int id)
        {
            var value = _context.Set<Prioridad>().Find(id);
            _context.Set<Prioridad>().Remove(value);
            _context.SaveChanges();
        }

        public List<Prioridad> GetAllPrioridad()
        {
            return _context.Set<Prioridad>().ToList();
        }

        public Prioridad GetByPrioridadId(int id)
        {
            return _context.Set<Prioridad>().Find(id);
        }

        public void InsertPrioridad(Prioridad entity)
        {
            _context.Set<Prioridad>().Add(entity);
            _context.SaveChanges();
        }

        public void UpdatePrioridad(Prioridad entity)
        {
            _context.Set<Prioridad>().Update(entity);
            _context.SaveChanges();
        }
        #endregion
        #region Catalogo PCN
        public void DeletePCN(int id)
        {
            var value = _context.Set<PCN>().Find(id);
            _context.Set<PCN>().Remove(value);
            _context.SaveChanges();
        }

        public List<PCN> GetAllPCN()
        {
            return _context.Set<PCN>().ToList();
        }

        public PCN GetByPCNId(int id)
        {
            return _context.Set<PCN>().Find(id);
        }

        public void InsertPCN(PCN entity)
        {
            _context.Set<PCN>().Add(entity);
            _context.SaveChanges();
        }

        public void UpdatePCN(PCN entity)
        {
            _context.Set<PCN>().Update(entity);
            _context.SaveChanges();
        }
        #endregion
        #region Catalogo EstatusMaterial
        public void DeleteEstatusMaterial(int id)
        {
            var value = _context.Set<EstatusMaterial>().Find(id);
            _context.Set<EstatusMaterial>().Remove(value);
            _context.SaveChanges();
        }

        public List<EstatusMaterial> GetAllEstatusMaterial()
        {
            return _context.Set<EstatusMaterial>().ToList();
        }

        public EstatusMaterial GetByEstatusMaterialId(int id)
        {
            return _context.Set<EstatusMaterial>().Find(id);
        }

        public void InsertEstatusMaterial(EstatusMaterial entity)
        {
            _context.Set<EstatusMaterial>().Add(entity);
            _context.SaveChanges();
        }

        public void UpdateEstatusMaterial(EstatusMaterial entity)
        {
            _context.Set<EstatusMaterial>().Update(entity);
            _context.SaveChanges();
        }
        #endregion
        #region Catalogo EstatusBrief
        public void DeleteEstatusBrief(int id)
        {
            var value = _context.Set<EstatusBrief>().Find(id);
            _context.Set<EstatusBrief>().Remove(value);
            _context.SaveChanges();
        }

        public List<EstatusBrief> GetAllEstatusBrief()
        {
            return _context.Set<EstatusBrief>().ToList();
        }

        public EstatusBrief GetByEstatusBriefId(int id)
        {
            return _context.Set<EstatusBrief>().Find(id);
        }

        public void InsertEstatusBrief(EstatusBrief entity)
        {
            _context.Set<EstatusBrief>().Add(entity);
            _context.SaveChanges();
        }

        public void UpdateEstatusBrief(EstatusBrief entity)
        {
            _context.Set<EstatusBrief>().Update(entity);
            _context.SaveChanges();
        }
        #endregion
        #region Catalogo Formato
        public void DeleteFormato(int id)
        {
            var value = _context.Set<Formato>().Find(id);
            _context.Set<Formato>().Remove(value);
            _context.SaveChanges();
        }

        public List<Formato> GetAllFormato()
        {
            return _context.Set<Formato>().ToList();
        }

        public Formato GetByFormatoId(int id)
        {
            return _context.Set<Formato>().Find(id);
        }

        public void InsertFormato(Formato entity)
        {
            _context.Set<Formato>().Add(entity);
            _context.SaveChanges();
        }

        public void UpdateFormato(Formato entity)
        {
            _context.Set<Formato>().Update(entity);
            _context.SaveChanges();
        }
        #endregion

    }
}
