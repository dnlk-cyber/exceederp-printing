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
using System.Web;
using ExceedERP.Core.Domain.Global;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    public class ProbationPerformanceDetailController : Controller
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
            ViewData["Measurments"] = db.Lookups.Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.PROBATION_MEASUREMENT).ToList();
        }
        public ActionResult ProbationPerformanceDetail_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProbationPerformanceDetail> details = db.ProbationPerformanceDetails.Where(x=>x.ProbationPerformanceId == id);
            DataSourceResult result = details.ToDataSourceResult(request, probation => new {

                ProbationPerformanceId = probation.ProbationPerformanceId,
                ProbationPerformanceDetailId = probation.ProbationPerformanceDetailId,
                Measurement = probation.Measurement,
                LookupId = probation.LookupId,
                Score = probation.Score
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationPerformanceDetail_Create([DataSourceRequest]DataSourceRequest request, ProbationPerformanceDetail probation, int id)
        {
           
            if (ModelState.IsValid)
            {
               
                var score = db.ProbationPerformanceSettings.FirstOrDefault(x=>x.IntialWeight <= probation.Score && x.EndWeight >= probation.Score);
                if (score != null)
                {

                    var entity = new ProbationPerformanceDetail
                    {
                        ProbationPerformanceId = id,
                        Measurement = score.ProbationMeasurement,
                        LookupId = probation.LookupId,
                        Score = probation.Score
                    };

                    db.ProbationPerformanceDetails.Add(entity);
                    db.SaveChanges();

                    #region operation log
                    var prob = db.ProbationPerformances.FirstOrDefault(x => x.ProbationPerformanceId == entity.ProbationPerformanceId);
                    if (prob != null)
                    {
                        var person1 = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == prob.EmployeeId);
                        if (person1 != null)
                        {
                            if (person1 != null)
                            {
                                UserHelper.OperationLog(Request.UserHostAddress, "Probation Performance Detail", "Success", User.Identity.Name, User.Identity.Name + " add Probation Performance Criteria for " + person1.FullNameaEnglish + " date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                            }

                        }
                        prob.Result += entity.Score;
                        db.Entry(prob).State = EntityState.Modified;
                        db.SaveChanges();



                    }
                }
                #endregion

            }

            return Json(new[] { probation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationPerformanceDetail_Update([DataSourceRequest]DataSourceRequest request, ProbationPerformanceDetail probation)
        {
            if (ModelState.IsValid)
            {
                var score = db.ProbationPerformanceSettings.FirstOrDefault(x => x.IntialWeight <= probation.Score && x.EndWeight >= probation.Score);
                if (score != null)
                {
                    var entity = new ProbationPerformanceDetail
                    {
                        ProbationPerformanceId = probation.ProbationPerformanceId,
                        ProbationPerformanceDetailId = probation.ProbationPerformanceDetailId,
                        Measurement = score.ProbationMeasurement,
                        Score = probation.Score,
                        LookupId = probation.LookupId
                    };

                    db.ProbationPerformanceDetails.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log
                    var prob = db.ProbationPerformances.FirstOrDefault(x => x.ProbationPerformanceId == entity.ProbationPerformanceId);
                    if (prob != null)
                    {
                        var person1 = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == prob.EmployeeId);
                        if (person1 != null)
                        {
                            if (person1 != null)
                            {
                                UserHelper.OperationLog(Request.UserHostAddress, "Probation Performance Detail", "Success", User.Identity.Name, User.Identity.Name + " update Probation Performance Criteria of " + person1.FullNameaEnglish + " date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                            }

                        }
                        prob.Result = (decimal)db.ProbationPerformanceDetails
                          .Where(x => x.ProbationPerformanceId == prob.ProbationPerformanceId).Sum(x => x.Score);
                        db.Entry(prob).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }

                #endregion

            }

            return Json(new[] { probation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProbationPerformanceDetail_Destroy([DataSourceRequest]DataSourceRequest request, ProbationPerformanceDetail probation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProbationPerformanceDetail
                {
                    ProbationPerformanceId = probation.ProbationPerformanceId,
                    ProbationPerformanceDetailId = probation.ProbationPerformanceDetailId,
                    Measurement = probation.Measurement,
                    Score = probation.Score,
                    LookupId = probation.LookupId
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Probation Performance " + probation.LookupId);
                #endregion
                db.ProbationPerformanceDetails.Attach(entity);
                db.ProbationPerformanceDetails.Remove(entity);
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
