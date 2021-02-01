﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain.Finance;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Finance.Bank;
 using ExceedERP.Core.Domain.Finance.GL;
 using ExceedERP.Core.Domain.Inventory;
 using ExceedERP.DataAccess.Context;
 using ExceedERP.Helpers.Finance;
 using ExceedERP.Web.Areas.FleetManagement.Controllers;
 using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Finance.Controllers.Bank
{
    public class ReconciliationBankServiceChargeController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReconciliationBankServiceCharges_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ReconciliationBankServiceCharge> reconciliationbankservicecharges = _db.ReconciliationBankServiceCharges.Where(x=>x.ReconciliationScheduleId==id);
            DataSourceResult result = reconciliationbankservicecharges.ToDataSourceResult(request, reconciliationBankServiceCharge => new {
                ReconciliationBankServiceChargeId = reconciliationBankServiceCharge.ReconciliationBankServiceChargeId,
                Amount = reconciliationBankServiceCharge.Amount,
                Date = reconciliationBankServiceCharge.Date,
                Remark = reconciliationBankServiceCharge.Remark,

                
                OrgBranchName = reconciliationBankServiceCharge.OrgBranchName,
                DateCreated = reconciliationBankServiceCharge.DateCreated,
                LastModified = reconciliationBankServiceCharge.LastModified,
                CreatedBy = reconciliationBankServiceCharge.CreatedBy,
                ModifiedBy = reconciliationBankServiceCharge.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconciliationBankServiceCharges_Create(int id,[DataSourceRequest]DataSourceRequest request, ReconciliationBankServiceCharge reconciliationBankServiceCharge)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReconciliationBankServiceCharge
                {
                    ReconciliationScheduleId = id,
                    Amount = reconciliationBankServiceCharge.Amount,
                    Date = reconciliationBankServiceCharge.Date.Date,
                    Remark = reconciliationBankServiceCharge.Remark,
                    DateCreated = reconciliationBankServiceCharge.DateCreated,
                    LastModified = reconciliationBankServiceCharge.LastModified,
                    CreatedBy = reconciliationBankServiceCharge.CreatedBy,
                    ModifiedBy = reconciliationBankServiceCharge.ModifiedBy
                };

                _db.ReconciliationBankServiceCharges.Add(entity);
                _db.SaveChanges();
                reconciliationBankServiceCharge.ReconciliationBankServiceChargeId = entity.ReconciliationBankServiceChargeId;

                _distributionHelper.UpdateBankServiceChargeDistribution(reconciliationBankServiceCharge.ReconciliationBankServiceChargeId);
            }

           
            return Json(new[] { reconciliationBankServiceCharge }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconciliationBankServiceCharges_Update([DataSourceRequest]DataSourceRequest request, ReconciliationBankServiceCharge reconciliationBankServiceCharge)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReconciliationBankServiceCharge
                {
                    ReconciliationBankServiceChargeId = reconciliationBankServiceCharge.ReconciliationBankServiceChargeId,
                    ReconciliationScheduleId = reconciliationBankServiceCharge.ReconciliationScheduleId,
                    Amount = reconciliationBankServiceCharge.Amount,
                    Date = reconciliationBankServiceCharge.Date,
                    Remark = reconciliationBankServiceCharge.Remark,

                  
                    OrgBranchName = reconciliationBankServiceCharge.OrgBranchName,
                    DateCreated = reconciliationBankServiceCharge.DateCreated,
                    LastModified = reconciliationBankServiceCharge.LastModified,
                    CreatedBy = reconciliationBankServiceCharge.CreatedBy,
                    ModifiedBy = reconciliationBankServiceCharge.ModifiedBy
                };

                _db.ReconciliationBankServiceCharges.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                _distributionHelper.UpdateBankServiceChargeDistribution(reconciliationBankServiceCharge.ReconciliationBankServiceChargeId);
            }

            return Json(new[] { reconciliationBankServiceCharge }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconciliationBankServiceCharges_Destroy([DataSourceRequest]DataSourceRequest request, ReconciliationBankServiceCharge reconciliationBankServiceCharge)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReconciliationBankServiceCharge
                {
                    ReconciliationBankServiceChargeId = reconciliationBankServiceCharge.ReconciliationBankServiceChargeId,
                    Amount = reconciliationBankServiceCharge.Amount,
                    Date = reconciliationBankServiceCharge.Date,
                    Remark = reconciliationBankServiceCharge.Remark,
                    DateCreated = reconciliationBankServiceCharge.DateCreated,
                    LastModified = reconciliationBankServiceCharge.LastModified,
                    CreatedBy = reconciliationBankServiceCharge.CreatedBy,
                    ModifiedBy = reconciliationBankServiceCharge.ModifiedBy
                };

                _db.ReconciliationBankServiceCharges.Attach(entity);
                _db.ReconciliationBankServiceCharges.Remove(entity);
                _db.SaveChanges();

                _distributionHelper.UpdateBankServiceChargeDistribution(reconciliationBankServiceCharge.ReconciliationBankServiceChargeId);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted reconciliation service charge " + reconciliationBankServiceCharge.ReconciliationScheduleId + " " + reconciliationBankServiceCharge.Remark, Modules.Finance);
                #endregion
            }

            return Json(new[] { reconciliationBankServiceCharge }.ToDataSourceResult(request, ModelState));
        }

       

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
