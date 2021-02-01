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
    public class ReceivableInvoiceDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult InvoiceLine_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ConsReceivableInvoiceDetail> invoiceDetails = db.ConsReceivableInvoiceDetails.Where(iDetail => iDetail.ConsReceivableInvoiceId == id);
            DataSourceResult result = invoiceDetails.ToDataSourceResult(request, invoiceDetail => new
            {
                ConsReceivableInvoiceDetailId = invoiceDetail.ConsReceivableInvoiceDetailId,
                ConsReceivableInvoiceId = invoiceDetail.ConsReceivableInvoiceId,
                Service = invoiceDetail.Service,
                WorkItemPerProjectId = invoiceDetail.WorkItemPerProjectId,
                ProjectPaymentCertificateId = invoiceDetail.ProjectPaymentCertificateId,
                Description = invoiceDetail.Description,
                AccountCode = invoiceDetail.AccountCode,
                Quantity = invoiceDetail.Quantity,
                UnitPrice = invoiceDetail.UnitPrice,
                SubTotal = invoiceDetail.SubTotal,
                GLTaxRateID = invoiceDetail.GLTaxRateID,
                TaxAmount = invoiceDetail.TaxAmount,
                TotalAmount = invoiceDetail.TotalAmount,
                Penality = invoiceDetail.Penality,
                Retention = invoiceDetail.Retention,
                PreviousPayment = invoiceDetail.PreviousPayment,
                AdvanceRepayment = invoiceDetail.AdvanceRepayment,
                Correction = invoiceDetail.Correction,
                Others = invoiceDetail.Others,
                NetAmount = invoiceDetail.NetAmount,

                OrgBranchName = invoiceDetail.OrgBranchName,
                DateCreated = invoiceDetail.DateCreated,
                LastModified = invoiceDetail.LastModified,
                CreatedBy = invoiceDetail.CreatedBy,
                ModifiedBy = invoiceDetail.ModifiedBy
            });

            return Json(result);
        }

        //private bool CheckExistance(int projectId)
        //{
        //    bool paymentExists = false;
        //    var invoiceDetail = db.ProjectAdvancePayments.Where(ap => ap.ProjectInformationId == projectId).FirstOrDefault();
        //    if (invoiceDetail != null)
        //    {
        //        paymentExists = true;
        //    }
        //    return paymentExists;
        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InvoiceLine_Create([DataSourceRequest]DataSourceRequest request, ConsReceivableInvoiceDetail invoiceDetail, int id)
        {
            if (ModelState.IsValid)
            {
                //if (CheckExistance(projectIdCreateAdvance))
                //{
                //    ModelState.AddModelError("", "Advance Payment Exists for this project!!");
                //}
                //else
                //{
                var currentInvoiceHeader = db.ConsReceivableInvoices.Find(id);
                var paymentCertificate = db.PaymentCertificates.Find(invoiceDetail.ProjectPaymentCertificateId);
                if(currentInvoiceHeader != null && currentInvoiceHeader.Source == ConstructionInvoiceSource.WIP && paymentCertificate != null)
                {
                    var entity12 = new ConsReceivableInvoiceDetail
                    {
                        ConsReceivableInvoiceDetailId = invoiceDetail.ConsReceivableInvoiceDetailId,
                        ConsReceivableInvoiceId = id,
                        WorkItemPerProjectId = invoiceDetail.WorkItemPerProjectId,
                        ProjectPaymentCertificateId = invoiceDetail.ProjectPaymentCertificateId,
                        Service = invoiceDetail.Service,
                        Description = invoiceDetail.Description,
                        AccountCode = invoiceDetail.AccountCode,
                        Quantity = invoiceDetail.Quantity,
                        //UnitPrice = ProjectEstimationHelper.GetUnitPriceOfProjectWorkItem(invoiceDetail.WorkItemPerProjectId),
                        UnitPrice = invoiceDetail.UnitPrice,
                        SubTotal = paymentCertificate.ValueOfWorkExecutedToDate,
                        GLTaxRateID = paymentCertificate.GLTaxRateID,
                        TaxAmount = paymentCertificate.TaxAmount,
                        TotalAmount = paymentCertificate.TotalWithVat,
                        Penality = paymentCertificate.Penality,
                        Retention = paymentCertificate.Retention,
                        PreviousPayment = paymentCertificate.PreviousPayment,
                        Correction = paymentCertificate.Correction,
                        Others = paymentCertificate.OtherDeductions,
                        MaterialOnSite = paymentCertificate.MaterialOnSite,
                        AdvanceRepayment = paymentCertificate.AdvanceRepayment,
                        NetAmount = paymentCertificate.NetPayableAmount,

                        OrgBranchName = invoiceDetail.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = invoiceDetail.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = invoiceDetail.ModifiedBy
                    };
                    var decuctiveSum12 = entity12.Penality + entity12.AdvanceRepayment + entity12.Retention +
                                       entity12.PreviousPayment + entity12.Correction + entity12.Others;
                    //entity12.SubTotal = entity12.Quantity * entity12.UnitPrice;
                    //entity12.TaxAmount = BillingHelper.CalculateTaxAmount(entity12.GLTaxRateID, entity12.SubTotal);
                    //entity12.TotalAmount = entity12.TaxAmount + entity12.SubTotal;

                    //entity12.NetAmount = entity12.TotalAmount - decuctiveSum12;

                    db.ConsReceivableInvoiceDetails.Add(entity12);
                    db.SaveChanges();
                    invoiceDetail.ConsReceivableInvoiceDetailId = entity12.ConsReceivableInvoiceDetailId;

                    invoiceDetail.SubTotal = entity12.SubTotal;
                    invoiceDetail.TaxAmount = entity12.TaxAmount;
                    invoiceDetail.TotalAmount = entity12.TotalAmount;

                    this.UpdateInvoiceAfterAddingItem(id);

                    new ConstructionDistributionHelper().UpdateReceivableInvoice(GetCurrentReceivableInvoice(entity12.ConsReceivableInvoiceId));

                }
                else
                {
                    var entity = new ConsReceivableInvoiceDetail
                    {
                        ConsReceivableInvoiceDetailId = invoiceDetail.ConsReceivableInvoiceDetailId,
                        ConsReceivableInvoiceId = id,
                        WorkItemPerProjectId = invoiceDetail.WorkItemPerProjectId,
                        ProjectPaymentCertificateId = invoiceDetail.ProjectPaymentCertificateId,
                        Service = invoiceDetail.Service,
                        Description = invoiceDetail.Description,
                        AccountCode = invoiceDetail.AccountCode,
                        Quantity = invoiceDetail.Quantity,
                        UnitPrice = ProjectEstimationHelper.GetUnitPriceOfProjectWorkItem(invoiceDetail.WorkItemPerProjectId),
                        SubTotal = invoiceDetail.SubTotal,
                        GLTaxRateID = invoiceDetail.GLTaxRateID,
                        TaxAmount = invoiceDetail.TaxAmount,
                        TotalAmount = invoiceDetail.TotalAmount,
                        Penality = invoiceDetail.Penality,
                        Retention = invoiceDetail.Retention,
                        PreviousPayment = invoiceDetail.PreviousPayment,
                        AdvanceRepayment = invoiceDetail.AdvanceRepayment,
                        Correction = invoiceDetail.Correction,
                        Others = invoiceDetail.Others,
                        NetAmount = invoiceDetail.NetAmount,

                        OrgBranchName = invoiceDetail.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = invoiceDetail.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = invoiceDetail.ModifiedBy
                    };
                    var decuctiveSum = entity.Penality + entity.AdvanceRepayment + entity.Retention +
                                       entity.PreviousPayment;
                    entity.SubTotal = entity.Quantity * entity.UnitPrice;
                    entity.TaxAmount = BillingHelper.CalculateTaxAmount(entity.GLTaxRateID, entity.SubTotal);
                    entity.TotalAmount = entity.TaxAmount + entity.SubTotal;

                    entity.NetAmount = entity.TotalAmount - decuctiveSum;

                    db.ConsReceivableInvoiceDetails.Add(entity);
                    db.SaveChanges();
                    invoiceDetail.ConsReceivableInvoiceDetailId = entity.ConsReceivableInvoiceDetailId;

                    invoiceDetail.SubTotal = entity.SubTotal;
                    invoiceDetail.TaxAmount = entity.TaxAmount;
                    invoiceDetail.TotalAmount = entity.TotalAmount;

                    this.UpdateInvoiceAfterAddingItem(id);

                    new ConstructionDistributionHelper().UpdateReceivableInvoice(GetCurrentReceivableInvoice(entity.ConsReceivableInvoiceId));
                }

            }

            return Json(new[] { invoiceDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InvoiceLine_Update([DataSourceRequest]DataSourceRequest request, ConsReceivableInvoiceDetail invoiceDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsReceivableInvoiceDetail
                {
                    ConsReceivableInvoiceDetailId = invoiceDetail.ConsReceivableInvoiceDetailId,
                    ConsReceivableInvoiceId = invoiceDetail.ConsReceivableInvoiceId,
                    WorkItemPerProjectId = invoiceDetail.WorkItemPerProjectId,
                    ProjectPaymentCertificateId = invoiceDetail.ProjectPaymentCertificateId,
                    Service = invoiceDetail.Service,
                    Description = invoiceDetail.Description,
                    AccountCode = invoiceDetail.AccountCode,
                    Quantity = invoiceDetail.Quantity,
                    UnitPrice = ProjectEstimationHelper.GetUnitPriceOfProjectWorkItem(invoiceDetail.WorkItemPerProjectId),
                    SubTotal = invoiceDetail.SubTotal,
                    GLTaxRateID = invoiceDetail.GLTaxRateID,
                    TaxAmount = invoiceDetail.TaxAmount,
                    TotalAmount = invoiceDetail.TotalAmount,
                    Penality = invoiceDetail.Penality,
                    Retention = invoiceDetail.Retention,
                    PreviousPayment = invoiceDetail.PreviousPayment,
                    AdvanceRepayment = invoiceDetail.AdvanceRepayment,
                    Correction = invoiceDetail.Correction,
                    Others = invoiceDetail.Others,
                    NetAmount = invoiceDetail.NetAmount,

                    OrgBranchName = invoiceDetail.OrgBranchName,
                    DateCreated = invoiceDetail.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = invoiceDetail.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                var decuctiveSum = entity.Penality + entity.AdvanceRepayment + entity.Retention +
                                   entity.PreviousPayment;
                entity.SubTotal = entity.Quantity * entity.UnitPrice;
                entity.TaxAmount = BillingHelper.CalculateTaxAmount(entity.GLTaxRateID, entity.SubTotal);
                entity.TotalAmount = entity.TaxAmount + entity.SubTotal;

                entity.NetAmount = entity.TotalAmount - decuctiveSum;

                db.ConsReceivableInvoiceDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                invoiceDetail.SubTotal = entity.SubTotal;
                invoiceDetail.TaxAmount = entity.TaxAmount;
                invoiceDetail.TotalAmount = entity.TotalAmount;

                this.UpdateInvoiceAfterAddingItem(entity.ConsReceivableInvoiceId);

                new ConstructionDistributionHelper().UpdateReceivableInvoice(GetCurrentReceivableInvoice(entity.ConsReceivableInvoiceId));

            }

            return Json(new[] { invoiceDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InvoiceLine_Destroy([DataSourceRequest]DataSourceRequest request, ConsReceivableInvoiceDetail invoiceDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConsReceivableInvoiceDetail
                {
                    ConsReceivableInvoiceDetailId = invoiceDetail.ConsReceivableInvoiceDetailId,
                    ConsReceivableInvoiceId = invoiceDetail.ConsReceivableInvoiceId,
                    WorkItemPerProjectId = invoiceDetail.WorkItemPerProjectId,
                    ProjectPaymentCertificateId = invoiceDetail.ProjectPaymentCertificateId,
                    Service = invoiceDetail.Service,
                    Description = invoiceDetail.Description,
                    AccountCode = invoiceDetail.AccountCode,
                    Quantity = invoiceDetail.Quantity,
                    UnitPrice = invoiceDetail.UnitPrice,
                    SubTotal = invoiceDetail.SubTotal,
                    GLTaxRateID = invoiceDetail.GLTaxRateID,
                    TaxAmount = invoiceDetail.TaxAmount,
                    TotalAmount = invoiceDetail.TotalAmount,
                    Penality = invoiceDetail.Penality,
                    Retention = invoiceDetail.Retention,
                    PreviousPayment = invoiceDetail.PreviousPayment,
                    AdvanceRepayment = invoiceDetail.AdvanceRepayment,
                    Correction = invoiceDetail.Correction,
                    Others = invoiceDetail.Others,
                    NetAmount = invoiceDetail.NetAmount,

                    OrgBranchName = invoiceDetail.OrgBranchName,
                    DateCreated = invoiceDetail.DateCreated,
                    LastModified = invoiceDetail.LastModified,
                    CreatedBy = invoiceDetail.CreatedBy,
                    ModifiedBy = invoiceDetail.ModifiedBy
                };

                db.ConsReceivableInvoiceDetails.Attach(entity);
                db.ConsReceivableInvoiceDetails.Remove(entity);
                db.SaveChanges();

                new ConstructionDistributionHelper().UpdateReceivableInvoice(GetCurrentReceivableInvoice(entity.ConsReceivableInvoiceId));
            }

            return Json(new[] { invoiceDetail }.ToDataSourceResult(request, ModelState));
        }

        private void UpdateInvoiceAfterAddingItem(int invoiceId)
        {
            var currentInvoice = db.ConsReceivableInvoices.Find(invoiceId);
            if (currentInvoice != null)
            {
                var invoiceItems = db.ConsReceivableInvoiceDetails
                    .Where(inv => inv.ConsReceivableInvoiceId == currentInvoice.ConsReceivableInvoiceId).ToList();
                if (invoiceItems.Any())
                {
                    currentInvoice.TotalAmount = invoiceItems.Sum(itm => itm.NetAmount);

                    db.ConsReceivableInvoices.Attach(currentInvoice);
                    db.Entry(currentInvoice).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        private ConsReceivableInvoice GetCurrentReceivableInvoice(int id)
        {
            var invoice = new ConsReceivableInvoice();
            var temp = db.ConsReceivableInvoices.Find(id);
            if (temp != null)
            {
                invoice = temp;
            }
            return invoice;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}