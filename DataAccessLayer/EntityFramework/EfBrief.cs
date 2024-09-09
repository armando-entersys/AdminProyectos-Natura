using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using DataAccessLayer.Repositories;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.EntityFramework
{
    public class EfBrief : BriefRepository<Brief>, IBriefDal
    {
        public EfBrief(DataAccesContext context) : base(context)
        {
        }
    }
}
