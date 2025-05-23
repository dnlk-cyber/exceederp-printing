using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.CRM;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using static ExceedERP.Core.Domain.CRM.CRMEnums;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CreditSettlementApprover, CRMRoles.CreditSettlementUser, FinanceConstants.GeneralSubLedgerTransfer)]
    public class LineCreditSettlementController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult LineItems_Read([DataSourceRequest]DataSourceRequest request, int voucherID)
        {
            CRMPaymentDistibutionHelper CRMPaymentDistibutionHelper = new CRMPaymentDistibutionHelper();
            List<LineCreditSettlement> LineCreditSettlements = db.LineCreditSettlements.Where(li => li.CreditSettlementId == voucherID).ToList();
            DataSourceResult result = LineCreditSettlements.ToDataSourceResult(request, lineItem => new
            {
                LineCreditSettlementId = lineItem.LineCreditSettlementId,
                InvoiceId = lineItem.InvoiceId,
                CurrentAmmount = lineItem.CurrentAmmount,
                RemainingAmount = lineItem.RemainingAmount,
                SettelmentAmount = lineItem.SettelmentAmount,
                CreditSettlementId = lineItem.CreditSettlementId,
                WithholdingType = lineItem.WithholdingType,
                WithholdingAmount = lineItem.WithholdingAmount,
                NetBalance = lineItem.NetBalance,
                //Source = lineItem.Source,
                //AdvancePaymentReference = lineItem.AdvancePaymentReference,

                CreatedBy = lineItem.CreatedBy,
                DateCreated = lineItem.DateCreated,
                LastModified = lineItem.LastModified,
                ModifiedBy = lineItem.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItems_Create([DataSourceRequest]DataSourceRequest request, LineCreditSettlement lineItem, int voucherID)
        {
            CRMPaymentDistibutionHelper CRMPaymentDistibutionHelper = new CRMPaymentDistibutionHelper();
            //if(lineItem.Source == SettlementAmountSource.AdvancePayment && lineItem.AdvancePaymentReference == null)
            //{
            //    ModelState.AddModelError("AdvancePaymentReference", "Please provide advance payment reference.");
            //}
            decimal CurrentAmount = decimal.Zero;
            decimal lineAmount = 0;
            decimal settlementAmount = 0;

            var lines = db.LineItems
                .AsNoTracking()
                .Where(x => x.Voucher == lineItem.InvoiceId.ToString() && x.VoucherDefinitionID == "2")
                .ToList();
            var settelmentline = db.LineCreditSettlements
                .AsNoTracking()
                .Where(x => x.InvoiceId == lineItem.InvoiceId)
                .ToList();
            if (lines.Any())
                lineAmount = lines.Sum(x => x.TotalAmount);
            if (settelmentline.Any())
                settlementAmount = settelmentline.Sum(x => x.SettelmentAmount);

            CurrentAmount = lineAmount - settlementAmount;

            if (lineItem.SettelmentAmount > CurrentAmount)
            {
                ModelState.AddModelError("AmountExceed", " Total settlement amount exceeds total credit amount");
            }
            if (ModelState.IsValid)
            {
                var entity = new LineCreditSettlement
                {
                    CreditSettlementId = voucherID,
                    InvoiceId = lineItem.InvoiceId,
                    //CurrentAmmount = CurrentAmount,
                    RemainingAmount = CurrentAmount - lineItem.SettelmentAmount,
                    SettelmentAmount = lineItem.SettelmentAmount,
                    WithholdingType = lineItem.WithholdingType,
                    WithholdingAmount = lineItem.WithholdingAmount,
                    NetBalance = lineItem.NetBalance,
                   // Source = lineItem.Source,
                   // AdvancePaymentReference = lineItem.AdvancePaymentReference,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,

                };
                entity.WithholdingAmount = this.CalculateWithholdingAmount(entity.WithholdingType, entity.SettelmentAmount);
                entity.NetBalance = entity.SettelmentAmount - entity.WithholdingAmount;

                db.LineCreditSettlements.Add(entity);
                int save = db.SaveChanges();

                UpdateCreditSalesInvoiceOnCreditSettlementLineChange(entity.InvoiceId);
               // CRMSalesHelper.UpdateAdvancePaymentOnInvoiceChange(entity.AdvancePaymentReference?.ToString());

                var creditSettlement = GetSettlementHeader(entity.CreditSettlementId);
                if (creditSettlement != null)
                {
                    UpdateHeaderOnLineChange(creditSettlement);
                    new CRMPaymentDistibutionHelper().UpdateCreditSettlementDistribution(creditSettlement);
                }
            }
            return Json(new[] { lineItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItems_Update([DataSourceRequest]DataSourceRequest request, LineCreditSettlement lineItem, int voucherID)
        {
        //    if (lineItem.SettelmentAmount > lineItem.CurrentAmmount)
        //    {
        //        ModelState.AddModelError("AmountExceed", " Total settlement amount exceeds total credit amount");
        //    }
            if (ModelState.IsValid)
            {
                var entity = new LineCreditSettlement
                {
                    LineCreditSettlementId = lineItem.LineCreditSettlementId,
                    CreditSettlementId = voucherID,
                    InvoiceId = lineItem.InvoiceId,
                    CurrentAmmount = lineItem.CurrentAmmount,
                   // RemainingAmount = lineItem.CurrentAmmount - lineItem.SettelmentAmount,
                    SettelmentAmount = lineItem.SettelmentAmount,
                    WithholdingType = lineItem.WithholdingType,
                    WithholdingAmount = lineItem.WithholdingAmount,
                    NetBalance = lineItem.NetBalance,
                   // Source = lineItem.Source,
                   // AdvancePaymentReference = lineItem.AdvancePaymentReference,

                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                };
                entity.WithholdingAmount = this.CalculateWithholdingAmount(entity.WithholdingType, entity.SettelmentAmount);
                entity.NetBalance = entity.SettelmentAmount - entity.WithholdingAmount;

                db.LineCreditSettlements.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                UpdateCreditSalesInvoiceOnCreditSettlementLineChange(entity.InvoiceId);
               // CRMSalesHelper.UpdateAdvancePaymentOnInvoiceChange(entity.AdvancePaymentReference?.ToString());

                var creditSettlement = GetSettlementHeader(entity.CreditSettlementId);
                if (creditSettlement != null)
                {
                    UpdateHeaderOnLineChange(creditSettlement);
                    new CRMPaymentDistibutionHelper().UpdateCreditSettlementDistribution(creditSettlement);
                }
            }

            return Json(new[] { lineItem }.ToDataSourceResult(request, ModelState));
        }

        private decimal CalculateWithholdingAmount(int id, decimal amnt)
        {
            decimal withholdingAmount = 0;
            var taxRate = db.GLTaxRates.Find(id);
            if (taxRate != null)
            {
                withholdingAmount = taxRate.CalculatedRate * amnt;
            }
            return withholdingAmount;
        }
        private void UpdateHeaderOnLineChange(CreditSettlement settlement)
        {
            var lines = db.LineCreditSettlements
                .AsNoTracking()
                .Where(l => l.CreditSettlementId == settlement.CreditSettlementId)
                .ToList();
            decimal totalSettlement = lines.Sum(l => l.SettelmentAmount);

            settlement.SettlementAmount = totalSettlement;

            db.CreditSettlement.Attach(settlement);
            db.Entry(settlement).State = EntityState.Modified;
            db.SaveChanges();
        }
        
        private void UpdateCreditSalesInvoiceOnCreditSettlementLineChange(int invoiceId)
        {
            var referedInvoice = db.CashSalesInvoices
            .AsNoTracking()
            .FirstOrDefault(i => i.CashSalesInvoiceID == invoiceId);
            if (referedInvoice != null)
            {
                var allLineSettlement = db.LineCreditSettlements
                .AsNoTracking()
                .Where(l => l.InvoiceId == referedInvoice.CashSalesInvoiceID)
                .ToList();

                if (allLineSettlement.Any())
                {
                    var totalSettlement = allLineSettlement.Sum(l => l.SettelmentAmount);
                    referedInvoice.AmountDue = referedInvoice.GrandTotal - totalSettlement;

                    db.CashSalesInvoices.Attach(referedInvoice);
                    db.Entry(referedInvoice).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public static void UpdateAdvancePaymentOnInvoiceChange(string advancePaymentReference)
        {
            var db = new ExceedDbContext();
            int.TryParse(advancePaymentReference, out int adId);
            var advancePayment = db.AdvancePayments.Find(adId);
            if (advancePayment != null)
            {
                //decimal totalUsedInCashInvoice = db.CashSalesInvoices
                //    .AsNoTracking()
                //    .Where(i => i.AdvancePaymentReference == advancePaymentReference)
                //    .Sum(i => (decimal?)i.AdvancePayment) ?? 0;
                //decimal totalUsedInCreditSettlement = db.LineCreditSettlements
                //    .AsNoTracking()
                //    .Where(i => i.AdvancePaymentReference == adId)
                //    .Sum(i => (decimal?)i.SettelmentAmount) ?? 0;

                //advancePayment.UsedAmount = totalUsedInCashInvoice + totalUsedInCreditSettlement;
                //advancePayment.Outstanding = advancePayment.GrandTotal - advancePayment.UsedAmount;

                db.Entry(advancePayment).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        private CreditSettlement GetSettlementHeader(int id)
        {
            return db.CreditSettlement.Find(id);
        }
    }

}