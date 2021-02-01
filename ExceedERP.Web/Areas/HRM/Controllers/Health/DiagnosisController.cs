using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Health
{
    public class DiagnosisController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/Diagnosis
        public ActionResult Index()
        {
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["employees"] = db.Person_Employee.ToList();
            ViewData["drugList"] = db.DrugLists.ToList();
            ViewData["drugUnit"] = db.DrugUnits.ToList();
        }
        public ActionResult Diagnosis_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var diagnosises = db.Diagnosis.Where(x => x.PatientResultId == id);
            DataSourceResult result = diagnosises.ToDataSourceResult(request, diagnosis => new
            {
                DiagnosisId = diagnosis.DiagnosisId,
                PatientResultId = diagnosis.PatientResultId,
                Description = diagnosis.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Diagnosis_Create([DataSourceRequest]DataSourceRequest request, Diagnosis diagnosis, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = new Diagnosis
                {
                    PatientResultId = id,
                    Description = diagnosis.Description
                };

                db.Diagnosis.Add(entity);
                db.SaveChanges();
                diagnosis.DiagnosisId = entity.DiagnosisId;

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Diagnosis", "Success", User.Identity.Name, User.Identity.Name + " add diagnosis at date" + DateTime.Today , Modules.HealthAndInsurance);

                #endregion
            }

            return Json(new[] { diagnosis }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Diagnosis_Update([DataSourceRequest]DataSourceRequest request, Diagnosis diagnosis)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new Diagnosis
                {
                    PatientResultId = diagnosis.PatientResultId,
                    DiagnosisId = diagnosis.DiagnosisId,
                    Description = diagnosis.Description
                };


                db.Diagnosis.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Diagnosis", "Success", User.Identity.Name, User.Identity.Name + " update diagnosis at date" + DateTime.Today, Modules.HealthAndInsurance);

                #endregion
            }

            return Json(new[] { diagnosis }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Diagnosis_Destroy([DataSourceRequest]DataSourceRequest request, Diagnosis diagnosis)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new Diagnosis
                {
                    PatientResultId = diagnosis.PatientResultId,
                    DiagnosisId = diagnosis.DiagnosisId,
                    Description = diagnosis.Description
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Diagnosis " + diagnosis.PatientResultId);
                #endregion
                db.Diagnosis.Attach(entity);
                db.Diagnosis.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { diagnosis }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}