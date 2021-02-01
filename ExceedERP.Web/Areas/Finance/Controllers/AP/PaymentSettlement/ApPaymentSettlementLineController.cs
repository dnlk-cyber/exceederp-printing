﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Finance.AP;
 using ExceedERP.DataAccess.Context;
 using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
    public class ApPaymentSettlementLineController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApPaymentSettlementLines_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApPaymentSettlementLine> ApPaymentSettlementLines = _db.ApPaymentSettlementLines.Where(x=>x.ApPaymentSettlementId==id);
            DataSourceResult result = ApPaymentSettlementLines.ToDataSourceResult(request, ApPaymentSettlementLine => new {
                ApPaymentSettlementLineId = ApPaymentSettlementLine.ApPaymentSettlementLineId,
                ApPaymentSettlementId = ApPaymentSettlementLine.ApPaymentSettlementId,
                ApDirectPaymentId = ApPaymentSettlementLine.ApDirectPaymentId,
                Purpose = ApPaymentSettlementLine.Purpose,
                Amount = ApPaymentSettlementLine.Amount,
                AccountCode = ApPaymentSettlementLine.AccountCode,
                Remark = ApPaymentSettlementLine.Remark,
                DateCreated = ApPaymentSettlementLine.DateCreated,
                LastModified = ApPaymentSettlementLine.LastModified,
                CreatedBy = ApPaymentSettlementLine.CreatedBy,
                ModifiedBy = ApPaymentSettlementLine.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlementLines_Create(int id,[DataSourceRequest]DataSourceRequest request, ApPaymentSettlementLine ApPaymentSettlementLine)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new ApPaymentSettlementLine
                {
                    ApPaymentSettlementId = id,
                    ApDirectPaymentId = ApPaymentSettlementLine.ApDirectPaymentId,
                    Purpose = ApPaymentSettlementLine.Purpose,
                    Amount = ApPaymentSettlementLine.Amount,
                    AccountCode = ApPaymentSettlementLine.AccountCode,
                    Remark = ApPaymentSettlementLine.Remark,
                    DateCreated = ApPaymentSettlementLine.DateCreated,
                    LastModified = ApPaymentSettlementLine.LastModified,
                    CreatedBy = ApPaymentSettlementLine.CreatedBy,
                    ModifiedBy = ApPaymentSettlementLine.ModifiedBy
                };

                _db.ApPaymentSettlementLines.Add(entity);
                _db.SaveChanges();
                ApPaymentSettlementLine.ApPaymentSettlementLineId = entity.ApPaymentSettlementLineId;
                #region update distribution
                _distributionHelper.UpdateApPaymentSettlementDistribution(id);
                #endregion
            }

            return Json(new[] { ApPaymentSettlementLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlementLines_Update([DataSourceRequest]DataSourceRequest request, ApPaymentSettlementLine ApPaymentSettlementLine)
        {
            
            if (ModelState.IsValid)
            {
                var entity = new ApPaymentSettlementLine
                {
                    ApPaymentSettlementId = ApPaymentSettlementLine.ApPaymentSettlementId,
                    ApPaymentSettlementLineId = ApPaymentSettlementLine.ApPaymentSettlementLineId,
                    ApDirectPaymentId = ApPaymentSettlementLine.ApDirectPaymentId,
                    Purpose = ApPaymentSettlementLine.Purpose,
                    Amount = ApPaymentSettlementLine.Amount,
                    AccountCode = ApPaymentSettlementLine.AccountCode,
                    Remark = ApPaymentSettlementLine.Remark,
                    DateCreated = ApPaymentSettlementLine.DateCreated,
                    LastModified = ApPaymentSettlementLine.LastModified,
                    CreatedBy = ApPaymentSettlementLine.CreatedBy,
                    ModifiedBy = ApPaymentSettlementLine.ModifiedBy
                };

                _db.ApPaymentSettlementLines.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region update distribution
                _distributionHelper.UpdateApPaymentSettlementDistribution(ApPaymentSettlementLine.ApPaymentSettlementId);
                #endregion
            }

            return Json(new[] { ApPaymentSettlementLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlementLines_Destroy([DataSourceRequest]DataSourceRequest request, ApPaymentSettlementLine ApPaymentSettlementLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApPaymentSettlementLine
                {
                    ApPaymentSettlementId = ApPaymentSettlementLine.ApPaymentSettlementId,
                    ApPaymentSettlementLineId = ApPaymentSettlementLine.ApPaymentSettlementLineId,
                    ApDirectPaymentId = ApPaymentSettlementLine.ApDirectPaymentId,
                    Purpose = ApPaymentSettlementLine.Purpose,
                    Amount = ApPaymentSettlementLine.Amount,
                    AccountCode = ApPaymentSettlementLine.AccountCode,
                    Remark = ApPaymentSettlementLine.Remark,
                    DateCreated = ApPaymentSettlementLine.DateCreated,
                    LastModified = ApPaymentSettlementLine.LastModified,
                    CreatedBy = ApPaymentSettlementLine.CreatedBy,
                    ModifiedBy = ApPaymentSettlementLine.ModifiedBy
                };

                _db.ApPaymentSettlementLines.Attach(entity);
                _db.ApPaymentSettlementLines.Remove(entity);
                _db.SaveChanges();
                #region update distribution
                _distributionHelper.UpdateApPaymentSettlementDistribution(ApPaymentSettlementLine.ApPaymentSettlementId);
                #endregion

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted payment settlement item " + ApPaymentSettlementLine.ApPaymentSettlementId + " " + ApPaymentSettlementLine.Purpose, Modules.Finance);
                #endregion
            }

            return Json(new[] { ApPaymentSettlementLine }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
