﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    public class PlanningDataProviderController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetGlobalIndicator()
        {
            var globalIndicators = db.GlobalIndicators.Select(A => new
            {
                A.GlobalIndicatorId,
                A.Indicator
            }).ToList();
            return Json(globalIndicators, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetIndicators()
        {
            var indicators = db.Indicators.Select(A => new
            {
                A.IndicatorId,
                A.Name
            }).ToList();
            return Json(indicators, JsonRequestBehavior.AllowGet);
        }
        
        
      

        public JsonResult GetProjects()
        {
            return Json(db.ProjectInformation.Select(x => new { x.ProjectInformationId, ProjectName = x.Code + "- " + x.Name }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectObjectives()
        {
            return Json(db.ProjectObjectives.Select(x => new { x.ProjectObjectiveId, Name = x.ProjectObjectiveName }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectKPIs()
        {
            return Json(db.ProjectKPIs.Select(x => new { x.ProjectKPIId, Name = x.KPIName  }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectSubKPIs(int kId)
        {
            return Json(db.ProjectSubKPIs.Where(x=>x.ProjectKPIId==kId).Select(x => new { x.ProjectSubKPIId, Name =  x.SubKPIName }), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetFiscalYear()
        {
            return Json(db.GLFiscalYears.Select(x => new { x.GlFiscalYearId, x.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWorkUnit()
        {

            return Json(db.MainCompanyStructures.ToList().Select(p => new
            {
                p.OrgStructureId,
                p.Name
                
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMyWorkUnit()
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var workUnitId = EmployeeHelper.GetEmployeeWorkUnitId(employeeId);
            return Json(db.MainCompanyStructures.Where(x=>x.OrgStructureId==workUnitId).ToList().Select(p => new
            {
                p.OrgStructureId,
                p.Name
                
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetObjectives(int workUnitId)
        {
            return Json(db.Objectives.Where(x => x.OrgStructureId == workUnitId).Select(x => new { x.ObjectiveId, x.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPlanObjectives(int workUnitId,int actualFiscalyearId)
        {
            var planfiscalyear = db.Plans.Where(x => x.GlFiscalYearId == actualFiscalyearId).FirstOrDefault();
            if (planfiscalyear != null)
            {
                 var collection = from p in db.BSCPlans.Where(x => x.OrgStructureId == workUnitId&&x.PlanId==planfiscalyear.PlanId)
                             join e in db.Objectives.Where(x => x.OrgStructureId == workUnitId)
                             on p.OrgStructureId equals e.OrgStructureId select
                             new
                             {
                                 p.BSCPlanId,
                                 e.ObjectiveId,
                                 e.Name,
                                 p.PlanId

                             };
                var objectives = collection.ToList();
                //return Json(db.Objectives.Where(x => x.OrgStructureId == workUnitId).Select(x => new { x.ObjectiveId, x.Name }), JsonRequestBehavior.AllowGet);
                return Json(objectives, JsonRequestBehavior.AllowGet);

            }


            return Json("", JsonRequestBehavior.AllowGet);

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
