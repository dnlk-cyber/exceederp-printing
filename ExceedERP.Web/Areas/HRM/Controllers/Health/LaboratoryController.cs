using ExceedERP.Core.Domain.APTS;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
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
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Health
{
    public class LaboratoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: APTS/Laboratory
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["employees"] = db.Person_Employee.ToList();
            ViewData["drugList"] = db.DrugLists.ToList();
            ViewData["drugUnit"] = db.DrugUnits.ToList();
        }
        public ActionResult Laboratory_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PatientResult> patientResults = db.PatientResults.Where(x => x.IsForLaboratory);
            DataSourceResult result = patientResults.ToDataSourceResult(request, patientResult => new
            {
                PatientResultId = patientResult.PatientResultId,
                PatientId = patientResult.PatientId,
                DoctorId = patientResult.DoctorId,
                Description = patientResult.Description,
                RegisterDate = patientResult.RegisterDate,
                RegisterDateAm = patientResult.RegisterDateAm,
                IsForDespensary = patientResult.IsForDespensary,
                IsForLaboratory = patientResult.IsForLaboratory
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Laboratory_Create([DataSourceRequest]DataSourceRequest request, PatientResult patientResult)
        {

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var registerDate = patientResult.RegisterDateAm.Split('/');
                var StartDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(registerDate[0]), Convert.ToInt32(registerDate[1]), Convert.ToInt32(registerDate[2]));
                var entity = new PatientResult
                {
                    PatientId = patientResult.PatientId,
                    DoctorId = UserInformationHelper.GetEmployeeId(User.Identity.Name),
                    Description = patientResult.Description,
                    RegisterDate = StartDate,
                    RegisterDateAm = patientResult.RegisterDateAm,
                    IsForDespensary = patientResult.IsForDespensary,
                    IsForLaboratory = patientResult.IsForLaboratory
                };

                db.PatientResults.Add(entity);
                db.SaveChanges();
                patientResult.PatientResultId = entity.PatientResultId;

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Laboratory", "Success", User.Identity.Name, User.Identity.Name + " add Laboratory at date " + DateTime.Today, Modules.HealthAndInsurance);
                
                #endregion
            }

            return Json(new[] { patientResult }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Laboratory_Update([DataSourceRequest]DataSourceRequest request, PatientResult patientResult)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var registerDate = patientResult.RegisterDateAm.Split('/');
                var StartDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(registerDate[0]), Convert.ToInt32(registerDate[1]), Convert.ToInt32(registerDate[2]));

                var entity = new PatientResult
                {
                    PatientResultId = patientResult.PatientResultId,
                    PatientId = patientResult.PatientId,
                    DoctorId = patientResult.DoctorId,
                    Description = patientResult.Description,
                    RegisterDate = StartDate,
                    RegisterDateAm = patientResult.RegisterDateAm,
                    IsForDespensary = patientResult.IsForDespensary,
                    IsForLaboratory = patientResult.IsForLaboratory
                };

                db.PatientResults.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Laboratory", "Success", User.Identity.Name, User.Identity.Name + " update Laboratory at date " + DateTime.Today, Modules.HealthAndInsurance);

                #endregion
            }

            return Json(new[] { patientResult }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Laboratory_Destroy([DataSourceRequest]DataSourceRequest request, PatientResult patientResult)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new PatientResult
                {
                    PatientResultId = patientResult.PatientResultId,
                    PatientId = patientResult.PatientId,
                    DoctorId = patientResult.DoctorId,
                    Description = patientResult.Description,
                    RegisterDate = patientResult.RegisterDate,
                    RegisterDateAm = patientResult.RegisterDateAm,
                    IsForDespensary = patientResult.IsForDespensary,
                    IsForLaboratory = patientResult.IsForLaboratory
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Diagnosis " + patientResult.PatientId);
                #endregion
                db.PatientResults.Attach(entity);
                db.PatientResults.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { patientResult }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Checked(int id)
        {
            if (ModelState.IsValid)
            {
                var patientRegister = db.PatientResults.FirstOrDefault(x => x.PatientResultId == id);

                if (patientRegister != null)
                {

                    patientRegister.OnChecking = false;


                    db.Entry(patientRegister).State = EntityState.Modified;
                    db.SaveChanges();
                }


            }



            return RedirectToAction("Index");

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}