using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
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
            var departmentId = PlanningSelfCareHelper.GetCurrentDepartmentId(User.Identity.Name);
            var globalIndicators = PlanningSelfCareHelper.GetDepartmentGlobalIndicators(db, departmentId)
                .Where(x => x.IsActive)
                .Select(A => new
            {
                A.GlobalIndicatorId,
                A.Indicator
            }).ToList();
            return Json(globalIndicators, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetIndicators()
        {
            var indicators = PlanningSelfCareHelper.GetDepartmentIndicatorList(db, User.Identity.Name, true);
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

        public JsonResult GetGlobalKPI()
        {
            return Json(db.GlobalKPIs.Where(x=>x.IsActive).Select(x => new { x.GlobalKPIId,  x.KPIName  }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubKPIs(int kId,string searchText)
        {
            var globalSubKPIs = db.GlobalSubKPIs.Where(x => x.GlobalKPIId == kId);
            if (!string.IsNullOrEmpty(searchText))
            {
                 globalSubKPIs = db.GlobalSubKPIs.Where(x => x.SubKPIName.ToUpper().Contains(searchText.ToUpper()));

            }
            return Json(globalSubKPIs.Select(x => new { x.GlobalSubKPIId, x.SubKPIName }), JsonRequestBehavior.AllowGet);
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

        public JsonResult GetUoMs()
        {
            var uoms = db.UoMs.Select(x => x.UoMName).ToList();
            var existingUnits = db.Indicators.Where(x => !string.IsNullOrEmpty(x.Unit)).Select(x => x.Unit).Distinct().ToList();
            var all = uoms.Union(existingUnits).Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x).Select(x => new
            {
                Name = x,
                Code = x
            }).ToList();
            return Json(all, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
