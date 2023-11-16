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
            DataSourceResult result = ApPaymentSettlementLines.ToDataSourceResult(request, apPaymentSettlementLine => new {
                ApPaymentSettlementLineId = apPaymentSettlementLine.ApPaymentSettlementLineId,
                ApPaymentSettlementId = apPaymentSettlementLine.ApPaymentSettlementId,
                Source = apPaymentSettlementLine.Source,
                SourceId = apPaymentSettlementLine.SourceId,
                Purpose = apPaymentSettlementLine.Purpose,
                Amount = apPaymentSettlementLine.Amount,
                NetAmount = apPaymentSettlementLine.NetAmount,
                AccountCode = apPaymentSettlementLine.AccountCode,

                GLTaxRateID = apPaymentSettlementLine.GLTaxRateID,
                TaxAmount = apPaymentSettlementLine.TaxAmount,
                WithholdTax = apPaymentSettlementLine.WithholdTax,
                WithholdTaxAmount = apPaymentSettlementLine.WithholdTaxAmount,
                WithholdingType = apPaymentSettlementLine.WithholdingType,
                WithholdingAmount = apPaymentSettlementLine.WithholdingAmount,

                Remark = apPaymentSettlementLine.Remark,
                DateCreated = apPaymentSettlementLine.DateCreated,
                LastModified = apPaymentSettlementLine.LastModified,
                CreatedBy = apPaymentSettlementLine.CreatedBy,
                ModifiedBy = apPaymentSettlementLine.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlementLines_Create(int id,[DataSourceRequest]DataSourceRequest request, ApPaymentSettlementLine apPaymentSettlementLine)
        {
            #region validation
            if(apPaymentSettlementLine.Source==SettlemetSource.NA.ToString())
            {
                if(apPaymentSettlementLine.Amount<=0)
                    ModelState.AddModelError("amount", "Please fill amount.");
                if (String.IsNullOrEmpty(apPaymentSettlementLine.AccountCode))
                    ModelState.AddModelError("account", "Please select account.");
            }

            if (apPaymentSettlementLine.Source != SettlemetSource.NA.ToString())
            {
                if (apPaymentSettlementLine.AccountCode == null)
                    ModelState.AddModelError("sourceid", "Please select source document.");
            }

            var exist = _db.ApPaymentSettlementLines.Where(x => x.Source !=SettlemetSource.NA.ToString())
                                                    .Where(x => x.Source == apPaymentSettlementLine.Source)
                                                    .Where(x => x.SourceId == apPaymentSettlementLine.SourceId).FirstOrDefault();
            if(exist!=null)
                ModelState.AddModelError("exist", $"The document number {apPaymentSettlementLine.SourceId} of type {apPaymentSettlementLine.Source} is already applied under settlment {id}");

            #endregion
            #region data
            if (apPaymentSettlementLine.Source == SettlemetSource.Prepayment.ToString() && apPaymentSettlementLine.AccountCode != null)
            {
                //get from related prepayment
                var prepayment = _db.APPayments.Find(apPaymentSettlementLine.SourceId);
                //apPaymentSettlementLine.Amount = prepayment.PrepaymentAmount;
                apPaymentSettlementLine.NetAmount = prepayment.NetAmount;
            }
            else if (apPaymentSettlementLine.Source == SettlemetSource.Payment.ToString() && apPaymentSettlementLine.AccountCode != null)
            {
                //get from related prepayment
                var prepayment = _db.APPayments.Find(apPaymentSettlementLine.SourceId);
                //apPaymentSettlementLine.Amount = prepayment.PaymentAmount;
                apPaymentSettlementLine.NetAmount = prepayment.NetAmount;

            }
            else if (apPaymentSettlementLine.Source == SettlemetSource.DirectPayment.ToString() && apPaymentSettlementLine.AccountCode != null)
            {
                //get from related prepayment
                var directLines = _db.ApDirectPaymentLines.Where(x => x.ApDirectPaymentLineId == apPaymentSettlementLine.SourceId).ToList();
                if (directLines.Any())
                    apPaymentSettlementLine.NetAmount = directLines.Sum(x => x.NetPayable);
                else
                    apPaymentSettlementLine.NetAmount = 0;
            }
            else if (apPaymentSettlementLine.Source == SettlemetSource.NA.ToString())
            {
                #region tax calc
                var tax = _db.GLTaxRates.Find(apPaymentSettlementLine.GLTaxRateID);
                if (tax != null)
                {
                    apPaymentSettlementLine.TaxAmount = apPaymentSettlementLine.Amount * tax.CalculatedRate;
                }
                #endregion
                #region withholding calc
                var withholding = _db.GLTaxRates.Find(apPaymentSettlementLine.WithholdingType);
                if (withholding != null)
                {
                    if (tax != null && tax.IsCost)
                    {
                        apPaymentSettlementLine.WithholdingAmount = (apPaymentSettlementLine.Amount + apPaymentSettlementLine.TaxAmount) * withholding.CalculatedRate;
                    }
                    else
                    {
                        apPaymentSettlementLine.WithholdingAmount = apPaymentSettlementLine.Amount * withholding.CalculatedRate;
                    }
                }
                #endregion
                #region withheld tax

                if (apPaymentSettlementLine.WithholdTax)
                {
                    //get the percentage
                    var varwitholding = _db.GLVatWithholdings.FirstOrDefault();
                    if (varwitholding != null)
                    {
                        apPaymentSettlementLine.WithholdTaxAmount = apPaymentSettlementLine.TaxAmount * (varwitholding.WithholdPercent / 100);
                    }
                    else
                    {
                        apPaymentSettlementLine.WithholdTaxAmount = apPaymentSettlementLine.TaxAmount;
                    }
                }
                else
                {
                    apPaymentSettlementLine.WithholdTaxAmount = 0;
                }
                #endregion
                #region net
                apPaymentSettlementLine.NetAmount = apPaymentSettlementLine.Amount
                                                        - apPaymentSettlementLine.WithholdingAmount
                                                        + apPaymentSettlementLine.TaxAmount
                                                        - apPaymentSettlementLine.WithholdTaxAmount;
                #endregion
            }

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ApPaymentSettlementLine
                {
                    ApPaymentSettlementId = id,
                    Source = apPaymentSettlementLine.Source,
                    SourceId = apPaymentSettlementLine.SourceId,
                    Purpose = apPaymentSettlementLine.Purpose,
                    Amount = apPaymentSettlementLine.Amount,
                    NetAmount = apPaymentSettlementLine.NetAmount,
                    AccountCode = apPaymentSettlementLine.AccountCode,

                    GLTaxRateID = apPaymentSettlementLine.GLTaxRateID,
                    TaxAmount = apPaymentSettlementLine.TaxAmount,
                    WithholdTax = apPaymentSettlementLine.WithholdTax,
                    WithholdTaxAmount = apPaymentSettlementLine.WithholdTaxAmount,
                    WithholdingType = apPaymentSettlementLine.WithholdingType,
                    WithholdingAmount = apPaymentSettlementLine.WithholdingAmount,

                    Remark = apPaymentSettlementLine.Remark,
                    DateCreated = apPaymentSettlementLine.DateCreated,
                    LastModified = apPaymentSettlementLine.LastModified,
                    CreatedBy = apPaymentSettlementLine.CreatedBy,
                    ModifiedBy = apPaymentSettlementLine.ModifiedBy
                };

                _db.ApPaymentSettlementLines.Add(entity);
                _db.SaveChanges();
                apPaymentSettlementLine.ApPaymentSettlementLineId = entity.ApPaymentSettlementLineId;
                #region update distribution
                _distributionHelper.UpdateApPaymentSettlementDistribution(id);
                #endregion
            }

            return Json(new[] { apPaymentSettlementLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlementLines_Update([DataSourceRequest]DataSourceRequest request, ApPaymentSettlementLine apPaymentSettlementLine)
        {
            #region validation
            if (apPaymentSettlementLine.Source == SettlemetSource.NA.ToString())
            {
                if (apPaymentSettlementLine.Amount <= 0)
                    ModelState.AddModelError("amount", "Please fill amount.");
                if (String.IsNullOrEmpty(apPaymentSettlementLine.AccountCode))
                    ModelState.AddModelError("account", "Please select account.");
            }

            if (apPaymentSettlementLine.Source != SettlemetSource.NA.ToString())
            {
                if (apPaymentSettlementLine.AccountCode == null)
                    ModelState.AddModelError("sourceid", "Please select source document.");
            }

            var exist = _db.ApPaymentSettlementLines.Where(x => x.Source != SettlemetSource.NA.ToString())
                                                    .Where(x => x.ApPaymentSettlementId != apPaymentSettlementLine.ApPaymentSettlementId)
                                                    .Where(x => x.Source == apPaymentSettlementLine.Source)
                                                    .Where(x => x.SourceId == apPaymentSettlementLine.SourceId).FirstOrDefault();
            if (exist != null)
                ModelState.AddModelError("exist", $"The document number {apPaymentSettlementLine.SourceId} of type {apPaymentSettlementLine.Source} is already applied under settlment {apPaymentSettlementLine.ApPaymentSettlementId}");

            #endregion
            #region data
            if (apPaymentSettlementLine.Source == SettlemetSource.Prepayment.ToString() && apPaymentSettlementLine.AccountCode != null)
            {
                //get from related prepayment
                var prepayment = _db.APPayments.Find(apPaymentSettlementLine.SourceId);
                //apPaymentSettlementLine.Amount = prepayment.PrepaymentAmount;
                apPaymentSettlementLine.NetAmount = prepayment.NetAmount;
            }
            else if (apPaymentSettlementLine.Source == SettlemetSource.Payment.ToString() && apPaymentSettlementLine.AccountCode != null)
            {
                //get from related prepayment
                var prepayment = _db.APPayments.Find(apPaymentSettlementLine.SourceId);
                //apPaymentSettlementLine.Amount = prepayment.PaymentAmount;
                apPaymentSettlementLine.NetAmount = prepayment.NetAmount;

            }
            else if (apPaymentSettlementLine.Source == SettlemetSource.DirectPayment.ToString() && apPaymentSettlementLine.AccountCode != null)
            {
                //get from related prepayment
                var directLines = _db.ApDirectPaymentLines.Where(x => x.ApDirectPaymentLineId == apPaymentSettlementLine.SourceId).ToList();
                if (directLines.Any())
                    apPaymentSettlementLine.NetAmount = directLines.Sum(x => x.NetPayable);
                else
                    apPaymentSettlementLine.NetAmount = 0;
            }
            else if (apPaymentSettlementLine.Source == SettlemetSource.NA.ToString())
            {
                #region tax calc
                var tax = _db.GLTaxRates.Find(apPaymentSettlementLine.GLTaxRateID);
                if (tax != null)
                {
                    apPaymentSettlementLine.TaxAmount = apPaymentSettlementLine.Amount * tax.CalculatedRate;
                }
                #endregion
                #region withholding calc
                var withholding = _db.GLTaxRates.Find(apPaymentSettlementLine.WithholdingType);
                if (withholding != null)
                {
                    if (tax != null && tax.IsCost)
                    {
                        apPaymentSettlementLine.WithholdingAmount = (apPaymentSettlementLine.Amount + apPaymentSettlementLine.TaxAmount) * withholding.CalculatedRate;
                    }
                    else
                    {
                        apPaymentSettlementLine.WithholdingAmount = apPaymentSettlementLine.Amount * withholding.CalculatedRate;
                    }
                }
                #endregion
                #region withheld tax

                if (apPaymentSettlementLine.WithholdTax)
                {
                    //get the percentage
                    var varwitholding = _db.GLVatWithholdings.FirstOrDefault();
                    if (varwitholding != null)
                    {
                        apPaymentSettlementLine.WithholdTaxAmount = apPaymentSettlementLine.TaxAmount * (varwitholding.WithholdPercent / 100);
                    }
                    else
                    {
                        apPaymentSettlementLine.WithholdTaxAmount = apPaymentSettlementLine.TaxAmount;
                    }
                }
                else
                {
                    apPaymentSettlementLine.WithholdTaxAmount = 0;
                }
                #endregion
                #region net
                apPaymentSettlementLine.NetAmount = apPaymentSettlementLine.Amount
                                                        - apPaymentSettlementLine.WithholdingAmount
                                                        + apPaymentSettlementLine.TaxAmount
                                                        - apPaymentSettlementLine.WithholdTaxAmount;
                #endregion
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ApPaymentSettlementLine
                {
                    ApPaymentSettlementId = apPaymentSettlementLine.ApPaymentSettlementId,
                    ApPaymentSettlementLineId = apPaymentSettlementLine.ApPaymentSettlementLineId,
                    Source = apPaymentSettlementLine.Source,
                    SourceId = apPaymentSettlementLine.SourceId,
                    Purpose = apPaymentSettlementLine.Purpose,
                    Amount = apPaymentSettlementLine.Amount,
                    NetAmount = apPaymentSettlementLine.NetAmount,
                    AccountCode = apPaymentSettlementLine.AccountCode,

                    GLTaxRateID = apPaymentSettlementLine.GLTaxRateID,
                    TaxAmount = apPaymentSettlementLine.TaxAmount,
                    WithholdTax = apPaymentSettlementLine.WithholdTax,
                    WithholdTaxAmount = apPaymentSettlementLine.WithholdTaxAmount,
                    WithholdingType = apPaymentSettlementLine.WithholdingType,
                    WithholdingAmount = apPaymentSettlementLine.WithholdingAmount,

                    Remark = apPaymentSettlementLine.Remark,
                    DateCreated = apPaymentSettlementLine.DateCreated,
                    LastModified = apPaymentSettlementLine.LastModified,
                    CreatedBy = apPaymentSettlementLine.CreatedBy,
                    ModifiedBy = apPaymentSettlementLine.ModifiedBy
                };

                _db.ApPaymentSettlementLines.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region update distribution
                _distributionHelper.UpdateApPaymentSettlementDistribution(apPaymentSettlementLine.ApPaymentSettlementId);
                #endregion
            }

            return Json(new[] { apPaymentSettlementLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlementLines_Destroy([DataSourceRequest]DataSourceRequest request, ApPaymentSettlementLine apPaymentSettlementLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApPaymentSettlementLine
                {
                    ApPaymentSettlementId = apPaymentSettlementLine.ApPaymentSettlementId,
                    ApPaymentSettlementLineId = apPaymentSettlementLine.ApPaymentSettlementLineId,
                    Source = apPaymentSettlementLine.Source,
                    SourceId = apPaymentSettlementLine.SourceId,
                    Purpose = apPaymentSettlementLine.Purpose,
                    Amount = apPaymentSettlementLine.Amount,
                    NetAmount = apPaymentSettlementLine.NetAmount,
                    AccountCode = apPaymentSettlementLine.AccountCode,

                    GLTaxRateID = apPaymentSettlementLine.GLTaxRateID,
                    TaxAmount = apPaymentSettlementLine.TaxAmount,
                    WithholdTax = apPaymentSettlementLine.WithholdTax,
                    WithholdTaxAmount = apPaymentSettlementLine.WithholdTaxAmount,
                    WithholdingType = apPaymentSettlementLine.WithholdingType,
                    WithholdingAmount = apPaymentSettlementLine.WithholdingAmount,

                    Remark = apPaymentSettlementLine.Remark,
                    DateCreated = apPaymentSettlementLine.DateCreated,
                    LastModified = apPaymentSettlementLine.LastModified,
                    CreatedBy = apPaymentSettlementLine.CreatedBy,
                    ModifiedBy = apPaymentSettlementLine.ModifiedBy
                };

                _db.ApPaymentSettlementLines.Attach(entity);
                _db.ApPaymentSettlementLines.Remove(entity);
                _db.SaveChanges();
                #region update distribution
                _distributionHelper.UpdateApPaymentSettlementDistribution(apPaymentSettlementLine.ApPaymentSettlementId);
                #endregion

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted payment settlement item " + apPaymentSettlementLine.ApPaymentSettlementId + " " + apPaymentSettlementLine.Purpose, Modules.Finance);
                #endregion
            }

            return Json(new[] { apPaymentSettlementLine }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
