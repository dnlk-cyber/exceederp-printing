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
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
    public class ApDirectPaymentLineController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApDirectPaymentLines_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApDirectPaymentLine> apDirectpaymentlines = _db.ApDirectPaymentLines.Where(x=>x.ApDirectPaymentId==id);
            DataSourceResult result = apDirectpaymentlines.ToDataSourceResult(request, apDirectPaymentLine => new {
                ApDirectPaymentLineId = apDirectPaymentLine.ApDirectPaymentLineId,
                ApDirectPaymentId = apDirectPaymentLine.ApDirectPaymentId,
                Purpose = apDirectPaymentLine.Purpose,
                Amount = apDirectPaymentLine.Amount,
                ForexAmount = apDirectPaymentLine.ForexAmount,
                ExchangeRate = apDirectPaymentLine.ExchangeRate,
                Currency = apDirectPaymentLine.Currency,
                AccountCode = apDirectPaymentLine.AccountCode,
                GLTaxRateID = apDirectPaymentLine.GLTaxRateID,
                TaxAmount = apDirectPaymentLine.TaxAmount,
                WithholdingType = apDirectPaymentLine.WithholdingType,
                WithholdingAmount = apDirectPaymentLine.WithholdingAmount,
                WithholdTax = apDirectPaymentLine.WithholdTax,
                WithholdTaxAmount = apDirectPaymentLine.WithholdTaxAmount,
                TotalAmount = apDirectPaymentLine.TotalAmount,
                NetPayable = apDirectPaymentLine.NetPayable,
                DeductAmount = apDirectPaymentLine.DeductAmount,
                Remark = apDirectPaymentLine.Remark,
                DateCreated = apDirectPaymentLine.DateCreated,
                LastModified = apDirectPaymentLine.LastModified,
                CreatedBy = apDirectPaymentLine.CreatedBy,
                ModifiedBy = apDirectPaymentLine.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApDirectPaymentLines_Create(int id,[DataSourceRequest]DataSourceRequest request, ApDirectPaymentLine apDirectPaymentLine)
        {
            //apDirectPaymentLine.Amount = apDirectPaymentLine.ForexAmount * apDirectPaymentLine.ExchangeRate;
            #region tax calc
            var tax = _db.GLTaxRates.Find(apDirectPaymentLine.GLTaxRateID);
            if (tax != null)
            {
                apDirectPaymentLine.TaxAmount = apDirectPaymentLine.Amount * tax.CalculatedRate;
                apDirectPaymentLine.TotalAmount =apDirectPaymentLine.Amount + apDirectPaymentLine.TaxAmount;
            }
            #endregion
            #region withholding calc
            var withholding = _db.GLTaxRates.Find(apDirectPaymentLine.WithholdingType);
            if (withholding != null)
                apDirectPaymentLine.WithholdingAmount = apDirectPaymentLine.Amount * withholding.CalculatedRate;
            #endregion
            #region withheld tax

            if (apDirectPaymentLine.WithholdTax)
            {
                //get the percentage
                var varwitholding = _db.GLVatWithholdings.FirstOrDefault();
                if (varwitholding != null)
                {
                    apDirectPaymentLine.WithholdTaxAmount = apDirectPaymentLine.TaxAmount * (varwitholding.WithholdPercent / 100);
                }
                else
                {
                    apDirectPaymentLine.WithholdTaxAmount = apDirectPaymentLine.TaxAmount;
                }
            }
            else
            {
                apDirectPaymentLine.WithholdTaxAmount = 0;
            }
            #endregion
            #region net
            apDirectPaymentLine.NetPayable = apDirectPaymentLine.Amount
                                                 - apDirectPaymentLine.WithholdingAmount
                                                 + apDirectPaymentLine.TaxAmount
                                                 - apDirectPaymentLine.WithholdTaxAmount
                                                 - apDirectPaymentLine.DeductAmount;
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ApDirectPaymentLine
                {
                    ApDirectPaymentId = id,
                    Purpose = apDirectPaymentLine.Purpose,
                    Amount = apDirectPaymentLine.ForexAmount * apDirectPaymentLine.ExchangeRate,
                    ForexAmount = apDirectPaymentLine.ForexAmount,
                    ExchangeRate = apDirectPaymentLine.ExchangeRate,
                    Currency = apDirectPaymentLine.Currency,
                    AccountCode = apDirectPaymentLine.AccountCode,
                    GLTaxRateID = apDirectPaymentLine.GLTaxRateID,
                    TaxAmount = apDirectPaymentLine.TaxAmount,
                    WithholdingType = apDirectPaymentLine.WithholdingType,
                    WithholdingAmount = apDirectPaymentLine.WithholdingAmount,
                    WithholdTax = apDirectPaymentLine.WithholdTax,
                    WithholdTaxAmount = apDirectPaymentLine.WithholdTaxAmount,
                    TotalAmount = apDirectPaymentLine.TotalAmount,
                    NetPayable = apDirectPaymentLine.NetPayable,
                    Remark = apDirectPaymentLine.Remark,
                    DeductAmount = apDirectPaymentLine.DeductAmount,
                    DateCreated = apDirectPaymentLine.DateCreated,
                    LastModified = apDirectPaymentLine.LastModified,
                    CreatedBy = apDirectPaymentLine.CreatedBy,
                    ModifiedBy = apDirectPaymentLine.ModifiedBy
                };

                _db.ApDirectPaymentLines.Add(entity);
                _db.SaveChanges();
                apDirectPaymentLine.ApDirectPaymentLineId = entity.ApDirectPaymentLineId;
                #region update distribution
                _distributionHelper.UpdateApDirectPaymentDistribution(id);
                #endregion
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP direct payment item", "Success", User.Identity.Name, $"created direct payment item {entity.AccountCode} under record {entity.ApDirectPaymentId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apDirectPaymentLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApDirectPaymentLines_Update([DataSourceRequest]DataSourceRequest request, ApDirectPaymentLine apDirectPaymentLine)
        {
           // apDirectPaymentLine.Amount = apDirectPaymentLine.ForexAmount * apDirectPaymentLine.ExchangeRate;
            #region tax calc
            var tax = _db.GLTaxRates.Find(apDirectPaymentLine.GLTaxRateID);
            if (tax != null)
            {
                apDirectPaymentLine.TaxAmount = apDirectPaymentLine.Amount * tax.CalculatedRate;
                apDirectPaymentLine.TotalAmount =
                    apDirectPaymentLine.Amount + apDirectPaymentLine.TaxAmount;
            }
            #endregion
            #region withholding calc
            var withholding = _db.GLTaxRates.Find(apDirectPaymentLine.WithholdingType);
            if (withholding != null)
                apDirectPaymentLine.WithholdingAmount = apDirectPaymentLine.Amount * withholding.CalculatedRate;
            #endregion
            #region withheld tax

            if (apDirectPaymentLine.WithholdTax)
            {
                //get the percentage
                var varwitholding = _db.GLVatWithholdings.FirstOrDefault();
                if (varwitholding != null)
                {
                    apDirectPaymentLine.WithholdTaxAmount = apDirectPaymentLine.TaxAmount * (varwitholding.WithholdPercent / 100);
                }
                else
                {
                    apDirectPaymentLine.WithholdTaxAmount = apDirectPaymentLine.TaxAmount;
                }
            }
            else
            {
                apDirectPaymentLine.WithholdTaxAmount = 0;
            }
            #endregion
            #region net
            apDirectPaymentLine.NetPayable = apDirectPaymentLine.Amount
                                                    - apDirectPaymentLine.WithholdingAmount
                                                    + apDirectPaymentLine.TaxAmount
                                                    - apDirectPaymentLine.WithholdTaxAmount
                                                    - apDirectPaymentLine.DeductAmount; 
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ApDirectPaymentLine
                {
                    ApDirectPaymentId = apDirectPaymentLine.ApDirectPaymentId,
                    ApDirectPaymentLineId = apDirectPaymentLine.ApDirectPaymentLineId,
                    Purpose = apDirectPaymentLine.Purpose,
                    Amount = apDirectPaymentLine.ForexAmount * apDirectPaymentLine.ExchangeRate,
                    ForexAmount = apDirectPaymentLine.ForexAmount,
                    ExchangeRate = apDirectPaymentLine.ExchangeRate,
                    Currency = apDirectPaymentLine.Currency,
                    AccountCode = apDirectPaymentLine.AccountCode,
                    GLTaxRateID = apDirectPaymentLine.GLTaxRateID,
                    TaxAmount = apDirectPaymentLine.TaxAmount,
                    WithholdingType = apDirectPaymentLine.WithholdingType,
                    WithholdingAmount = apDirectPaymentLine.WithholdingAmount,
                    WithholdTax = apDirectPaymentLine.WithholdTax,
                    WithholdTaxAmount = apDirectPaymentLine.WithholdTaxAmount,
                    TotalAmount = apDirectPaymentLine.TotalAmount,
                    NetPayable = apDirectPaymentLine.NetPayable,
                    Remark = apDirectPaymentLine.Remark,
                    DeductAmount = apDirectPaymentLine.DeductAmount,
                    DateCreated = apDirectPaymentLine.DateCreated,
                    LastModified = apDirectPaymentLine.LastModified,
                    CreatedBy = apDirectPaymentLine.CreatedBy,
                    ModifiedBy = apDirectPaymentLine.ModifiedBy
                };

                _db.ApDirectPaymentLines.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region update distribution
                _distributionHelper.UpdateApDirectPaymentDistribution(apDirectPaymentLine.ApDirectPaymentId);
                #endregion
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP direct payment item", "Success", User.Identity.Name, $"updated direct payment item {entity.AccountCode} under record {entity.ApDirectPaymentId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apDirectPaymentLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApDirectPaymentLines_Destroy([DataSourceRequest]DataSourceRequest request, ApDirectPaymentLine apDirectPaymentLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApDirectPaymentLine
                {
                    ApDirectPaymentLineId = apDirectPaymentLine.ApDirectPaymentLineId,
                    Purpose = apDirectPaymentLine.Purpose,
                    Amount = apDirectPaymentLine.Amount,
                    ForexAmount = apDirectPaymentLine.ForexAmount,
                    ExchangeRate = apDirectPaymentLine.ExchangeRate,
                    Currency = apDirectPaymentLine.Currency,
                    AccountCode = apDirectPaymentLine.AccountCode,
                    GLTaxRateID = apDirectPaymentLine.GLTaxRateID,
                    TaxAmount = apDirectPaymentLine.TaxAmount,
                    WithholdingType = apDirectPaymentLine.WithholdingType,
                    WithholdingAmount = apDirectPaymentLine.WithholdingAmount,
                    WithholdTax = apDirectPaymentLine.WithholdTax,
                    WithholdTaxAmount = apDirectPaymentLine.WithholdTaxAmount,
                    TotalAmount = apDirectPaymentLine.TotalAmount,
                    NetPayable = apDirectPaymentLine.NetPayable,
                    Remark = apDirectPaymentLine.Remark,
                    DateCreated = apDirectPaymentLine.DateCreated,
                    LastModified = apDirectPaymentLine.LastModified,
                    CreatedBy = apDirectPaymentLine.CreatedBy,
                    ModifiedBy = apDirectPaymentLine.ModifiedBy
                };

                _db.ApDirectPaymentLines.Attach(entity);
                _db.ApDirectPaymentLines.Remove(entity);
                _db.SaveChanges();
                #region update distribution
                _distributionHelper.UpdateApDirectPaymentDistribution(apDirectPaymentLine.ApDirectPaymentId);
                #endregion
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted direct payment item " + apDirectPaymentLine.ApDirectPaymentId + " " + apDirectPaymentLine.AccountCode, Modules.Finance);
                #endregion
            }

            return Json(new[] { apDirectPaymentLine }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
