﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Finance.Bank;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Web.Areas.Finance.Controllers.Bank
{
    public class ReconciliationBankServiceChargeDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReconciliationBankServiceChargeDistributions_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ReconciliationBankServiceChargeDistribution> reconciliationbankservicechargedistributions = db.ReconciliationBankServiceChargeDistributions.Where(x=>x.ReconciliationScheduleId==id);
            DataSourceResult result = reconciliationbankservicechargedistributions.ToDataSourceResult(request, reconciliationBankServiceChargeDistribution => new {
                ReconciliationBankServiceChargeDistributionId = reconciliationBankServiceChargeDistribution.ReconciliationBankServiceChargeDistributionId,
                AccountCode = reconciliationBankServiceChargeDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(reconciliationBankServiceChargeDistribution.AccountCode),
                AdditionalDescription = reconciliationBankServiceChargeDistribution.AdditionalDescription,
                Debit = reconciliationBankServiceChargeDistribution.Debit,
                Credit = reconciliationBankServiceChargeDistribution.Credit,               
                OrgBranchName = reconciliationBankServiceChargeDistribution.OrgBranchName,
                DateCreated = reconciliationBankServiceChargeDistribution.DateCreated,
                LastModified = reconciliationBankServiceChargeDistribution.LastModified,
                CreatedBy = reconciliationBankServiceChargeDistribution.CreatedBy,
                ModifiedBy = reconciliationBankServiceChargeDistribution.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconciliationBankServiceChargeDistributions_Create(int id,[DataSourceRequest]DataSourceRequest request, ReconciliationBankServiceChargeDistribution reconciliationBankServiceChargeDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReconciliationBankServiceChargeDistribution
                {
                    ReconciliationScheduleId=id,
                    AccountCode = reconciliationBankServiceChargeDistribution.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(reconciliationBankServiceChargeDistribution.AccountCode),
                    AdditionalDescription = reconciliationBankServiceChargeDistribution.AdditionalDescription,
                    Debit = reconciliationBankServiceChargeDistribution.Debit,
                    Credit = reconciliationBankServiceChargeDistribution.Credit,
                    OrgBranchName = reconciliationBankServiceChargeDistribution.OrgBranchName,
                    DateCreated = reconciliationBankServiceChargeDistribution.DateCreated,
                    LastModified = reconciliationBankServiceChargeDistribution.LastModified,
                    CreatedBy = reconciliationBankServiceChargeDistribution.CreatedBy,
                    ModifiedBy = reconciliationBankServiceChargeDistribution.ModifiedBy
                };

                db.ReconciliationBankServiceChargeDistributions.Add(entity);
                db.SaveChanges();
                reconciliationBankServiceChargeDistribution.ReconciliationBankServiceChargeDistributionId = entity.ReconciliationBankServiceChargeDistributionId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Bank reconciliation service charge", "Success", User.Identity.Name, $"created service charge {entity.AccountCode}  {entity.AdditionalDescription}", Modules.Finance);
                #endregion
            }

            return Json(new[] { reconciliationBankServiceChargeDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconciliationBankServiceChargeDistributions_Update([DataSourceRequest]DataSourceRequest request, ReconciliationBankServiceChargeDistribution reconciliationBankServiceChargeDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReconciliationBankServiceChargeDistribution
                {
                    ReconciliationScheduleId = reconciliationBankServiceChargeDistribution.ReconciliationScheduleId,
                    ReconciliationBankServiceChargeDistributionId = reconciliationBankServiceChargeDistribution.ReconciliationBankServiceChargeDistributionId,
                    AccountCode = reconciliationBankServiceChargeDistribution.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(reconciliationBankServiceChargeDistribution.AccountCode),
                    AdditionalDescription = reconciliationBankServiceChargeDistribution.AdditionalDescription,
                    Debit = reconciliationBankServiceChargeDistribution.Debit,
                    Credit = reconciliationBankServiceChargeDistribution.Credit,
                    OrgBranchName = reconciliationBankServiceChargeDistribution.OrgBranchName,
                    DateCreated = reconciliationBankServiceChargeDistribution.DateCreated,
                    LastModified = reconciliationBankServiceChargeDistribution.LastModified,
                    CreatedBy = reconciliationBankServiceChargeDistribution.CreatedBy,
                    ModifiedBy = reconciliationBankServiceChargeDistribution.ModifiedBy
                };

                db.ReconciliationBankServiceChargeDistributions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Bank reconciliation service charge", "Success", User.Identity.Name, $"updated service charge {entity.AccountCode}  {entity.AdditionalDescription}", Modules.Finance);
                #endregion
            }

            return Json(new[] { reconciliationBankServiceChargeDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconciliationBankServiceChargeDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ReconciliationBankServiceChargeDistribution reconciliationBankServiceChargeDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReconciliationBankServiceChargeDistribution
                {
                    ReconciliationBankServiceChargeDistributionId = reconciliationBankServiceChargeDistribution.ReconciliationBankServiceChargeDistributionId,
                    AccountCode = reconciliationBankServiceChargeDistribution.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(reconciliationBankServiceChargeDistribution.AccountCode),
                    AdditionalDescription = reconciliationBankServiceChargeDistribution.AdditionalDescription,
                    Debit = reconciliationBankServiceChargeDistribution.Debit,
                    Credit = reconciliationBankServiceChargeDistribution.Credit,
                    OrgBranchName = reconciliationBankServiceChargeDistribution.OrgBranchName,
                    DateCreated = reconciliationBankServiceChargeDistribution.DateCreated,
                    LastModified = reconciliationBankServiceChargeDistribution.LastModified,
                    CreatedBy = reconciliationBankServiceChargeDistribution.CreatedBy,
                    ModifiedBy = reconciliationBankServiceChargeDistribution.ModifiedBy
                };

                db.ReconciliationBankServiceChargeDistributions.Attach(entity);
                db.ReconciliationBankServiceChargeDistributions.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { reconciliationBankServiceChargeDistribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
