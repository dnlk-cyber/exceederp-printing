using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Global;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead,
        HrmConstants.HRPerformanceUser,
        HrmConstants.HRPerformanceAdmin,
        HrmConstants.HREmployeeSelfService)]
    public class ProbationPerformanceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/ProbationPerformance
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["Employees"] = db.Person_Employee.ToList();
            ViewData["Org"] = db.MainCompanyStructures.ToList();
            ViewData["Jobtitle"] = db.Lookups.ToList();
            ViewData["Measurments"] = db.Lookups.Where(x=>x.LookupType == Core.Domain.Common.LookUpTypes.PROBATION_MEASUREMENT).ToList();
        }
        public ActionResult ProbationPerformance_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProbationPerformance> probations = db.ProbationPerformances;
            DataSourceResult result = probations.ToDataSourceResult(request, probation => new {

                ProbationPerformanceId = probation.ProbationPerformanceId,
                EmployeeId = probation.EmployeeId,
                HireDate = probation.HireDate,
                HireDateAm = probation.HireDateAm,
                LookupId = probation.LookupId,
                OrgStructureId = probation.OrgStructureId,
                ProbationEndDate = probation.ProbationEndDate,
                ProbationEndDateAm = probation.ProbationEndDateAm,
                Status = probation.Status
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationPerformance_Create([DataSourceRequest]DataSourceRequest request, ProbationPerformance probation)
        {
            ModelState.Clear();
            DateTime? hd = null;
            DateTime? ed = null;
            if(probation.HireDateAm != null)
            {
                var splitDate = probation.HireDateAm.Split('/');
                 hd = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            if (probation.ProbationEndDateAm != null)
            {
                var splitDateEnd = probation.ProbationEndDateAm.Split('/');
                 ed = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateEnd[0]), Convert.ToInt32(splitDateEnd[1]), Convert.ToInt32(splitDateEnd[2]));

            }

            var person = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == probation.EmployeeId);
            var hda = probation.HireDateAm;
            if(person != null)
            {
                hd = person.HiredDate;
                hda = person.HiredDateAmharic;

            }
            var place = db.Placements.FirstOrDefault(x=>x.EmployeeId == probation.EmployeeId && x.isCurrent);
            var look = 0;
            var org = 0;
            if(place != null)
            {
                 look = place.LookupId;
                org = place.OrgStructureId;
            }

            if (ModelState.IsValid)
            {

                var entity = new ProbationPerformance
                {
                    EmployeeId = probation.EmployeeId,
                    HireDate = hd,
                    HireDateAm = hda,
                    LookupId = look,
                    OrgStructureId = org,
                    ProbationEndDate = ed,
                    ProbationEndDateAm = probation.ProbationEndDateAm,
                    Status = probation.Status
                };

                db.ProbationPerformances.Add(entity);
                db.SaveChanges();

                #region operation log
                var person1 = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == entity.EmployeeId);
                if(person1 != null)
                {
                    if(person1 != null)
                    {
                       
                            UserHelper.OperationLog(Request.UserHostAddress, "Probation Performance ", "Success", User.Identity.Name, User.Identity.Name + " add Probation Performance for " + person1.FullNameaEnglish + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                        
                    }

                }

                #endregion

            }

            return Json(new[] { probation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationPerformance_Update([DataSourceRequest]DataSourceRequest request, ProbationPerformance probation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var splitDate = probation.HireDateAm.Split('/');
                DateTime hd = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

                var splitDateEnd = probation.ProbationEndDateAm.Split('/');
                DateTime ed = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateEnd[0]), Convert.ToInt32(splitDateEnd[1]), Convert.ToInt32(splitDateEnd[2]));


                var entity = new ProbationPerformance
                {
                    ProbationPerformanceId = probation.ProbationPerformanceId,
                    EmployeeId = probation.EmployeeId,
                    HireDate = hd,
                    HireDateAm = probation.HireDateAm,
                    LookupId = probation.LookupId,
                    OrgStructureId = probation.OrgStructureId,
                    ProbationEndDate = ed,
                    ProbationEndDateAm = probation.ProbationEndDateAm,
                    Status = probation.Status
                };

                db.ProbationPerformances.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var person1 = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person1 != null)
                {
                    if (person1 != null)
                    {
                            UserHelper.OperationLog(Request.UserHostAddress, "Probation Performance ", "Success", User.Identity.Name, User.Identity.Name + " update Probation Performance of " + person1.FullNameaEnglish + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                       

                    }

                }

                #endregion
            }

            return Json(new[] { probation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationPerformance_Destroy([DataSourceRequest]DataSourceRequest request, ProbationPerformance probation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProbationPerformance
                {
                    ProbationPerformanceId = probation.ProbationPerformanceId,
                    EmployeeId = probation.EmployeeId,
                    HireDate = probation.HireDate,
                    HireDateAm = probation.HireDateAm,
                    LookupId = probation.LookupId,
                    OrgStructureId = probation.OrgStructureId,
                    ProbationEndDate = probation.ProbationEndDate,
                    ProbationEndDateAm = probation.ProbationEndDateAm,
                    Status = probation.Status
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Probation Performance " + probation.ProbationPerformanceId);
                #endregion
                db.ProbationPerformances.Attach(entity);
                db.ProbationPerformances.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { probation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
