using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    public class StandardCollectionDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult CollectionLines_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ConsARStandardCollectionDetail> collectionDetails = db.ConsARStandardCollectionDetail.Where(detail => detail.ConsARStandardCollectionId == id);
            DataSourceResult result = collectionDetails.ToDataSourceResult(request, collectionDetail => new
            {
                ConsARStandardCollectionDetailId = collectionDetail.ConsARStandardCollectionDetailId,
                ConsARStandardCollectionId = collectionDetail.ConsARStandardCollectionId,
                InvoiceNumber = collectionDetail.InvoiceNumber,
                ApplyAmount = collectionDetail.ApplyAmount,
                BalanceDue = collectionDetail.BalanceDue,
                AdvanceAmount = collectionDetail.AdvanceAmount,
                Retention = collectionDetail.Retention,
                GLTaxRateID = collectionDetail.GLTaxRateID,
                TaxAmount = collectionDetail.TaxAmount,
                VatWithholding = collectionDetail.VatWithholding,
                WithholdingTaxRateId = collectionDetail.WithholdingTaxRateId,
                WithholdingTaxAmount = collectionDetail.WithholdingTaxAmount,
                Remark = collectionDetail.Remark,


                OrgBranchName = collectionDetail.OrgBranchName,
                DateCreated = collectionDetail.DateCreated,
                LastModified = collectionDetail.LastModified,
                CreatedBy = collectionDetail.CreatedBy,
                ModifiedBy = collectionDetail.ModifiedBy
            });

            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CollectionLine_Create([DataSourceRequest]DataSourceRequest request, ConsARStandardCollectionDetail collectionDetail, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsARStandardCollectionDetail
                {
                    ConsARStandardCollectionId = id,
                    InvoiceNumber = collectionDetail.InvoiceNumber,
                    ApplyAmount = collectionDetail.ApplyAmount,
                    BalanceDue = collectionDetail.BalanceDue,
                    AdvanceAmount = collectionDetail.AdvanceAmount,
                    Retention = collectionDetail.Retention,
                    GLTaxRateID = collectionDetail.GLTaxRateID,
                    TaxAmount = collectionDetail.TaxAmount,
                    VatWithholding = collectionDetail.VatWithholding,
                    WithholdingTaxRateId = collectionDetail.WithholdingTaxRateId,
                    WithholdingTaxAmount = collectionDetail.WithholdingTaxAmount,
                    Remark = collectionDetail.Remark,

                    OrgBranchName = collectionDetail.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = collectionDetail.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = collectionDetail.ModifiedBy
                };
                entity.TaxAmount = BillingHelper.CalculateTaxAmount(entity.GLTaxRateID, entity.ConsARStandardCollectionId, entity);
                entity.WithholdingTaxAmount = BillingHelper.CalculateWithholdingTaxAmount(entity.WithholdingTaxRateId, entity.ConsARStandardCollectionId, entity);
                entity.BalanceDue = this.CalculateBalanceDue(entity.ConsARStandardCollectionId, entity);

                //var decuctiveSum = entity.Penality + entity.AdvanceRepayment + entity.Retention +
                //                   entity.PreviousPayment;
                //entity.SubTotal = entity.Quantity * entity.UnitPrice;
                //entity.TaxAmount = BillingHelper.CalculateTaxAmount(entity.GLTaxRateID, entity.SubTotal);
                //entity.TotalAmount = entity.TaxAmount + entity.SubTotal;

                //entity.NetAmount = entity.TotalAmount - decuctiveSum;

                db.ConsARStandardCollectionDetail.Add(entity);
                db.SaveChanges();
                collectionDetail.ConsARStandardCollectionDetailId = entity.ConsARStandardCollectionDetailId;

                collectionDetail.WithholdingTaxAmount = entity.WithholdingTaxAmount;
                //collectionDetail.TaxAmount = entity.TaxAmount;
                //collectionDetail.TotalAmount = entity.TotalAmount;

                this.UpdateInvoiceStatusBasedOnCollection(id, entity);

                this.UpdateOutstandingAndPrepaymentBalances(entity.ConsARStandardCollectionId);

                new ConstructionDistributionHelper().UpdateStandardCollectionDistribution(this.GetCurrentStandarCollection(entity.ConsARStandardCollectionId));

                //}
            }

            return Json(new[] { collectionDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CollectionLine_Update([DataSourceRequest]DataSourceRequest request, ConsARStandardCollectionDetail collectionDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsARStandardCollectionDetail
                {
                    ConsARStandardCollectionDetailId = collectionDetail.ConsARStandardCollectionDetailId,
                    ConsARStandardCollectionId = collectionDetail.ConsARStandardCollectionId,
                    InvoiceNumber = collectionDetail.InvoiceNumber,
                    ApplyAmount = collectionDetail.ApplyAmount,
                    BalanceDue = collectionDetail.BalanceDue,
                    AdvanceAmount = collectionDetail.AdvanceAmount,
                    Retention = collectionDetail.Retention,
                    GLTaxRateID = collectionDetail.GLTaxRateID,
                    TaxAmount = collectionDetail.TaxAmount,
                    VatWithholding = collectionDetail.VatWithholding,
                    WithholdingTaxRateId = collectionDetail.WithholdingTaxRateId,
                    WithholdingTaxAmount = collectionDetail.WithholdingTaxAmount,
                    Remark = collectionDetail.Remark,

                    OrgBranchName = collectionDetail.OrgBranchName,
                    DateCreated = collectionDetail.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = collectionDetail.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.TaxAmount = BillingHelper.CalculateTaxAmount(entity.GLTaxRateID, entity.ConsARStandardCollectionId, entity);
                entity.WithholdingTaxAmount = BillingHelper.CalculateWithholdingTaxAmount(entity.WithholdingTaxRateId, entity.ConsARStandardCollectionId, entity);
                entity.BalanceDue = this.CalculateBalanceDue(entity.ConsARStandardCollectionId, entity);
                //var decuctiveSum = entity.Penality + entity.AdvanceRepayment + entity.Retention +
                //                   entity.PreviousPayment;
                //entity.SubTotal = entity.Quantity * entity.UnitPrice;
                //entity.TaxAmount = BillingHelper.CalculateTaxAmount(entity.GLTaxRateID, entity.SubTotal);
                //entity.TotalAmount = entity.TaxAmount + entity.SubTotal;

                //entity.NetAmount = entity.TotalAmount - decuctiveSum;

                db.ConsARStandardCollectionDetail.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                this.UpdateInvoiceStatusBasedOnCollection(entity.ConsARStandardCollectionId, entity);

                this.UpdateOutstandingAndPrepaymentBalances(entity.ConsARStandardCollectionId);

                new ConstructionDistributionHelper().UpdateStandardCollectionDistribution(this.GetCurrentStandarCollection(entity.ConsARStandardCollectionId));

            }

            return Json(new[] { collectionDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CollectionLine_Destroy([DataSourceRequest]DataSourceRequest request, ConsARStandardCollectionDetail collectionDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsARStandardCollectionDetail
                {
                    ConsARStandardCollectionDetailId = collectionDetail.ConsARStandardCollectionDetailId,
                    ConsARStandardCollectionId = collectionDetail.ConsARStandardCollectionId,
                    InvoiceNumber = collectionDetail.InvoiceNumber,
                    ApplyAmount = collectionDetail.ApplyAmount,
                    BalanceDue = collectionDetail.BalanceDue,
                    AdvanceAmount = collectionDetail.AdvanceAmount,
                    Retention = collectionDetail.Retention,
                    GLTaxRateID = collectionDetail.GLTaxRateID,
                    TaxAmount = collectionDetail.TaxAmount,
                    VatWithholding = collectionDetail.VatWithholding,
                    WithholdingTaxRateId = collectionDetail.WithholdingTaxRateId,
                    WithholdingTaxAmount = collectionDetail.WithholdingTaxAmount,
                    Remark = collectionDetail.Remark,

                    OrgBranchName = collectionDetail.OrgBranchName,
                    DateCreated = collectionDetail.DateCreated,
                    LastModified = collectionDetail.LastModified,
                    CreatedBy = collectionDetail.CreatedBy,
                    ModifiedBy = collectionDetail.ModifiedBy
                };

                db.ConsARStandardCollectionDetail.Attach(entity);
                db.ConsARStandardCollectionDetail.Remove(entity);
                db.SaveChanges();

                this.UpdateOutstandingAndPrepaymentBalances(entity.ConsARStandardCollectionId);

                new ConstructionDistributionHelper().UpdateStandardCollectionDistribution(this.GetCurrentStandarCollection(entity.ConsARStandardCollectionId));

            }

            return Json(new[] { collectionDetail }.ToDataSourceResult(request, ModelState));
        }

        private decimal CalculateBalanceDue(int collectionId, ConsARStandardCollectionDetail model)
        {
            var balanceDue = model.BalanceDue;
            var currentCollection = db.ConsARStandardCollections.Find(collectionId);
            if (currentCollection?.ReceiptType == StandardCollectionReceiptType.Invoice)
            {
                var totalAmntFromInvoice = BillingHelper.GetApplyAmountFromInvoice(model.InvoiceNumber);

                balanceDue = totalAmntFromInvoice - model.ApplyAmount;
            }
            return balanceDue;
        }

        private void UpdateInvoiceStatusBasedOnCollection(int collectionId, ConsARStandardCollectionDetail model)
        {
            var currentCollection = db.ConsARStandardCollections.Find(collectionId);
            if (currentCollection?.ReceiptType == StandardCollectionReceiptType.Invoice)
            {
                var currentInvoice =
                    db.ConsReceivableInvoices.FirstOrDefault(inv => inv.InvoiceNumber == model.InvoiceNumber);
                if (currentInvoice != null)
                {
                    currentInvoice.CollectedAmount = currentInvoice.CollectedAmount + model.ApplyAmount;
                    currentInvoice.ConstructionInvoiceStatus = ConstructionInvoiceStatus.PartiallyCollected;

                    // if all the amount in the invoice is collected ... 
                    // change the status to copletly collected
                    if (currentInvoice.CollectedAmount >= currentInvoice.TotalAmount)
                    {
                        currentInvoice.CollectedAmount = currentInvoice.TotalAmount;
                        currentInvoice.ConstructionInvoiceStatus = ConstructionInvoiceStatus.Collected;
                    }

                    db.ConsReceivableInvoices.Attach(currentInvoice);
                    db.Entry(currentInvoice).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        private void UpdateOutstandingAndPrepaymentBalances(int collectionId)
        {
            var netPrepaymentAmnt = new decimal();
            var outstandingBalance = new decimal();

            var currentCollectionHeader = db.ConsARStandardCollections.Find(collectionId);
            if (currentCollectionHeader != null)
            {
                var lineItems = db.ConsARStandardCollectionDetail
                    .Where(d => d.ConsARStandardCollectionId == currentCollectionHeader.ConsARStandardCollectionId)
                    .ToList();
                if (lineItems.Any())
                {
                   
                    if (currentCollectionHeader.ReceiptType == StandardCollectionReceiptType.Prepayment)
                    {
                        //netPrepaymentAmnt =
                        //    lineItems.Sum(itm => (itm.AdvanceAmount + itm.TaxAmount) - itm.WithholdingTax -
                        //                         itm.Retention);
                        foreach (var item in lineItems)
                        {
                            // there is no VAT withholding .... 
                            if (!item.VatWithholding)
                            {
                                netPrepaymentAmnt += (item.AdvanceAmount + item.TaxAmount) - item.WithholdingTaxAmount -
                                                     item.Retention;
                            }
                            // VAT withhold
                            else
                            {
                                netPrepaymentAmnt += item.AdvanceAmount - item.WithholdingTaxAmount - item.Retention;
                            }
                        }

                        //outstandingBalance = lineItems.Sum();


                    }
                    // its from invoice
                    else
                    {
                        //netPrepaymentAmnt = lineItems.Sum(itm => itm.AdvanceAmount);

                        outstandingBalance =
                            lineItems.Sum(itm => (itm.ApplyAmount + itm.TaxAmount) -
                                                 (itm.Retention + itm.AdvanceAmount + itm.WithholdingTaxAmount));

                    }
                }
                currentCollectionHeader.PrepaymentBalance = netPrepaymentAmnt;
                currentCollectionHeader.OutstandingBalance = outstandingBalance;

                db.ConsARStandardCollections.Attach(currentCollectionHeader);
                db.Entry(currentCollectionHeader).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        private ConsARStandardCollection GetCurrentStandarCollection(int id)
        {
            var collection = new ConsARStandardCollection();
            var temp = db.ConsARStandardCollections.Find(id);
            if (temp != null)
            {
                collection = temp;
            }
            return collection;
        }

        public decimal GetTaxAmountFromInvoice(string invoiceNumber, int taxId)
        {
            var taxFromInvoice = new decimal();
            //var collectedTaxAmounFromInvoice = new decimal();
            var currentInvoice = db.ConsReceivableInvoices.FirstOrDefault(inv => inv.InvoiceNumber == invoiceNumber);
            if (currentInvoice != null)
            {
                var invoiceItems = db.ConsReceivableInvoiceDetails
                    .Where(invItem => invItem.ConsReceivableInvoiceId == currentInvoice.ConsReceivableInvoiceId &&
                                      invItem.GLTaxRateID == taxId).ToList();
                if (invoiceItems.Any())
                {
                    taxFromInvoice = invoiceItems.Sum(inv => inv.TaxAmount);
                }

                var collectionItems = db.ConsARStandardCollectionDetail
                    .Where(cd => cd.InvoiceNumber == currentInvoice.InvoiceNumber).ToList();
                if (collectionItems.Any())
                {
                    taxFromInvoice = taxFromInvoice - collectionItems.Sum(it => it.TaxAmount);
                }

            }
            return taxFromInvoice;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}