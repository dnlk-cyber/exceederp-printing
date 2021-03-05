using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CMS.Controllers
{
    public class CMSDataProviderController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public JsonResult GetPrograms()
        {
            var programs = db.CMSPrograms.ToList();
            return Json(programs.Select(p => new { p.CMSProgramId, p.Name }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCourses(int programId, string filterString)
        {
            var programs = db.CMSCourses.Where(p => p.CMSProgramId == programId).ToList();
            if (!string.IsNullOrEmpty(filterString))
            {
                programs = programs.Where(c => c.Name.ToLower().Contains(filterString.ToLower())).ToList();
            }
            return Json(programs.Select(p => new { p.CMSCourseId, Name = p.Code + " " + p.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllCourses()
        {
            var data = db.CMSCourses.ToList();

            return Json(data.Select(c => new { c.CMSCourseId, Name= c.Code + " " + c.Name }), JsonRequestBehavior.AllowGet);
        }
             

        public JsonResult GetCities(Region region, string filterString)
        {
            var data = db.Cities.Where(c => c.Region == region).ToList();
            if (!string.IsNullOrEmpty(filterString))
            {
                data = data.Where(c => c.Name.ToLower().Contains(filterString.ToLower())).ToList();
            }
            return Json(data.Select(p => new { p.CityId, p.Name }), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetTraineePrograms(int traineeId)
        //{
        //    var currentTrainee = db.Trainees.Find(traineeId);
        //    if(current)
        //}
    }
}