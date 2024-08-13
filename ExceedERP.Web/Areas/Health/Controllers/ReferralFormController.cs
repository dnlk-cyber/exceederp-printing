﻿using System.Data.Entity;
﻿using System.Linq;
﻿using System.Web.Mvc;
﻿using ExceedERP.Core.Domain.Health;
﻿using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Helpers.Common;
﻿using ExceedERP.Web.Controllers;
﻿using Kendo.Mvc.Extensions;
﻿using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Areas.HRM.Controllers;

namespace ExceedERP.Web.Areas.Health.Controllers
{
    [AuthorizeRoles(HrmConstants.HRHealthAdmin, HrmConstants.HRHealthUser)]
    public class ReferralFormController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string ReferredDateAm { get; set; }
        public static DateTime ReferredDate { get; set; }
        public static string ReturnedDateAm { get; set; }
        public static DateTime? ReturnedDate { get; set; }
        public ActionResult Index()
        {
           HRMInputs inputs = new HRMInputs();
           var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "ReferredDateAmharic")
            {
                ReferredDateAm = value;
                var splitDate = ReferredDateAm.Split('/');
                ReferredDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

            if (name == "ReturnedDateAmharic")
            {
                ReturnedDateAm = value;
                var splitDate = ReturnedDateAm.Split('/');
                ReturnedDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }
        public ActionResult ReferralForm_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ReferralForm> referralform = db.ReferralForm.Where( x=> x.MedicalRequestId ==id);
            DataSourceResult result = referralform.ToDataSourceResult(request, referralForm => new {
                ReferralFormId = referralForm.ReferralFormId,
                MedicalRequestId = referralForm.MedicalRequestId,
                ReferredDate = referralForm.ReferredDate,
                ReferredTime = referralForm.ReferredTime,
                ReturnedTime = referralForm.ReturnedTime,
                ReturnedDate = referralForm.ReturnedDate,
                ReturnedDateAmharic = referralForm.ReturnedDateAmharic,
                ReferredDateAmharic = referralForm.ReferredDateAmharic,
                Remark = referralForm.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReferralForm_Create([DataSourceRequest]DataSourceRequest request, ReferralForm referralForm, int id)
        {
            ModelState.Clear();
            if (ReferredDateAm == null)
            {
                ModelState.AddModelError("", "Referred date value  is mandatory!!");
            }
            //if (ReturnedDateAm == null)
            //{
            //    ReturnedDateAm = null;
            //}
            if (ReturnedDateAm == null)
            {
                ReturnedDate=null;
            }
            if (ModelState.IsValid)
            {
                var entity = new ReferralForm
                {
                    MedicalRequestId = id,
                    ReferredDate = ReferredDate,
                    ReferredTime = referralForm.ReferredTime,
                    ReturnedTime = referralForm.ReturnedTime,
                    ReturnedDate = ReturnedDate,
                    ReturnedDateAmharic = ReturnedDateAm,
                    ReferredDateAmharic = ReferredDateAm,
                    Remark = referralForm.Remark
                };

                db.ReferralForm.Add(entity);
                db.SaveChanges();
                referralForm.ReferralFormId = entity.ReferralFormId;
            }

            return Json(new[] { referralForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReferralForm_Update([DataSourceRequest]DataSourceRequest request, ReferralForm referralForm)
        {
            ModelState.Clear();
            if (ReferredDateAm == null)
            {
                ReferredDateAm = referralForm.ReferredDateAmharic;
                var splitDate = ReferredDateAm.Split('/');
                ReferredDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (ReturnedDateAm == null)
            {
                ReturnedDateAm = referralForm.ReturnedDateAmharic;
                var splitDate = ReferredDateAm.Split('/');
                ReturnedDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (ModelState.IsValid)
            {
                var entity = new ReferralForm
                {
                    ReferralFormId = referralForm.ReferralFormId,
                    MedicalRequestId = referralForm.MedicalRequestId,
                    ReferredDate = ReferredDate,
                    ReferredTime = referralForm.ReferredTime,
                    ReturnedTime = referralForm.ReturnedTime,
                    ReturnedDate = ReturnedDate,
                    ReturnedDateAmharic = ReturnedDateAm,
                    ReferredDateAmharic = ReferredDateAm,
                    Remark = referralForm.Remark
                };

                db.ReferralForm.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { referralForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReferralForm_Destroy([DataSourceRequest]DataSourceRequest request, ReferralForm referralForm)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReferralForm
                {
                    ReferralFormId = referralForm.ReferralFormId,
                    MedicalRequestId = referralForm.MedicalRequestId,
                    ReferredDate = referralForm.ReferredDate,
                    ReferredTime = referralForm.ReferredTime,
                    ReturnedTime = referralForm.ReturnedTime,
                    ReturnedDate = referralForm.ReturnedDate,
                    ReturnedDateAmharic = referralForm.ReturnedDateAmharic,
                    ReferredDateAmharic = referralForm.ReferredDateAmharic,
                    Remark = referralForm.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted referral information of medical request id " + referralForm.MedicalRequestId);
                #endregion
                db.ReferralForm.Attach(entity);
                db.ReferralForm.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { referralForm }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
