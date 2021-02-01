using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using XAPI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Health
{
 
    [AuthorizeRoles(HrmConstants.HRHealthAdmin, HrmConstants.HRHealthUser)]
    public class PrescriptionController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime Date { get; set; }
        public static string DateAmharic { get; set; }
        string ErrorMessage = "";
        public ActionResult Index()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "DateAmharic")
            {
                DateAmharic = value;
                var splitDate = DateAmharic.Split('/');
                Date = EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }



        }
        private bool CheckPrescriptionValidation(int id)
        {
            ErrorMessage = "";


            #region Treatment
            var Treatment = db.Treatment.Where(x => x.PrescriptionId == id).FirstOrDefault();
            if (Treatment != null)
            {

                ErrorMessage = "You cannot delete this Prescription.Please delete Treatment information first.";
                return false;


            }
            #endregion
            return true;
        }
        [AuthorizeRoles(HrmConstants.HRHealthAdmin, HrmConstants.HRHealthUser)]
        public ActionResult Prescription_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<Prescription> prescriptions = db.Prescription.Where(x => x.PatientId == id);
            DataSourceResult result = prescriptions.ToDataSourceResult(request, prescription => new
            {
                PrescriptionId = prescription.PrescriptionId,
                PatientId = prescription.PatientId,
                Date = prescription.Date,
                Rate = prescription.Rate,
                TreatedBy = prescription.TreatedBy,
                Remark = prescription.Remark,
                DateAmharic = prescription.DateAmharic
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [AuthorizeRoles( HrmConstants.HRHealthUser)]
        public ActionResult Prescription_Create([DataSourceRequest]DataSourceRequest request, Prescription prescription, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    ModelState.AddModelError("", "Date value is mandatory!!");
                }
                else
                {
                  
                    var entity = new Prescription
                    {
                        PatientId = id,
                        Date = Date,
                        Rate = prescription.Rate,
                        TreatedBy = prescription.TreatedBy,
                        Remark = prescription.Remark,
                        DateAmharic = DateAmharic
                    };

                    db.Prescription.Add(entity);
                    db.SaveChanges();
                    prescription.PrescriptionId = entity.PrescriptionId;
                }
            }

            return Json(new[] { prescription }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [AuthorizeRoles(HrmConstants.HRHealthUser)]
        public ActionResult Prescription_Update([DataSourceRequest]DataSourceRequest request, Prescription prescription)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    DateAmharic = prescription.DateAmharic;
                    var splitDate = DateAmharic.Split('/');
                    Date = EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
            
                var entity = new Prescription
                {
                    PrescriptionId = prescription.PrescriptionId,
                    PatientId = prescription.PatientId,
                    Date = Date,
                    Rate = prescription.Rate,
                    TreatedBy = prescription.TreatedBy,
                    Remark = prescription.Remark,
                    DateAmharic = DateAmharic
                };

                db.Prescription.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { prescription }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [AuthorizeRoles(HrmConstants.HRHealthAdmin)]
        public ActionResult Prescription_Destroy([DataSourceRequest]DataSourceRequest request, Prescription prescription)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (!CheckPrescriptionValidation(prescription.PrescriptionId))
                {
                    ModelState.AddModelError("", ErrorMessage);
                }
                else
                {

                    var entity = new Prescription
                    {
                        PrescriptionId = prescription.PrescriptionId,
                        PatientId = prescription.PatientId,
                        Date = prescription.Date,
                        Rate = prescription.Rate,
                        TreatedBy = prescription.TreatedBy,
                        Remark = prescription.Remark,
                        DateAmharic = prescription.DateAmharic
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted prescription information of card id " + prescription.PatientId);
                    #endregion
                    db.Prescription.Attach(entity);
                    db.Prescription.Remove(entity);
                    db.SaveChanges();
                }
            }
            return Json(new[] { prescription }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
