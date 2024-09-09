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
    public class BriefService : IBriefService
    {
        private readonly IBriefDal _briefDal;
        public BriefService(IBriefDal briefDal)
        {
            _briefDal = briefDal;
        }

        public void Delete(int id)
        {
            _briefDal.Delete(id);
        }
        public List<Brief> GetAllByUserId(int id)
        {
            return _briefDal.GetAllByUserId(id);
        }
        public List<Column<Brief>> GetColumnsByUserId(int id)
        {
            return _briefDal.GetColumnsByUserId(id);
        }
        public List<Brief> GetAll()
        {
            return _briefDal.GetAll();
        }

        public Brief GetById(int id)
        {
            return _briefDal.GetById(id);
        }


        public void Insert(Brief entity)
        {
            _briefDal.Insert(entity);
        }

        public void Update(Brief entity)
        {
            _briefDal.Update(entity);
        }
    }
}
