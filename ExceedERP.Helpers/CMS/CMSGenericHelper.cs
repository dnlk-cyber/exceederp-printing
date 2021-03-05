using ExceedERP.Core.Domain.CMS;
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
        public static string GetCityById(int id)
        {
            var db = new ExceedDbContext();
            return db.Cities.Find(id)?.Name;
        }
        public static string GetTraineeSystemGeneratedIdNumber(int traineeId)
        {
            var db = new ExceedDbContext();
            return db.TraineeIdNumberComponents.FirstOrDefault(i => i.CmsTraineeId == traineeId)?.SystemId;
        }

        public static CmsTraineeIdentification GetMethodOfIdentification(int cmsTraineeId)
        {
            var db = new ExceedDbContext();
            return db.CmsTraineeIdentifications.FirstOrDefault(i => i.CmsTraineeId == cmsTraineeId &&( i.IdentificationType == Core.Domain.CMS.CmsTraineeIdentificationType.PASSPORT || i.IdentificationType == Core.Domain.CMS.CmsTraineeIdentificationType.RESIDENTIAL_ID));
        }

        public static string GetCourseCode(int?courseId)
        {
            var db = new ExceedDbContext();
            return db.CMSCourses.Find(courseId)?.Code;
        }
    }
}
