using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;

using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using System;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    public class CommitmentCosignController : Controller
    {
        // GET: HRM/EmployeeExperience
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime AgreementDate { get; set; }
        public static string AgreementDateAm { get; set; }
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var commtTypes = db.Lookups.Where(x => x.LookupType == LookUpTypes.COMMITMENT_TYPE);
            ViewData["commitment"] = commtTypes;
            var cosignTypes = db.Lookups.Where(x => x.LookupType == LookUpTypes.COSIGN_TYPE);
            ViewData["CosignType"] = cosignTypes;

            ViewData["EmployeeList"] = inputs.GetEmployees(User.Identity.Name).Where(x => x.IsActive);
        }
        public ActionResult Cosigns_Read([DataSourceRequest]DataSourceRequest request, int id)
        {

            IQueryable<Cosign> cosigns = db.Cosigns.Where(c => c.EmployeeId == id);
            DataSourceResult result = cosigns.ToDataSourceResult(request, cosign => new
            {
                CosignId = cosign.CosignId,
                EmployeeId = cosign.EmployeeId,
                AgreementDate = cosign.AgreementDate,
                FullName = cosign.FullName,
                BirthPlace = cosign.BirthPlace,
                LookupId = cosign.LookupId,
                Occupation = cosign.Occupation,
                Workorganization = cosign.Workorganization,
                Remark = cosign.Remark,
                AgreementDateAm = cosign.AgreementDateAm,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Cosigns_Create([DataSourceRequest]DataSourceRequest request, Cosign cosign, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (AgreementDateAm == null)
                {
                    ModelState.AddModelError("", "Agreement date is mandatory!!");
                }
                if (AgreementDate.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Agreement date should be less or equal to the current date!!");
                }

                else
                {
                    var entity = new Cosign
                    {
                        EmployeeId = id,// cosign.EmployeeId,
                        AgreementDate = AgreementDate,
                        FullName = cosign.FullName,
                        BirthPlace = cosign.BirthPlace,
                        Occupation = cosign.Occupation,
                        Workorganization = cosign.Workorganization,
                        LookupId = cosign.LookupId,
                        AgreementDateAm = AgreementDateAm,
                        Remark = cosign.Remark
                    };

                    db.Cosigns.Add(entity);
                    db.SaveChanges();
                    cosign.CosignId = entity.CosignId;

                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    if (person != null)
                    {
                      
                            UserHelper.OperationLog(Request.UserHostAddress, "Commitment Cosign", "Success", User.Identity.Name, "Commitment Cosign of " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);
                        
                    }
                    #endregion
                }
            }

            return Json(new[] { cosign }.ToDataSourceResult(request, ModelState));
        }
        public void SetDate(string name, string value)
        {
            if (name == "AgreementDateAm")
            {
                AgreementDateAm = value;
                var splitDate = AgreementDateAm.Split('/');
                AgreementDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }


        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Cosigns_Update([DataSourceRequest]DataSourceRequest request, Cosign cosign)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (AgreementDateAm == null)
                {
                    AgreementDateAm = cosign.AgreementDateAm;
                    var splitDate = AgreementDateAm.Split('/');
                    AgreementDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (AgreementDate.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Agreement date should be less or equal to the current date!!");
                }

                else
                {
                    var entity = new Cosign
                    {
                        CosignId = cosign.CosignId,
                        EmployeeId = cosign.EmployeeId,
                        AgreementDate = AgreementDate,
                        FullName = cosign.FullName,
                        BirthPlace = cosign.BirthPlace,
                        Occupation = cosign.Occupation,
                        Workorganization = cosign.Workorganization,
                        LookupId = cosign.LookupId,
                        Remark = cosign.Remark,
                        AgreementDateAm = AgreementDateAm,
                    };

                    db.Cosigns.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    if (person != null)
                    {
                        
                            UserHelper.OperationLog(Request.UserHostAddress, "Commitment Cosign", "Success", User.Identity.Name, "Commitment Cosign of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name,Modules.HRM);
                        
                    }
                    #endregion
                }
            }

            return Json(new[] { cosign }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Cosigns_Destroy([DataSourceRequest]DataSourceRequest request, Cosign cosign)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Cosign
                {
                    CosignId = cosign.CosignId,
                    EmployeeId = cosign.EmployeeId,
                    AgreementDate = cosign.AgreementDate,
                    FullName = cosign.FullName,
                    BirthPlace = cosign.BirthPlace,
                    Occupation = cosign.Occupation,
                    Workorganization = cosign.Workorganization,
                    LookupId = cosign.LookupId,
                    Remark = cosign.Remark,
                    AgreementDateAm = cosign.AgreementDateAm,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Cosign commitment of employee Id" + cosign.EmployeeId);
                #endregion
                db.Cosigns.Attach(entity);
                db.Cosigns.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { cosign }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}