﻿using System;
﻿using System.Data.Entity;
﻿using System.Linq;
﻿using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Health;
﻿using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
﻿using Kendo.Mvc.UI;
using XAPI;

namespace ExceedERP.Web.Areas.Health.Controllers
{
    [AuthorizeRoles(HrmConstants.HRHealthAdmin, HrmConstants.HRHealthUser)]
    public class ReferralSheetController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime Date { get; set; }
        public static string DateAmharic { get; set; }
        public ActionResult Index()
        {
           HRMInputs inputs = new HRMInputs();
           var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            var hospitals = db.Companies.Where(x => x.Category =="2"  && x.IsActive );
            ViewData["Hospitals"] = hospitals;
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
        [AuthorizeRoles( HrmConstants.HRHealthUser)]
        public ActionResult ReferralSheet_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ReferralSheet> referralsheet = db.ReferralSheet.Where( x=> x.PatientId ==id);
            DataSourceResult result = referralsheet.ToDataSourceResult(request, referralSheet => new {
                ReferralSheetId = referralSheet.ReferralSheetId,
                PatientId = referralSheet.PatientId,
                Date = referralSheet.Date,
                ReferredFrom = referralSheet.ReferredFrom,
                MainCompanyId = referralSheet.MainCompanyId,
                BriefHistory = referralSheet.BriefHistory,
                PhiscalFindings = referralSheet.PhiscalFindings,
                LabXrayFindings = referralSheet.LabXrayFindings,
                Diagnosis = referralSheet.Diagnosis,
                TreatmentGiven = referralSheet.TreatmentGiven,
                Recommendation = referralSheet.Recommendation,
                RecommendedBy = referralSheet.RecommendedBy,
                Remark = referralSheet.Remark,
                DateAmharic = referralSheet.DateAmharic
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [AuthorizeRoles(HrmConstants.HRHealthAdmin, HrmConstants.HRHealthUser)]
        public ActionResult ReferralSheet_Create([DataSourceRequest]DataSourceRequest request, ReferralSheet referralSheet, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    ModelState.AddModelError("", "Date value is mandatory!!");
                }
                else if (Date.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Date should be less or equal to the current date!!");
                }
                
                else
                {
                    var entity = new ReferralSheet
                    {
                        PatientId = id,
                        Date = Date,
                        ReferredFrom = referralSheet.ReferredFrom,
                        MainCompanyId = referralSheet.MainCompanyId,
                        BriefHistory = referralSheet.BriefHistory,
                        PhiscalFindings = referralSheet.PhiscalFindings,
                        LabXrayFindings = referralSheet.LabXrayFindings,
                        Diagnosis = referralSheet.Diagnosis,
                        TreatmentGiven = referralSheet.TreatmentGiven,
                        Recommendation = referralSheet.Recommendation,
                        RecommendedBy = referralSheet.RecommendedBy,
                        Remark = referralSheet.Remark,
                        DateAmharic = DateAmharic
                    };

                    db.ReferralSheet.Add(entity);
                    db.SaveChanges();
                    referralSheet.ReferralSheetId = entity.ReferralSheetId;
                }
            }

            return Json(new[] { referralSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [AuthorizeRoles(HrmConstants.HRHealthUser)]
        public ActionResult ReferralSheet_Update([DataSourceRequest]DataSourceRequest request, ReferralSheet referralSheet)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    DateAmharic = referralSheet.DateAmharic;
                    var splitDate = DateAmharic.Split('/');
                    Date = EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (Date.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new ReferralSheet
                    {
                        ReferralSheetId = referralSheet.ReferralSheetId,
                        PatientId = referralSheet.PatientId,
                        Date = Date,
                        ReferredFrom = referralSheet.ReferredFrom,
                        MainCompanyId = referralSheet.MainCompanyId,
                        BriefHistory = referralSheet.BriefHistory,
                        PhiscalFindings = referralSheet.PhiscalFindings,
                        LabXrayFindings = referralSheet.LabXrayFindings,
                        Diagnosis = referralSheet.Diagnosis,
                        TreatmentGiven = referralSheet.TreatmentGiven,
                        Recommendation = referralSheet.Recommendation,
                        RecommendedBy = referralSheet.RecommendedBy,
                        Remark = referralSheet.Remark,
                        DateAmharic = DateAmharic
                    };

                    db.ReferralSheet.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json(new[] { referralSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [AuthorizeRoles(HrmConstants.HRHealthAdmin)]
        public ActionResult ReferralSheet_Destroy([DataSourceRequest]DataSourceRequest request, ReferralSheet referralSheet)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ReferralSheet
                {
                    ReferralSheetId = referralSheet.ReferralSheetId,
                    PatientId = referralSheet.PatientId,
                    Date = referralSheet.Date,
                    ReferredFrom = referralSheet.ReferredFrom,
                    MainCompanyId = referralSheet.MainCompanyId,
                    BriefHistory = referralSheet.BriefHistory,
                    PhiscalFindings = referralSheet.PhiscalFindings,
                    LabXrayFindings = referralSheet.LabXrayFindings,
                    Diagnosis = referralSheet.Diagnosis,
                    TreatmentGiven = referralSheet.TreatmentGiven,
                    Recommendation = referralSheet.Recommendation,
                    RecommendedBy = referralSheet.RecommendedBy,
                    Remark = referralSheet.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted referral information of patient card id " + referralSheet.PatientId);
                #endregion
                db.ReferralSheet.Attach(entity);
                db.ReferralSheet.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { referralSheet }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
