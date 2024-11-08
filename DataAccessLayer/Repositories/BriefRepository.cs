using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Context;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class BriefRepository<T> : IBriefDal where T : class
    {

        private readonly DataAccesContext _context;

        public BriefRepository(DataAccesContext context)
        {
            _context = context;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public List<Brief> GetAll()
        {
            return _context.Set<Brief>().ToList();
        }

        public List<Brief> GetAllByUserId(int id)
        {
           return _context.Briefs.Where(q=> q.UsuarioId == id).ToList();
        }
        public List<Column<Brief>> GetColumnsByUserId(int id)
        {
            List<Column<Brief>> Columns = null;
            List<Brief> Briefs = null;
           Usuario usuario = _context.Usuarios.Where(q => q.Id == id).FirstOrDefault();
            if(usuario == null)
            {
                Briefs = _context.Briefs.Where(q => q.UsuarioId == id).ToList();

            }
            else
            {
                Briefs = _context.Briefs.ToList();
            }
            if (Briefs != null)
            {
                List<EstatusBrief> EstatusBrief = _context.EstatusBriefs.ToList();
                Columns = new List<Column<Brief>>();
                foreach (var item in EstatusBrief)
                {
                    Columns.Add(new Column<Brief>
                    {
                        Id = item.Id,
                        Name = item.Descripcion,
                        Tasks = Briefs.Where(q => q.EstatusBriefId == item.Id)
                                .Select(i => new Tasks<Brief>
                                {
                                    Id = i.Id,
                                    Title = i.Nombre,
                                    UsuarioId = i.UsuarioId,
                                    NombreUsuario = _context.Usuarios.Where(p=> p.Id == i.UsuarioId).Select(u=>u.Nombre).FirstOrDefault(),
                                    FechaEntrega = i.FechaEntrega.ToShortDateString()
                                }).ToList()
                    });
                }
            }
            return Columns;


        }

        public Brief GetById(int id)
        {
            Brief brief = _context.Set<Brief>().Find(id);
            if(brief == null) {
                brief.Usuario = _context.Usuarios.Where(q=> q.Id == brief.UsuarioId).FirstOrDefault();
            }
            return brief;
        }
        public IEnumerable<Brief> GetAllbyUserId(int usuarioId)
        {
            var usuario = _context.Usuarios.Where(u => u.Id == usuarioId && u.RolId == 1).FirstOrDefault();
            IEnumerable<Brief> briefs = _context.Briefs.Where(q => q.UsuarioId == usuarioId).ToList();
            if (usuario != null)
            {
                briefs = _context.Briefs.ToList();
            }


            return briefs;
        }
        public void Insert(Brief entity)
        {
            _context.Set<Brief>().Add(entity);
            _context.SaveChanges();
        }

        public void Update(Brief entity)
        {
            var existingEntity = _context.Set<Brief>().Find(entity.Id);  // Carga la entidad original
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);  // Solo copia los valores modificados
                _context.SaveChanges();
            }
        }
        public IEnumerable<EstatusBrief> GetAllEstatusBrief()
        {
            return _context.EstatusBriefs.ToList();
        }
        public IEnumerable<TipoBrief> GetAllTipoBrief()
        {
            return _context.TiposBrief.ToList();
        }
        public IEnumerable<ClasificacionProyecto> GetAllClasificacionProyecto()
        {
            return _context.clasificacionProyectos.Where(q => q.Estatus == true).ToList();
        }
        public void InsertProyecto(Proyecto entity)
        {
            _context.Set<Proyecto>().Add(entity);
            _context.SaveChanges();
        }
        public void InsertMaterial(Material entity)
        {
            try
            {
                _context.Set<Material>().Add(entity);
                _context.SaveChanges();
            }
            catch(Exception ex)
            {

            }
            
        }
        public Proyecto GetProyectoByBriefId(int id)
        {
            return _context.Proyectos.Where(q => q.BriefId == id).FirstOrDefault();
        }
        public List<Material> GetMaterialesByBriefId(int id)
        {
            return _context.Materiales.Where(q => q.BriefId == id).Select(p => new Material
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Mensaje = p.Mensaje,
                Prioridad = _context.Prioridad.Where(u => u.Id == p.Id).FirstOrDefault(),
                Ciclo = p.Ciclo,
                PCN = _context.PCN.Where(u => u.Id == p.Id).FirstOrDefault(),
                Audiencia = _context.Audiencia.Where(u => u.Id == p.Id).FirstOrDefault(),
                Formato = _context.Formato.Where(u => u.Id == p.Id).FirstOrDefault(),
                FechaEntrega = p.FechaEntrega,
                Proceso = p.Proceso,
                Produccion = p.Produccion,
                Responsable = p.Responsable,
                FechaRegistro = p.FechaRegistro,
                FechaModificacion = p.FechaModificacion,
                Brief = _context.Briefs.Where(q => q.Id == p.BriefId).FirstOrDefault(),
                EstatusMaterial = _context.EstatusMateriales.Where(u => u.Id == p.EstatusMaterialId).FirstOrDefault()
            }).ToList();
        }
        public void EliminarMaterial(int id)
        {
           var material = _context.Materiales.Where(q => q.Id == id).FirstOrDefault();
            if (material != null) {

                _context.Remove(material);
                _context.SaveChanges();
            }
                
        }
        public void EliminarParticipante(int id)
        {
            var participante = _context.Participantes.Where(q => q.Id == id).FirstOrDefault();
            if (participante != null)
            {

                _context.Remove(participante);
                _context.SaveChanges();
            }

        }
        public ConteoProyectos ObtenerConteoProyectos(int UsuarioId)
        {
            var conteoProyectos = new ConteoProyectos();
            var isAdmin = _context.Usuarios.Where(q => q.Id == UsuarioId && q.RolId == 1).FirstOrDefault();
            var briefs = _context.Briefs.Where(q => q.UsuarioId == UsuarioId).ToList();
            if (isAdmin !=  null )
            {
                briefs = _context.Briefs.ToList();
            }
            
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfNextWeek = startOfWeek.AddDays(7);
            var endOfNextWeek = startOfNextWeek.AddDays(7).AddDays(-1);

            // Alertas de hoy
            conteoProyectos.Hoy = briefs
                .Where(q => q.EstatusBriefId == 1
                            && q.FechaPublicacion.Date == today)
                .Count();

            // Alertas de esta semana
            conteoProyectos.EstaSemana = briefs
                .Where(q => q.EstatusBriefId == 1
                            && q.FechaPublicacion.Date >= startOfWeek
                            && q.FechaPublicacion.Date <= today)
                .Count();

            // Alertas de la próxima semana
            conteoProyectos.ProximaSemana = briefs
                .Where(q =>  q.EstatusBriefId == 1
                            && q.FechaPublicacion.Date >= startOfNextWeek
                            && q.FechaPublicacion.Date <= endOfNextWeek)
                .Count();

            // Total de proyectos con IdTipoAlerta = 1
            conteoProyectos.TotalProyectos = briefs
                .Count();

            return conteoProyectos;
        }
        public ConteoProyectos ObtenerConteoMateriales(int UsuarioId)
        {
            var conteoProyectos = new ConteoProyectos();
            var isAdmin = _context.Usuarios.Where(q => q.Id == UsuarioId && q.RolId == 1).FirstOrDefault();
            var idsbrief = _context.Briefs.Where(q => q.UsuarioId == UsuarioId).Select(p => p.Id).ToList();
            var materiales = _context.Materiales.Where(q => idsbrief.Contains(q.BriefId)).ToList();

            if (isAdmin !=  null)
            {
                materiales = _context.Materiales.Where(q => q.EstatusMaterialId == 1).ToList();
            }

            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfNextWeek = startOfWeek.AddDays(7);
            var endOfNextWeek = startOfNextWeek.AddDays(7).AddDays(-1);

            // Alertas de hoy
            conteoProyectos.Hoy = materiales
                .Where(q => q.EstatusMaterialId == 1
                            && q.FechaRegistro.Date == today)
                .Count();

            // Alertas de esta semana
            conteoProyectos.EstaSemana = materiales
                .Where(q => q.EstatusMaterialId == 1
                            && q.FechaRegistro.Date >= startOfWeek
                            && q.FechaRegistro.Date <= today)
                .Count();

            // Alertas de la próxima semana
            conteoProyectos.ProximaSemana = materiales
                .Where(q => q.EstatusMaterialId == 1
                            && q.FechaRegistro.Date >= startOfNextWeek
                            && q.FechaRegistro.Date <= endOfNextWeek)
                .Count();

            // Total de proyectos con IdTipoAlerta = 1
            conteoProyectos.TotalProyectos = materiales
                .Count();

            return conteoProyectos;
        }
        public int ObtenerConteoProyectoFecha(int UsuarioId)
        {
            var isAdmin = _context.Usuarios.Where(q => q.Id == UsuarioId && q.RolId == 1).FirstOrDefault();
            if(isAdmin != null)
            {
                return _context.Briefs.Where(q => q.FechaEntrega >= q.FechaPublicacion).Count();
            }
            else
            {
                return 0;
            }
        }
        public List<Material> GetMaterialesByUser(int id)
        {
            var usuarioAdmin = _context.Usuarios.Where(q => q.Id == id && q.RolId == 1).FirstOrDefault();
            var materiales = _context.Materiales.Where(q => q.Brief.UsuarioId == id)
                             .Select(p => new Material
                             {
                                 Id = p.Id,
                                 Nombre = p.Nombre,
                                 Mensaje = p.Mensaje,
                                 Prioridad = _context.Prioridad.Where(u => u.Id == p.PrioridadId).FirstOrDefault(),
                                 Ciclo = p.Ciclo,
                                 PCN = _context.PCN.Where(u => u.Id == p.PCNId).FirstOrDefault(),
                                 Audiencia = _context.Audiencia.Where(u => u.Id == p.AudienciaId).FirstOrDefault(),
                                 Formato = _context.Formato.Where(u => u.Id == p.FormatoId).FirstOrDefault(),
                                 FechaEntrega = p.FechaEntrega,
                                 Proceso = p.Proceso,
                                 Produccion = p.Produccion,
                                 Responsable = p.Responsable,
                                 FechaRegistro = p.FechaRegistro,
                                 FechaModificacion = p.FechaModificacion,
                                 Brief = _context.Briefs.Where(q => q.Id == p.BriefId).FirstOrDefault(),
                                 EstatusMaterial = _context.EstatusMateriales.Where(u => u.Id == p.EstatusMaterialId).FirstOrDefault()
                             }).ToList();

            if (usuarioAdmin != null)
            {
                materiales = _context.Materiales.Select(p => new Material
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Mensaje = p.Mensaje,
                    Prioridad = _context.Prioridad.Where(u => u.Id == p.PrioridadId).FirstOrDefault(),
                    Ciclo = p.Ciclo,
                    PCN = _context.PCN.Where(u => u.Id == p.PCNId).FirstOrDefault(),
                    Audiencia = _context.Audiencia.Where(u => u.Id == p.AudienciaId).FirstOrDefault(),
                    Formato = _context.Formato.Where(u => u.Id == p.FormatoId).FirstOrDefault(),
                    FechaEntrega = p.FechaEntrega,
                    Proceso = p.Proceso,
                    Produccion = p.Produccion,
                    Responsable = p.Responsable,
                    FechaRegistro = p.FechaRegistro,
                    FechaModificacion = p.FechaModificacion,
                    Brief = _context.Briefs.Where(q => q.Id == p.BriefId).FirstOrDefault(),
                    EstatusMaterial = _context.EstatusMateriales.Where(u => u.Id == p.EstatusMaterialId).FirstOrDefault()
                }).ToList();
            }
            return materiales;
        }
        public List<Material> GetMaterialesFilter(Material material)
        {
            var usuarioAdmin = _context.Usuarios.Where(q => q.Id == material.Id && q.RolId == 1).FirstOrDefault();
            var materiales = _context.Materiales.Where(q => q.Brief.UsuarioId == material.Id && q.Nombre == material.Nombre && q.FechaRegistro == material.FechaRegistro)
                             .Select(p => new Material
                             {
                                 Id = p.Id,
                                 Nombre = p.Nombre,
                                 Mensaje = p.Mensaje,
                                 Prioridad = _context.Prioridad.Where(u => u.Id == p.PrioridadId).FirstOrDefault(),
                                 Ciclo = p.Ciclo,
                                 PCN = _context.PCN.Where(u => u.Id == p.PCNId).FirstOrDefault(),
                                 Audiencia = _context.Audiencia.Where(u => u.Id == p.AudienciaId).FirstOrDefault(),
                                 Formato = _context.Formato.Where(u => u.Id == p.FormatoId).FirstOrDefault(),
                                 FechaEntrega = p.FechaEntrega,
                                 Proceso = p.Proceso,
                                 Produccion = p.Produccion,
                                 Responsable = p.Responsable,
                                 FechaRegistro = p.FechaRegistro,
                                 FechaModificacion = p.FechaModificacion,
                                 Brief = _context.Briefs.Where(q => q.Id == p.BriefId).FirstOrDefault(),
                                 EstatusMaterial = _context.EstatusMateriales.Where(u => u.Id == p.EstatusMaterialId).FirstOrDefault()
                             }).ToList();

            if (usuarioAdmin != null)
            {
                materiales = _context.Materiales.Select(p => new Material
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Mensaje = p.Mensaje,
                    Prioridad = _context.Prioridad.Where(u => u.Id == p.PrioridadId).FirstOrDefault(),
                    Ciclo = p.Ciclo,
                    PCN = _context.PCN.Where(u => u.Id == p.PCNId).FirstOrDefault(),
                    Audiencia = _context.Audiencia.Where(u => u.Id == p.AudienciaId).FirstOrDefault(),
                    Formato = _context.Formato.Where(u => u.Id == p.FormatoId).FirstOrDefault(),
                    FechaEntrega = p.FechaEntrega,
                    Proceso = p.Proceso,
                    Produccion = p.Produccion,
                    Responsable = p.Responsable,
                    FechaRegistro = p.FechaRegistro,
                    FechaModificacion = p.FechaModificacion,
                    Brief = _context.Briefs.Where(q => q.Id == p.BriefId).FirstOrDefault(),
                    EstatusMaterial = _context.EstatusMateriales.Where(u => u.Id == p.EstatusMaterialId).FirstOrDefault()
                }).ToList();
            }
            return materiales;
        }
        public IEnumerable<Audiencia> GetAllAudiencias()
        {
            return _context.Audiencia.ToList();
        }
        public IEnumerable<Formato> GetAllFormatos()
        {
            return _context.Formato.ToList();
        }
        public IEnumerable<PCN> GetAllPCN()
        {
            return _context.PCN.ToList();
        }
        public IEnumerable<Prioridad> GetAllPrioridades()
        {
            var prioridades =new List<Prioridad>();
            try
            {
                prioridades = _context.Prioridad.ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
            return prioridades;

        }
        public ConteoMateriales ObtenerConteoEstatusMateriales(int UsuarioId)
        {
            var usuarioAdmin = _context.Usuarios.Where(q => q.Id == UsuarioId).FirstOrDefault();
            var materiales = _context.Materiales.Where(q => q.Brief.UsuarioId == UsuarioId).ToList();
            ConteoMateriales conteoMateriales = new ConteoMateriales();
            if (usuarioAdmin == null)
            {
                materiales = _context.Materiales.ToList();
            }

            conteoMateriales.Registros = materiales.Where(q => q.EstatusMaterialId == 1).Count();
            conteoMateriales.Revision = materiales.Where(q => q.EstatusMaterialId == 2).Count();
            conteoMateriales.Produccion = materiales.Where(q => q.EstatusMaterialId == 3).Count();
            conteoMateriales.FaltaInfo = materiales.Where(q => q.EstatusMaterialId == 4).Count();
            conteoMateriales.Programado = materiales.Where(q => q.EstatusMaterialId == 5).Count();
            conteoMateriales.Entregado = materiales.Where(q => q.EstatusMaterialId == 6).Count();
            conteoMateriales.InicioCiclo = materiales.Where(q => q.EstatusMaterialId == 7).Count();
            conteoMateriales.NoCompartio = materiales.Where(q => q.EstatusMaterialId == 8).Count();

            return conteoMateriales;
        }
        public IEnumerable<EstatusMaterial> GetAllEstatusMateriales()
        {
            return _context.EstatusMateriales.ToList();
        }
        public void ActualizaHistorialMaterial(HistorialMaterial historialMaterial)
        {
            _context.Add(historialMaterial);
            _context.SaveChanges();
        }
        public void ActualizaRetrasoMaterial(RetrasoMaterial retrasoMaterial)
        {
            _context.Add(retrasoMaterial);
            _context.SaveChanges();
        }
    }
}
