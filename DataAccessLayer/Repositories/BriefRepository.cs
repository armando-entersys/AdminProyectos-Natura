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
            return _context.Set<Brief>().Where(q => q.FechaEntrega >= DateTime.Now.AddDays(-15)).ToList();
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
                Briefs = _context.Briefs.Where(q => q.UsuarioId == id).OrderByDescending(u=> u.FechaEntrega).ToList();

            }
            else
            {
                Briefs = _context.Briefs.OrderByDescending(u => u.FechaEntrega).ToList();
            }
            if (Briefs != null)
            {
                List<EstatusBrief> EstatusBrief = _context.EstatusBriefs.Where(q => q.Activo == true).ToList();
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
                                })
                                .OrderByDescending(u => u.FechaEntrega)
                                .ToList()
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
        public IEnumerable<Brief> GetAllbyUserId(int usuarioId,bool onlybrief)
        {
            IEnumerable<Brief> briefs = _context.Briefs.Where(q => q.UsuarioId == usuarioId).ToList();
            if (onlybrief)
            {
                var usuarioAdmin = _context.Usuarios.Where(u => u.Id == usuarioId && (u.RolId == 1 || u.RolId == 3)).FirstOrDefault();

                if (usuarioAdmin != null)
                {
                    briefs = _context.Briefs.OrderByDescending(u => u.FechaRegistro).ToList();
                }
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
            try
            {
                var existingEntity = _context.Set<Brief>().Find(entity.Id);  // Carga la entidad original
                if (existingEntity != null)
                {
                    _context.Entry(existingEntity).CurrentValues.SetValues(entity);  // Solo copia los valores modificados
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
          
        }
        public IEnumerable<EstatusBrief> GetAllEstatusBrief()
        {
            return _context.EstatusBriefs.Where(q => q.Activo == true).ToList();
        }
        public IEnumerable<TipoBrief> GetAllTipoBrief()
        {
            return _context.TiposBrief.Where(q => q.Activo == true).ToList();
        }
        public void InsertProyecto(Proyecto entity)
        {
            var proyecto = _context.Proyectos.Where(q => q.BriefId == entity.BriefId).FirstOrDefault();
            
            if(proyecto != null)
            {
                proyecto.RequierePlan = entity.RequierePlan;
                proyecto.FechaPublicacion = entity.FechaPublicacion;
                proyecto.FechaModificacion = DateTime.Now;
                proyecto.Comentario = entity.Comentario;
                proyecto.Estado = entity.Estado;
                _context.Update(proyecto);
            }
            else
            {
                _context.Set<Proyecto>().Add(entity);
                
            }
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
                Prioridad = _context.Prioridad.Where(u => u.Id == p.PrioridadId).FirstOrDefault(),
                Ciclo = p.Ciclo,
                PCN = _context.PCN.Where(u => u.Id == p.Id).FirstOrDefault(),
                Audiencia = _context.Audiencia.Where(u => u.Id == p.Id).FirstOrDefault(),
                Formato = _context.Formato.Where(u => u.Id == p.Id).FirstOrDefault(),
                FechaEntrega = p.FechaEntrega,
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
            var isAdmin = _context.Usuarios.Where(q => q.Id == UsuarioId && (q.RolId == 1 || q.RolId == 3) ).FirstOrDefault();
            List<Brief> briefs = new List<Brief>();
            briefs.AddRange(_context.Briefs.Where(q => q.UsuarioId == UsuarioId).ToList());
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
            var IdsBrief = briefs.Select(q => q.Id).ToList();
            var materiales = _context.Materiales.Where(q => IdsBrief.Contains(q.BriefId) && (q.EstatusMaterialId == 4 || q.EstatusMaterialId == 5))
                             .Select(p => new Material
                             {
                                 Id = p.Id,
                                 Nombre = p.Nombre,
                                 Mensaje = p.Mensaje,
                                 Prioridad = _context.Prioridad.Where(u => u.Id == p.PrioridadId).FirstOrDefault(),
                                 Ciclo = p.Ciclo,
                                 PCN = _context.PCN.Where(u => u.Id == p.Id).FirstOrDefault(),
                                 Audiencia = _context.Audiencia.Where(u => u.Id == p.AudienciaId).FirstOrDefault(),
                                 Formato = _context.Formato.Where(u => u.Id == p.FormatoId).FirstOrDefault(),
                                 FechaEntrega = p.FechaEntrega,
                                 Responsable = p.Responsable,
                                 FechaRegistro = p.FechaRegistro,
                                 FechaModificacion = p.FechaModificacion,
                                 Brief = _context.Briefs.Where(q => q.Id == p.BriefId).FirstOrDefault(),
                                 EstatusMaterial = _context.EstatusMateriales.Where(u => u.Id == p.EstatusMaterialId).FirstOrDefault()
                             }).ToList();

            // Conteo de proyectos con materiales cuya FechaEntrega es menor o igual a la fecha actual
            conteoProyectos.ProyectoTiempo = materiales
                .Where(m => m.FechaEntrega >= DateTime.Now && (m.Brief.EstatusBriefId == 4 || m.Brief.EstatusBriefId == 5))
                .Select(m => m.Brief)
                .Distinct()
                .Count(); // Contamos proyectos únicos.

            var ProyectoTiempo = materiales
              .Where(m => m.FechaEntrega >= DateTime.Now && (m.Brief.EstatusBriefId == 4 || m.Brief.EstatusBriefId == 5))
              .Select(m => m.Brief)
              .Distinct().ToList();

            // Conteo de proyectos con materiales cuya FechaEntrega es mayor o igual a hoy
            conteoProyectos.ProyectoExtra = materiales
                .Where(m => m.FechaEntrega <= DateTime.Now && (m.Brief.EstatusBriefId == 4 || m.Brief.EstatusBriefId == 5))
                .Select(m => m.Brief)
                .Distinct()
                .Count(); // Contamos proyectos únicos.
            var ProyectoExtra = materiales
                .Where(m => m.FechaEntrega <= DateTime.Now && (m.Brief.EstatusBriefId == 4 || m.Brief.EstatusBriefId == 5))
                .Select(m => m.Brief)
                .Distinct().ToList();

            return conteoProyectos;
        }
        public ConteoProyectos ObtenerConteoMateriales(int UsuarioId)
        {
            var conteoProyectos = new ConteoProyectos();
            var isAdmin = _context.Usuarios.Where(q => q.Id == UsuarioId && (q.RolId == 1 || q.RolId == 3)).FirstOrDefault();
            var idsbrief = _context.Briefs.Where(q => q.UsuarioId == UsuarioId).Select(p => p.Id).ToList();
            var materiales = _context.Materiales.Where(q => idsbrief.Contains(q.BriefId)).ToList();

            if (isAdmin !=  null)
            {
                materiales = _context.Materiales.ToList();
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
            var isAdmin = _context.Usuarios.Where(q => q.Id == UsuarioId && (q.RolId == 1 || q.RolId == 3)).FirstOrDefault();
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
            var usuarioAdmin = _context.Usuarios.Where(q => q.Id == id && (q.RolId == 1 || q.RolId == 3)).FirstOrDefault();
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
                                 Responsable = p.Responsable,
                                 Area = p.Area,
                                 FechaRegistro = p.FechaRegistro,
                                 FechaModificacion = p.FechaModificacion,
                                 Brief = _context.Briefs.Where(q => q.Id == p.BriefId).FirstOrDefault(),
                                 EstatusMaterial = _context.EstatusMateriales.Where(u => u.Id == p.EstatusMaterialId).FirstOrDefault(),
                                 EstatusMaterialId = p.EstatusMaterialId
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
                    Responsable = p.Responsable,
                    Area = p.Area,
                    FechaRegistro = p.FechaRegistro,
                    FechaModificacion = p.FechaModificacion,
                    Brief = _context.Briefs.Where(q => q.Id == p.BriefId).FirstOrDefault(),
                    EstatusMaterialId = p.EstatusMaterialId,
                    EstatusMaterial = _context.EstatusMateriales.Where(u => u.Id == p.EstatusMaterialId).FirstOrDefault()
                }).ToList();
            }
            return materiales;
        }
        public Material GetMaterial(int id)
        {
            var material = _context.Materiales.Where(q => q.Id == id)
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
                                 Responsable = p.Responsable,
                                 Area = p.Area,
                                 FechaRegistro = p.FechaRegistro,
                                 FechaModificacion = p.FechaModificacion,
                                 Brief = _context.Briefs.Where(q => q.Id == p.BriefId).FirstOrDefault(),
                                 EstatusMaterial = _context.EstatusMateriales.Where(u => u.Id == p.EstatusMaterialId).FirstOrDefault(),
                                 EstatusMaterialId = p.EstatusMaterialId
                             }).FirstOrDefault();

            return material;
        }
        public List<Material> GetMaterialesFilter(Material material)
        {
            var usuarioAdmin = _context.Usuarios.Where(q => q.Id == material.Id && (q.RolId == 1 || q.RolId == 3)).FirstOrDefault();
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
                                 Responsable = p.Responsable,
                                 Area = p.Area,
                                 FechaRegistro = p.FechaRegistro,
                                 FechaModificacion = p.FechaModificacion,
                                 Brief = _context.Briefs.Where(q => q.Id == p.BriefId).FirstOrDefault(),
                                 EstatusMaterialId = p.EstatusMaterialId,
                                 EstatusMaterial = _context.EstatusMateriales.Where(u => u.Id == p.EstatusMaterialId).FirstOrDefault()
                             }).OrderByDescending(u => u.FechaEntrega).ToList();

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
                    Responsable = p.Responsable,
                    Area = p.Area,
                    FechaRegistro = p.FechaRegistro,
                    FechaModificacion = p.FechaModificacion,
                    Brief = _context.Briefs.Where(q => q.Id == p.BriefId).FirstOrDefault(),
                    EstatusMaterialId = p.EstatusMaterialId,
                    EstatusMaterial = _context.EstatusMateriales.Where(u => u.Id == p.EstatusMaterialId).FirstOrDefault()
                }).OrderByDescending(u => u.FechaEntrega).ToList();
            }
            return materiales;
        }
        public IEnumerable<Audiencia> GetAllAudiencias()
        {
            return _context.Audiencia.Where(q=> q.Activo == true).ToList();
        }
        public IEnumerable<Formato> GetAllFormatos()
        {
            return _context.Formato.Where(q => q.Activo == true).ToList();
        }
        public IEnumerable<PCN> GetAllPCN()
        {
            return _context.PCN.Where(q => q.Activo == true).ToList();
        }
        public IEnumerable<Prioridad> GetAllPrioridades()
        {
            var prioridades =new List<Prioridad>();
            try
            {
                prioridades = _context.Prioridad.Where(q => q.Activo == true).ToList();
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
            if (usuarioAdmin != null)
            {
                materiales = _context.Materiales.ToList();
            }

            conteoMateriales.Registros = materiales.Count();
            conteoMateriales.Revision = materiales.Where(q => q.EstatusMaterialId == 1).Count();
            conteoMateriales.FaltaInfo = materiales.Where(q => q.EstatusMaterialId == 2).Count();
            conteoMateriales.Aprobado = materiales.Where(q => q.EstatusMaterialId == 3).Count();
            conteoMateriales.Programado = materiales.Where(q => q.EstatusMaterialId == 4).Count();
            conteoMateriales.Entregado = materiales.Where(q => q.EstatusMaterialId == 5).Count();
            conteoMateriales.InicioCiclo = materiales.Where(q => q.EstatusMaterialId == 6).Count();


            return conteoMateriales;
        }
        public IEnumerable<EstatusMaterial> GetAllEstatusMateriales()
        {
            return _context.EstatusMateriales.Where(q => q.Activo == true).ToList();
        }
        public void ActualizaHistorialMaterial(HistorialMaterial historialMaterial)
        {
            _context.Add(historialMaterial);
            var material = _context.Materiales.Where(q => q.Id == historialMaterial.MaterialId).FirstOrDefault();
            material.EstatusMaterialId = historialMaterial.EstatusMaterialId;
            _context.Update(material);
            _context.SaveChanges();
            if(historialMaterial.EstatusMaterialId == 4)
            {
                var Briefs = _context.Materiales.Where(q => q.BriefId == material.BriefId && q.EstatusMaterialId != 4).ToList();
                if(Briefs == null || Briefs.Count == 0)
                {
                    var brief = _context.Briefs.Where(q => q.Id == material.BriefId).FirstOrDefault();
                    brief.EstatusBriefId = 4;
                    _context.Update(brief);
                    _context.SaveChanges();
                }
            }
            if (historialMaterial.EstatusMaterialId == 5)
            {
                var Briefs = _context.Materiales.Where(q => q.BriefId == material.BriefId && material.EstatusMaterialId != 5).ToList();
                if (Briefs == null)
                {
                    var brief = _context.Briefs.Where(q => q.Id == material.BriefId).FirstOrDefault();
                    brief.EstatusBriefId = 5;
                    _context.Update(brief);
                    _context.SaveChanges();
                }
            }


        }
        public void ActualizaRetrasoMaterial(RetrasoMaterial retrasoMaterial)
        {
            _context.Add(retrasoMaterial);
            _context.SaveChanges();
        }
        public IEnumerable<HistorialMaterial> GetAllHistorialMateriales(int MaterialId)
        {

            return _context.HistorialMateriales.Where(q => q.MaterialId == MaterialId)
                                               .Select(p => new HistorialMaterial
                                               {
                                                   Id = p.Id,
                                                   Comentarios = p.Comentarios,
                                                   FechaRegistro = p.FechaRegistro,
                                                   EstatusMaterialId = p.EstatusMaterialId,
                                                   UsuarioId = p.UsuarioId,
                                                   Usuario = _context.Usuarios.Where(w => w.Id == p.UsuarioId).FirstOrDefault(),
                                                   MaterialId = p.MaterialId
                                               })
                                               .OrderByDescending( u => u.Id)
                                               .ToList();
        }
    }
}
