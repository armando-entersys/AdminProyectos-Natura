using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
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
    }
}
