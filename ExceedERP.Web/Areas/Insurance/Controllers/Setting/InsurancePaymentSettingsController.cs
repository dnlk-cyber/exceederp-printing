﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Insurance.Setting;

namespace ExceedERP.Web.Areas.Insurance.Controllers.Setting
{
 
    [AuthorizeRoles(HrmConstants.HRInsuranceAdmin)]

    public class InsurancePaymentSettingsController: BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
      
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InsurancePaymentSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InsurancePaymentSetting> insurancePaymentSettings = db.InsurancePaymentSettings;
            DataSourceResult result = insurancePaymentSettings.ToDataSourceResult(request, insurancePaymentSetting => new {
                InsurancePaymentSettingId = insurancePaymentSetting.InsurancePaymentSettingId,
                EmployeeType = insurancePaymentSetting.EmployeeType,
                BenefitType = insurancePaymentSetting.BenefitType,
                Level = insurancePaymentSetting.Level,
                CurrentValue = insurancePaymentSetting.CurrentValue,
                PaymentAmount = insurancePaymentSetting.PaymentAmount,
                Remark = insurancePaymentSetting.Remark,
                DateCreated = insurancePaymentSetting.DateCreated,
                LastModified = insurancePaymentSetting.LastModified,
                CreatedBy = insurancePaymentSetting.CreatedBy,
                ModifiedBy = insurancePaymentSetting.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsurancePaymentSettings_Create([DataSourceRequest]DataSourceRequest request, InsurancePaymentSetting insurancePaymentSetting)
        {
            ModelState.Clear();
            if(insurancePaymentSetting.PaymentAmount== PaymentAmount.NA
                ||insurancePaymentSetting.Level==ResponsibilityLevel.NOTSELECTED
                ||insurancePaymentSetting.BenefitType==InsuranceRefuendType.NA
                || insurancePaymentSetting.EmployeeType == EmployeeStatus.NOTSELECTED)
            {
                ModelState.AddModelError("exist", "Select proper values of Employee Type, Employee Level,Payment Amount and Benefit Type");
                return Json(new[] { insurancePaymentSetting }.ToDataSourceResult(request, ModelState));

            }
            if(db.InsurancePaymentSettings.Any(x=>x.BenefitType == insurancePaymentSetting.BenefitType
                                            &&x.Level== insurancePaymentSetting.Level
                                            &&x.EmployeeType== insurancePaymentSetting.EmployeeType))
            {
                ModelState.AddModelError("exist", "The record exists; you cannot add it again.");
                return Json(new[] { insurancePaymentSetting }.ToDataSourceResult(request, ModelState));

            }

            if (insurancePaymentSetting.PaymentAmount == PaymentAmount.Unlimited)
            {
                insurancePaymentSetting.CurrentValue = null;
                insurancePaymentSetting.Remark = "Unlimitted Value";
            }

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(insurancePaymentSetting);

                db.InsurancePaymentSettings.Add(entity);
                db.SaveChanges();
                insurancePaymentSetting.InsurancePaymentSettingId = entity.InsurancePaymentSettingId;

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Insurance Payment Setting", "Success", User.Identity.Name, User.Identity.Name + " add Insurance Payment  Setting at Date " + DateTime.Today + " Reference " + entity.InsurancePaymentSettingId, Modules.Insurance);

                #endregion
            }

            return Json(new[] { insurancePaymentSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsurancePaymentSettings_Update([DataSourceRequest]DataSourceRequest request, InsurancePaymentSetting insurancePaymentSetting)
        {
            ModelState.Clear();
            if (insurancePaymentSetting.PaymentAmount == PaymentAmount.NA
                || insurancePaymentSetting.Level == ResponsibilityLevel.NOTSELECTED
                || insurancePaymentSetting.BenefitType == InsuranceRefuendType.NA
                || insurancePaymentSetting.EmployeeType == EmployeeStatus.NOTSELECTED)
            {
                ModelState.AddModelError("exist", "Select proper values of Employee Type, Employee Level,Payment Amount and Benefit Type");
                return Json(new[] { insurancePaymentSetting }.ToDataSourceResult(request, ModelState));

            }
            if (db.InsurancePaymentSettings.Any(x => x.BenefitType == insurancePaymentSetting.BenefitType
                                             && x.Level == insurancePaymentSetting.Level
                                             && x.EmployeeType == insurancePaymentSetting.EmployeeType
                                             && x.InsurancePaymentSettingId != insurancePaymentSetting.InsurancePaymentSettingId))
            {
                ModelState.AddModelError("exist", "The record exists; you cannot add it again.");
                return Json(new[] { insurancePaymentSetting }.ToDataSourceResult(request, ModelState));

            }
            if (insurancePaymentSetting.PaymentAmount == PaymentAmount.Unlimited)
            {
                insurancePaymentSetting.CurrentValue = null;
                insurancePaymentSetting.Remark = "Unlimitted Value";
            }

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(insurancePaymentSetting);
                db.InsurancePaymentSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Insurance Setting", "Success", User.Identity.Name, User.Identity.Name + " update Insurance Setting at Date " + DateTime.Today + " Reference " + entity.InsurancePaymentSettingId, Modules.Insurance);

                #endregion
            }

            return Json(new[] { insurancePaymentSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsurancePaymentSettings_Destroy([DataSourceRequest]DataSourceRequest request, InsurancePaymentSetting insurancePaymentSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(insurancePaymentSetting);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted insrance Payement  setting  information of " + insurancePaymentSetting.InsurancePaymentSettingId);
                #endregion
                db.InsurancePaymentSettings.Attach(entity);
                db.InsurancePaymentSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { insurancePaymentSetting }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetCascadeAttributes(string Reference)
        {
            List<string> attributes = new List<string>();
            switch (Reference)
            {


                case "OVERTIME":

                    attributes =
                          Enum.GetValues(typeof(OverTimeAttribute))
                              .Cast<OverTimeAttribute>()
                              .Select(v => v.ToString())
                              .ToList();
                    break;
                case "PENSION":
                    attributes =
                          Enum.GetValues(typeof(PensionAttribute))
                              .Cast<PensionAttribute>()
                              .Select(v => v.ToString())
                              .ToList();
                    break;
                case "PROVIDENT_FUND":
                    attributes =
                          Enum.GetValues(typeof(ProvidentAttribute))
                              .Cast<ProvidentAttribute>()
                              .Select(v => v.ToString())
                              .ToList();
                    break;

            }


            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategories()
        {
            List<string> attributes = new List<string>();
            var businessClass = db.Lookups.Where(x => x.LookupType == LookUpTypes.INSURANCE_CLASS_OF_BUSINESS).ToList();

            attributes = businessClass.Select(x => x.Description).ToList() ;
                 
            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }

        private InsurancePaymentSetting GetMappedObject(InsurancePaymentSetting insurancePaymentSetting)
        {
            var entity = new InsurancePaymentSetting
            {
                InsurancePaymentSettingId = insurancePaymentSetting.InsurancePaymentSettingId,
                EmployeeType = insurancePaymentSetting.EmployeeType,
                BenefitType = insurancePaymentSetting.BenefitType,
                Level = insurancePaymentSetting.Level,
                CurrentValue = insurancePaymentSetting.CurrentValue,
                PaymentAmount = insurancePaymentSetting.PaymentAmount,
                Remark = insurancePaymentSetting.Remark,
                DateCreated = insurancePaymentSetting.DateCreated,
                LastModified = insurancePaymentSetting.LastModified,
                CreatedBy = insurancePaymentSetting.CreatedBy,
                ModifiedBy = insurancePaymentSetting.ModifiedBy
            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
