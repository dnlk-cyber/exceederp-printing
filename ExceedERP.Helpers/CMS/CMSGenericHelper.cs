using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.CMS
{
   public static class CMSGenericHelper
    {
        public static string GetProgramById(int id)
        {
            var db = new ExceedDbContext();
            return  db.CMSPrograms.Find(id)?.Name;
        }
        public static string GetCourseById(int id)
        {
            var db = new ExceedDbContext();
            return db.CMSCourses.Find(id)?.Name;
        }
    }
}
