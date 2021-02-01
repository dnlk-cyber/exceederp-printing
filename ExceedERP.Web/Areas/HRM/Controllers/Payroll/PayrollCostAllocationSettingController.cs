﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Payroll;
 using ExceedERP.Core.Domain.HRM.Payroll.Setting;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
      [Authorize(Roles = PayrollAdmin)]
    public class PayrollCostAllocationSettingController : BaseController
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private const string PayrollAdmin = "Payroll_Admin";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PayrollCostAllocationSetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PayrollCostAllocationSetting> payrollCostAllocationSettings = _db.PayrollCostAllocationSetting;
            DataSourceResult result = payrollCostAllocationSettings.ToDataSourceResult(request, payrollCostAllocationSetting => new {
                PayrollCostAllocationSettingId = payrollCostAllocationSetting.PayrollCostAllocationSettingId,
                CostCenter = payrollCostAllocationSetting.CostCenter,
                ProductAccount = payrollCostAllocationSetting.ProductAccount,
                Percentage = payrollCostAllocationSetting.Percentage,
                Remark = payrollCostAllocationSetting.Remark,
                DateCreated = payrollCostAllocationSetting.DateCreated,
                LastModified = payrollCostAllocationSetting.LastModified,

                CreatedBy = payrollCostAllocationSetting.CreatedBy,

                ModifiedBy = payrollCostAllocationSetting.ModifiedBy 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PayrollCostAllocationSetting_Create([DataSourceRequest]DataSourceRequest request, PayrollCostAllocationSetting payrollCostAllocationSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new PayrollCostAllocationSetting
                {
                    CostCenter = payrollCostAllocationSetting.CostCenter,
                    ProductAccount = payrollCostAllocationSetting.ProductAccount,
                    Percentage = payrollCostAllocationSetting.Percentage,
                    Remark = payrollCostAllocationSetting.Remark,
                    DateCreated =DateTime .Now ,
                    LastModified = payrollCostAllocationSetting.LastModified,
                    CreatedBy = User.Identity .Name ,

                    ModifiedBy = payrollCostAllocationSetting.ModifiedBy
                };

                _db.PayrollCostAllocationSetting.Add(entity);
                _db.SaveChanges();
                payrollCostAllocationSetting.PayrollCostAllocationSettingId = entity.PayrollCostAllocationSettingId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll cost allocation", "Success", User.Identity.Name, $"added cost allocation for cost center {entity.CostCenter} product account {entity.ProductAccount} percentage {entity.Percentage}", Modules.Payroll);
                #endregion
            }

            return Json(new[] { payrollCostAllocationSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PayrollCostAllocationSetting_Update([DataSourceRequest]DataSourceRequest request, PayrollCostAllocationSetting payrollCostAllocationSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new PayrollCostAllocationSetting
                {
                    PayrollCostAllocationSettingId = payrollCostAllocationSetting.PayrollCostAllocationSettingId,
                    CostCenter = payrollCostAllocationSetting.CostCenter,
                    ProductAccount = payrollCostAllocationSetting.ProductAccount,
                    Percentage = payrollCostAllocationSetting.Percentage,
                    Remark = payrollCostAllocationSetting.Remark,
                    DateCreated = payrollCostAllocationSetting.DateCreated ,
                    LastModified = DateTime.Now,

                    CreatedBy = payrollCostAllocationSetting.CreatedBy ,

                    ModifiedBy = User.Identity.Name
                };

                _db.PayrollCostAllocationSetting.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll cost allocation", "Success", User.Identity.Name, $"updated cost allocation for cost center {entity.CostCenter} product account {entity.ProductAccount} percentage {entity.Percentage}", Modules.Payroll);
                #endregion
            }

            return Json(new[] { payrollCostAllocationSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PayrollCostAllocationSetting_Destroy([DataSourceRequest]DataSourceRequest request, PayrollCostAllocationSetting payrollCostAllocationSetting)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new PayrollCostAllocationSetting
                {
                    PayrollCostAllocationSettingId = payrollCostAllocationSetting.PayrollCostAllocationSettingId,
                    CostCenter = payrollCostAllocationSetting.CostCenter,
                    ProductAccount = payrollCostAllocationSetting.ProductAccount,
                    Percentage = payrollCostAllocationSetting.Percentage,
                    Remark = payrollCostAllocationSetting.Remark,
                    DateCreated = payrollCostAllocationSetting.DateCreated,
                    LastModified = payrollCostAllocationSetting.LastModified,
                    CreatedBy = payrollCostAllocationSetting.CreatedBy,
                    ModifiedBy = payrollCostAllocationSetting.ModifiedBy 
                };
                #region delete log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  cost center product allocation setting information of setting " + payrollCostAllocationSetting.Percentage,Modules.Payroll);
                #endregion
                _db.PayrollCostAllocationSetting.Attach(entity);
                _db.PayrollCostAllocationSetting.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { payrollCostAllocationSetting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
