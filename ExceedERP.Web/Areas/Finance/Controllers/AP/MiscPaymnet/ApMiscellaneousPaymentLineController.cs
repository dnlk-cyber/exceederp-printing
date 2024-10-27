using System;
using System.Data.Entity;
using System.Linq;
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
    public class ApMiscellaneousPaymentLineController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApMiscellaneousPaymentLines_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApMiscellaneousPaymentLine> apmiscellaneouspaymentlines = _db.ApMiscellaneousPaymentLines.Where(x=>x.ApMiscellaneousPaymentId==id);
            DataSourceResult result = apmiscellaneouspaymentlines.ToDataSourceResult(request, apMiscellaneousPaymentLine => new {
                ApMiscellaneousPaymentId = apMiscellaneousPaymentLine.ApMiscellaneousPaymentId,
                ApMiscellaneousPaymentLineId = apMiscellaneousPaymentLine.ApMiscellaneousPaymentLineId,
                Purpose = apMiscellaneousPaymentLine.Purpose,
                Amount = apMiscellaneousPaymentLine.Amount,
                AccountCode = apMiscellaneousPaymentLine.AccountCode,
                GLTaxRateID = apMiscellaneousPaymentLine.GLTaxRateID,
                TaxAmount = apMiscellaneousPaymentLine.TaxAmount,
                WithholdingType = apMiscellaneousPaymentLine.WithholdingType,
                WithholdingAmount = apMiscellaneousPaymentLine.WithholdingAmount,
                WithholdTax = apMiscellaneousPaymentLine.WithholdTax,
                WithholdTaxAmount = apMiscellaneousPaymentLine.WithholdTaxAmount,
                TotalAmount = apMiscellaneousPaymentLine.TotalAmount,
                NetPayable = apMiscellaneousPaymentLine.NetPayable,
                DeductAmount = apMiscellaneousPaymentLine.DeductAmount,
                Remark = apMiscellaneousPaymentLine.Remark,
                DateCreated = apMiscellaneousPaymentLine.DateCreated,
                LastModified = apMiscellaneousPaymentLine.LastModified,
                CreatedBy = apMiscellaneousPaymentLine.CreatedBy,
                ModifiedBy = apMiscellaneousPaymentLine.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApMiscellaneousPaymentLines_Create(int id,[DataSourceRequest]DataSourceRequest request, ApMiscellaneousPaymentLine apMiscellaneousPaymentLine)
        {
            #region tax calc
            var tax = _db.GLTaxRates.Find(apMiscellaneousPaymentLine.GLTaxRateID);
            if (tax != null)
            {
                apMiscellaneousPaymentLine.TaxAmount = apMiscellaneousPaymentLine.Amount * tax.CalculatedRate;
                apMiscellaneousPaymentLine.TotalAmount =apMiscellaneousPaymentLine.Amount + apMiscellaneousPaymentLine.TaxAmount;
            }
            #endregion
            #region withholding calc
            var withholding = _db.GLTaxRates.Find(apMiscellaneousPaymentLine.WithholdingType);
            if (withholding != null)
            {
                if (tax != null && tax.IsCost)
                {
                    apMiscellaneousPaymentLine.WithholdingAmount = (apMiscellaneousPaymentLine.Amount+ apMiscellaneousPaymentLine.TaxAmount) * withholding.CalculatedRate;
                }
                else
                {
                    apMiscellaneousPaymentLine.WithholdingAmount = apMiscellaneousPaymentLine.Amount * withholding.CalculatedRate;
                }
            }
            #endregion
            #region withheld tax

            if (apMiscellaneousPaymentLine.WithholdTax)
            {
                //get the percentage
                var varwitholding = _db.GLVatWithholdings.FirstOrDefault();
                if (varwitholding != null)
                {
                    apMiscellaneousPaymentLine.WithholdTaxAmount = apMiscellaneousPaymentLine.TaxAmount * (varwitholding.WithholdPercent / 100);
                }
                else
                {
                    apMiscellaneousPaymentLine.WithholdTaxAmount = apMiscellaneousPaymentLine.TaxAmount;
                }
            }
            else
            {
                apMiscellaneousPaymentLine.WithholdTaxAmount = 0;
            }
            #endregion
            #region net
            apMiscellaneousPaymentLine.NetPayable = apMiscellaneousPaymentLine.Amount
                                                 - apMiscellaneousPaymentLine.WithholdingAmount
                                                 + apMiscellaneousPaymentLine.TaxAmount
                                                 - apMiscellaneousPaymentLine.WithholdTaxAmount 
                                                 - apMiscellaneousPaymentLine.DeductAmount;
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ApMiscellaneousPaymentLine
                {
                    ApMiscellaneousPaymentId = id,
                    Purpose = apMiscellaneousPaymentLine.Purpose,
                    Amount = apMiscellaneousPaymentLine.Amount,
                    AccountCode = apMiscellaneousPaymentLine.AccountCode,
                    GLTaxRateID = apMiscellaneousPaymentLine.GLTaxRateID,
                    TaxAmount = apMiscellaneousPaymentLine.TaxAmount,
                    WithholdingType = apMiscellaneousPaymentLine.WithholdingType,
                    WithholdingAmount = apMiscellaneousPaymentLine.WithholdingAmount,
                    WithholdTax = apMiscellaneousPaymentLine.WithholdTax,
                    WithholdTaxAmount = apMiscellaneousPaymentLine.WithholdTaxAmount,
                    TotalAmount = apMiscellaneousPaymentLine.TotalAmount,
                    NetPayable = apMiscellaneousPaymentLine.NetPayable,
                    DeductAmount = apMiscellaneousPaymentLine.DeductAmount,
                    Remark = apMiscellaneousPaymentLine.Remark,
                    DateCreated = apMiscellaneousPaymentLine.DateCreated,
                    LastModified = apMiscellaneousPaymentLine.LastModified,
                    CreatedBy = apMiscellaneousPaymentLine.CreatedBy,
                    ModifiedBy = apMiscellaneousPaymentLine.ModifiedBy
                };

                _db.ApMiscellaneousPaymentLines.Add(entity);
                _db.SaveChanges();
                apMiscellaneousPaymentLine.ApMiscellaneousPaymentLineId = entity.ApMiscellaneousPaymentLineId;
                #region update distribution
                _distributionHelper.UpdateApMiscPaymentDistribution(id);
                UpdatePaymentBalances(id);
                #endregion
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP misc payment item", "Success", User.Identity.Name, $"created misc payment item {entity.AccountCode} under record {entity.ApMiscellaneousPaymentId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apMiscellaneousPaymentLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApMiscellaneousPaymentLines_Update([DataSourceRequest]DataSourceRequest request, ApMiscellaneousPaymentLine apMiscellaneousPaymentLine)
        {
            #region tax calc
            var tax = _db.GLTaxRates.Find(apMiscellaneousPaymentLine.GLTaxRateID);
            if (tax != null)
            {
                apMiscellaneousPaymentLine.TaxAmount = apMiscellaneousPaymentLine.Amount * tax.CalculatedRate;
                apMiscellaneousPaymentLine.TotalAmount =
                    apMiscellaneousPaymentLine.Amount + apMiscellaneousPaymentLine.TaxAmount;
            }
            #endregion
            #region withholding calc
            var withholding = _db.GLTaxRates.Find(apMiscellaneousPaymentLine.WithholdingType);
            if (withholding != null)
            {
                if (tax != null && tax.IsCost)
                {
                    apMiscellaneousPaymentLine.WithholdingAmount = (apMiscellaneousPaymentLine.Amount + apMiscellaneousPaymentLine.TaxAmount) * withholding.CalculatedRate;
                }
                else
                {
                    apMiscellaneousPaymentLine.WithholdingAmount = apMiscellaneousPaymentLine.Amount * withholding.CalculatedRate;
                }
            }
            #endregion
            #region withheld tax

            if (apMiscellaneousPaymentLine.WithholdTax)
            {
                //get the percentage
                var varwitholding = _db.GLVatWithholdings.FirstOrDefault();
                if (varwitholding != null)
                {
                    apMiscellaneousPaymentLine.WithholdTaxAmount = apMiscellaneousPaymentLine.TaxAmount * (varwitholding.WithholdPercent / 100);
                }
                else
                {
                    apMiscellaneousPaymentLine.WithholdTaxAmount = apMiscellaneousPaymentLine.TaxAmount;
                }
            }
            else
            {
                apMiscellaneousPaymentLine.WithholdTaxAmount = 0;
            }
            #endregion
            #region net
            apMiscellaneousPaymentLine.NetPayable = apMiscellaneousPaymentLine.Amount
                                                    - apMiscellaneousPaymentLine.WithholdingAmount
                                                    + apMiscellaneousPaymentLine.TaxAmount
                                                    - apMiscellaneousPaymentLine.WithholdTaxAmount
                                                    -apMiscellaneousPaymentLine.DeductAmount;
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ApMiscellaneousPaymentLine
                {
                    ApMiscellaneousPaymentId = apMiscellaneousPaymentLine.ApMiscellaneousPaymentId,
                    ApMiscellaneousPaymentLineId = apMiscellaneousPaymentLine.ApMiscellaneousPaymentLineId,
                    Purpose = apMiscellaneousPaymentLine.Purpose,
                    Amount = apMiscellaneousPaymentLine.Amount,
                    AccountCode = apMiscellaneousPaymentLine.AccountCode,
                    GLTaxRateID = apMiscellaneousPaymentLine.GLTaxRateID,
                    TaxAmount = apMiscellaneousPaymentLine.TaxAmount,
                    WithholdingType = apMiscellaneousPaymentLine.WithholdingType,
                    WithholdingAmount = apMiscellaneousPaymentLine.WithholdingAmount,
                    WithholdTax = apMiscellaneousPaymentLine.WithholdTax,
                    WithholdTaxAmount = apMiscellaneousPaymentLine.WithholdTaxAmount,
                    TotalAmount = apMiscellaneousPaymentLine.TotalAmount,
                    NetPayable = apMiscellaneousPaymentLine.NetPayable,
                    DeductAmount = apMiscellaneousPaymentLine.DeductAmount,
                    Remark = apMiscellaneousPaymentLine.Remark,
                    DateCreated = apMiscellaneousPaymentLine.DateCreated,
                    LastModified = apMiscellaneousPaymentLine.LastModified,
                    CreatedBy = apMiscellaneousPaymentLine.CreatedBy,
                    ModifiedBy = apMiscellaneousPaymentLine.ModifiedBy
                };

                _db.ApMiscellaneousPaymentLines.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region update distribution
                _distributionHelper.UpdateApMiscPaymentDistribution(apMiscellaneousPaymentLine.ApMiscellaneousPaymentId);
                UpdatePaymentBalances(apMiscellaneousPaymentLine.ApMiscellaneousPaymentId);
                #endregion
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP misc payment item", "Success", User.Identity.Name, $"updated misc payment item {entity.AccountCode} under record {entity.ApMiscellaneousPaymentId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apMiscellaneousPaymentLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApMiscellaneousPaymentLines_Destroy([DataSourceRequest]DataSourceRequest request, ApMiscellaneousPaymentLine apMiscellaneousPaymentLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApMiscellaneousPaymentLine
                {
                    ApMiscellaneousPaymentId = apMiscellaneousPaymentLine.ApMiscellaneousPaymentId,
                    ApMiscellaneousPaymentLineId = apMiscellaneousPaymentLine.ApMiscellaneousPaymentLineId,
                    Purpose = apMiscellaneousPaymentLine.Purpose,
                    Amount = apMiscellaneousPaymentLine.Amount,
                    AccountCode = apMiscellaneousPaymentLine.AccountCode,
                    GLTaxRateID = apMiscellaneousPaymentLine.GLTaxRateID,
                    TaxAmount = apMiscellaneousPaymentLine.TaxAmount,
                    WithholdingType = apMiscellaneousPaymentLine.WithholdingType,
                    WithholdingAmount = apMiscellaneousPaymentLine.WithholdingAmount,
                    WithholdTax = apMiscellaneousPaymentLine.WithholdTax,
                    WithholdTaxAmount = apMiscellaneousPaymentLine.WithholdTaxAmount,
                    TotalAmount = apMiscellaneousPaymentLine.TotalAmount,
                    NetPayable = apMiscellaneousPaymentLine.NetPayable,
                    DeductAmount = apMiscellaneousPaymentLine.DeductAmount,
                    Remark = apMiscellaneousPaymentLine.Remark,
                    DateCreated = apMiscellaneousPaymentLine.DateCreated,
                    LastModified = apMiscellaneousPaymentLine.LastModified,
                    CreatedBy = apMiscellaneousPaymentLine.CreatedBy,
                    ModifiedBy = apMiscellaneousPaymentLine.ModifiedBy
                };

                _db.ApMiscellaneousPaymentLines.Attach(entity);
                _db.ApMiscellaneousPaymentLines.Remove(entity);
                _db.SaveChanges();
                #region update distribution
                _distributionHelper.UpdateApMiscPaymentDistribution(apMiscellaneousPaymentLine.ApMiscellaneousPaymentId);
                UpdatePaymentBalances(apMiscellaneousPaymentLine.ApMiscellaneousPaymentId);

                #endregion
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted misc. payment item " + apMiscellaneousPaymentLine.ApMiscellaneousPaymentId + " " + apMiscellaneousPaymentLine.AccountCode, Modules.Finance);
                #endregion
            }

            return Json(new[] { apMiscellaneousPaymentLine }.ToDataSourceResult(request, ModelState));
        }

        private void UpdatePaymentBalances(int? id)
        {
            var payment = _db.APMiscellaneousPayments.Find(id);
            if (payment != null)
            {
                #region update parent balances
                var lines = _db.ApMiscellaneousPaymentLines.Where(x => x.ApMiscellaneousPaymentId == id).ToList();
                payment.LineAmount = Math.Round(lines.Sum(x => x.Amount), 2);
                payment.TaxAmount = Math.Round(lines.Sum(x => x.TaxAmount), 2);
                payment.TotalAmount = Math.Round(lines.Sum(x => x.TotalAmount), 2);
                payment.NetPayable = Math.Round(lines.Sum(x => x.NetPayable), 2);
                payment.DeductAmount = Math.Round(lines.Sum(x => x.DeductAmount), 2);
                payment.WithholdingAmount = Math.Round(lines.Sum(x => x.WithholdingAmount), 2);
                payment.WithholdTaxAmount = Math.Round(lines.Sum(x => x.WithholdTaxAmount), 2);

                _db.Entry(payment).State = EntityState.Modified;
                _db.SaveChanges();
                #endregion

            }

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
