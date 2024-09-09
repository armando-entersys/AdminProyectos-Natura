using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IBriefService
    {
        void Delete(int id);
        List<Brief> GetAll();
        List<Brief> GetAllByUserId(int id);
        List<Column<Brief>> GetColumnsByUserId(int id);
        Brief GetById(int id);
        void Insert(Brief entity);
        void Update(Brief entity);
    }
}
