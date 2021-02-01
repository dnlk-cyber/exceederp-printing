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
    public class PatientResultDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/PatientResultDetail
        public ActionResult Index()
        {
            return View();
        }

        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
        }
        public ActionResult PatientResultDetailForDoctor_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<PatientResultDetail> patientResultDetailLits = new List<PatientResultDetail>();
            var patientDetail = db.PatientResults.FirstOrDefault(x => x.PatientResultId == id);
            if (patientDetail != null)
            {
                IQueryable<PatientResultDetail> patientResultDetails = db.PatientResultDetails.Where(x => x.PatientResultId == patientDetail.PatientResultId);
                patientResultDetailLits.AddRange(patientResultDetails);
            }
            DataSourceResult result = patientResultDetailLits.ToDataSourceResult(request, patientResultDetail => new
            {
                PatientResultDetailId = patientResultDetail.PatientResultDetailId,
                PatientResultId = patientResultDetail.PatientResultId,
                Description = patientResultDetail.Description,
                TestType = patientResultDetail.TestType
            });

            return Json(result);
        }
        public ActionResult PatientResultDetailForLaboratory_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<PatientResultDetail> patientResultDetailLits = new List<PatientResultDetail>();
            var patientDetail = db.PatientResults.FirstOrDefault(x => x.PatientResultId == id && x.IsForLaboratory);
            if (patientDetail != null)
            {
                IQueryable<PatientResultDetail> patientResultDetails = db.PatientResultDetails.Where(x => x.PatientResultId == patientDetail.PatientResultId);
                patientResultDetailLits.AddRange(patientResultDetails);
            }
            DataSourceResult result = patientResultDetailLits.ToDataSourceResult(request, patientResultDetail => new
            {
                PatientResultDetailId = patientResultDetail.PatientResultDetailId,
                PatientResultId = patientResultDetail.PatientResultId,
                Description = patientResultDetail.Description,
                TestType = patientResultDetail.TestType
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PatientResultDetail_Create([DataSourceRequest]DataSourceRequest request, PatientResultDetail patientResultDetail, int id)
        {

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new PatientResultDetail
                {
                    PatientResultId = id,
                    Description = patientResultDetail.Description,
                    TestType = patientResultDetail.TestType
                };

                db.PatientResultDetails.Add(entity);
                db.SaveChanges();
                patientResultDetail.PatientResultDetailId = entity.PatientResultDetailId;

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Patient Result Detail", "Success", User.Identity.Name, User.Identity.Name + " add Patient result detail at date " + DateTime.Today+" test type "+entity.TestType, Modules.HealthAndInsurance);

                #endregion
            }

            return Json(new[] { patientResultDetail }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PatientResultDetail> patientResultDetails, int id)
        {


            if (patientResultDetails != null && ModelState.IsValid)
            {

                foreach (var resultDetail in patientResultDetails)
                {


                    var patientResult = new PatientResultDetail();

                    patientResult.PatientResultId = id;
                    patientResult.Description = resultDetail.Description;
                    patientResult.TestType = resultDetail.TestType;




                    db.PatientResultDetails.Add(patientResult);
                    db.SaveChanges();



                }

            }

            return Json(patientResultDetails.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PatientResultDetail> patientResultDetails)
        {


            if (patientResultDetails != null && ModelState.IsValid)
            {

                foreach (var resultDetail in patientResultDetails)
                {


                    var patientResult = new PatientResultDetail();
                    patientResult.PatientResultDetailId = resultDetail.PatientResultDetailId;
                    patientResult.PatientResultId = resultDetail.PatientResultId;
                    patientResult.Description = resultDetail.Description;
                    patientResult.TestType = resultDetail.TestType;

                    db.PatientResultDetails.Attach(patientResult);
                    db.Entry(patientResult).State = EntityState.Modified;
                    db.SaveChanges();

                }

            }

            return Json(patientResultDetails.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PatientResultDetail_Update([DataSourceRequest]DataSourceRequest request, PatientResultDetail patientResultDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new PatientResultDetail
                {
                    PatientResultDetailId = patientResultDetail.PatientResultDetailId,
                    PatientResultId = patientResultDetail.PatientResultId,
                    Description = patientResultDetail.Description,
                    TestType = patientResultDetail.TestType
                };

                db.PatientResultDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Patient Result Detail", "Success", User.Identity.Name, User.Identity.Name + " update Patient result detail at date " + DateTime.Today + " test type " + entity.TestType, Modules.HealthAndInsurance);

                #endregion
            }

            return Json(new[] { patientResultDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PatientResultDetail_Destroy([DataSourceRequest]DataSourceRequest request, PatientResultDetail patientResultDetail)
        {

            if (ModelState.IsValid)
            {
                var entity = new PatientResultDetail
                {
                    PatientResultDetailId = patientResultDetail.PatientResultDetailId,
                    PatientResultId = patientResultDetail.PatientResultId,
                    Description = patientResultDetail.Description,
                    TestType = patientResultDetail.TestType
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Patient Result Detail " + patientResultDetail.PatientResultId);
                #endregion
                db.PatientResultDetails.Attach(entity);
                db.PatientResultDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { patientResultDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
