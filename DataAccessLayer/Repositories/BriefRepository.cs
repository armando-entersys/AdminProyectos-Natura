using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            IEnumerable<Brief> briefs = _context.Briefs.Where(q=> q.UsuarioId == usuarioId).ToList();

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
        public void InsertProyecto(Proyecto entity)
        {
            _context.Set<Proyecto>().Add(entity);
            _context.SaveChanges();
        }
        public void InsertMaterial(Material entity)
        {
            _context.Set<Material>().Add(entity);
            _context.SaveChanges();
        }
    }
}
