using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.Data.Entity;
using ExceedERP.Helpers.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CreditSettlementApprover, CRMRoles.CreditSettlementUser)]
    public class LineCreditSettlementController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        private ExceedDbContext db = new ExceedDbContext();
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
                Status = lineItem.Status,

                CreatedBy = lineItem.CreatedBy,
                DateCreated = lineItem.DateCreated,
                LastModified = lineItem.LastModified,
                ModifiedBy = lineItem.ModifiedBy,
                OrgBranchName = lineItem.OrgBranchName,
                Remark = lineItem.Remark,


            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItems_Create([DataSourceRequest]DataSourceRequest request, LineCreditSettlement lineItem, int voucherID)
        {
            CRMPaymentDistibutionHelper CRMPaymentDistibutionHelper = new CRMPaymentDistibutionHelper();
            db = new ExceedDbContext();

            decimal CurrentAmount = decimal.Zero;
            decimal lineAmount = 0;
            decimal settlementAmount = 0;

            var lines = db.LineItems.Where(x => x.Voucher == lineItem.InvoiceId.ToString() && x.VoucherDefinitionID == "2").ToList();
            var settelmentline = db.LineCreditSettlements.Where(x => x.InvoiceId == lineItem.InvoiceId).ToList();
            if (lines.Any())
                lineAmount = lines.Sum(x => x.TotalAmount);
            if (settelmentline.Any())
                settlementAmount = settelmentline.Sum(x => x.SettelmentAmount);

            CurrentAmount = lineAmount - settlementAmount;
            //List<LineCreditSettlement> lit = db.LineCreditSettlements.Where(li => li.InvoiceId == lineItem.InvoiceId  ).ToList();


            //if (lit.Any())
            //{
            //    ModelState.AddModelError("InvoiceExist", " Invoice Already Exist ! ");

            //}
            //else

            if (lineItem.SettelmentAmount > CurrentAmount)
            {
                ModelState.AddModelError("AmountExceed", " Amount Exceed Required ! ");
            }
            else
            {
                var entity = new LineCreditSettlement
                {
                    CreditSettlementId = voucherID,
                    InvoiceId = lineItem.InvoiceId,
                    CurrentAmmount = CurrentAmount.ToString(),
                    RemainingAmount = CurrentAmount - lineItem.SettelmentAmount,
                    SettelmentAmount = lineItem.SettelmentAmount,
                    WithholdingType = lineItem.WithholdingType,
                    WithholdingAmount = lineItem.WithholdingAmount,
                    NetBalance = lineItem.NetBalance,
                    Remark = lineItem.Remark,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,

                };
                entity.WithholdingAmount = this.CalculateWithholdingAmount(entity.WithholdingType, entity.SettelmentAmount);
                entity.NetBalance = entity.SettelmentAmount - entity.WithholdingAmount;
                try
                {
                    db.LineCreditSettlements.Add(entity);
                    int save = db.SaveChanges();
                    //if (save == 1)
                    //{

                    //    // lineItem = entity;
                    //    var LineCredit = db.CreditSettlement.Where(x => x.CreditSettlementId == entity.CreditSettlementId).FirstOrDefault();
                    //    var PaymentMethod = db.PaymentMethods.Where(x => x.PaymentMethodID == LineCredit.RecieveMethod).FirstOrDefault();
                    //    var AccountMap = db.AccountMaps.ToList();
                    //    var SalesInvoicePaymentDistibutions = CRMPaymentDistibutionHelper.DistributeCreditSettelment(entity, PaymentMethod, LineCredit.Customer, AccountMap);

                    //    if (SalesInvoicePaymentDistibutions.Any())
                    //    {
                    //        db.SalesInvoicePaymentDistibutions.AddRange(SalesInvoicePaymentDistibutions);
                    //        db.SaveChanges();
                    //    }
                    //}
                    UpdateCreditSalesInvoiceOnCreditSettlementLineChange(entity.LineCreditSettlementId);

                    var creditSettlement = GetSettlementHeader(entity.CreditSettlementId);
                    if (creditSettlement != null)
                    {
                        new CRMPaymentDistibutionHelper().UpdateCreditSettlementDistribution(creditSettlement);
                    }
                }
                catch (Exception e)
                {
                    return Json(new[] { lineItem }.ToDataSourceResult(request));

                }
            }
            return Json(new[] { lineItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItems_Update([DataSourceRequest]DataSourceRequest request, LineCreditSettlement lineItem, int voucherID)
        {
            db = new ExceedDbContext();

            //decimal CurrentAmount = decimal.Zero;

            //var lines =
            //    db.LineItems.Where(
            //        x => x.Voucher == lineItem.InvoiceId.ToString() && x.VoucherDefinitionID == "2").ToList();

            //if (lines.Any())
            //{
            //    CurrentAmount = lines.Sum(x => x.TotalAmount);
            //}

            //List<LineCreditSettlement> lit = db.LineCreditSettlements.Where(li => li.InvoiceId == lineItem.InvoiceId && li.CreditSettlementId == voucherID && li.LineCreditSettlementId != lineItem.LineCreditSettlementId).ToList();

            //if (lit.Any())
            //{
            //    ModelState.AddModelError("InvoiceExist", " Invoice Already Exist ! ");

            //}
            if (lineItem.SettelmentAmount > decimal.Parse(lineItem.CurrentAmmount))
            {
                ModelState.AddModelError("AmountExceed", "Settlement Amount cannot exceed Current Amount! ");

            }

            else
            {
                var entity = new LineCreditSettlement
                {
                    LineCreditSettlementId = lineItem.LineCreditSettlementId,
                    CreditSettlementId = voucherID,
                    InvoiceId = lineItem.InvoiceId,
                    //CurrentAmmount = CurrentAmount.ToString(),
                    //RemainingAmount = CurrentAmount - lineItem.SettelmentAmount,
                    CurrentAmmount = lineItem.CurrentAmmount,
                    RemainingAmount = decimal.Parse(lineItem.CurrentAmmount) - lineItem.SettelmentAmount,
                    SettelmentAmount = lineItem.SettelmentAmount,
                    WithholdingType = lineItem.WithholdingType,
                    WithholdingAmount = lineItem.WithholdingAmount,
                    NetBalance = lineItem.NetBalance,

                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,

                };
                entity.WithholdingAmount = this.CalculateWithholdingAmount(entity.WithholdingType, entity.SettelmentAmount);
                entity.NetBalance = entity.SettelmentAmount - entity.WithholdingAmount;
                try
                {
                    db.LineCreditSettlements.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    UpdateCreditSalesInvoiceOnCreditSettlementLineChange(entity.LineCreditSettlementId);

                    var creditSettlement = GetSettlementHeader(entity.CreditSettlementId);
                    if (creditSettlement != null)
                    {
                        new CRMPaymentDistibutionHelper().UpdateCreditSettlementDistribution(creditSettlement);
                    }
                }
                catch (Exception e)
                {

                    String err = e.InnerException.Data.Values.ToString();
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

        private void UpdateCreditSalesInvoiceOnCreditSettlementLineChange(int lineItemId)
        {
            var currentSettlementLine = db.LineCreditSettlements.Find(lineItemId);
            if (currentSettlementLine != null)
            {
                var referedInvoice = db.CashSalesInvoices.FirstOrDefault(i => i.CashSalesInvoiceID == currentSettlementLine.InvoiceId);
                if (referedInvoice != null)
                {
                    var allLineSettlement = db.LineCreditSettlements.Where(l => l.InvoiceId == referedInvoice.CashSalesInvoiceID).ToList();
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
        }
        private CreditSettlement GetSettlementHeader(int id)
        {
            return db.CreditSettlement.Find(id);
        }
    }

}